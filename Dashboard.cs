using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace projectsem4
{
    public partial class Dashboard: Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void btnDatadosen(object sender, EventArgs e)
        {
            this.Hide();
            KelolaDataDosen form = new KelolaDataDosen();
            form.Show();
        }

        private void btnDatamahasiswa(object sender, EventArgs e)
        {
            this.Hide();
            KelolaMahasiswacs formKelolaMahasiswa = new KelolaMahasiswacs();
            formKelolaMahasiswa.Show();
        }

        private void btnDataMatkul(object sender, EventArgs e)
        {
            this.Hide();
            KelolaMatakuliah formKelolaMatakuliah = new KelolaMatakuliah();
            formKelolaMatakuliah.Show();
        }

        private void btnJadwal(object sender, EventArgs e)
        {
            this.Hide();
            KelolaJadwal formKelolaJadwal = new KelolaJadwal();
            formKelolaJadwal.Show();
        }

        private void btnPresensi(object sender, EventArgs e)
        {
            this.Hide();
            Presensi formPresensi = new Presensi();
            formPresensi.Show();
        }

        private void btnLogout(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Yakin ingin logout?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Hide(); // sembunyikan dashboard
                Login login = new Login();
                login.Show();
            }
        }

        private void btnRecap_Click(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            LihatRekap lihatrekap = new LihatRekap();
            lihatrekap.Show();
        }

        private void btnGrafikRekap_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormRekapChart formGrafik = new FormRekapChart();
            formGrafik.Show();
        }
    }
}
