namespace Matjari;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblHint = new Label();
        SuspendLayout();
        //
        // lblTitle
        //
        lblTitle.Dock = DockStyle.Top;
        lblTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(15, 24, 56);
        lblTitle.Location = new Point(0, 60);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(900, 70);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "متجري";
        lblTitle.TextAlign = ContentAlignment.MiddleCenter;
        //
        // lblHint
        //
        lblHint.Dock = DockStyle.Top;
        lblHint.Font = new Font("Segoe UI", 12F);
        lblHint.ForeColor = Color.FromArgb(67, 83, 122);
        lblHint.Location = new Point(0, 130);
        lblHint.Name = "lblHint";
        lblHint.Size = new Size(900, 40);
        lblHint.TabIndex = 1;
        lblHint.Text = "برنامج إدارة المبيعات والمخازن";
        lblHint.TextAlign = ContentAlignment.MiddleCenter;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(245, 248, 255);
        ClientSize = new Size(900, 600);
        Controls.Add(lblHint);
        Controls.Add(lblTitle);
        Font = new Font("Segoe UI", 10F);
        Name = "MainForm";
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "متجري — برنامج إدارة المبيعات والمخازن";
        ResumeLayout(false);
    }

    private Label lblTitle;
    private Label lblHint;
}
