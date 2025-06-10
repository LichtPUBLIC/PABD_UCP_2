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
    public partial class KelolaJadwal: Form
    {

        private string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";

        public KelolaJadwal()
        {
            InitializeComponent();
            Load += Kelola_Data_Jadwal_Load;
        }

        private void Kelola_Data_Jadwal_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadComboMK();
            cmbHari.Items.Clear();
            cmbHari.Items.AddRange(new string[] { "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" });
            cmbHari.SelectedIndex = 0;
        }


        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_jadwal AS [ID Jadwal], kode_mk AS [Kode MK], hari AS [Hari], jam_mulai AS [Jam Mulai], jam_selesai AS [Jam Selesai] FROM JadwalKuliah";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvJadwal.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void LoadComboMK()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT kode_mk, nama_mk FROM MataKuliah";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbMK.DataSource = dt;
                    cmbMK.DisplayMember = "nama_mk"; // yang ditampilkan ke user
                    cmbMK.ValueMember = "kode_mk";   // yang disimpan ke database

                    if (cmbMK.Items.Count > 0)
                        cmbMK.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saat load mata kuliah: " + ex.Message);
                }
            }
        }

        private void ClearForm()
        {
            txtIDjadwal.Clear();
            cmbMK.SelectedIndex = 0;
            cmbHari.SelectedIndex = 0;
            dtpJamMulai.Value = DateTime.Now;
            dtpJamSelesai.Value = DateTime.Now;
            txtIDjadwal.Focus();
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private bool ValidateInputJadwal()
        {
            StringBuilder errorMessages = new StringBuilder();

            // Validasi ID Jadwal
            if (string.IsNullOrWhiteSpace(txtIDjadwal.Text))
                errorMessages.AppendLine("ID Jadwal tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtIDjadwal.Text, @"^[A-Za-z0-9]{3,10}$"))
                errorMessages.AppendLine("ID Jadwal harus berupa kombinasi huruf dan angka, 3-10 karakter.");

            // Validasi Kode MK (pastikan ada yang dipilih)
            if (cmbMK.SelectedIndex < 0)
                errorMessages.AppendLine("Mata Kuliah harus dipilih.");

            // Validasi Hari (pastikan ada yang dipilih)
            if (cmbHari.SelectedIndex < 0)
                errorMessages.AppendLine("Hari harus dipilih.");

            // Validasi Jam Mulai dan Jam Selesai
            TimeSpan jamMulai = dtpJamMulai.Value.TimeOfDay;
            TimeSpan jamSelesai = dtpJamSelesai.Value.TimeOfDay;

            if (jamMulai == jamSelesai)
                errorMessages.AppendLine("Jam Mulai dan Jam Selesai tidak boleh sama.");
            else if (jamSelesai < jamMulai)
                errorMessages.AppendLine("Jam Selesai harus lebih besar dari Jam Mulai.");

            // Jika ada error, tampilkan pesan
            if (errorMessages.Length > 0)
            {
                MessageBox.Show(errorMessages.ToString(), "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void btnTambahJadwal(object sender, EventArgs e)
        {
            if (!ValidateInputJadwal())
                return;

            DialogResult result = MessageBox.Show("Yakin ingin menambahkan data ini?", "Konfirmasi Tambah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("AddJadwalKuliah", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_jadwal", txtIDjadwal.Text.Trim());
                        cmd.Parameters.AddWithValue("@kode_mk", cmbMK.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@hari", cmbHari.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@jam_mulai", dtpJamMulai.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@jam_selesai", dtpJamSelesai.Value.TimeOfDay);
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "Data berhasil ditambahkan.";
                        LoadData();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error: " + ex.Message;
                    }
                }
            }
        }

        private void btnRefreshJadwal(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnUbahJadwal(object sender, EventArgs e)
        {
            if (!ValidateInputJadwal())
                return;

            DialogResult result = MessageBox.Show("Yakin ingin mengubah data ini?", "Konfirmasi Ubah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("UpdateJadwalKuliah", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_jadwal", txtIDjadwal.Text.Trim());
                        cmd.Parameters.AddWithValue("@kode_mk", cmbMK.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@hari", cmbHari.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@jam_mulai", dtpJamMulai.Value.TimeOfDay);
                        cmd.Parameters.AddWithValue("@jam_selesai", dtpJamSelesai.Value.TimeOfDay);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Data berhasil diubah.";
                            LoadData();
                            ClearForm();
                        }
                        else
                        {
                            lblMessage.Text = "Gagal mengubah data: ID Jadwal tidak ditemukan atau format tidak sesuai.";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error: " + ex.Message;
                    }
                }
            }
        }

        private void btnHapusJadwal(object sender, EventArgs e)
        {
            if (dgvJadwal.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih data yang akan dihapus.");
                return;
            }

            string idJadwal = dgvJadwal.SelectedRows[0].Cells["ID Jadwal"].Value.ToString();
            DialogResult result = MessageBox.Show("Yakin ingin menghapus data?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DeleteJadwalKuliah", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_jadwal", idJadwal);
                        cmd.ExecuteNonQuery();
                        lblMessage.Text = "Data berhasil dihapus.";
                        LoadData();
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error: " + ex.Message;
                    }
                }
            }
        }

        private void btnKembaliJadwal(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void dgvJadwal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvJadwal.Rows[e.RowIndex];
                txtIDjadwal.Text = row.Cells["ID Jadwal"].Value.ToString();
                cmbMK.SelectedValue = row.Cells["Kode MK"].Value.ToString();
                cmbHari.SelectedItem = row.Cells["Hari"].Value.ToString();
                dtpJamMulai.Value = DateTime.Today.Add(TimeSpan.Parse(row.Cells["Jam Mulai"].Value.ToString()));
                dtpJamSelesai.Value = DateTime.Today.Add(TimeSpan.Parse(row.Cells["Jam Selesai"].Value.ToString()));
            }
        }

        private void KelolaJadwal_Load(object sender, EventArgs e)
        {

        }
    }
}
