using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Penting untuk Chart

namespace projectsem4
{
    public partial class FormRekapChart : Form
    {
        // Menggunakan class Koneksi untuk koneksi dinamis
        private Koneksi koneksi = new Koneksi();
        private string connectionString;

        public FormRekapChart()
        {
            InitializeComponent();
            connectionString = koneksi.GetConnectionString();
        }

        

        /// <summary>
        /// Mengisi ComboBox dengan daftar mata kuliah.
        /// </summary>
        private void LoadComboBoxMataKuliah()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Mengambil kode dan nama mata kuliah untuk ComboBox
                    SqlCommand cmd = new SqlCommand("SELECT kode_mk, nama_mk FROM MataKuliah ORDER BY nama_mk", conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cbMataKuliah.DataSource = dt;
                    cbMataKuliah.DisplayMember = "nama_mk";  // Yang ditampilkan ke pengguna
                    cbMataKuliah.ValueMember = "kode_mk";    // Nilai yang digunakan di belakang layar
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error memuat data Mata Kuliah: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Event handler yang berjalan saat tombol "Tampilkan Grafik" diklik.
        /// </summary>
       

        private void btnTampilkan_Click_1(object sender, EventArgs e)
        {
            if (cbMataKuliah.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih mata kuliah terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string kodeMK = cbMataKuliah.SelectedValue.ToString();
            DateTime tanggalMulai = dtpMulai.Value.Date;
            DateTime tanggalSelesai = dtpSelesai.Value.Date;

            AnalyzeRekapQuery(kodeMK, tanggalMulai, tanggalSelesai);

            // Mengosongkan chart sebelum diisi data baru
            chartRekap.Series.Clear();
            Series series = new Series("Jumlah Presensi")
            {
                ChartType = SeriesChartType.Column, // Jenis grafik (bisa diubah ke Pie, Bar, dll.)
                Font = new Font("Segoe UI", 9)
            };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // Memanggil stored procedure yang sudah ada di database Anda
                    SqlCommand cmd = new SqlCommand("GetRekapPresensiByMataKuliah", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@kode_mk", kodeMK);
                    cmd.Parameters.AddWithValue("@tanggal_mulai", tanggalMulai);
                    cmd.Parameters.AddWithValue("@tanggal_selesai", tanggalSelesai);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Mengisi data dari hasil query ke dalam chart
                    while (reader.Read())
                    {
                        string status = reader["status"].ToString();
                        int jumlah = Convert.ToInt32(reader["jumlah"]);
                        series.Points.AddXY(status, jumlah);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error menampilkan grafik: " + ex.Message);
                }
            }

            chartRekap.Series.Add(series);

            // Menambahkan gaya dan judul pada chart
            chartRekap.ChartAreas[0].AxisX.Title = "Status Kehadiran";
            chartRekap.ChartAreas[0].AxisY.Title = "Jumlah Mahasiswa";
            chartRekap.ChartAreas[0].AxisY.Interval = 1; // Interval sumbu Y
            chartRekap.Legends.Clear(); // Menghilangkan legenda jika tidak perlu
        
        }
        private void AnalyzeRekapQuery(string kodeMK, DateTime tanggalMulai, DateTime tanggalSelesai)
        {
            string sql = $@"
            SET STATISTICS IO ON;
            SET STATISTICS TIME ON;

            SELECT p.status, COUNT(*) AS jumlah
            FROM Presensi p
            JOIN JadwalKuliah j ON p.id_jadwal = j.id_jadwal
            WHERE j.kode_mk = '{kodeMK}'
            AND p.tanggal BETWEEN '{tanggalMulai:yyyy-MM-dd}' AND '{tanggalSelesai:yyyy-MM-dd}'
            GROUP BY p.status;

            SET STATISTICS IO OFF;
            SET STATISTICS TIME OFF;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    // --- PERUBAHAN UTAMA DI SINI ---
                    StringBuilder allMessages = new StringBuilder();
                    conn.InfoMessage += (s, e) => {
                        allMessages.AppendLine(e.Message);
                    };

                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Setelah query selesai, proses pesan yang terkumpul
                    if (allMessages.Length > 0)
                    {
                        string rawMessage = allMessages.ToString();
                        string elapsedTime = "Tidak terdeteksi";

                        // Mencari baris yang mengandung 'elapsed time'
                        var timeLines = rawMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Where(line => line.Contains("elapsed time"));

                        foreach (var line in timeLines)
                        {
                            var parts = line.Split(',');
                            foreach (var part in parts)
                            {
                                if (part.Trim().StartsWith("elapsed time"))
                                {
                                    // Mengambil angka dari string "  elapsed time = 15 ms."
                                    var timeValue = System.Text.RegularExpressions.Regex.Match(part, @"\d+").Value;
                                    if (!string.IsNullOrEmpty(timeValue))
                                    {
                                        elapsedTime = $"{timeValue} milidetik";
                                        break;
                                    }
                                }
                            }
                            if (elapsedTime != "Tidak terdeteksi") break;
                        }

                        // Membuat dan menampilkan pesan yang ramah pengguna
                        string friendlyMessage = $"Analisis performa selesai.\n\n" +
                                                 $"Waktu pemrosesan data: {elapsedTime}";

                        MessageBox.Show(friendlyMessage, "Informasi Performa Sistem");
                    }
                    // --- AKHIR DARI PERUBAHAN ---
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Analisis Query Error: " + ex.Message);
                }
            }
        }
        private void EnsurePresensiIndexes()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string script = @"
                    IF OBJECT_ID('dbo.Presensi', 'U') IS NOT NULL
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'idx_Presensi_idjadwal_tanggal_status' 
                            AND object_id = OBJECT_ID('dbo.Presensi'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX idx_Presensi_idjadwal_tanggal_status 
                            ON dbo.Presensi (id_jadwal, tanggal, status);
                        END
                    END

                    IF OBJECT_ID('dbo.JadwalKuliah', 'U') IS NOT NULL
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'idx_JadwalKuliah_kodeMK' 
                            AND object_id = OBJECT_ID('dbo.JadwalKuliah'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX idx_JadwalKuliah_kodeMK 
                            ON dbo.JadwalKuliah (kode_mk);
                        END
                    END";
                    using (SqlCommand cmd = new SqlCommand(script, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ensure Indexes Error: " + ex.Message);
                }
            }
        }
        private void chartRekap_Click(object sender, EventArgs e)
        {

        }

        private void FormRekapChart_Load_1(object sender, EventArgs e)
        {
            // Memuat data awal saat form dibuka
            LoadComboBoxMataKuliah();
            EnsurePresensiIndexes();
            // Mengatur tanggal default
            dtpMulai.Value = DateTime.Today.AddDays(-30); // Default 30 hari terakhir
            dtpSelesai.Value = DateTime.Today;

            // Mengosongkan chart pada awalnya
            chartRekap.Series.Clear();
            chartRekap.Titles.Add("Grafik Rekap Presensi Mahasiswa");
        }

        private void btnKembali_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }
    }
}
