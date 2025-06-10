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
    public partial class Presensi: Form
    {

        private string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";
        private string selectedPresensiID = "";

        public Presensi()
        {
            InitializeComponent();
            Load += Kelola_Data_Presensi_Load;
        }

        private void Kelola_Data_Presensi_Load(object sender, EventArgs e)
        {
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Hadir", "Tidak Hadir", "Izin", "Sakit" });
            cmbStatus.SelectedIndex = 0;

            LoadComboBoxMahasiswa();
            LoadComboBoxJadwal();      
            LoadData();
        }

        private void LoadComboBoxMahasiswa()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT nim FROM Mahasiswa";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbNIM.Items.Clear();
                    while (reader.Read())
                    {
                        cmbNIM.Items.Add(reader["nim"].ToString());
                    }

                    if (cmbNIM.Items.Count > 0)
                        cmbNIM.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading Mahasiswa: " + ex.Message);
                }
            }
        }


        private void LoadComboBoxJadwal()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_jadwal FROM JadwalKuliah";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbIDjadwal.Items.Clear();
                    while (reader.Read())
                    {
                        cmbIDjadwal.Items.Add(reader["id_jadwal"].ToString());
                    }

                    if (cmbIDjadwal.Items.Count > 0)
                        cmbIDjadwal.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading JadwalKuliah: " + ex.Message);
                }
            }
        }


        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id_presensi AS [ID Presensi], tanggal AS [Tanggal], status AS [Status], nim AS [NIM], id_jadwal AS [ID Jadwal] FROM Presensi";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvPresensi.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void ClearForm()
        {

            cmbNIM.SelectedIndex = 0;
            cmbIDjadwal.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            dtpTanggal.Value = DateTime.Now;
        }

        private void dgvPresensi_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPresensi.Rows[e.RowIndex];
                selectedPresensiID = row.Cells["ID Presensi"].Value.ToString(); // ini penting!
                dtpTanggal.Value = Convert.ToDateTime(row.Cells["Tanggal"].Value);
                cmbStatus.SelectedItem = row.Cells["Status"].Value.ToString();
                cmbNIM.Text = row.Cells["NIM"].Value.ToString();
                cmbIDjadwal.Text = row.Cells["ID Jadwal"].Value.ToString();
            }

        }

        private void btnTambahPresensi(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Yakin ingin menambahkan data presensi?", "Konfirmasi Tambah", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;

                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    SqlCommand cmd = new SqlCommand("PresensiMahasiswa", conn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_jadwal", cmbIDjadwal.Text.Trim());
                    cmd.Parameters.AddWithValue("@nim", cmbNIM.Text.Trim());
                    cmd.Parameters.AddWithValue("@tanggal", dtpTanggal.Value.Date);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());

                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    lblMessage.Text = "Data presensi berhasil ditambahkan.";
                    LoadData();
                    ClearForm();
                }
                catch (SqlException ex)
                {
                    transaction?.Rollback();

                    // Tampilkan pesan error dari RAISERROR jika ada
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnRefreshPresensi(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnUbahPresensi(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(selectedPresensiID))
            {
                MessageBox.Show("Silakan pilih data yang ingin diubah terlebih dahulu.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Yakin ingin mengubah data presensi ini?", "Konfirmasi Ubah", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UpdatePresensi", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@tanggal", dtpTanggal.Value.Date);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@nim", cmbNIM.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_jadwal", cmbIDjadwal.Text.Trim());
                    cmd.Parameters.AddWithValue("@id_presensi", selectedPresensiID);

                    cmd.ExecuteNonQuery();
                    lblMessage.Text = "Data berhasil diperbarui.";
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnHapusPresensi(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(selectedPresensiID))
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus terlebih dahulu.");
                return;
            }

            DialogResult result = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("DeletePresensi", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@id_presensi", selectedPresensiID);

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

        private void btnKembaliPresensi(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void Presensi_Load(object sender, EventArgs e)
        {

        }

        
    }
}
