using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace DatabaseSeeder
{
    public class SQLEncryptor
    {
        private static MySqlConnection sourceConnection;
        private static MySqlConnection targetConnection;

        public SQLEncryptor(string sourceConnectionString, string targetConnectionString)
        {
            sourceConnection = new MySqlConnection(sourceConnectionString);
            targetConnection = new MySqlConnection(targetConnectionString);
        }

        public static void CreateNewDatabase(string databaseName)
        {
            string query = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            ExecuteNonQuery(query, "CreateNewDatabase");
        }

        public void CreateBiodataTable()
        {
            string query = @"
            CREATE TABLE IF NOT EXISTS `biodata` (
              `NIK` TEXT NOT NULL,
              `nama` TEXT NULL,
              `tempat_lahir` TEXT NULL,
              `tanggal_lahir` TEXT NULL,
              `jenis_kelamin` ENUM('Laki-Laki','Perempuan') NULL,
              `golongan_darah` ENUM('O','A','AB','B') NULL,
              `alamat` TEXT NULL,
              `agama` ENUM('Islam','Kristen','Katolik','Hindu','Buddha') NULL,
              `status_perkawinan` ENUM('Belum Menikah','Menikah','Cerai') NULL,
              `pekerjaan` TEXT NULL,
              `kewarganegaraan` ENUM('WNI','WNA') NULL,
              PRIMARY KEY (`NIK`(255))
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";

            ExecuteNonQuery(query, "CreateBiodataTable");
        }

        public void CreateSidikJariTable()
        {
            string query = @"
            CREATE TABLE IF NOT EXISTS `sidik_jari` (
              `berkas_citra` TEXT NULL,
              `realName` TEXT NULL,
              `path` TEXT NOT NULL,
              PRIMARY KEY (`path`(255))
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";

            ExecuteNonQuery(query, "CreateSidikJariTable");
        }

        private static void ExecuteNonQuery(string query, string methodName)
        {
            using (var cmd = new MySqlCommand(query, targetConnection))
            {
                try
                {
                    targetConnection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"Query in {methodName} executed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing query in {methodName}: {ex.Message}");
                }
                finally
                {
                    targetConnection.Close();
                }
            }
        }

        public void EncryptDatabase()
        {
            sourceConnection.Open();

            byte[] key = new byte[16] {
                0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x4e, 0x35, 0x0b, 0x34, 0x78, 0x55
            };

            byte[] iv = new byte[16] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
            };

            CustomAes aes = new CustomAes(key, iv);

            var biodataRecords = new List<(string, string, string, string, string, string, string, string, string, string, string)>();
            var sidikJariRecords = new List<(string, string, string)>();
            var uniquePaths = new HashSet<string>();

            Console.WriteLine("Encrypting biodata...");
            string selectQuery = "SELECT * FROM biodata";
            using (var selectCommand = new MySqlCommand(selectQuery, sourceConnection))
            {
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string NIK = reader.GetString(0);
                        byte[] NIKbyteArray = new byte[64];
                        byte[] NIKBytes = Encoding.UTF8.GetBytes(NIK);
                        Array.Copy(NIKBytes, NIKbyteArray, Math.Min(NIKBytes.Length, NIKbyteArray.Length));
                        int paddingSize = NIKbyteArray.Length - NIKBytes.Length;
                        byte padValue = (byte)paddingSize;

                        for (int j = NIKBytes.Length; j < NIKbyteArray.Length; j++)
                        {
                            NIKbyteArray[j] = padValue;
                        }
                        NIK = BitConverter.ToString(aes.Encrypt(NIKbyteArray));

                        string nama = reader.GetString(1);
                        byte[] namaByteArray = new byte[64];
                        byte[] namaBytes = Encoding.UTF8.GetBytes(nama);
                        Array.Copy(namaBytes, namaByteArray, Math.Min(namaBytes.Length, namaByteArray.Length));

                        for (int j = namaBytes.Length; j < namaByteArray.Length; j++)
                        {
                            namaByteArray[j] = 0;
                        }
                        nama = BitConverter.ToString(aes.Encrypt(namaByteArray));

                        string tempat_lahir = reader.GetString(2);
                        byte[] tempatLahirByteArray = new byte[64];
                        byte[] tempatLahirBytes = Encoding.UTF8.GetBytes(tempat_lahir);
                        Array.Copy(tempatLahirBytes, tempatLahirByteArray, Math.Min(tempatLahirBytes.Length, tempatLahirByteArray.Length));

                        for (int j = tempatLahirBytes.Length; j < tempatLahirByteArray.Length; j++)
                        {
                            tempatLahirByteArray[j] = 0;
                        }
                        tempat_lahir = BitConverter.ToString(aes.Encrypt(tempatLahirByteArray));

                        DateTime tanggal_lahir = reader.GetDateTime(3);
                        string day = tanggal_lahir.Day.ToString();
                        string month = tanggal_lahir.Month.ToString();
                        string year = tanggal_lahir.Year.ToString();
                        string tanggalLahir = year + "-" + month + "-" + day;
                        byte[] tanggalLahirByteArray = new byte[64];
                        byte[] tanggalLahirBytes = Encoding.UTF8.GetBytes(tanggalLahir);
                        Array.Copy(tanggalLahirBytes, tanggalLahirByteArray, Math.Min(tanggalLahirBytes.Length, tanggalLahirByteArray.Length));

                        for (int j = tanggalLahirBytes.Length; j < tanggalLahirByteArray.Length; j++)
                        {
                            tanggalLahirByteArray[j] = 0;
                        }
                        tanggalLahir = BitConverter.ToString(aes.Encrypt(tanggalLahirByteArray));

                        string jenis_kelamin = reader.GetString(4);
                        string golongan_darah = reader.GetString(5);

                        string alamat = reader.GetString(6);
                        byte[] alamatByteArray = new byte[64];
                        byte[] alamatBytes = Encoding.UTF8.GetBytes(alamat);
                        Array.Copy(alamatBytes, alamatByteArray, Math.Min(alamatBytes.Length, alamatByteArray.Length));

                        for (int j = alamatBytes.Length; j < alamatByteArray.Length; j++)
                        {
                            alamatByteArray[j] = 0;
                        }
                        alamat = BitConverter.ToString(aes.Encrypt(alamatByteArray));

                        string agama = reader.GetString(7);
                        string status_perkawinan = reader.GetString(8);

                        string pekerjaan = reader.GetString(9);
                        byte[] pekerjaanByteArray = new byte[64];
                        byte[] pekerjaanBytes = Encoding.UTF8.GetBytes(pekerjaan);
                        Array.Copy(pekerjaanBytes, pekerjaanByteArray, Math.Min(pekerjaanBytes.Length, pekerjaanByteArray.Length));

                        for (int j = pekerjaanBytes.Length; j < pekerjaanByteArray.Length; j++)
                        {
                            pekerjaanByteArray[j] = 0;
                        }
                        pekerjaan = BitConverter.ToString(aes.Encrypt(pekerjaanByteArray));

                        string kewarganegaraan = reader.GetString(10);

                        var biodataData = (NIK, nama, tempat_lahir, tanggalLahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan);
                        biodataRecords.Add(biodataData);
                    }
                }
            }

            Console.WriteLine("Encrypting sidik_jari...");
            selectQuery = "SELECT * FROM sidik_jari";
            using (var selectCommand = new MySqlCommand(selectQuery, sourceConnection))
            {
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string berkas_citra = reader.GetString(0);

                        string path = reader.GetString(2);
                        byte[] pathByteArray = new byte[64];
                        byte[] pathBytes = Encoding.UTF8.GetBytes(path);
                        Array.Copy(pathBytes, pathByteArray, Math.Min(pathBytes.Length, pathByteArray.Length));

                        for (int k = pathBytes.Length; k < pathByteArray.Length; k++)
                        {
                            pathByteArray[k] = 0;
                        }
                        path = BitConverter.ToString(aes.Encrypt(pathByteArray));

                        if (!uniquePaths.Contains(path))
                        {
                            byte[] berkasCitraByteArray = new byte[2304];
                            byte[] berkasCitraBytes = Encoding.UTF8.GetBytes(berkas_citra);
                            Array.Copy(berkasCitraBytes, berkasCitraByteArray, Math.Min(berkasCitraBytes.Length, berkasCitraByteArray.Length));

                            for (int k = berkasCitraBytes.Length; k < berkasCitraByteArray.Length; k++)
                            {
                                berkasCitraByteArray[k] = 0;
                            }
                            berkas_citra = BitConverter.ToString(aes.Encrypt(berkasCitraByteArray));

                            string realName = reader.GetString(1);
                            byte[] realNameByteArray = new byte[64];
                            byte[] realNameBytes = Encoding.UTF8.GetBytes(realName);
                            Array.Copy(realNameBytes, realNameByteArray, Math.Min(realNameBytes.Length, realNameByteArray.Length));

                            for (int k = realNameBytes.Length; k < realNameByteArray.Length; k++)
                            {
                                realNameByteArray[k] = 0;
                            }
                            realName = BitConverter.ToString(aes.Encrypt(realNameByteArray));

                            var sidikJariData = (berkas_citra, realName, path);
                            sidikJariRecords.Add(sidikJariData);
                            uniquePaths.Add(path);
                        }
                        else
                        {
                            Console.WriteLine($"Duplicate path detected: {path}");
                        }
                    }
                }
            }

            targetConnection.Open();
            InsertBiodataRecords(biodataRecords);
            InsertSidikJariRecords(sidikJariRecords);
            targetConnection.Close();
            sourceConnection.Close();
        }

        private void InsertBiodataRecords(List<(string, string, string, string, string, string, string, string, string, string, string)> biodataRecords)
        {
            using (var transaction = targetConnection.BeginTransaction())
            {
                try
                {
                    var insertBiodata = @"
                    INSERT INTO biodata (
                        NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah,
                        alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan
                    ) VALUES (@NIK, @nama, @tempat_lahir, @tanggalLahir, @jenis_kelamin, @golongan_darah,
                        @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)
                ";

                    using (var command = new MySqlCommand(insertBiodata, targetConnection, transaction))
                    {
                        command.Parameters.Add("@NIK", MySqlDbType.Text);
                        command.Parameters.Add("@nama", MySqlDbType.Text);
                        command.Parameters.Add("@tempat_lahir", MySqlDbType.Text);
                        command.Parameters.Add("@tanggalLahir", MySqlDbType.Text);
                        command.Parameters.Add("@jenis_kelamin", MySqlDbType.Enum);
                        command.Parameters.Add("@golongan_darah", MySqlDbType.Enum);
                        command.Parameters.Add("@alamat", MySqlDbType.Text);
                        command.Parameters.Add("@agama", MySqlDbType.Enum);
                        command.Parameters.Add("@status_perkawinan", MySqlDbType.Enum);
                        command.Parameters.Add("@pekerjaan", MySqlDbType.Text);
                        command.Parameters.Add("@kewarganegaraan", MySqlDbType.Enum);

                        foreach (var biodata in biodataRecords)
                        {
                            command.Parameters["@NIK"].Value = biodata.Item1;
                            command.Parameters["@nama"].Value = biodata.Item2;
                            command.Parameters["@tempat_lahir"].Value = biodata.Item3;
                            command.Parameters["@tanggalLahir"].Value = biodata.Item4;
                            command.Parameters["@jenis_kelamin"].Value = biodata.Item5;
                            command.Parameters["@golongan_darah"].Value = biodata.Item6;
                            command.Parameters["@alamat"].Value = biodata.Item7;
                            command.Parameters["@agama"].Value = biodata.Item8;
                            command.Parameters["@status_perkawinan"].Value = biodata.Item9;
                            command.Parameters["@pekerjaan"].Value = biodata.Item10;
                            command.Parameters["@kewarganegaraan"].Value = biodata.Item11;

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Inserted records into biodata table.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error inserting records into biodata table: {ex.Message}");
                }
            }
        }

        private void InsertSidikJariRecords(List<(string, string, string)> sidikJariRecords)
        {
            using (var transaction = targetConnection.BeginTransaction())
            {
                try
                {
                    var insertSidikJari = @"
                    INSERT INTO sidik_jari (
                        berkas_citra, realName, path
                    ) VALUES (@berkas_citra, @realName, @path)
                ";

                    using (var command = new MySqlCommand(insertSidikJari, targetConnection, transaction))
                    {
                        command.Parameters.Add("@berkas_citra", MySqlDbType.Text);
                        command.Parameters.Add("@realName", MySqlDbType.Text);
                        command.Parameters.Add("@path", MySqlDbType.Text);

                        foreach (var sidikJari in sidikJariRecords)
                        {
                            command.Parameters["@berkas_citra"].Value = sidikJari.Item1;
                            command.Parameters["@realName"].Value = sidikJari.Item2;
                            command.Parameters["@path"].Value = sidikJari.Item3;

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Inserted records into sidik_jari table.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error inserting records into sidik_jari table: {ex.Message}");
                }
            }
        }

        static void Main(string[] args)
        {
            var customServer = "Fairuz";
            var customUser = "root";
            var sourceDatabase = "dummy";
            var targetDatabase = "dummyenc";
            var customPassword = "bismillah.33";
            var sourceConnectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={sourceDatabase}";
            var targetConnectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={targetDatabase}";

            SQLEncryptor sqe = new SQLEncryptor(sourceConnectionString, targetConnectionString);

            try
            {
                sqe.CreateBiodataTable();
                sqe.CreateSidikJariTable();
                sqe.EncryptDatabase();

                Console.WriteLine("Data successfully encrypted and copied to the new database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
   
   
    }
}
