using System;
using System.Collections;
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
    public partial class KelolaDataDosen: Form
    {

        private string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";

        public KelolaDataDosen()
        {
            InitializeComponent();
            this.Load += KelolaDataDosenLoad;
        }

        private void KelolaDataDosenLoad(object sender, EventArgs e)
        {
            LoadData();
        }

        private void ClearForm()
        {
            txtIDdosen.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtNamadosen.Clear();
            txtIDdosen.Focus(); // samakan dengan nama kontrol textbox yang benar
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_dosen AS [ID], emailkampus AS [Email Kampus], password AS [Password], nama_dosen AS [Nama Dosen] FROM Dosen";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvDosen.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menampilkan data: " + ex.Message);
                }
            }
        }

        private bool ValidateInputDosen()
        {
            StringBuilder errorMessages = new StringBuilder();

            // Validasi ID Dosen (misalnya: D0001)
            if (string.IsNullOrWhiteSpace(txtIDdosen.Text))
                errorMessages.AppendLine("ID Dosen tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtIDdosen.Text, @"^D\d{4}$"))
                errorMessages.AppendLine("ID Dosen harus berformat D diikuti 4 digit angka (misal: D0001).");

            // Validasi Email Kampus
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                errorMessages.AppendLine("Email kampus tidak boleh kosong.");
            else if (!txtEmail.Text.Contains("@") || !txtEmail.Text.EndsWith(".ac.id"))
                errorMessages.AppendLine("Email kampus harus valid dan diakhiri dengan .ac.id");

            // Validasi Password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                errorMessages.AppendLine("Password tidak boleh kosong.");
            else if (txtPassword.Text.Length < 6)
                errorMessages.AppendLine("Password minimal terdiri dari 6 karakter.");

            // Validasi Nama Dosen
            if (string.IsNullOrWhiteSpace(txtNamadosen.Text))
                errorMessages.AppendLine("Nama dosen tidak boleh kosong.");
            else if (!txtNamadosen.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                errorMessages.AppendLine("Nama dosen hanya boleh mengandung huruf dan spasi.");

            // Tampilkan pesan jika ada error
            if (errorMessages.Length > 0)
            {
                MessageBox.Show(errorMessages.ToString(), "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void btnTambahDosen(object sender, EventArgs e)
        {
            if (!ValidateInputDosen()) return;

            DialogResult result = MessageBox.Show("Yakin ingin menambahkan data ini?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand("AddDosen", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_dosen", txtIDdosen.Text.Trim());
                            cmd.Parameters.AddWithValue("@emailkampus", txtEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                            cmd.Parameters.AddWithValue("@nama_dosen", txtNamadosen.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }
                           
                        lblMessage.Text = "Data berhasil ditambahkan!";
                        LoadData();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Gagal menambah data: " + ex.Message;
                    }
                }
            }
            else
            {
                MessageBox.Show("Penambahan data dibatalkan.");
            }
        }

        private void btnRefreshDosen(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnEditdosen(object sender, EventArgs e)
        {
            if (!ValidateInputDosen()) return;


            DialogResult result = MessageBox.Show("Yakin ingin memperbarui data ini?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (var cmd = new SqlCommand("UpdateDosen", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@id_dosen", txtIDdosen.Text.Trim());
                            cmd.Parameters.AddWithValue("@emailkampus", txtEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                            cmd.Parameters.AddWithValue("@nama_dosen", txtNamadosen.Text.Trim());

                            // Cek apakah ada baris yang terpengaruh
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                lblMessage.Text = "Data berhasil diperbarui!";
                                LoadData();
                                ClearForm();
                            }
                            else
                            {
                                lblMessage.Text = "Gagal mengubah data: ID Dosen tidak ditemukan atau format tidak sesuai.";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Gagal mengubah data: " + ex.Message;
                    }
                }
            }
            else
            {
                MessageBox.Show("Perubahan data dibatalkan.");
            }
        }

        private void btnHapusdosen(object sender, EventArgs e)
        {
            if (dgvDosen.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih data yang akan dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idDosen = dgvDosen.SelectedRows[0].Cells["ID"].Value.ToString();

            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Cek apakah dosen masih mengajar mata kuliah
                    string checkQuery = "SELECT COUNT(*) FROM MataKuliah WHERE id_dosen = @id_dosen";
                    using (var checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id_dosen", idDosen);
                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            MessageBox.Show(
                                "Dosen ini tidak bisa dihapus karena masih tercatat sebagai pengajar di satu atau lebih Mata Kuliah.",
                                "Aksi Dibatalkan",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }
                    }

                    // Jika aman, lanjutkan proses hapus
                    DialogResult confirm = MessageBox.Show($"Yakin ingin menghapus dosen dengan ID: {idDosen}?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        using (var deleteCmd = new SqlCommand("DeleteDosen", conn))
                        {
                            deleteCmd.CommandType = CommandType.StoredProcedure;
                            deleteCmd.Parameters.AddWithValue("@id_dosen", idDosen);
                            deleteCmd.ExecuteNonQuery();
                        }

                        lblMessage.Text = "Data dosen berhasil dihapus!";
                        LoadData();
                        ClearForm();
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Terjadi error: " + ex.Message;
                }
            }
        }

        private void btnKembaliDosen(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void dgvDosen_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDosen.Rows[e.RowIndex];
                txtIDdosen.Text = row.Cells["ID"].Value.ToString();
                txtEmail.Text = row.Cells["Email Kampus"].Value.ToString();
                txtPassword.Text = row.Cells["Password"].Value.ToString();
                txtNamadosen.Text = row.Cells["Nama Dosen"].Value.ToString();
            }
        }
    }
}
