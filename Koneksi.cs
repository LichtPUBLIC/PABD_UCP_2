using System;
using System.Net;
using System.Net.Sockets;

namespace projectsem4
{
    internal class Koneksi
    {
        public string connectionString()
        {
            try
            {
                string serverIP = "192.168.1.7"; // IP kamu
                string connectStr = $"Server={serverIP},1433;" +
                                    $"Initial Catalog=PresensiMahasiswaProdiTI;" +
                                    $"User ID=sa;" +
                                    $"Password=Polang123;" +
                                    $"TrustServerCertificate=True;";
                return connectStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Koneksi gagal: " + ex.Message);
                return string.Empty;
            }
        }

    }
}
