using Matjari.Core;

namespace Matjari.Forms;

// النافذة الرئيسة: شكلها كلّه مكتوب هنا بالكود، سطرًا سطرًا.
public partial class MainForm : Form
{

    // اللوحات الثلاث حقولًا في الصنف: بقيّة الدوالّ تحتاجها بالاسم.
    private readonly Panel pnlSide = new()
    {
        Dock = DockStyle.Left,
        Width = 230,
                BackColor = UiTheme.Navy
    };

    private readonly Panel pnlTop = new()
    {
        Dock = DockStyle.Top,
        Height = 56,
                BackColor = UiTheme.Surface
    };

    private readonly Panel pnlContent = new()
    {
        Dock = DockStyle.Fill,
                BackColor = UiTheme.Background
    };

    // لوحة الشعار في أعلى الشريط الجانبيّ، لونها من الشريط نفسه.
    private readonly Panel pnlBrand = new()
    {
        Dock = DockStyle.Top,
        Height = 110
    };

    // اسم البرنامج: لافتة تُعرض ولا يُكتب فيها.
    private readonly Label lblBrand = new()
    {
        Dock = DockStyle.Fill,
        Text = "متجري",
        ForeColor = Color.White,
        Font = new Font("Segoe UI", 20, FontStyle.Bold),
        TextAlign = ContentAlignment.MiddleCenter
    };

    // لوحة الأزرار: تملأ ما بقي من الشريط تحت الشعار.
    private readonly Panel pnlNav = new()
    {
        Dock = DockStyle.Fill
    };

    // الزرّ المفتوح الآن: نحفظه لنُطفئه حين يُفتح غيره.
    private Button? _activeNav;

    // أسفل الشريط: اسم المستخدم وزرّ الخروج.
    private readonly Panel pnlUser = new()
    {
        Dock = DockStyle.Bottom,
        Height = 70,
                BackColor = UiTheme.NavyLight

    };

    private readonly Label lblUser = new()
    {
        Dock = DockStyle.Top,
        Height = 26,
        Text = "المدير",
                ForeColor = UiTheme.NavText,
        TextAlign = ContentAlignment.MiddleCenter
    };

    private readonly Button btnLogout = new()
    {
        Dock = DockStyle.Bottom,
        Height = 44,
        Text = "خروج",
        FlatStyle = FlatStyle.Flat,
        UseVisualStyleBackColor = false,
                BackColor = UiTheme.NavyLight,
                ForeColor = UiTheme.Danger
    };

    // الشريط العلويّ: اسم الشاشة المفتوحة، وسطر الكاشير والساعة.
    private readonly Label lblScreenTitle = new()
    {
        Dock = DockStyle.Right,
        Width = 420,
        Text = "نقطة البيع",
                ForeColor = UiTheme.Text,
        Font = new Font("Segoe UI", 15, FontStyle.Bold),
        TextAlign = ContentAlignment.MiddleRight,
        Padding = new Padding(0, 0, 20, 0)
    };

    private readonly Label lblStatus = new()
    {
        Dock = DockStyle.Left,
        Width = 340,
                ForeColor = UiTheme.TextMuted,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(20, 0, 0, 0)
    };

    private readonly System.Windows.Forms.Timer _clock = new()
    {
        Interval = 1000
    };

    public MainForm()
    {
        // هذه الدالّة كتبها المصمّم عنّا، وتبقى أوّل سطر في الباني.
        InitializeComponent();

        // عنوان النافذة في شريطها العلويّ.
        Text = "متجري، برنامج إدارة المبيعات والمخازن";

        // الاتّجاه من اليمين إلى اليسار: الكتابة والقوائم تنضبط.
        RightToLeft = RightToLeft.Yes;

        // وقلب التخطيط كلّه معه: العنوان يمينًا وأزرار النافذة يسارًا.
        RightToLeftLayout = true;

        // لا تصغر النافذة عن هذا الحجم مهما سُحبت حافّتها.
        MinimumSize = new Size(1100, 700);

        // تُفتح في منتصف الشاشة، وبكامل مساحتها.
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;

        // قاعدة الإرساء: آخر ما يُضاف يرسو أوّلًا ويأخذ حافّته.
        // وأوّل ما يُضاف يأخذ ما بقي من المساحة.
        // فما يملأ يُضاف أوّلًا، وما يأخذ حافّةً يُضاف بعده.
        Controls.Add(pnlContent);
        Controls.Add(pnlTop);
        Controls.Add(pnlSide);

        // داخل الشريط الجانبيّ، القاعدة نفسها: ما يملأ أوّلًا.
        pnlSide.Controls.Add(pnlNav);
        pnlSide.Controls.Add(pnlUser);

        pnlSide.Controls.Add(pnlBrand);
        pnlBrand.Controls.Add(lblBrand);
        pnlUser.Controls.Add(btnLogout);
        pnlUser.Controls.Add(lblUser);
        btnLogout.FlatAppearance.BorderSize = 0;
        // الأحجام بالكود لا تكبر مع تكبير الشاشة، فنسألها بأنفسنا.
        pnlSide.Width = LogicalToDeviceUnits(230);
        pnlTop.Height = LogicalToDeviceUnits(56);
        pnlUser.Height = LogicalToDeviceUnits(70);

        pnlTop.Controls.Add(lblScreenTitle);
        pnlTop.Controls.Add(lblStatus);

        // الساعة: سطرٌ يكتب الوقت، وآخر يطلبه كلّ ثانية.
        _clock.Tick += (s, e) => lblStatus.Text =
            "كاشير ١  ·  " + DateTime.Now.ToString("HH:mm:ss");
        _clock.Start();

        // الأزرار تُبنى بعد أن صار للوحتها مكان.
        BuildNav();

    }

