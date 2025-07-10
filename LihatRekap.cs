using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace projectsem4
{
    public partial class LihatRekap : Form
    {
        // --- PERUBAHAN 1: Deklarasikan class Koneksi ---
        Koneksi kn = new Koneksi();
        string connect = "";

        public LihatRekap()
        {
            InitializeComponent();
            connect = kn.connectionString();
        }

        private void LihatRekap_Load(object sender, EventArgs e)
        {
            // Mempersiapkan form saat pertama kali dibuka
            EnsureRekapIndexes();
            LoadComboBoxMataKuliah();
            // Memuat laporan dengan menampilkan semua data pada awalnya
            if (cmbMataKuliah.SelectedItem != null)
            {
                LoadReportData(cmbMataKuliah.SelectedItem.ToString());
            }
        }

        /// <summary>
        /// Mengisi ComboBox dengan daftar mata kuliah dari database.
        /// </summary>
        private void LoadComboBoxMataKuliah()
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                try
                {
                    conn.Open();
                    // Mengambil nama mata kuliah yang unik untuk dijadikan pilihan filter
                    string query = "SELECT DISTINCT nama_mk FROM MataKuliah ORDER BY nama_mk";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbMataKuliah.Items.Clear();
                    // Menambahkan opsi untuk menampilkan semua data
                    cmbMataKuliah.Items.Add("-- Semua Matkul --");

                    while (reader.Read())
                    {
                        cmbMataKuliah.Items.Add(reader["nama_mk"].ToString());
                    }

                    // Mengatur pilihan default
                    cmbMataKuliah.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error memuat data Mata Kuliah: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Event handler yang berjalan setiap kali pilihan di ComboBox berubah.
        /// </summary>
        private void cmbMataKuliah_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMataKuliah.SelectedItem != null)
            {
                string selectedMK = cmbMataKuliah.SelectedItem.ToString();
                // Menganalisis performa query (opsional, dari kode teman Anda)
                AnalyzeLaporanQuery(selectedMK);
                // Memuat ulang data laporan sesuai dengan filter yang dipilih
                LoadReportData(selectedMK);
            }
        }

        /// <summary>
        /// Metode utama untuk memuat data ke ReportViewer berdasarkan filter mata kuliah.
        /// </summary>
        /// <param name="selectedMK">Nama mata kuliah yang dipilih untuk filter.</param>
        private void LoadReportData(string selectedMK)
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(@"
                SELECT 
                    p.id_presensi, m.nim, m.nama_mhs, m.kelas,
                    mk.nama_mk, d.nama_dosen, jk.hari,
                    jk.jam_mulai, jk.jam_selesai, p.tanggal, p.status
                FROM 
                    Presensi p
                JOIN Mahasiswa m ON p.nim = m.nim
                JOIN JadwalKuliah jk ON p.id_jadwal = jk.id_jadwal
                JOIN MataKuliah mk ON jk.kode_mk = mk.kode_mk
                JOIN Dosen d ON mk.id_dosen = d.id_dosen
            ");

            // Menambahkan klausa WHERE jika mata kuliah spesifik dipilih
            if (!string.IsNullOrEmpty(selectedMK) && selectedMK != "-- Semua Matkul --")
            {
                queryBuilder.Append("WHERE mk.nama_mk = @nama_mk ");
            }

            queryBuilder.Append("ORDER BY p.tanggal DESC, m.nama_mhs");

            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                {
                    if (!string.IsNullOrEmpty(selectedMK) && selectedMK != "-- Semua Matkul --")
                    {
                        cmd.Parameters.AddWithValue("@nama_mk", selectedMK);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                // Mengatur sumber data untuk ReportViewer
                ReportDataSource rds = new ReportDataSource("DataSet1", dt); // Pastikan "DataSet1" sesuai dengan nama DataSet di file .rdlc Anda
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(rds);

                // --- PERUBAHAN 3: Menggunakan path relatif untuk file laporan ---
                // Ini membuat aplikasi portabel dan bisa dijalankan dari mana saja.
                // Pastikan file 'PresensiReport.rdlc' ada di folder output (bin\Release atau bin\Debug).
                string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PresensiReport.rdlc");
                reportViewer1.LocalReport.ReportPath = reportPath;

                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error memuat laporan: " + ex.Message);
            }
        }

        // Nama metode diubah agar cocok dengan file Designer untuk memperbaiki error
        private void btnKembali(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }
        private string SummarizeSqlAnalysis(string raw)
        {
            var lines = raw
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            var summary = new StringBuilder();
            bool cpuTimeCaptured = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("Table"))
                {
                    var parts = line.Split(new[] { '\'' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        string tableName = parts[1];

                        int logicalReads = ExtractValue(line, "logical reads");
                        int physicalReads = ExtractValue(line, "physical reads");

                        summary.AppendLine($"{tableName} → logical reads: {logicalReads}, physical reads: {physicalReads}");
                    }
                }
                else if (line.StartsWith("SQL Server Execution Times"))
                {
                    summary.AppendLine(line);
                }
                else if ((line.Trim().StartsWith("CPU time") || line.Trim().StartsWith("elapsed time")) && !cpuTimeCaptured)
                {
                    summary.AppendLine(line.Trim());
                    cpuTimeCaptured = true;
                }
            }

            return summary.ToString();
        }



        private int ExtractValue(string line, string keyword)
        {
            int result = 0;
            try
            {
                int idx = line.IndexOf(keyword);
                if (idx >= 0)
                {
                    string temp = line.Substring(idx + keyword.Length).TrimStart(' ', ':');
                    string numberStr = temp.Split(',')[0];
                    int.TryParse(numberStr, out result);
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }


        /// <summary>
        /// Menganalisis performa query laporan (diambil dari kode teman Anda).
        /// </summary>
        private void AnalyzeLaporanQuery(string namaMK)
        {
            string sql = @"
    SET STATISTICS IO ON;
    SET STATISTICS TIME ON;

    SELECT 
        p.id_presensi, m.nim, m.nama_mhs, m.kelas, mk.nama_mk, d.nama_dosen,
        jk.hari, jk.jam_mulai, jk.jam_selesai, p.tanggal, p.status
    FROM 
        Presensi p
    JOIN Mahasiswa m ON p.nim = m.nim
    JOIN JadwalKuliah jk ON p.id_jadwal = jk.id_jadwal
    JOIN MataKuliah mk ON jk.kode_mk = mk.kode_mk
    JOIN Dosen d ON mk.id_dosen = d.id_dosen
    WHERE (@nama_mk IS NULL OR mk.nama_mk = @nama_mk)
    ORDER BY p.tanggal DESC, m.nama_mhs;

    SET STATISTICS IO OFF;
    SET STATISTICS TIME OFF;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    List<string> messageBlocks = new List<string>();
                    StringBuilder currentBlock = new StringBuilder();

                    conn.InfoMessage += (s, e) =>
                    {
                        if (e.Message.StartsWith("Table") ||
                            e.Message.StartsWith("SQL Server Execution Times") ||
                            e.Message.Trim().StartsWith("CPU time") ||
                            e.Message.Trim().StartsWith("elapsed time"))
                        {
                            currentBlock.AppendLine(e.Message);
                        }
                    };

                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama_mk", string.IsNullOrEmpty(namaMK) || namaMK == "-- Semua Matkul --" ? DBNull.Value : (object)namaMK);
                        cmd.ExecuteNonQuery();
                    }

                    // proses ringkas jika ada pesan
                    if (currentBlock.Length > 0)
                    {
                        string summary = SummarizeSqlAnalysis(currentBlock.ToString());

                        string friendlyExplanation =
                            "📊 **Analisis Performa Query**\n\n" +
                            "• Logical Reads → Data berhasil dibaca langsung dari memori. Ini bagus karena lebih cepat.\n" +
                            "• Physical Reads → Jika nilainya 0, artinya tidak membaca dari hard disk. Ini juga bagus.\n" +
                            "• CPU Time → Waktu kerja prosesor untuk menjalankan query.\n" +
                            "• Elapsed Time → Lama waktu dari awal sampai selesai.\n\n" +
                            "📝 Rincian teknis (ringkas):\n" +
                            summary;

                        MessageBox.Show(friendlyExplanation, "Hasil Analisis Query (Bahasa Sederhana)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Analisis Query Error: " + ex.Message);
            }

        }

        /// <summary>
        /// Memastikan indeks yang diperlukan untuk query laporan sudah ada di database (diambil dari kode teman Anda).
        /// </summary>
        private void EnsureRekapIndexes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    conn.Open();
                    string indexScript = @"
                    IF OBJECT_ID('dbo.MataKuliah', 'U') IS NOT NULL
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'idx_MataKuliah_namaMK' AND object_id = OBJECT_ID('dbo.MataKuliah'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX idx_MataKuliah_namaMK ON dbo.MataKuliah(nama_mk);
                        END
                    END

                    IF OBJECT_ID('dbo.Presensi', 'U') IS NOT NULL
                    BEGIN
                        IF NOT EXISTS (
                            SELECT 1 FROM sys.indexes 
                            WHERE name = 'idx_Presensi_Tanggal' AND object_id = OBJECT_ID('dbo.Presensi'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX idx_Presensi_Tanggal ON dbo.Presensi(tanggal);
                        END
                    END";
                    using (SqlCommand cmd = new SqlCommand(indexScript, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ensure Indexes Error: " + ex.Message);
            }
        }
    }
}
