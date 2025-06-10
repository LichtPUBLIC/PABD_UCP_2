using Microsoft.Reporting.WinForms;
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
    public partial class LihatRekap: Form
    {
        public LihatRekap()
        {
            InitializeComponent();
        }

        private void LihatRekap_Load(object sender, EventArgs e)
        {
            SetupReportViewer();
            this.reportViewer1.RefreshReport();
        }

        private void SetupReportViewer()
        {
            string connectionString = "Data Source=MSI\\DAFFAALYANDRA;Initial Catalog=PresensiMahasiswaProdiTI;Integrated Security=True;";

            string query = @"
                    SELECT 
                        p.id_presensi,
                        m.nim,
                        m.nama_mhs,
                        m.kelas,
                        mk.nama_mk,
                        d.nama_dosen,
                        jk.hari,
                        jk.jam_mulai,
                        jk.jam_selesai,
                        p.tanggal,
                        p.status
                    FROM 
                        Presensi p
                    JOIN Mahasiswa m ON p.nim = m.nim
                    JOIN JadwalKuliah jk ON p.id_jadwal = jk.id_jadwal
                    JOIN MataKuliah mk ON jk.kode_mk = mk.kode_mk
                    JOIN Dosen d ON mk.id_dosen = d.id_dosen
                    ORDER BY 
                        p.tanggal DESC, m.nama_mhs";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            ReportDataSource rds = new ReportDataSource("DataSet1", dt);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            reportViewer1.LocalReport.ReportPath = @"D:\SEMESTER 4\PABD\Revisi PABD\projectsem4\PresensiReport.rdlc";

            reportViewer1.RefreshReport();
        }

        private void btnKembali(object sender, EventArgs e)
        {
            this.Close(); // atau this.Close();
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }
    }
}
