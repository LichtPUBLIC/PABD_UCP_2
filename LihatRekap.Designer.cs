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
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Location = new System.Drawing.Point(27, 21);
            this.reportViewer1.Margin = new System.Windows.Forms.Padding(2);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(1100, 193);
            this.reportViewer1.TabIndex = 0;
            // 
            // Kembali
            // 
            this.Kembali.Location = new System.Drawing.Point(1031, 231);
            this.Kembali.Margin = new System.Windows.Forms.Padding(2);
            this.Kembali.Name = "Kembali";
            this.Kembali.Size = new System.Drawing.Size(96, 29);
            this.Kembali.TabIndex = 1;
            this.Kembali.Text = "Kembali";
            this.Kembali.UseVisualStyleBackColor = true;
            this.Kembali.Click += new System.EventHandler(this.btnKembali);
            // 
            // LihatRekap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 273);
            this.Controls.Add(this.Kembali);
            this.Controls.Add(this.reportViewer1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LihatRekap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LihatRekap";
            this.Load += new System.EventHandler(this.LihatRekap_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.Button Kembali;
    }
}