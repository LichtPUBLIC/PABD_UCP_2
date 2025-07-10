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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.cbMataKuliah = new System.Windows.Forms.ComboBox();
            this.dtpMulai = new System.Windows.Forms.DateTimePicker();
            this.dtpSelesai = new System.Windows.Forms.DateTimePicker();
            this.btnTampilkan = new System.Windows.Forms.Button();
            this.chartRekap = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnKembali = new System.Windows.Forms.Button();
            this.IDdosen = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartRekap)).BeginInit();
            this.SuspendLayout();
            // 
            // cbMataKuliah
            // 
            this.cbMataKuliah.FormattingEnabled = true;
            this.cbMataKuliah.Location = new System.Drawing.Point(341, 34);
            this.cbMataKuliah.Name = "cbMataKuliah";
            this.cbMataKuliah.Size = new System.Drawing.Size(200, 21);
            this.cbMataKuliah.TabIndex = 0;
            // 
            // dtpMulai
            // 
            this.dtpMulai.Location = new System.Drawing.Point(341, 82);
            this.dtpMulai.Name = "dtpMulai";
            this.dtpMulai.Size = new System.Drawing.Size(200, 20);
            this.dtpMulai.TabIndex = 1;
            // 
            // dtpSelesai
            // 
            this.dtpSelesai.Location = new System.Drawing.Point(341, 123);
            this.dtpSelesai.Name = "dtpSelesai";
            this.dtpSelesai.Size = new System.Drawing.Size(200, 20);
            this.dtpSelesai.TabIndex = 2;
            // 
            // btnTampilkan
            // 
            this.btnTampilkan.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnTampilkan.Location = new System.Drawing.Point(626, 112);
            this.btnTampilkan.Name = "btnTampilkan";
            this.btnTampilkan.Size = new System.Drawing.Size(122, 46);
            this.btnTampilkan.TabIndex = 3;
            this.btnTampilkan.Text = "Tampilkan Grafik";
            this.btnTampilkan.UseVisualStyleBackColor = false;
            this.btnTampilkan.Click += new System.EventHandler(this.btnTampilkan_Click_1);
            // 
            // chartRekap
            // 
            chartArea4.Name = "ChartArea1";
            this.chartRekap.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartRekap.Legends.Add(legend4);
            this.chartRekap.Location = new System.Drawing.Point(128, 187);
            this.chartRekap.Name = "chartRekap";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartRekap.Series.Add(series4);
            this.chartRekap.Size = new System.Drawing.Size(636, 344);
            this.chartRekap.TabIndex = 4;
            this.chartRekap.Text = "chart1";
            this.chartRekap.Click += new System.EventHandler(this.chartRekap_Click);
            // 
            // btnKembali
            // 
            this.btnKembali.Location = new System.Drawing.Point(689, 550);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(75, 23);
            this.btnKembali.TabIndex = 5;
            this.btnKembali.Text = "Kembali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click_1);
            // 
            // IDdosen
            // 
            this.IDdosen.AutoSize = true;
            this.IDdosen.Location = new System.Drawing.Point(231, 37);
            this.IDdosen.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.IDdosen.Name = "IDdosen";
            this.IDdosen.Size = new System.Drawing.Size(100, 13);
            this.IDdosen.TabIndex = 6;
            this.IDdosen.Text = "Nama Mata Kuliah :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(257, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Dari Tanggal :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(244, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Sampai dengan :";
            // 
            // FormRekapChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 595);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}