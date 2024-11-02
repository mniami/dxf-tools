namespace DxfTool.Forms
{
    partial class ResultForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultForm));
            label1 = new Label();
            dgvGps = new DataGridView();
            longitudeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            latitudeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            heightDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            gpsCoordinatesBindingSource = new BindingSource(components);
            dxfParseResultBindingSource = new BindingSource(components);
            label2 = new Label();
            label3 = new Label();
            lblTextFound = new Label();
            label5 = new Label();
            lblDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvGps).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gpsCoordinatesBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dxfParseResultBindingSource).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label1.ForeColor = Color.White;
            label1.Location = new Point(22, 21);
            label1.Name = "label1";
            label1.Size = new Size(161, 30);
            label1.TabIndex = 0;
            label1.Text = "Wyniki eksportu";
            // 
            // dgvGps
            // 
            dgvGps.AllowUserToAddRows = false;
            dgvGps.AllowUserToDeleteRows = false;
            dgvGps.AutoGenerateColumns = false;
            dgvGps.BackgroundColor = SystemColors.Control;
            dgvGps.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGps.Columns.AddRange(new DataGridViewColumn[] { longitudeDataGridViewTextBoxColumn, latitudeDataGridViewTextBoxColumn, heightDataGridViewTextBoxColumn });
            dgvGps.DataSource = gpsCoordinatesBindingSource;
            dgvGps.Location = new Point(22, 106);
            dgvGps.Name = "dgvGps";
            dgvGps.ReadOnly = true;
            dgvGps.Size = new Size(361, 150);
            dgvGps.TabIndex = 1;
            // 
            // longitudeDataGridViewTextBoxColumn
            // 
            longitudeDataGridViewTextBoxColumn.DataPropertyName = "Longitude";
            longitudeDataGridViewTextBoxColumn.HeaderText = "Longitude";
            longitudeDataGridViewTextBoxColumn.Name = "longitudeDataGridViewTextBoxColumn";
            longitudeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // latitudeDataGridViewTextBoxColumn
            // 
            latitudeDataGridViewTextBoxColumn.DataPropertyName = "Latitude";
            latitudeDataGridViewTextBoxColumn.HeaderText = "Latitude";
            latitudeDataGridViewTextBoxColumn.Name = "latitudeDataGridViewTextBoxColumn";
            latitudeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // heightDataGridViewTextBoxColumn
            // 
            heightDataGridViewTextBoxColumn.DataPropertyName = "Height";
            heightDataGridViewTextBoxColumn.HeaderText = "Height";
            heightDataGridViewTextBoxColumn.Name = "heightDataGridViewTextBoxColumn";
            heightDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // gpsCoordinatesBindingSource
            // 
            gpsCoordinatesBindingSource.DataSource = typeof(Models.GpsCoordinates);
            // 
            // dxfParseResultBindingSource
            // 
            dxfParseResultBindingSource.DataSource = typeof(Models.DxfParseResult);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label2.ForeColor = Color.White;
            label2.Location = new Point(22, 79);
            label2.Name = "label2";
            label2.Size = new Size(166, 17);
            label2.TabIndex = 2;
            label2.Text = "Znalezione koordynaty GPS";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label3.ForeColor = Color.White;
            label3.Location = new Point(22, 278);
            label3.Name = "label3";
            label3.Size = new Size(158, 17);
            label3.TabIndex = 3;
            label3.Text = "Ilość znalezionych wpisów";
            // 
            // lblTextFound
            // 
            lblTextFound.AutoSize = true;
            lblTextFound.BackColor = Color.Transparent;
            lblTextFound.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold, GraphicsUnit.Point, 238);
            lblTextFound.ForeColor = Color.White;
            lblTextFound.Location = new Point(22, 295);
            lblTextFound.Name = "lblTextFound";
            lblTextFound.Size = new Size(56, 45);
            lblTextFound.TabIndex = 4;
            lblTextFound.Text = "25";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label5.ForeColor = Color.White;
            label5.Location = new Point(22, 352);
            label5.Name = "label5";
            label5.Size = new Size(35, 17);
            label5.TabIndex = 5;
            label5.Text = "Opis";
            // 
            // lblDescription
            // 
            lblDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblDescription.BackColor = Color.Transparent;
            lblDescription.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 238);
            lblDescription.ForeColor = Color.White;
            lblDescription.Location = new Point(22, 379);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(332, 53);
            lblDescription.TabIndex = 6;
            lblDescription.Text = "25";
            // 
            // ResultForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(411, 450);
            Controls.Add(lblDescription);
            Controls.Add(label5);
            Controls.Add(lblTextFound);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(dgvGps);
            Controls.Add(label1);
            Name = "ResultForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ResultForm";
            Controls.SetChildIndex(label1, 0);
            Controls.SetChildIndex(dgvGps, 0);
            Controls.SetChildIndex(label2, 0);
            Controls.SetChildIndex(label3, 0);
            Controls.SetChildIndex(lblTextFound, 0);
            Controls.SetChildIndex(label5, 0);
            Controls.SetChildIndex(lblDescription, 0);
            ((System.ComponentModel.ISupportInitialize)dgvGps).EndInit();
            ((System.ComponentModel.ISupportInitialize)gpsCoordinatesBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dxfParseResultBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView dgvGps;
        private DataGridViewTextBoxColumn longitudeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn latitudeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn heightDataGridViewTextBoxColumn;
        private BindingSource gpsCoordinatesBindingSource;
        private BindingSource dxfParseResultBindingSource;
        private Label label2;
        private Label label3;
        private Label lblTextFound;
        private Label label5;
        private Label lblDescription;
    }
}