    // تبني أزرار الوجهات من جدول: صفّ لكلّ زرّ.
    private void BuildNav()
    {
        var nav = new (string Id, string Text, string Key)[]
        {
            ("navSale", "نقطة البيع", "F2"),
            ("navProducts", "الأصناف", "F3"),
            ("navInvoices", "الفواتير", "F4"),
            ("navStock", "المخزون والجرد", "F5"),
            ("navCustomers", "العملاء والديون", "F6"),
            ("navSuppliers", "الموردون", ""),
            ("navPurchases", "المشتريات", ""),
            ("navReports", "التقارير", "F7"),
            ("navPrints", "المطبوعات", ""),
            ("navUsers", "المستخدمون", ""),
            ("navSettings", "الإعدادات", ""),
        };

        // من آخر الجدول إلى أوّله: آخر ما يُضاف يرسو أوّلًا.
        for (int i = nav.Length - 1; i >= 0; i--)
        {
            var item = nav[i];
            var button = new Button
            {
                Name = item.Id,
                Text = item.Text,
                Tag = item.Key,
                Dock = DockStyle.Top,
                Height = 42,
                UseVisualStyleBackColor = true
            };
            StyleNav(button);

            button.Click += Nav_Click;

            // مفتاح الزرّ داخله: لافتةٌ صغيرة على حافّته.
            if (item.Key.Length > 0)
            {
                button.Controls.Add(new Label
                {
                    Dock = DockStyle.Left,
                    Width = 52,
                    Text = item.Key,
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            pnlNav.Controls.Add(button);
                        if (i == 0) OpenScreen(button);

        }
    }

    // تُلبس الزرّ شكله: مسطّح بلا حدّ، بلون الشريط وحروف فاتحة.
    private void StyleNav(Button button)
    {
        // مسطّح: لا بروز من ويندوز ولا إطار حوله.
        button.FlatStyle = FlatStyle.Flat;
        // وسُمك حدّه صفرًا: لا خطّ يفصله عمّا فوقه وتحته.
        button.FlatAppearance.BorderSize = 0;

        // ونلوّنه بأنفسنا، فنلغي شكل ويندوز الذي طلبناه قبل قليل.
        button.UseVisualStyleBackColor = false;

        // لونه من الشريط نفسه، فلا يظهر له وجهٌ منفصل.
                button.BackColor = UiTheme.Navy;

        // وحروفه فاتحة تُقرأ على الكحليّ بلا إجهاد.
                button.ForeColor = UiTheme.NavText;

        // النصّ إلى اليمين في منتصف الارتفاع، كالعربيّة.
                button.TextAlign = ContentAlignment.MiddleLeft;
        // وفراغٌ يمينَ النصّ يبعده عن حافّة الشريط.
        button.Padding = new Padding(0, 0, 24, 0);

        // ويدُ المؤشّر تقول للكاشير: هذا يُنقر.
        button.Cursor = Cursors.Hand;
    }

    // تنقل الضوء: السابق يعود هادئًا، والمنقور يُضيء.
    private void SetActive(Button button)
    {
        if (_activeNav != null) StyleNav(_activeNav);

        _activeNav = button;

                button.BackColor = UiTheme.Brand;
        button.ForeColor = Color.White;
    }

    // ما يجري عند النقر: المنقور هو المفتوح.
    private void Nav_Click(object? sender, EventArgs e)
    {
                if (sender is Button button) OpenScreen(button);
    }

    // تفتح وجهةً: تُضيء زرّها، وتكتب اسمها، وتملأ لوحة المحتوى.
    private void OpenScreen(Button button)
    {
        SetActive(button);
        lblScreenTitle.Text = button.Text;

        var screen = new Panel
        {
            Dock = DockStyle.Fill,
                        BackColor = UiTheme.Background
        };

        screen.Controls.Add(new Label
        {
            Dock = DockStyle.Fill,
            Text = button.Text,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        });

        pnlContent.Controls.Clear();
        pnlContent.Controls.Add(screen);
    }

    // المفاتيح: يسألنا ويندوز قبل أن يصل المفتاح إلى أيّ عنصر.
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        foreach (Button button in pnlNav.Controls.OfType<Button>())
        {
            if (button.Tag as string == keyData.ToString())
            {
                OpenScreen(button);
                return true;
            }
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

}

