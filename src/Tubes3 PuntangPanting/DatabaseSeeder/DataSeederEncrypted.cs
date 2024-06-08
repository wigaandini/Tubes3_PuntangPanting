using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Bogus;
using System.IO;
using System.Text;
using System.Runtime.Intrinsics.Arm;
// using Microsoft.Identity.Client;

namespace DatabaseSeeder
{
    public class DataSeederEncrypted
    {
        private readonly MySqlConnection connection;
        private readonly Random random;

        public DataSeederEncrypted(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            random = new Random();
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

            Console.WriteLine("Biodata created");
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

        private void ExecuteNonQuery(string query, string methodName)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Query in {methodName} executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query in {methodName}: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        private string GenerateUniqueName(Faker fake)
        {
            string fakeName = fake.Name.FullName();
            string cleanedName = RemoveNonAlphabetic(fakeName);
            string realName = RemoveTitles(cleanedName);
            return realName;
        }

        private string RemoveNonAlphabetic(string name)
        {
            string cleanedName = new string(name.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
            return cleanedName;
        }

        private string RemoveTitles(string name)
        {
            List<string> titles = new List<string>
            {
            "SE",
            "SEI",
            "SPsi",
            "MM",
            "SPt",
            "MPd",
            "Dr",
            "Drs",
            "Ir",
            "SH",
            "MH",
            "MSi",
            "MHum",
            "MA",
            "MSc",
            "PhD",
            "Prof",
            "Prof Dr",
            "Dr Hc",
            "BSc",
            "BA",
            "BEng",
            "MBA",
            "LLB",
            "LLM",
            "MPhil",
            "DPhil",
            "EdD",
            "DDS",
            "DMD",
            "DO",
            "DVM",
            "MD",
            "MFA",
            "JD",
            "PsyD",
            "ThD",
            "DMin",
            "BBA",
            "AB",
            "BS",
            "BM",
            "BFA",
            "MLIS",
            "MSW",
            "MPH",
            "MEd",
            "MEng",
            "MArch",
            "MDes",
            "MSN",
            "DSc",
            "DHEd",
            "DMus",
            "DPT",
            "OTD",
            "PharmD",
            "RN",
            "NP",
            "CFA",
            "CPA",
            "Esq",
            "PGDip",
            "DipHE",
            "CertHE",
            "PGCE",
            "BEd",
            "MSt",
            "MMus",
            "MAEd",
            "MChem",
            "MBiol",
            "MSocSci",
            "BAA",
            "BAppSc",
            "MComp",
            "MAcc",
            "AMd",
            "SAg",
            "SPd",
            "SE",
            "SKom",
            "SHum",
            "SIKom",
            "SPt",
            "SFarm",
            "SKed",
            "SKes",
            "SSi",
            "SKH",
            "ST",
            "STP",
            "SPsi",
            "SPt",
            "SSos",
            "SH",
            "SHut",
            "SPi",
            "SGz",
            "SStat",
            "STrT",
            "STrKes",
            "MAg",
            "MPd",
            "MSi",
            "MKom",
            "MHum",
            "MIKom",
            "MFarm",
            "MKed",
            "MKes",
            "MSi",
            "MKH",
            "MT",
            "MTP",
            "MPsi",
            "MSos",
            "MH",
            "MHut",
            "MPi",
            "MGz",
            "MStat",
            "MTrT",
            "MTrKes",
            "DS",
            "DAg",
            "DPd",
            "DSi",
            "DKom",
            "DHum",
            "DIKom",
            "DFarm",
            "DKed",
            "DKes",
            "DT",
            "DTP",
            "DPsi",
            "DSos",
            "DH",
            "DHut",
            "DPi",
            "DGz",
            "DStat",
            "DTrT",
            "DTrKes",
            "MTi",
            "Mak, msip",
            "Mak, msi",
            };

            string cleanedName = name.ToLower();
            foreach (string title in titles)
            {
                var aha = title.ToLower();
                cleanedName = cleanedName.Replace(aha, "");
            }

            cleanedName = cleanedName.Trim();
            return cleanedName;

        }

        public string GenerateRandomCorruptName(string name)
        {
            string corruptName = RandomRemoveVocal(name);
            corruptName = RandomCase(corruptName);
            corruptName = RandomAlay(corruptName);
            if(corruptName[corruptName.Length - 1] == '0'){
                corruptName = corruptName.Substring(0, corruptName.Length - 1) + 'o';
            }
            return corruptName;
        }

        private string RandomRemoveVocal(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                char charLower = char.ToLower(c);
                if (charLower == 'a' || charLower == 'i' || charLower == 'u' || charLower == 'e' || charLower == 'o')
                {
                    bool remove = random.Next(2) == 0;
                    if (!remove)
                    {
                        result.Append(c);
                    }
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        private string RandomCase(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                bool change = random.Next(2) == 0;
                if (change)
                {
                    result.Append(char.ToUpper(c));
                }
                else
                {
                    result.Append(char.ToLower(c));
                }
            }
            return result.ToString();
        }

        private string RandomAlay(string input)
        {
            Dictionary<char, char> mapping = new Dictionary<char, char>
        {
            {'i', '1'},
            {'z', '2'},
            {'e', '3'},
            {'a', '4'},
            {'s', '5'},
            {'t', '7'},
            {'b', '8'},
            {'g', '9'},
            {'o', '0'}
        };

            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                if (mapping.ContainsKey(char.ToLower(c)))
                {
                    bool change = random.Next(2) == 0;
                    if (change)
                    {
                        result.Append(mapping[char.ToLower(c)]);
                    }
                    else
                    {
                        result.Append(c);
                    }
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            // Remove any delimiters, assuming they might be present (like "-")
            string cleanHexString = hexString.Replace("-", "");

            // Convert the cleaned hex string to a byte array
            byte[] byteArray = Enumerable.Range(0, cleanHexString.Length)
                                        .Where(x => x % 2 == 0)
                                        .Select(x => Convert.ToByte(cleanHexString.Substring(x, 2), 16))
                                        .ToArray();
            return byteArray;
        }
        
        public void SeedFakeData(string imageFolder)
        {
            connection.Open();

            byte[] key = new byte[16] {
                0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x4e, 0x35, 0x0b, 0x34, 0x78, 0x55
            };

            // Example IV (128-bit)
            byte[] iv = new byte[16] {
                0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
                0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
            };

            CustomAes aes = new CustomAes(key, iv);

            using (var command = connection.CreateCommand())
            {
                var fake = new Faker("id_ID");
                var imageFiles = Directory.GetFiles(imageFolder, "*.BMP");
                var insertQueries = new List<string>();

                var biodataRecords = new List<(string, string, string, string, string, string, string, string, string, string, string)>();
                var sidikJariRecords = new List<(string, string, string)>();
                var uniquePaths = new HashSet<string>();

                int i = 0;
                while (i < imageFiles.Length)
                {
                    string generatorNIK = fake.Random.Number(10000000, 99999999).ToString() + fake.Random.Number(10000000, 99999999).ToString();
                    byte[] NIKbyteArray = new byte[64];
                    byte[] NIKBytes = Encoding.UTF8.GetBytes(generatorNIK);
                    Array.Copy(NIKBytes, NIKbyteArray, Math.Min(NIKBytes.Length, NIKbyteArray.Length));
                    int paddingSize = NIKbyteArray.Length - NIKBytes.Length;
                    byte padValue = (byte)paddingSize;

                    for (int j = NIKBytes.Length; j < NIKbyteArray.Length; j++)
                    {
                        NIKbyteArray[j] = padValue;
                    }
                    string NIK = BitConverter.ToString(aes.Encrypt(NIKbyteArray));
                    // Console.WriteLine("Generated NIK: " + generatorNIK);
                    // Console.WriteLine("Decrypted NIK: " + Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(NIK))).Substring(0, 16));

                    string realName = GenerateUniqueName(fake);
                    string nama = GenerateRandomCorruptName(realName);
                    byte[] namaByteArray = new byte[64];
                    byte[] namaBytes = Encoding.UTF8.GetBytes(nama);
                    Array.Copy(namaBytes, namaByteArray, Math.Min(namaBytes.Length, namaByteArray.Length));

                    for (int j = namaBytes.Length; j < namaByteArray.Length; j++)
                    {
                        namaByteArray[j] = 0;
                    }

                    //Console.WriteLine("Generated nama: " + nama);
                    nama = BitConverter.ToString(aes.Encrypt(namaByteArray));
                    //string decryptedNama = Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(nama)));
                    //Console.WriteLine("Decrypted nama: " + decryptedNama.TrimEnd('0'));

                    string tempat_lahir = fake.Address.City();
                    byte[] tempatLahirByteArray = new byte[64];
                    byte[] tempatLahirBytes = Encoding.UTF8.GetBytes(tempat_lahir);
                    Array.Copy(tempatLahirBytes, tempatLahirByteArray, Math.Min(tempatLahirBytes.Length, tempatLahirByteArray.Length));

                    for (int j = tempatLahirBytes.Length; j < tempatLahirByteArray.Length; j++)
                    {
                        tempatLahirByteArray[j] = 0;
                    }
                    tempat_lahir = BitConverter.ToString(aes.Encrypt(tempatLahirByteArray));

                    DateTime tanggal_lahir = fake.Date.Past(50, DateTime.Now.AddYears(-18));
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
                    //Console.WriteLine("Generated TanggalLahir: " + tanggalLahir);
                    tanggalLahir = BitConverter.ToString(aes.Encrypt(tanggalLahirByteArray));
                    //string decryptedTanggalLahir = Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(tanggalLahir)));
                    //Console.WriteLine("Decrypted Tanggal Lahir: " + decryptedTanggalLahir);


                    string jenis_kelamin = fake.PickRandom("Laki-Laki", "Perempuan");
                    string golongan_darah = fake.PickRandom("A", "B", "AB", "O");

                    string alamat = fake.Address.FullAddress();
                    byte[] alamatByteArray = new byte[64];
                    byte[] alamatBytes = Encoding.UTF8.GetBytes(alamat);
                    Array.Copy(alamatBytes, alamatByteArray, Math.Min(alamatBytes.Length, alamatByteArray.Length));

                    for (int j = alamatBytes.Length; j < alamatByteArray.Length; j++){
                        alamatByteArray[j] = 0;
                    }
                    //Console.WriteLine("Generated alamat: " + alamat);
                    alamat = BitConverter.ToString(aes.Encrypt(alamatByteArray));
                    //string decryptedAlamat = Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(alamat)));
                    //Console.WriteLine("Decrypted alamat: " + decryptedAlamat.TrimEnd('0'));

                    string agama = fake.PickRandom("Islam", "Kristen", "Katolik", "Hindu", "Buddha");
                    string status_perkawinan = fake.PickRandom("Belum Menikah", "Menikah", "Cerai");

                    string pekerjaan = fake.Name.JobTitle();
                    byte[] pekerjaanByteArray = new byte[64];
                    byte[] pekerjaanBytes = Encoding.UTF8.GetBytes(pekerjaan);
                    Array.Copy(pekerjaanBytes, pekerjaanByteArray, Math.Min(pekerjaanBytes.Length, pekerjaanByteArray.Length));

                    for (int j = pekerjaanBytes.Length; j < pekerjaanByteArray.Length; j++){
                        pekerjaanByteArray[j] = 0;
                    }
                    //Console.WriteLine("Pekerjaan: " + pekerjaan);
                    pekerjaan = BitConverter.ToString(aes.Encrypt(pekerjaanByteArray));

                    string kewarganegaraan = fake.PickRandom("WNI", "WNA");

                    var biodataData = (NIK, nama, tempat_lahir, tanggalLahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan);
                    biodataRecords.Add(biodataData);

                    byte[] realNameByteArray = new byte[64];
                    byte[] realNameBytes = Encoding.UTF8.GetBytes(realName);
                    Array.Copy(realNameBytes, realNameByteArray, Math.Min(realNameBytes.Length, realNameByteArray.Length));

                    for(int k = realNameBytes.Length; k < realNameByteArray.Length; k++){
                        realNameByteArray[k] = 0;
                    }
                    //Console.WriteLine("Generated real name: " + realName);
                    realName = BitConverter.ToString(aes.Encrypt(realNameByteArray));
                    //string decryptedRealName = Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(realName)));
                    //Console.WriteLine("Decrypted Real Name: " + decryptedRealName.TrimEnd('0'));

                    int remainingRecords = imageFiles.Length - i;
                    int rand = fake.Random.Number(1, 5);
                    int randomCount = Math.Min(rand, remainingRecords);
                    for (int j = 0; j < randomCount; j++)
                    {
                        string bmpFile = imageFiles[i + j];
                        string path = Path.Combine("data", Path.GetFileName(bmpFile));
                        byte[] pathByteArray = new byte[64];
                        byte[] pathBytes = Encoding.UTF8.GetBytes(path);
                        Array.Copy(pathBytes, pathByteArray, Math.Min(pathBytes.Length, pathByteArray.Length));

                        for(int k = pathBytes.Length; k < pathByteArray.Length; k++){
                            pathByteArray[k] = 0;
                        }
                        path = BitConverter.ToString(aes.Encrypt(pathByteArray));

                        string berkas_citra = AsciiConverter.ImageToAscii(Path.Combine(bmpFile));

                        if (!uniquePaths.Contains(path))
                        {
                            byte[] berkasCitraByteArray = new byte[2304];
                            byte[] berkasCitraBytes = Encoding.UTF8.GetBytes(berkas_citra);
                            Array.Copy(berkasCitraBytes, berkasCitraByteArray, Math.Min(berkasCitraBytes.Length, berkasCitraByteArray.Length));
                            
                            for(int k = berkasCitraBytes.Length; k < berkasCitraByteArray.Length; k++){
                                berkasCitraByteArray[k] = 0;
                            }
                            //Console.WriteLine("Generated berkas: " + berkas_citra);
                            berkas_citra = BitConverter.ToString(aes.Encrypt(berkasCitraByteArray));
                            //string decryptedBerkas = Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(berkas_citra)));
                            //Console.WriteLine("Decrypted berkas: " + decryptedBerkas.TrimEnd('0'));

                            var sidikJariData = (berkas_citra, realName, path);
                            sidikJariRecords.Add(sidikJariData);
                            uniquePaths.Add(path);
                        }
                        else
                        {
                            Console.WriteLine($"Duplicate path detected: {path}");
                        }
                    }

                    i += randomCount;
                    Console.WriteLine($"Generated {i}/{imageFiles.Length} records");
                }

                // Now insert the generated data into the database
                using (var transaction = connection.BeginTransaction())
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

                        var insertSidikJari = @"
                            INSERT INTO sidik_jari (berkas_citra, realName, path) VALUES (@berkas_citra, @realName, @path)
                        ";

                        command.CommandText = insertBiodata;
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

                        command.CommandText = insertSidikJari;
                        command.Parameters.Clear();
                        command.Parameters.Add("@berkas_citra", MySqlDbType.LongText);
                        command.Parameters.Add("@realName", MySqlDbType.VarChar);
                        command.Parameters.Add("@path", MySqlDbType.VarChar);

                        foreach (var sidikJari in sidikJariRecords)
                        {
                            command.Parameters["@berkas_citra"].Value = sidikJari.Item1;
                            command.Parameters["@realName"].Value = sidikJari.Item2;
                            command.Parameters["@path"].Value = sidikJari.Item3;

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        Console.WriteLine("Inserted records into biodata and sidik_jari tables.");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error inserting records: {ex.Message}");
                    }
                }

            }

            connection.Close();
        }

    }


}
