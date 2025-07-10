using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projectsem4
{
    public partial class KelolaMahasiswacs : Form
    {
        Koneksi kn = new Koneksi();
        string connect = "";
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly CacheItemPolicy _policy = new CacheItemPolicy 
        { 
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) 
        };
        private const string CacheKey = "MahasiswaData";
        public KelolaMahasiswacs()
        {
            InitializeComponent();
            connect = kn.connectionString();
            this.Load += KelolaMahasiswacs_Load;
        }

        private void KelolaMahasiswacs_Load(object sender, EventArgs e)
        {
            EnsureIndexes();
            
        }

        private void ClearForm()
        {
            txtNIM.Clear();
            txtNamamhs.Clear();
            txtKelas.Clear();
            txtAngkatan.Clear();
            txtSemester.Clear();
            txtNIM.Focus();
            dgvMahasiswa.ClearSelection();
        }

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connect))
            {
                conn.Open();
                var indexScript = @"
                    IF OBJECT_ID('dbo.Mahasiswa', 'U') IS NOT NULL
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_Mahasiswa_nama_mhs' AND object_id = OBJECT_ID('dbo.Mahasiswa'))
                            CREATE NONCLUSTERED INDEX idx_Mahasiswa_nama_mhs ON dbo.Mahasiswa(nama_mhs);
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_Mahasiswa_angkatan' AND object_id = OBJECT_ID('dbo.Mahasiswa'))
                            CREATE NONCLUSTERED INDEX idx_Mahasiswa_angkatan ON dbo.Mahasiswa(angkatan);
                        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_Mahasiswa_semester' AND object_id = OBJECT_ID('dbo.Mahasiswa'))
                            CREATE NONCLUSTERED INDEX idx_Mahasiswa_semester ON dbo.Mahasiswa(semester);
                    END";
                using (var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void LoadData()
        {
            DataTable dt;
            if (_cache.Contains(CacheKey))
            {
                dt = _cache.Get(CacheKey) as DataTable;
            }
            else
            {
                dt = new DataTable();
                using (var conn = new SqlConnection(connect))
                {
                    conn.Open();
                    string query = "SELECT nim, nama_mhs, kelas, angkatan, semester FROM dbo.Mahasiswa";
                    using (var da = new SqlDataAdapter(query, conn))
                    {
                        da.Fill(dt);
                    }
                }
                _cache.Add(CacheKey, dt, _policy);
            }

            dgvMahasiswa.AutoGenerateColumns = true;
            dgvMahasiswa.DataSource = dt;
        }

        private void AnalyzeQuery(string sqlQuery)
        {
            using (var conn = new SqlConnection(connect))
            {
                // Menggunakan StringBuilder untuk mengumpulkan semua pesan dari event InfoMessage.
                StringBuilder allMessages = new StringBuilder();
                conn.InfoMessage += (s, e) => {
                    allMessages.AppendLine(e.Message);
                };

                conn.Open();
                var wrapped = $@"
                SET STATISTICS IO ON;
                SET STATISTICS TIME ON;
                {sqlQuery};
                SET STATISTICS IO OFF;
                SET STATISTICS TIME OFF;";

                using (var cmd = new SqlCommand(wrapped, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Setelah perintah dieksekusi, proses pesan yang terkumpul.
                if (allMessages.Length > 0)
                {
                    string rawMessage = allMessages.ToString();
                    string logicalReads = "N/A";
                    string elapsedTime = "N/A";

                    // Membagi pesan menjadi baris-baris terpisah untuk diproses.
                    var lines = rawMessage.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    // Mencari baris yang berisi "logical reads".
                    var ioLine = lines.FirstOrDefault(line => line.Contains("logical reads"));
                    if (ioLine != null)
                    {
                        // Mengambil angka dari string "logical reads 9".
                        var match = System.Text.RegularExpressions.Regex.Match(ioLine, @"logical reads (\d+)");
                        if (match.Success)
                        {
                            logicalReads = match.Groups[1].Value;
                        }
                    }

                    // Mencari baris yang berisi "elapsed time".
                    var timeLine = lines.FirstOrDefault(line => line.Contains("elapsed time"));
                    if (timeLine != null)
                    {
                        // Mengambil angka dari string "  elapsed time = 0 ms.".
                        var match = System.Text.RegularExpressions.Regex.Match(timeLine, @"elapsed time = (\d+)");
                        if (match.Success)
                        {
                            elapsedTime = $"{match.Groups[1].Value} milidetik";
                        }
                    }

                    // Membuat pesan yang ramah pengguna.
                    string friendlyMessage = "Hasil Analisis Performa Query:\n\n" +
                                             $"• Efisiensi Baca Data (Logical Reads): {logicalReads}\n" +
                                             $"• Waktu Eksekusi: {elapsedTime}\n\n" +
                                             "(Semakin kecil angkanya, semakin cepat dan efisien prosesnya)";

                    MessageBox.Show(friendlyMessage, "Info Performa Query", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errorMessages = new StringBuilder();

            // Validasi NIM
            if (string.IsNullOrWhiteSpace(txtNIM.Text))
                errorMessages.AppendLine("• NIM tidak boleh kosong.");
            else if (!txtNIM.Text.All(char.IsDigit))
                errorMessages.AppendLine("• NIM hanya boleh terdiri dari angka.");
            else if (txtNIM.Text.Length != 11)
                errorMessages.AppendLine("• NIM harus terdiri dari 11 digit.");

            // Validasi Nama Mahasiswa
            if (string.IsNullOrWhiteSpace(txtNamamhs.Text))
                errorMessages.AppendLine("• Nama mahasiswa tidak boleh kosong.");
            else if (!txtNamamhs.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                errorMessages.AppendLine("• Nama hanya boleh mengandung huruf dan spasi.");

            // Validasi Kelas
            if (string.IsNullOrWhiteSpace(txtKelas.Text))
                errorMessages.AppendLine("• Kelas tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtKelas.Text, @"^[A-Z]$"))
                errorMessages.AppendLine("• Kelas hanya boleh diisi dengan satu huruf kapital (A, B, C, dst.).");

            // --- PERUBAHAN VALIDASI ANGKATAN DIMULAI DI SINI ---
            if (string.IsNullOrWhiteSpace(txtAngkatan.Text))
            {
                errorMessages.AppendLine("• Angkatan tidak boleh kosong.");
            }
            else if (!int.TryParse(txtAngkatan.Text, out int angkatan))
            {
                errorMessages.AppendLine("• Angkatan harus berupa angka.");
            }
            else if (txtAngkatan.Text.Length != 4)
            {
                errorMessages.AppendLine("• Angkatan harus 4 karakter (contoh: 2023).");
            }
            // --- AKHIR VALIDASI ANGKATAN ---

            // --- PERUBAHAN VALIDASI SEMESTER DIMULAI DI SINI ---
            if (string.IsNullOrWhiteSpace(txtSemester.Text))
            {
                errorMessages.AppendLine("• Semester tidak boleh kosong.");
            }
            else if (!int.TryParse(txtSemester.Text, out int semester))
            {
                errorMessages.AppendLine("• Semester harus berupa angka.");
            }
            else if (semester < 1 || semester > 14)
            {
                errorMessages.AppendLine("• Semester harus antara 1 sampai 14.");
            }
            // --- AKHIR VALIDASI SEMESTER ---

            // Tampilkan semua pesan error jika ada
            if (errorMessages.Length > 0)
            {
                MessageBox.Show("Terjadi kesalahan validasi:\n\n" + errorMessages.ToString(),
                                "Validasi Gagal",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool NimExists(string nim)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                // Query untuk menghitung jumlah baris dengan NIM yang sama
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Mahasiswa WHERE nim = @nim", conn);
                cmd.Parameters.AddWithValue("@nim", nim);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                // Jika count > 0, berarti NIM sudah ada
                return count > 0;
            }
        }

        private void btnTambahMhs(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            if (NimExists(txtNIM.Text.Trim()))
            {
                MessageBox.Show("NIM " + txtNIM.Text + " sudah terdaftar. Silakan gunakan NIM lain.",
                                "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Hentikan proses jika NIM sudah ada
            }
            // --- AKHIR DARI VALIDASI DATA DUPLIKAT ---

            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin menambahkan data ini?", "Konfirmasi Tambah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                using (var conn = new SqlConnection(connect))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("AddMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nim", txtNIM.Text.Trim());
                        cmd.Parameters.AddWithValue("@nama_mhs", txtNamamhs.Text.Trim());
                        cmd.Parameters.AddWithValue("@kelas", txtKelas.Text.Trim());
                        cmd.Parameters.AddWithValue("@angkatan", int.Parse(txtAngkatan.Text.Trim()));
                        cmd.Parameters.AddWithValue("@semester", int.Parse(txtSemester.Text.Trim()));
                        cmd.ExecuteNonQuery();
                    }

                    _cache.Remove(CacheKey);
                    lblMessage.Text = "Data berhasil ditambahkan!";
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }
        }

        private void btnRefreshMhs(object sender, EventArgs e)
        {
            _cache.Remove(CacheKey);
            LoadData();
            MessageBox.Show("Tampilan data mahasiswa berhasil diperbarui.", "Refresh Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUbahMhs(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin mengubah data ini?", "Konfirmasi Ubah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;


            try
            {
                using (var conn = new SqlConnection(connect))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nim", txtNIM.Text.Trim());
                        cmd.Parameters.AddWithValue("@nama_mhs", txtNamamhs.Text.Trim());
                        cmd.Parameters.AddWithValue("@kelas", txtKelas.Text.Trim());
                        cmd.Parameters.AddWithValue("@angkatan", int.Parse(txtAngkatan.Text.Trim()));
                        cmd.Parameters.AddWithValue("@semester", int.Parse(txtSemester.Text.Trim()));
                        cmd.ExecuteNonQuery();
                    }

                    _cache.Remove(CacheKey);
                    lblMessage.Text = "Data berhasil diubah!";
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
            }

        }

        private void btnHapusMhs(object sender, EventArgs e)
        {
            if (dgvMahasiswa.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih data yang akan dihapus.");
                return;
            }

            string nim = dgvMahasiswa.SelectedRows[0].Cells["NIM"].Value.ToString();

            DialogResult result = MessageBox.Show("Yakin ingin menghapus data NIM: " + nim + "?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(connect))
            {
                SqlTransaction transaction = null;

                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    using (SqlCommand cmd = new SqlCommand("DeleteMahasiswaWithPresensi", conn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nim", nim);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    _cache.Remove(CacheKey);
                    lblMessage.Text = "Data berhasil dihapus!";
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    lblMessage.Text = "Error saat menghapus: " + ex.Message;
                }
            }
        }


        private void PreviewData(string filePath)
        {
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs); // Membuka workbook Excel
                    ISheet sheet = workbook.GetSheetAt(0);     // Mendapatkan worksheet pertama
                    DataTable dt = new DataTable();

                    // Membaca header kolom
                    IRow headerRow = sheet.GetRow(0);
                    foreach (var cell in headerRow.Cells)
                    {
                        dt.Columns.Add(cell.ToString());
                    }

                    // Membaca sisa data
                    for (int i = 1; i <= sheet.LastRowNum; i++) // Lewati baris header
                    {
                        IRow dataRow = sheet.GetRow(i);
                        DataRow newRow = dt.NewRow();
                        int cellIndex = 0;

                        foreach (var cell in dataRow.Cells)
                        {
                            newRow[cellIndex++] = cell.ToString();

                        }
                        dt.Rows.Add(newRow);
                    }

                    // Membuka PreviewForm dan mengirimkan DataTable ke form tersebut
                    PreviewDataMhs FormPreviewDataMhs = new PreviewDataMhs(dt);
                    FormPreviewDataMhs.ShowDialog(); // Tampilkan PreviewForm
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading the Excel file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void btnKembalimhs(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void dgvMahasiswa_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvMahasiswa.Rows[e.RowIndex];
            txtNIM.Text = row.Cells[0].Value?.ToString() ?? string.Empty;
            txtNamamhs.Text = row.Cells[1].Value?.ToString() ?? string.Empty;
            txtKelas.Text = row.Cells[2].Value?.ToString() ?? string.Empty;
            txtAngkatan.Text = row.Cells[3].Value?.ToString() ?? string.Empty;
            txtSemester.Text = row.Cells[4].Value?.ToString() ?? string.Empty;
        }

        private void btnImportData(object sender, EventArgs e)
        {
            using (var openFile = new OpenFileDialog())
            {
                openFile.Filter = "Excel Files |*.xlsx;*.xlsm";
                if (openFile.ShowDialog() == DialogResult.OK)
                    PreviewData(openFile.FileName);
            }
        }

        private void dgvAnalisisMhs(object sender, EventArgs e)
        {
            var heavyQuery = "SELECT * FROM Mahasiswa WHERE nama_mhs LIKE 'A%'";
            AnalyzeQuery(heavyQuery);
        }

        private void txtNIM_TextChanged(object sender, EventArgs e)
        {

        }

    }
}