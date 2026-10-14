using Matjari.Core;
using Matjari.Data;
using Matjari.Models;

namespace Matjari.Forms;

// نافذة التصنيفات: قائمة، وخانة اسم، وثلاثة أزرار
public class CategoriesForm : Form
{
    private readonly TableLayoutPanel table = new()
    {
        AutoSize = true,
        ColumnCount = 3,
        Padding = new Padding(20)
    };

    private readonly ListBox list = new();
    private readonly TextBox txtName = new();
    private readonly Button btnAdd = new() { Text = "إضافة" };
    private readonly Button btnRename = new() { Text = "إعادة تسمية" };
    private readonly Button btnDelete = new() { Text = "حذف" };

    public CategoriesForm()
    {
        // شكلها كنافذة الصنف: خطّها، واتّجاهها، وبياضها
        Text = "التصنيفات";
        Font = new Font("Segoe UI", 12);
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        BackColor = UiTheme.Surface;

        // وسلوكها كذلك: لا تكبير ولا تصغير، وفي وسط أمّها
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Controls.Add(table);

        // القائمة والخانة بعرض الأعمدة الثلاثة
        list.Size = LogicalToDeviceUnits(new Size(360, 240));
        list.Dock = DockStyle.Fill;
        table.Controls.Add(list);
        table.SetColumnSpan(list, 3);
        txtName.Dock = DockStyle.Fill;
        table.Controls.Add(txtName);
        table.SetColumnSpan(txtName, 3);

        // ثلاثة أزرار بمقاس واحد
        foreach (var b in new[] { btnAdd, btnRename, btnDelete })
        {
            b.Size = LogicalToDeviceUnits(new Size(120, 44));
            table.Controls.Add(b);
        }

        // ما تفعله الأزرار الثلاثة
        btnAdd.Click += (s, e) => Save(new Category());
        btnRename.Click += (s, e) => Save(Selected());
        btnDelete.Click += (s, e) => DeleteSelected();

        // ما يُختار في القائمة يصعد إلى الخانة
        list.SelectedIndexChanged +=
            (s, e) => txtName.Text = list.Text;
        LoadList();
    }

    // تصنيف السطر المختار، إن وُجد
    private Category? Selected() => list.SelectedItem as Category;

    // تقرأ التصنيفات من القاعدة، وتفرغ الخانة
    private void LoadList()
    {
        list.Items.Clear();
        foreach (var c in CategoryRepository.All()) list.Items.Add(c);
        txtName.Clear();
    }

    // الاسم من الخانة إلى التصنيف
    private void Save(Category? c)
    {
        var name = txtName.Text.Trim();
        if (c == null || name == "") return;

        c.Name = name;
        if (c.Id == 0) CategoryRepository.Insert(c);
        else CategoryRepository.Rename(c);
        LoadList();
    }

    // نسأل قبل الحذف، و«لا» هو المختار
    private void DeleteSelected()
    {
        var c = Selected();
        if (c == null) return;

        var answer = MessageBox.Show(
            "هل تحذف «" + c.Name + "»؟", "حذف تصنيف",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2,
            MessageBoxOptions.RtlReading |
            MessageBoxOptions.RightAlign);
        if (answer != DialogResult.Yes) return;

        CategoryRepository.Delete(c);
        LoadList();
    }

}