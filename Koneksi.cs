using System;
using System.Net;
using System.Net.Sockets;

namespace projectsem4
{
    internal class Koneksi
    {
        /// <summary>
        /// Metode ini membangun dan mengembalikan connection string secara dinamis.
        /// </summary>
        /// <returns>String koneksi ke database SQL Server.</returns>
        public string GetConnectionString()
        {
            try
            {
                // Kembali menggunakan Integrated Security (Windows Authentication)
                // Tidak perlu User ID dan Password
                string connectStr = "Server=MSI\\DAFFAALYANDRA; Initial Catalog=PresensiMahasiswaProdiTI;" +
                                    "Integrated Security=True; TrustServerCertificate=True";

                return connectStr;
            }
            catch (Exception ex)
            {
                // Menampilkan error di console jika gagal mendapatkan IP atau membangun string
                Console.WriteLine("Gagal membuat connection string: " + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Metode statis untuk mendapatkan alamat IPv4 dari mesin lokal.
        /// </summary>
        /// <returns>String alamat IP lokal.</returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Filter untuk hanya mengambil alamat IPv4
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Tidak ada alamat IP v4 yang ditemukan untuk komputer ini.");
        }
    }
}
