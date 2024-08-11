namespace DxfTool;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        ofdSource = new OpenFileDialog();
        ofdDestination = new OpenFileDialog();
        txtDxfHighPointName = new TextBox();
        btnExportGpsFromDxf = new Button();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        btnClose = new Button();
        label4 = new Label();
        SuspendLayout();
        // 
        // ofdSource
        // 
        ofdSource.FileName = "plik.dxf";
        ofdSource.Filter = "DXF pliki|*.dxf";
        // 
        // ofdDestination
        // 
        ofdDestination.CheckFileExists = false;
        ofdDestination.CheckPathExists = false;
        ofdDestination.DefaultExt = "*.csv";
        ofdDestination.FileName = "export.csv";
        ofdDestination.Filter = "CSV plik|*.csv";
        ofdDestination.SelectReadOnly = false;
        // 
        // txtDxfHighPointName
        // 
        txtDxfHighPointName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDxfHighPointName.BorderStyle = BorderStyle.None;
        txtDxfHighPointName.Font = new Font("BreezeSans", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
        txtDxfHighPointName.Location = new Point(71, 175);
        txtDxfHighPointName.Margin = new Padding(9);
        txtDxfHighPointName.Name = "txtDxfHighPointName";
        txtDxfHighPointName.Size = new Size(237, 23);
        txtDxfHighPointName.TabIndex = 1;
        txtDxfHighPointName.Text = "punkt wysokościowy";
        // 
        // btnExportGpsFromDxf
        // 
        btnExportGpsFromDxf.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnExportGpsFromDxf.BackColor = Color.FromArgb(65, 111, 212);
        btnExportGpsFromDxf.FlatAppearance.BorderSize = 0;
        btnExportGpsFromDxf.FlatAppearance.MouseOverBackColor = Color.FromArgb(107, 150, 228);
        btnExportGpsFromDxf.FlatStyle = FlatStyle.Flat;
        btnExportGpsFromDxf.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        btnExportGpsFromDxf.ForeColor = SystemColors.Window;
        btnExportGpsFromDxf.Location = new Point(61, 243);
        btnExportGpsFromDxf.Name = "btnExportGpsFromDxf";
        btnExportGpsFromDxf.Size = new Size(253, 40);
        btnExportGpsFromDxf.TabIndex = 3;
        btnExportGpsFromDxf.Text = "Eksportuj";
        btnExportGpsFromDxf.UseVisualStyleBackColor = false;
        btnExportGpsFromDxf.Click += btnExportGpsFromDxf_Click;
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label1.AutoSize = true;
        label1.BackColor = Color.Transparent;
        label1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
        label1.Location = new Point(61, 136);
        label1.Name = "label1";
        label1.Size = new Size(43, 15);
        label1.TabIndex = 2;
        label1.Text = "Nazwa";
        // 
        // label2
        // 
        label2.Anchor = AnchorStyles.Top;
        label2.AutoSize = true;
        label2.BackColor = Color.Transparent;
        label2.Font = new Font("Cambria", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label2.ForeColor = Color.FromArgb(41, 61, 118);
        label2.Location = new Point(118, 43);
        label2.Name = "label2";
        label2.Size = new Size(141, 37);
        label2.TabIndex = 4;
        label2.Text = "DXF GPS";
        label2.TextAlign = ContentAlignment.TopCenter;
        // 
        // label3
        // 
        label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label3.BackColor = SystemColors.Window;
        label3.Location = new Point(61, 162);
        label3.Name = "label3";
        label3.Size = new Size(253, 48);
        label3.TabIndex = 5;
        // 
        // btnClose
        // 
        btnClose.BackColor = Color.Transparent;
        btnClose.FlatAppearance.BorderColor = Color.White;
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.FlatAppearance.MouseDownBackColor = Color.Transparent;
        btnClose.FlatAppearance.MouseOverBackColor = Color.Transparent;
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.Font = new Font("Gadugi", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
        btnClose.ForeColor = Color.DarkSlateGray;
        btnClose.Location = new Point(335, -1);
        btnClose.Name = "btnClose";
        btnClose.Size = new Size(40, 39);
        btnClose.TabIndex = 7;
        btnClose.Text = "x";
        btnClose.UseVisualStyleBackColor = true;
        btnClose.Click += btnClose_Click;
        btnClose.MouseEnter += btnClose_MouseEnter;
        btnClose.MouseLeave += btnClose_MouseLeave;
        // 
        // label4
        // 
        label4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label4.BackColor = Color.Transparent;
        label4.Font = new Font("Microsoft YaHei", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
        label4.ForeColor = Color.GhostWhite;
        label4.Location = new Point(61, 89);
        label4.Name = "label4";
        label4.Size = new Size(267, 47);
        label4.TabIndex = 8;
        label4.Text = "Eksport punktów wysokościowych z plików DXF";
        label4.TextAlign = ContentAlignment.TopCenter;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
        BackgroundImageLayout = ImageLayout.Center;
        ClientSize = new Size(374, 339);
        Controls.Add(label4);
        Controls.Add(btnClose);
        Controls.Add(txtDxfHighPointName);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(btnExportGpsFromDxf);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.None;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Eksport DXF GPS do CSV";
        MouseDown += Form1_MouseDown;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private OpenFileDialog ofdSource;
    private OpenFileDialog ofdDestination;
    private TextBox txtDxfHighPointName;
    private Button btnExportGpsFromDxf;
    private Label label1;
    private Label label2;
    private Label label3;
    private Button btnClose;
    private Label label4;
}
