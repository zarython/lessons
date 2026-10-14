using Matjari.Core;
using Matjari.Data;
using Matjari.Models;
using Microsoft.Data.Sqlite;

namespace Matjari.Forms;

// نافذة الصنف: للجديد وللتعديل
public class ProductEditForm : Form
{
    private readonly Product _product;

    private readonly TableLayoutPanel table = new()
    {
        AutoSize = true,
        ColumnCount = 2,
        Padding = new Padding(20)
    };

    private readonly TextBox txtBarcode = new();
    private readonly TextBox txtName = new();
    private readonly NumericUpDown numPrice = Number();
    private readonly NumericUpDown numQty = Number();

    private readonly Button btnSave = new()
    {
        Text = "حفظ",
        Dock = DockStyle.Fill,
        FlatStyle = FlatStyle.Flat,
        BackColor = UiTheme.Brand,
        ForeColor = Color.White
    };

    public ProductEditForm(Product product)
    {
        _product = product;

        // نافذة حوار: عنوانها من حالتها، ولا تكبير فيها
        Text = product.Id == 0
            ? "صنف جديد"
            : "تعديل الصنف " + product.Code;
        Font = new Font("Segoe UI", 12);
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        BackColor = UiTheme.Surface;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;

        // النافذة على مقاس ما فيها
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(table);
        AddRow("الباركود", txtBarcode);
        AddRow("اسم الصنف", txtName);
        AddRow("سعر البيع", numPrice);
        AddRow("الكمية", numQty);

        // الحفظ بعرض العمودين
        table.Controls.Add(btnSave);
        table.SetColumnSpan(btnSave, 2);
        btnSave.Height = LogicalToDeviceUnits(44);
        btnSave.Click += (s, e) => Save();
        Fill();
    }

    // سطر: لافتة، وبجانبها خانتها
    private void AddRow(string text, Control box)
    {
        var label = new Label { Text = text, AutoSize = true };
        label.Anchor = AnchorStyles.Left;
        box.Width = LogicalToDeviceUnits(260);
        table.Controls.Add(label);
        table.Controls.Add(box);
    }

    // خانة أرقام: لا حروف فيها، ولا أقلّ من صفر
    private static NumericUpDown Number() =>
        new() { Maximum = 1000000, DecimalPlaces = 2 };

    // التعديل يفتح النافذة ممتلئة
    private void Fill()
    {
        txtBarcode.Text = _product.Barcode;
        txtName.Text = _product.Name;
        numPrice.Value = _product.SalePrice;
        numQty.Value = _product.Quantity;
    }

    // من الخانات إلى الصنف، ثمّ إلى القاعدة
    private void Save()
    {
        if (txtName.Text.Trim() == "")
        {
            Warn("اكتب اسم الصنف");
            return;
        }

        _product.Barcode = txtBarcode.Text.Trim();
        _product.Name = txtName.Text.Trim();
        _product.SalePrice = numPrice.Value;
        _product.Quantity = numQty.Value;

        try
        {
            if (_product.Id == 0) ProductRepository.Insert(_product);
            else ProductRepository.Update(_product);
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            // الباركود فريد: القاعدة ترفض المكرّر
            Warn("هذا الباركود لصنف آخر");
            return;
        }

        Close();
    }

    // رسالة تُقرأ من اليمين
    private static void Warn(string text) => MessageBox.Show(
        text, "متجري", MessageBoxButtons.OK, MessageBoxIcon.Warning,
        MessageBoxDefaultButton.Button1,
        MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);
}