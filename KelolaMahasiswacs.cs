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

        private readonly string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";

        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly CacheItemPolicy _policy = new CacheItemPolicy 
        { 
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5) 
        };
        private const string CacheKey = "MahasiswaData";
        public KelolaMahasiswacs()
        {
            InitializeComponent();
            this.Load += KelolaMahasiswacs_Load;
        }

        private void KelolaMahasiswacs_Load(object sender, EventArgs e)
        {
            EnsureIndexes();
            LoadData();
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
            using (var conn = new SqlConnection(connectionString))
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
                using (var conn = new SqlConnection(connectionString))
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
            using (var conn = new SqlConnection(connectionString))
            {
                conn.InfoMessage += (s, e) => MessageBox.Show(e.Message, "STATISTICS INFO");
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
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errorMessages = new StringBuilder();

            if (string.IsNullOrWhiteSpace(txtNIM.Text))
                errorMessages.AppendLine("NIM tidak boleh kosong.");
            else if (!txtNIM.Text.All(char.IsDigit))
                errorMessages.AppendLine("NIM hanya boleh terdiri dari angka.");
            else if (txtNIM.Text.Length != 11)
                errorMessages.AppendLine("NIM harus terdiri dari 11 digit.");


            if (string.IsNullOrWhiteSpace(txtNamamhs.Text))
                errorMessages.AppendLine("Nama mahasiswa tidak boleh kosong.");
            else if (!txtNamamhs.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                errorMessages.AppendLine("Nama hanya boleh mengandung huruf dan spasi.");

            if (string.IsNullOrWhiteSpace(txtKelas.Text))
                errorMessages.AppendLine("Kelas tidak boleh kosong.");

            if (!int.TryParse(txtAngkatan.Text, out int angkatan))
                errorMessages.AppendLine("Angkatan harus berupa angka.");
            else if (angkatan < 2000 || angkatan > DateTime.Now.Year)
                errorMessages.AppendLine("Angkatan harus 4 karakter.");

            if (!int.TryParse(txtSemester.Text, out int semester))
                errorMessages.AppendLine("Semester harus berupa angka.");
            else if (semester < 1 || semester > 14)
                errorMessages.AppendLine("Semester harus antara 1 sampai 14.");

            if (errorMessages.Length > 0)
            {
                MessageBox.Show(errorMessages.ToString(), "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnTambahMhs(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin menambahkan data ini?", "Konfirmasi Tambah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                using (var conn = new SqlConnection(connectionString))
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
        }

        private void btnUbahMhs(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            DialogResult result = MessageBox.Show("Apakah Anda yakin ingin mengubah data ini?", "Konfirmasi Ubah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;


            try
            {
                using (var conn = new SqlConnection(connectionString))
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

            using (SqlConnection conn = new SqlConnection(connectionString))
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