using Matjari.Core;
using Matjari.Data;
using Matjari.Models;

namespace Matjari.Forms;

// شاشة الأصناف: شريط بحث فوق، وجدول يملأ ما تحته.
public class ProductsScreen : UserControl
{
    private readonly Panel pnlTools = new()
    {
        Dock = DockStyle.Top,
        Height = 64,
        BackColor = UiTheme.Surface
    };

    // لوحة تدفّق ترتّب ما فيها وحدها.
    private readonly FlowLayoutPanel flowTools = new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(18, 14, 18, 14),
        WrapContents = false
    };

    private readonly Label lblSearch = new()
    {
        Text = "بحث",
        AutoSize = true,
        Margin = new Padding(6, 9, 10, 3),
        ForeColor = UiTheme.TextMuted
    };

    private readonly TextBox txtSearch = new()
    {
        Width = 360,
        PlaceholderText = "الاسم أو الكود أو الباركود"
    };

    private readonly DataGridView grid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AutoGenerateColumns = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        RowHeadersVisible = false,
        BorderStyle = BorderStyle.None,
        BackgroundColor = UiTheme.Background
    };

    private readonly Panel pnlButtons = new()
    {
        Dock = DockStyle.Bottom,
        Height = 72,
        BackColor = UiTheme.Surface
    };

    private readonly Label lblCount = new()
    {
        Dock = DockStyle.Left,
        Width = 300,
        ForeColor = UiTheme.TextMuted,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(20, 0, 0, 0)
    };

    private readonly FlowLayoutPanel flowButtons = new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(18, 15, 18, 15),
        WrapContents = false
    };

    private readonly Button btnNew = new() { Text = "صنف جديد" };
    private readonly Button btnEdit = new() { Text = "تعديل" };
    private readonly Button btnDelete = new() { Text = "حذف" };
    private readonly Button btnCategories =
        new() { Text = "التصنيفات" };

    public ProductsScreen()
    {
        Dock = DockStyle.Fill;
        RightToLeft = RightToLeft.Yes;
        BackColor = UiTheme.Background;

        // الأحجام تتبع تكبير الشاشة.
        pnlTools.Height = LogicalToDeviceUnits(64);
        pnlButtons.Height = LogicalToDeviceUnits(72);
        txtSearch.Width = LogicalToDeviceUnits(360);
        lblCount.Width = LogicalToDeviceUnits(300);

        // ما يملأ يُضاف أوّلًا، وما يأخذ حافّةً يُضاف بعده.
        Controls.Add(grid);
        Controls.Add(pnlButtons);
        Controls.Add(pnlTools);

        pnlTools.Controls.Add(flowTools);
        flowTools.Controls.Add(lblSearch);
        flowTools.Controls.Add(txtSearch);

        pnlButtons.Controls.Add(flowButtons);
        pnlButtons.Controls.Add(lblCount);
        flowButtons.Controls.Add(btnNew);
        flowButtons.Controls.Add(btnEdit);
        flowButtons.Controls.Add(btnDelete);
        flowButtons.Controls.Add(btnCategories);

        Style(btnNew, UiTheme.Brand, Color.White);
        Style(btnEdit, UiTheme.Quiet, UiTheme.Text);
        Style(btnDelete, UiTheme.Quiet, UiTheme.DeepRed);
        Style(btnCategories, UiTheme.Quiet, UiTheme.Text);

        BuildColumns();

        txtSearch.TextChanged += (s, e) => LoadProducts();
        LoadProducts();

        // ما تفعله الأزرار الأربعة
        btnNew.Click += (s, e) => OpenEditor(new Product());
        btnEdit.Click += (s, e) => OpenEditor(Selected());
        btnDelete.Click += (s, e) => DeleteSelected();
        btnCategories.Click += (s, e) => OpenCategories();
    }

    // زرٌّ واحد بثلاثة ألوان: نُلبسه مرّةً وننادي الدالّة ثلاثًا.
    private void Style(Button button, Color back, Color fore)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.UseVisualStyleBackColor = false;
        button.BackColor = back;
        button.ForeColor = fore;
        button.Cursor = Cursors.Hand;
        button.Size = LogicalToDeviceUnits(new Size(140, 42));
        button.Margin = new Padding(3, 3, 12, 3);
    }

    // أعمدة الجدول بأسمائنا نحن، لا بأسماء الخصائص.
    private void BuildColumns()
    {
        AddColumn("Code", "الكود", 90);
        AddColumn("Name", "اسم الصنف", 260, fill: true);
        AddColumn("CategoryName", "التصنيف", 140);
        AddColumn("Unit", "الوحدة", 80);
        AddColumn("SalePrice", "سعر البيع", 110);
        AddColumn("Quantity", "الكمية", 100);
    }

    private void AddColumn(string name, string header, int width,
        bool fill = false)
    {
        var column = new DataGridViewTextBoxColumn
        {
            Name = name,
            HeaderText = header,
            Width = width
        };
        if (fill)
        {
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        grid.Columns.Add(column);
    }

    // تطلب الأصناف من القاعدة وتعرضها.
    private void LoadProducts()
    {
        var products = ProductRepository.Search(txtSearch.Text);

        grid.Rows.Clear();
        foreach (var product in products)
        {
            int row = grid.Rows.Add(
                product.Code,
                product.Name,
                product.CategoryName,
                product.Unit,
                product.SalePrice,
                product.Quantity);

            // الصفّ يحمل صنفه معه
            grid.Rows[row].Tag = product;
        }

        lblCount.Text = products.Count + " صنف";
    }

    // صنف الصفّ المختار، إن وُجد
    private Product? Selected() => grid.CurrentRow?.Tag as Product;

    // تفتح النافذة، ثمّ تقرأ القائمة من جديد
    private void OpenEditor(Product? product)
    {
        if (product == null) return;

        using var form = new ProductEditForm(product);
        form.ShowDialog();
        LoadProducts();
    }

    // نسأل قبل الحذف، و«لا» هو المختار
    private void DeleteSelected()
    {
        var product = Selected();
        if (product == null) return;

        var answer = MessageBox.Show(
            "هل تحذف «" + product.Name + "»؟", "حذف صنف",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading |
            MessageBoxOptions.RightAlign);
        if (answer != DialogResult.Yes) return;

        ProductRepository.Delete(product);
        LoadProducts();
    }

    // بعد النافذة نقرأ القائمة: الأسماء قد تغيّرت
    private void OpenCategories()
    {
        using var form = new CategoriesForm();
        form.ShowDialog();
        LoadProducts();
    }

}