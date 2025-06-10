namespace projectsem4
{
    partial class PreviewDataMhs
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
            this.dgvPreviewDataMhs = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviewDataMhs)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPreviewDataMhs
            // 
            this.dgvPreviewDataMhs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPreviewDataMhs.Location = new System.Drawing.Point(26, 27);
            this.dgvPreviewDataMhs.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvPreviewDataMhs.Name = "dgvPreviewDataMhs";
            this.dgvPreviewDataMhs.RowHeadersWidth = 62;
            this.dgvPreviewDataMhs.RowTemplate.Height = 28;
            this.dgvPreviewDataMhs.Size = new System.Drawing.Size(489, 202);
            this.dgvPreviewDataMhs.TabIndex = 0;
            this.dgvPreviewDataMhs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPreview_CellClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(442, 246);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 25);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnOkePreview);
            // 
            // PreviewDataMhs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 292);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvPreviewDataMhs);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PreviewDataMhs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PreviewDataMhs";
            this.Load += new System.EventHandler(this.PreviewDataMhs_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPreviewDataMhs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPreviewDataMhs;
        private System.Windows.Forms.Button button1;
    }
}