namespace projectsem4
{
    partial class FormRekapChart
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.cbMataKuliah = new System.Windows.Forms.ComboBox();
            this.dtpMulai = new System.Windows.Forms.DateTimePicker();
            this.dtpSelesai = new System.Windows.Forms.DateTimePicker();
            this.btnTampilkan = new System.Windows.Forms.Button();
            this.chartRekap = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnKembali = new System.Windows.Forms.Button();
            this.IDdosen = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartRekap)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMataKuliah
            // 
            this.cbMataKuliah.FormattingEnabled = true;
            this.cbMataKuliah.Location = new System.Drawing.Point(291, 29);
            this.cbMataKuliah.Name = "cbMataKuliah";
            this.cbMataKuliah.Size = new System.Drawing.Size(234, 21);
            this.cbMataKuliah.TabIndex = 0;
            // 
            // dtpMulai
            // 
            this.dtpMulai.Location = new System.Drawing.Point(291, 77);
            this.dtpMulai.Name = "dtpMulai";
            this.dtpMulai.Size = new System.Drawing.Size(200, 20);
            this.dtpMulai.TabIndex = 1;
            // 
            // dtpSelesai
            // 
            this.dtpSelesai.Location = new System.Drawing.Point(291, 118);
            this.dtpSelesai.Name = "dtpSelesai";
            this.dtpSelesai.Size = new System.Drawing.Size(200, 20);
            this.dtpSelesai.TabIndex = 2;
            // 
            // btnTampilkan
            // 
            this.btnTampilkan.Location = new System.Drawing.Point(619, 100);
            this.btnTampilkan.Name = "btnTampilkan";
            this.btnTampilkan.Size = new System.Drawing.Size(89, 38);
            this.btnTampilkan.TabIndex = 3;
            this.btnTampilkan.Text = "Tampilkan Grafik";
            this.btnTampilkan.UseVisualStyleBackColor = true;
            this.btnTampilkan.Click += new System.EventHandler(this.btnTampilkan_Click_1);
            // 
            // chartRekap
            // 
            chartArea3.Name = "ChartArea1";
            this.chartRekap.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartRekap.Legends.Add(legend3);
            this.chartRekap.Location = new System.Drawing.Point(128, 187);
            this.chartRekap.Name = "chartRekap";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartRekap.Series.Add(series3);
            this.chartRekap.Size = new System.Drawing.Size(636, 344);
            this.chartRekap.TabIndex = 4;
            this.chartRekap.Text = "chart1";
            this.chartRekap.Click += new System.EventHandler(this.chartRekap_Click);
            // 
            // btnKembali
            // 
            this.btnKembali.Location = new System.Drawing.Point(718, 537);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(75, 23);
            this.btnKembali.TabIndex = 5;
            this.btnKembali.Text = "Kemabali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click_1);
            // 
            // IDdosen
            // 
            this.IDdosen.AutoSize = true;
            this.IDdosen.Location = new System.Drawing.Point(181, 32);
            this.IDdosen.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IDdosen.Name = "IDdosen";
            this.IDdosen.Size = new System.Drawing.Size(100, 13);
            this.IDdosen.TabIndex = 6;
            this.IDdosen.Text = "Nama Mata Kuliah :";
            // 
            // FormRekapChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 595);
            this.Controls.Add(this.IDdosen);
            this.Controls.Add(this.btnKembali);
            this.Controls.Add(this.chartRekap);
            this.Controls.Add(this.btnTampilkan);
            this.Controls.Add(this.dtpSelesai);
            this.Controls.Add(this.dtpMulai);
            this.Controls.Add(this.cbMataKuliah);
            this.Name = "FormRekapChart";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormRekapChart";
            this.Load += new System.EventHandler(this.FormRekapChart_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.chartRekap)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMataKuliah;
        private System.Windows.Forms.DateTimePicker dtpMulai;
        private System.Windows.Forms.DateTimePicker dtpSelesai;
        private System.Windows.Forms.Button btnTampilkan;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartRekap;
        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.Label IDdosen;
    }
}