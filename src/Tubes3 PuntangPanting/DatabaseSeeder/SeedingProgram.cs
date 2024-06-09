using System;
using System.Text;

namespace DatabaseSeeder
{
    class SeedingProgram
    {
        public static string imageFolder = @"../Tubes3 PuntangPanting/data";

        static void Main(string[] args)
        {
            Console.WriteLine("Pilihan program:");
            Console.WriteLine("1. Generate Database Baru yang belum dienkripsi");
            Console.WriteLine("2. Generate Database Baru yang akan dienkripsi");
            Console.WriteLine("3. Punya database yang belum dienkripsi dan ingin dienkripsi");
            Console.WriteLine("Pilih program (1/2/3):");

            string programChoice = Console.ReadLine();

            switch (programChoice)
            {
                case "1":
                    GenerateNewDatabase(false);
                    break;
                case "2":
                    GenerateNewDatabase(true);
                    break;
                case "3":
                    EncryptExistingDatabase();
                    break;
                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    break;
            }
        }

        static void GenerateNewDatabase(bool isEncrypt)
        {
            Console.WriteLine("Apakah Anda ingin menyesuaikan server, pengguna, database, dan kata sandi? (y/n)");
            string customizeInput = Console.ReadLine().ToLower();

            string customServer = "Fairuz";
            string customUser = "root";
            string customDatabase = "stimadbenc";
            string customPassword = "bismillah.33";

            if (customizeInput == "y")
            {
                Console.WriteLine("Masukkan server:");
                customServer = Console.ReadLine();

                Console.WriteLine("Masukkan pengguna:");
                customUser = Console.ReadLine();

                Console.WriteLine("Masukkan database:");
                customDatabase = Console.ReadLine();

                Console.WriteLine("Masukkan kata sandi:");
                customPassword = Console.ReadLine();
            }

            var connectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={customDatabase}";

            Console.WriteLine("Masukkan folder gambar(skip jika ingin menggunakan default):");
            string imageFolderInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(imageFolderInput))
            {
                imageFolder = imageFolderInput;
            }

            if (isEncrypt)
            {
                DataSeederEncrypted db = new DataSeederEncrypted(connectionString);
                db.CreateBiodataTable();
                db.CreateSidikJariTable();
                db.SeedFakeData(imageFolder);

            }
            else
            {
                DataSeeder db = new DataSeeder(connectionString);
                db.CreateBiodataTable();
                db.CreateSidikJariTable();
                db.SeedFakeData(imageFolder);
            }

            Console.WriteLine("Database baru berhasil dibuat.");
        }

        static void EncryptExistingDatabase()
        {
            Console.WriteLine("Apakah Anda ingin menyesuaikan server, pengguna, database, dan kata sandi? (y/n)");
            string customizeInput = Console.ReadLine().ToLower();

            string customServer = "Fairuz";
            string customUser = "root";
            string sourceDatabase = "stimadb";
            string targetDatabase = "stimaencdb";
            string customPassword = "bismillah.33";

            if (customizeInput == "y")
            {
                Console.WriteLine("Masukkan server:");
                customServer = Console.ReadLine();

                Console.WriteLine("Masukkan pengguna:");
                customUser = Console.ReadLine();

                Console.WriteLine("Masukkan database sumber:");
                sourceDatabase = Console.ReadLine();

                Console.WriteLine("Masukkan database tujuan:");
                targetDatabase = Console.ReadLine();

                Console.WriteLine("Masukkan kata sandi:");
                customPassword = Console.ReadLine();
            }

            var sourceConnectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={sourceDatabase}";
            var targetConnectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={targetDatabase}";

            SQLEncryptor sqe = new SQLEncryptor(sourceConnectionString, targetConnectionString);

            try
            {
                sqe.CreateBiodataTable();
                sqe.CreateSidikJariTable();
                sqe.EncryptDatabase();

                Console.WriteLine("Data berhasil dienkripsi dan disalin ke database baru.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan: " + ex.Message);
            }
        }
    }
}
