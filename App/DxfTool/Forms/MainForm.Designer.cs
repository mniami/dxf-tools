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
        txtDxfFilePath = new TextBox();
        btnExportGpsFromDxf = new Button();
        label1 = new Label();
        label2 = new Label();
        label4 = new Label();
        label3 = new Label();
        btnDxfFile = new Button();
        btnDestinationFilePath = new Button();
        txtDestionationFilePath = new TextBox();
        label5 = new Label();
        label6 = new Label();
        sfdDestinationFile = new SaveFileDialog();
        SuspendLayout();
        // 
        // ofdSource
        // 
        ofdSource.FileName = "plik.dxf";
        ofdSource.Filter = "DXF pliki|*.dxf";
        // 
        // txtDxfFilePath
        // 
        txtDxfFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDxfFilePath.BorderStyle = BorderStyle.None;
        txtDxfFilePath.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 238);
        txtDxfFilePath.Location = new Point(73, 171);
        txtDxfFilePath.Margin = new Padding(9, 9, 9, 9);
        txtDxfFilePath.Multiline = true;
        txtDxfFilePath.Name = "txtDxfFilePath";
        txtDxfFilePath.PlaceholderText = "C:\\moj.dxf";
        txtDxfFilePath.Size = new Size(535, 40);
        txtDxfFilePath.TabIndex = 1;
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
        btnExportGpsFromDxf.Location = new Point(55, 329);
        btnExportGpsFromDxf.Name = "btnExportGpsFromDxf";
        btnExportGpsFromDxf.Size = new Size(594, 40);
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
        label1.Size = new Size(101, 15);
        label1.TabIndex = 2;
        label1.Text = "Importuj plik DXF";
        // 
        // label2
        // 
        label2.Anchor = AnchorStyles.Top;
        label2.AutoSize = true;
        label2.BackColor = Color.Transparent;
        label2.Font = new Font("Cambria", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label2.ForeColor = Color.FromArgb(41, 61, 118);
        label2.Location = new Point(289, 44);
        label2.Name = "label2";
        label2.Size = new Size(141, 37);
        label2.TabIndex = 4;
        label2.Text = "DXF GPS";
        label2.TextAlign = ContentAlignment.TopCenter;
        // 
        // label4
        // 
        label4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label4.BackColor = Color.Transparent;
        label4.Font = new Font("Microsoft YaHei", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
        label4.ForeColor = Color.SlateGray;
        label4.Location = new Point(61, 89);
        label4.Name = "label4";
        label4.Size = new Size(608, 46);
        label4.TabIndex = 8;
        label4.Text = "Eksport punktów wysokościowych z plików DXF";
        label4.TextAlign = ContentAlignment.TopCenter;
        // 
        // label3
        // 
        label3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label3.BackColor = SystemColors.Window;
        label3.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 238);
        label3.Location = new Point(61, 160);
        label3.Name = "label3";
        label3.Size = new Size(594, 51);
        label3.TabIndex = 5;
        // 
        // btnDxfFile
        // 
        btnDxfFile.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnDxfFile.BackColor = Color.Transparent;
        btnDxfFile.BackgroundImage = Properties.Resources.folder_3121629;
        btnDxfFile.BackgroundImageLayout = ImageLayout.Zoom;
        btnDxfFile.FlatAppearance.BorderSize = 0;
        btnDxfFile.FlatAppearance.MouseOverBackColor = Color.FromArgb(107, 150, 228);
        btnDxfFile.Font = new Font("Segoe UI Emoji", 10.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
        btnDxfFile.ForeColor = SystemColors.Window;
        btnDxfFile.Location = new Point(616, 171);
        btnDxfFile.Name = "btnDxfFile";
        btnDxfFile.Size = new Size(34, 30);
        btnDxfFile.TabIndex = 9;
        btnDxfFile.UseVisualStyleBackColor = false;
        btnDxfFile.Click += btnDxfFile_Click;
        // 
        // btnDestinationFilePath
        // 
        btnDestinationFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnDestinationFilePath.BackColor = Color.Transparent;
        btnDestinationFilePath.BackgroundImage = Properties.Resources.folder_3121629;
        btnDestinationFilePath.BackgroundImageLayout = ImageLayout.Zoom;
        btnDestinationFilePath.FlatAppearance.BorderSize = 0;
        btnDestinationFilePath.FlatAppearance.MouseOverBackColor = Color.FromArgb(107, 150, 228);
        btnDestinationFilePath.Font = new Font("Segoe UI Emoji", 10.875F, FontStyle.Bold, GraphicsUnit.Point, 0);
        btnDestinationFilePath.ForeColor = SystemColors.Window;
        btnDestinationFilePath.Location = new Point(616, 265);
        btnDestinationFilePath.Name = "btnDestinationFilePath";
        btnDestinationFilePath.Size = new Size(34, 30);
        btnDestinationFilePath.TabIndex = 12;
        btnDestinationFilePath.UseVisualStyleBackColor = false;
        btnDestinationFilePath.Click += btnDestinationFilePath_Click;
        // 
        // txtDestionationFilePath
        // 
        txtDestionationFilePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDestionationFilePath.BorderStyle = BorderStyle.None;
        txtDestionationFilePath.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 238);
        txtDestionationFilePath.Location = new Point(73, 265);
        txtDestionationFilePath.Margin = new Padding(9, 9, 9, 9);
        txtDestionationFilePath.Multiline = true;
        txtDestionationFilePath.Name = "txtDestionationFilePath";
        txtDestionationFilePath.PlaceholderText = "C:\\wynik.txt";
        txtDestionationFilePath.Size = new Size(535, 40);
        txtDestionationFilePath.TabIndex = 10;
        txtDestionationFilePath.WordWrap = false;
        // 
        // label5
        // 
        label5.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label5.BackColor = SystemColors.Window;
        label5.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 238);
        label5.Location = new Point(61, 254);
        label5.Name = "label5";
        label5.Size = new Size(594, 51);
        label5.TabIndex = 11;
        // 
        // label6
        // 
        label6.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        label6.AutoSize = true;
        label6.BackColor = Color.Transparent;
        label6.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 238);
        label6.Location = new Point(61, 224);
        label6.Name = "label6";
        label6.Size = new Size(102, 15);
        label6.TabIndex = 13;
        label6.Text = "Eksportuj do pliku";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = SystemColors.Control;
        BackgroundImageLayout = ImageLayout.Center;
        ClientSize = new Size(716, 410);
        Controls.Add(label6);
        Controls.Add(btnDestinationFilePath);
        Controls.Add(txtDestionationFilePath);
        Controls.Add(label5);
        Controls.Add(btnDxfFile);
        Controls.Add(label4);
        Controls.Add(txtDxfFilePath);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(btnExportGpsFromDxf);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(6, 6, 6, 6);
        Name = "MainForm";
        Text = "Eksport DXF GPS do CSV";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private OpenFileDialog ofdSource;
    private TextBox txtDxfFilePath;
    private Button btnExportGpsFromDxf;
    private Label label1;
    private Label label2;
    private Label label4;
    private Label label3;
    private Button btnDxfFile;
    private Button btnDestinationFilePath;
    private TextBox txtDestionationFilePath;
    private Label label5;
    private Label label6;
    private SaveFileDialog sfdDestinationFile;
}
