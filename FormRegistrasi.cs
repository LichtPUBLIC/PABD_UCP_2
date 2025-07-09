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
    public partial class FormRegistrasi : Form
    {
        private Koneksi koneksi = new Koneksi();
        private string connectionString;

        public FormRegistrasi()
        {
            InitializeComponent();
            connectionString = koneksi.GetConnectionString();
        }

        private void btnDaftar_Click(object sender, EventArgs e)
        {
            // 1. Validasi semua input terlebih dahulu
            if (!ValidateInput())
            {
                return; // Hentikan proses jika validasi gagal
            }

            // 2. Panggil stored procedure untuk menambahkan dosen baru
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("AddDosen", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_dosen", txtIdDosen.Text.Trim());
                    cmd.Parameters.AddWithValue("@emailkampus", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text); // Simpan password
                    cmd.Parameters.AddWithValue("@nama_dosen", txtNamaDosen.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registrasi berhasil! Silakan login dengan akun baru Anda.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Tutup form registrasi setelah berhasil
                }
            }
            catch (SqlException ex)
            {
                // Menangani error spesifik dari SQL, seperti duplikat Primary Key
                if (ex.Number == 2627) // Kode error untuk duplikat PK/Unique
                {
                    MessageBox.Show("ID Dosen atau Email sudah terdaftar. Silakan gunakan yang lain.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Terjadi error pada database: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private bool ValidateInput()
        {
            StringBuilder errors = new StringBuilder();

            // Validasi ID Dosen
            if (string.IsNullOrWhiteSpace(txtIdDosen.Text))
                errors.AppendLine("• ID Dosen tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtIdDosen.Text, @"^D\d{4}$"))
                errors.AppendLine("• Format ID Dosen salah (contoh: D0001).");

            // Validasi Nama Dosen
            if (string.IsNullOrWhiteSpace(txtNamaDosen.Text))
                errors.AppendLine("• Nama Lengkap tidak boleh kosong.");

            // Validasi Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                errors.AppendLine("• Email Kampus tidak boleh kosong.");
            else if (!txtEmail.Text.EndsWith(".ac.id")) // Contoh validasi sederhana
                errors.AppendLine("• Email harus menggunakan domain kampus (contoh: .ac.id).");

            // Validasi Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                errors.AppendLine("• Password tidak boleh kosong.");
            else if (txtPassword.Text.Length < 6)
                errors.AppendLine("• Password minimal harus 6 karakter.");

            if (errors.Length > 0)
            {
                MessageBox.Show("Terjadi kesalahan validasi:\n\n" + errors.ToString(), "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void linkKembali_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }
    }
}
