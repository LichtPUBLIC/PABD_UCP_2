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

        Koneksi kn = new Koneksi();
        string connect = "";
        private string selectedPresensiID = "";

        public Presensi()
        {
            InitializeComponent();
            connect = kn.connectionString();
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
        private bool ValidateInputPresensi()
        {
            StringBuilder errorMessages = new StringBuilder();

            if (cmbIDjadwal.SelectedItem == null)
            {
                errorMessages.AppendLine("• Anda harus memilih ID Jadwal.");
            }
            if (cmbNIM.SelectedItem == null || string.IsNullOrWhiteSpace(cmbNIM.Text))
            {
                errorMessages.AppendLine("• Anda harus memilih atau mengisi NIM Mahasiswa.");
            }
            if (cmbStatus.SelectedItem == null)
            {
                errorMessages.AppendLine("• Anda harus memilih Status Kehadiran.");
            }

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

        private void LoadComboBoxMahasiswa()
        {
            using (SqlConnection conn = new SqlConnection(connect))
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
            using (SqlConnection conn = new SqlConnection(connect))
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
            using (SqlConnection conn = new SqlConnection(connect))
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

        private bool PresensiExists(string idJadwal, string nim, DateTime tanggal)
        {
            using (SqlConnection conn = new SqlConnection(connect))
            {
                // Query sederhana untuk menghitung data yang identik
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Presensi WHERE id_jadwal = @id_jadwal AND nim = @nim AND tanggal = @tanggal", conn);
                cmd.Parameters.AddWithValue("@id_jadwal", idJadwal);
                cmd.Parameters.AddWithValue("@nim", nim);
                cmd.Parameters.AddWithValue("@tanggal", tanggal.Date);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();

                // Jika hasil hitungan lebih dari 0, data sudah ada
                return count > 0;
            }
        }

        private void btnTambahPresensi(object sender, EventArgs e)
        {
            if (!ValidateInputPresensi())
            {
                return;
            }

            string idJadwal = cmbIDjadwal.Text.Trim();
            string nim = cmbNIM.Text.Trim();
            DateTime tanggal = dtpTanggal.Value;

            // Panggil fungsi pengecekan duplikat sebelum melakukan apapun
            if (PresensiExists(idJadwal, nim, tanggal))
            {
                MessageBox.Show($"Mahasiswa dengan NIM {nim} sudah tercatat melakukan presensi pada jadwal ini di tanggal yang sama.",
                                "Data Duplikat",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return; // Hentikan proses jika data sudah ada
            }

            DialogResult confirm = MessageBox.Show("Yakin ingin menambahkan data presensi?", "Konfirmasi Tambah", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            using (SqlConnection conn = new SqlConnection(connect))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("PresensiMahasiswa", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id_jadwal", idJadwal);
                    cmd.Parameters.AddWithValue("@nim", nim);
                    cmd.Parameters.AddWithValue("@tanggal", tanggal.Date);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());

                    cmd.ExecuteNonQuery();

                    lblMessage.Text = "Data presensi berhasil ditambahkan.";
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    // Menangkap error umum lainnya, bukan lagi error duplikat dari SQL
                    lblMessage.Text = "Terjadi kesalahan saat menyimpan: " + ex.Message;
                }
            }
        }

        private void btnRefreshPresensi(object sender, EventArgs e)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            LoadData();

            stopwatch.Stop();
            MessageBox.Show($"Tampilan data presensi berhasil diperbarui dalam {stopwatch.ElapsedMilliseconds} milidetik.", "Refresh Selesai", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            using (SqlConnection conn = new SqlConnection(connect))
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
                using (SqlConnection conn = new SqlConnection(connect))
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
