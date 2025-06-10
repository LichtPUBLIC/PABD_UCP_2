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
        private string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";

        public PreviewDataMhs(DataTable data)
        {
            InitializeComponent();
            dgvPreviewDataMhs.DataSource = data;
        }

        private bool ValidateRow(DataRow row)
        {
            string nim = row["NIM"].ToString();

            // Validasi NIM (misalnya, harus berjumlah 11 karakter)
            if (nim.Length != 11)
            {
                MessageBox.Show("NIM harus terdiri dari 11 karakter.", "Kesalahan Validasi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Jika perlu, tambahkan validasi lain sesuai dengan kebutuhan (misalnya, pola tertentu untuk NIM)
            return true;
        }

        private void ImportDataToDatabase()
        {
            try
            {
                DataTable dt = (DataTable)dgvPreviewDataMhs.DataSource;

                foreach (DataRow row in dt.Rows)
                {
                    // Validasi setiap baris sebelum diimpor
                    if (!ValidateRow(row))
                    {
                        //Jika validasi gagal, lanjutkan ke baris berikutnya
                        continue; // Lewati baris ini jika tidak valid
                    }

                    string query = "INSERT INTO Mahasiswa (nim, nama_mhs, kelas, angkatan, semester) VALUES (@nim, @nama, @kelas, @angkatan, @semester)";
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@nim", row["nim"]);
                            cmd.Parameters.AddWithValue("@nama", row["nama_mhs"]);
                            cmd.Parameters.AddWithValue("@kelas", row["kelas"]);
                            cmd.Parameters.AddWithValue("@angkatan", row["angkatan"]);
                            cmd.Parameters.AddWithValue("@semester", row["semester"]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Data berhasil diimpor ke database.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Tutup PreviewForm setelah data diimpor
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat mengimpor data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
