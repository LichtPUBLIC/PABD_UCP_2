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
    public partial class KelolaMatakuliah: Form
    {

        private string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";
        public KelolaMatakuliah()
        {
            InitializeComponent();
            this.Load += kelola_Data_MataKuliah_Load;

        }

        private void kelola_Data_MataKuliah_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadDosen();

        }

        private void ClearForm()
        {
            txtKodemk.Clear();
            txtNamamk.Clear();
            comboBoxDosen.SelectedIndex = -1;
            txtKodemk.Focus();
        }

        private void LoadDosen()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT id_dosen, nama_dosen FROM Dosen";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBoxDosen.DataSource = dt;
                comboBoxDosen.DisplayMember = "nama_dosen"; // Yang ditampilkan
                comboBoxDosen.ValueMember = "id_dosen";     // Yang disimpan
            }
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT mk.kode_mk AS [Kode MK], 
                               mk.nama_mk AS [Nama Mata Kuliah], 
                               d.nama_dosen AS [Dosen], 
                               mk.id_dosen
                        FROM MataKuliah mk
                        JOIN Dosen d ON mk.id_dosen = d.id_dosen";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvMataKuliah.DataSource = dt;
                    dgvMataKuliah.Columns["id_dosen"].Visible = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private bool ValidateInputMatakuliah()
        {
            StringBuilder errors = new StringBuilder();

            // Validasi kode mata kuliah: tidak kosong, alfanumerik 3-10 karakter
            if (string.IsNullOrWhiteSpace(txtKodemk.Text))
                errors.AppendLine("Kode Mata Kuliah tidak boleh kosong.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtKodemk.Text.Trim(), @"^[A-Za-z0-9]{3,10}$"))
                errors.AppendLine("Kode Mata Kuliah harus berupa kombinasi huruf dan angka, 3-10 karakter.");

            // Validasi nama mata kuliah: tidak kosong
            if (string.IsNullOrWhiteSpace(txtNamamk.Text))
                errors.AppendLine("Nama Mata Kuliah tidak boleh kosong.");

            // Validasi dosen: harus ada yang dipilih
            if (comboBoxDosen.SelectedIndex < 0)
                errors.AppendLine("Dosen harus dipilih.");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        private void btnTambahMK(object sender, EventArgs e)
        {
            if (!ValidateInputMatakuliah())
                return;

            if (txtKodemk.Text == "" || txtNamamk.Text == "" || comboBoxDosen.Text == "")
            {
                MessageBox.Show("Harap isi semua data!");
                return;
            }

            DialogResult confirm = MessageBox.Show("Yakin ingin menambahkan data?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("AddMataKuliah", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@kode_mk", txtKodemk.Text.Trim());
                        cmd.Parameters.AddWithValue("@nama_mk", txtNamamk.Text.Trim());
                        cmd.Parameters.AddWithValue("@id_dosen", comboBoxDosen.SelectedValue.ToString());
                        cmd.ExecuteNonQuery();
                    }
                       

                    lblMessage.Text = "Data berhasil ditambahkan!";
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnRefreshMK(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnUbahMK(object sender, EventArgs e)
        {

            if (!ValidateInputMatakuliah())
                return;

            if (txtKodemk.Text == "")
            {
                MessageBox.Show("Silakan pilih data yang ingin diubah terlebih dahulu.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Yakin ingin mengubah data ini?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("UpdateMataKuliah", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@kode_mk", txtKodemk.Text.Trim());
                        cmd.Parameters.AddWithValue("@nama_mk", txtNamamk.Text.Trim());
                        cmd.Parameters.AddWithValue("@id_dosen", comboBoxDosen.SelectedValue.ToString());

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMessage.Text = "Data berhasil diubah!";
                            LoadData();
                            ClearForm();
                        }
                        else
                        {
                            lblMessage.Text = "Gagal mengubah data: Kode Mata Kuliah tidak ditemukan atau format tidak sesuai.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnHapusMK(object sender, EventArgs e)
        {
            if (dgvMataKuliah.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih data yang akan dihapus.");
                return;
            }

            string kodeMK = dgvMataKuliah.SelectedRows[0].Cells["Kode MK"].Value.ToString();
            DialogResult confirm = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.No) return;

            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("DeleteMataKuliah", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@kode_mk", kodeMK);
                        cmd.ExecuteNonQuery();
                    }
                        

                    lblMessage.Text = "Data berhasil dihapus!";
                    LoadData();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }
            }
        }

        private void btnKembaliMK(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }

        private void dgvMatakuliah_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dgvMataKuliah.Rows[e.RowIndex];
                    txtKodemk.Text = row.Cells["Kode MK"].Value.ToString();
                    txtNamamk.Text = row.Cells["Nama Mata Kuliah"].Value.ToString();
                    string idDosen = row.Cells["id_dosen"].Value.ToString();
                    comboBoxDosen.SelectedValue = idDosen;
                }
            }
        }

        private void comboBoxDosen_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
