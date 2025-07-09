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

    public partial class Login: Form
    {
        private Koneksi koneksi = new Koneksi();
        private string connectionString;
        
        public Login()
        {
            InitializeComponent();
            connectionString = koneksi.GetConnectionString();
        }

        private void btnLogin(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email dan password harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Dosen WHERE emailkampus = @Email AND password = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Login berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide(); // Jangan close, supaya bisa balik dari logout
                        Dashboard dashboard = new Dashboard();
                        dashboard.Show();
                    }
                    else
                    {
                        MessageBox.Show("Email atau password salah!", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saat login: " + ex.Message);
                }
            }
        }

        private void linkRegistrasi_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Membuat instance form registrasi
            FormRegistrasi formRegistrasi = new FormRegistrasi();

            // Menyembunyikan form login sementara
            this.Hide();

            // Menampilkan form registrasi
            formRegistrasi.ShowDialog(); // Gunakan ShowDialog agar form login menunggu

            // Setelah form registrasi ditutup, tampilkan kembali form login
            this.Show();
        }
    }
}
   

