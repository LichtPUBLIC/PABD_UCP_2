using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projectsem4
{
    public partial class FormRegistrasi : Form
    {
        Koneksi kn = new Koneksi();
        string connect = "";
        public FormRegistrasi()
        {
            InitializeComponent();
            connect = kn.connectionString();
        }

        private void btnDaftar_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                return; // Hentikan proses jika validasi gagal
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connect))
                {
                    conn.Open();

                    // Pengecekan Duplikat ID
                    SqlCommand checkIdCmd = new SqlCommand("SELECT COUNT(*) FROM Dosen WHERE id_dosen = @id_dosen", conn);
                    checkIdCmd.Parameters.AddWithValue("@id_dosen", txtIdDosen.Text.Trim());
                    if ((int)checkIdCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("ID Dosen sudah terdaftar. Silakan gunakan yang lain.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Pengecekan Duplikat Email
                    SqlCommand checkEmailCmd = new SqlCommand("SELECT COUNT(*) FROM Dosen WHERE emailkampus = @email", conn);
                    checkEmailCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    if ((int)checkEmailCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Email sudah terdaftar. Silakan gunakan yang lain.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Jika lolos pengecekan, lanjutkan penyimpanan
                    SqlCommand cmd = new SqlCommand("AddDosen", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_dosen", txtIdDosen.Text.Trim());
                    cmd.Parameters.AddWithValue("@emailkampus", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@nama_dosen", txtNamaDosen.Text.Trim());

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Registrasi berhasil! Silakan login dengan akun baru Anda.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
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

            if (string.IsNullOrWhiteSpace(txtIdDosen.Text))
                errors.AppendLine("• ID Dosen tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtIdDosen.Text, @"^D\d{4}$"))
                errors.AppendLine("• Format ID Dosen salah (contoh: D0001).");

            // Validasi Nama Dosen ditambahkan
            if (string.IsNullOrWhiteSpace(txtNamaDosen.Text))
                errors.AppendLine("• Nama Lengkap tidak boleh kosong.");
            else if (!txtNamaDosen.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                errors.AppendLine("• Nama hanya boleh mengandung huruf dan spasi.");

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                errors.AppendLine("• Email Kampus tidak boleh kosong.");
            else if (!txtEmail.Text.EndsWith(".ac.id"))
                errors.AppendLine("• Email harus menggunakan domain kampus (contoh: .ac.id).");

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
