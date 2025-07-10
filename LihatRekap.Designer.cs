namespace projectsem4
{
    partial class LihatRekap
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.Kembali = new System.Windows.Forms.Button();
            this.cmbMataKuliah = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(27, 107);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(2);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(1100, 322);
            this.reportViewer1.TabIndex = 0;
            // 
            // Kembali
            // 
            this.Kembali.Location = new System.Drawing.Point(1031, 433);
            this.Kembali.Margin = new System.Windows.Forms.Padding(2);
            this.Kembali.Name = "Kembali";
            this.Kembali.Size = new System.Drawing.Size(96, 29);
            this.Kembali.TabIndex = 1;
            this.Kembali.Text = "Kembali";
            this.Kembali.UseVisualStyleBackColor = true;
            this.Kembali.Click += new System.EventHandler(this.btnKembali);
            // 
            // cmbMataKuliah
            // 
            this.cmbMataKuliah.FormattingEnabled = true;
            this.cmbMataKuliah.Location = new System.Drawing.Point(412, 46);
            this.cmbMataKuliah.Name = "cmbMataKuliah";
            this.cmbMataKuliah.Size = new System.Drawing.Size(406, 21);
            this.cmbMataKuliah.TabIndex = 2;
            this.cmbMataKuliah.SelectedIndexChanged += new System.EventHandler(this.cmbMataKuliah_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Filter Berdasarkan Mata Kuliah : ";
            // 
            // LihatRekap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 473);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbMataKuliah);
            this.Controls.Add(this.Kembali);
            this.Controls.Add(this.reportViewer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LihatRekap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LihatRekap";
            this.Load += new System.EventHandler(this.LihatRekap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Button Kembali;
        private System.Windows.Forms.ComboBox cmbMataKuliah;
        private System.Windows.Forms.Label label1;
    }
}