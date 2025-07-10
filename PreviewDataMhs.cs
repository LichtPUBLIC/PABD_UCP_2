   using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projectsem4
{
    public partial class PreviewDataMhs: Form
    {
        Koneksi kn = new Koneksi();
        string connect = "";

        public PreviewDataMhs(DataTable data)
        {
            InitializeComponent(); 
            connect = kn.connectionString(); 
            dgvPreviewDataMhs.DataSource = data;
        }

        private bool NimExists(string nim, SqlConnection conn, SqlTransaction transaction)
        {
            // Query untuk mengecek apakah NIM sudah ada di database
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Mahasiswa WHERE nim = @nim", conn, transaction);
            cmd.Parameters.AddWithValue("@nim", nim);

            // Mengembalikan true jika jumlahnya > 0 (artinya NIM sudah ada)
            return (int)cmd.ExecuteScalar() > 0;
        }
        // --- GANTI SELURUH FUNGSI INI ---
        private bool ValidateRow(DataRow row, int rowIndex)
        {
            StringBuilder errorMessages = new StringBuilder();
            string nim = row["nim"]?.ToString() ?? "";
            string nama = row["nama_mhs"]?.ToString() ?? "";
            string kelas = row["kelas"]?.ToString() ?? "";
            string angkatanStr = row["angkatan"]?.ToString() ?? "";
            string semesterStr = row["semester"]?.ToString() ?? "";

            // Validasi NIM
            if (string.IsNullOrWhiteSpace(nim) || nim.Length != 11 || !nim.All(char.IsDigit))
                errorMessages.AppendLine("• NIM tidak valid (harus 11 digit angka).");

            // Validasi lainnya
            if (string.IsNullOrWhiteSpace(nama))
                errorMessages.AppendLine("• Nama Mahasiswa tidak boleh kosong.");

            if (string.IsNullOrWhiteSpace(kelas))
                errorMessages.AppendLine("• Kelas tidak boleh kosong.");

            if (string.IsNullOrWhiteSpace(angkatanStr) || !int.TryParse(angkatanStr, out _) || angkatanStr.Length != 4)
                errorMessages.AppendLine("• Angkatan harus 4 digit angka (misal: 2023).");

            if (string.IsNullOrWhiteSpace(semesterStr) || !int.TryParse(semesterStr, out int semester) || semester < 1 || semester > 14)
                errorMessages.AppendLine("• Semester harus angka antara 1-14.");

            if (errorMessages.Length > 0)
            {
                MessageBox.Show($"Kesalahan validasi pada baris ke-{rowIndex + 1} (NIM: {nim}):\n\n{errorMessages}",
                                "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ImportDataToDatabase()
        {
            DataTable dt = (DataTable)dgvPreviewDataMhs.DataSource;
            int successCount = 0;
            int skippedCount = 0;
            StringBuilder skippedNims = new StringBuilder();

            using (SqlConnection conn = new SqlConnection(connect))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try // <-- Hanya satu blok 'try' ini yang kita butuhkan
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string nimToImport = row["nim"]?.ToString() ?? "";

                        if (!ValidateRow(row, i))
                        {
                            skippedCount++;
                            skippedNims.AppendLine($"- {nimToImport} (Format data salah)");
                            continue;
                        }

                        if (NimExists(nimToImport, conn, transaction))
                        {
                            skippedCount++;
                            skippedNims.AppendLine($"- {nimToImport} (NIM sudah terdaftar)");
                            continue;
                        }

                        string query = "INSERT INTO Mahasiswa (nim, nama_mhs, kelas, angkatan, semester) VALUES (@nim, @nama_mhs, @kelas, @angkatan, @semester)";
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@nim", nimToImport);
                            cmd.Parameters.AddWithValue("@nama_mhs", row["nama_mhs"].ToString());
                            cmd.Parameters.AddWithValue("@kelas", row["kelas"].ToString());
                            cmd.Parameters.AddWithValue("@angkatan", Convert.ToInt32(row["angkatan"]));
                            cmd.Parameters.AddWithValue("@semester", Convert.ToInt32(row["semester"]));
                            cmd.ExecuteNonQuery();
                            successCount++;
                        }
                    }

                    transaction.Commit();

                    StringBuilder summary = new StringBuilder();
                    summary.AppendLine("Proses impor selesai.");
                    summary.AppendLine($"✅ {successCount} data berhasil diimpor.");
                    if (skippedCount > 0)
                    {
                        summary.AppendLine($"⚠️ {skippedCount} data dilewati karena duplikat atau format salah.");
                        summary.AppendLine("\nNIM yang dilewati:");
                        summary.Append(skippedNims.ToString());
                    }
                    MessageBox.Show(summary.ToString(), "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Terjadi kesalahan fatal saat mengimpor data, semua perubahan telah dibatalkan. \n\nError: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }      
    
        private void dgvPreview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void btnOkePreview(object sender, EventArgs e)
        {
            // Menanyakan kepada pengguna jika mereka ingin mengimpor data
            DialogResult result = MessageBox.Show("Apakah Anda ingin mengimpor data ini ke database?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                ImportDataToDatabase();
            }
        }

        private void PreviewDataMhs_Load(object sender, EventArgs e)
        {

        }
    }
}
