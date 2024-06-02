// using MySql.Data.MySqlClient;
// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.IO;
// using System.Linq;
// using System.Text;
// using Bogus;

// namespace Test
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             var titles = new List<string>
//             {
//                 "SE", "SEI", "SPsi", "MM", "SPt", "MPd", "Dr", "Drs", "Ir", "SH", "MH", "MSi", "MHum", "MA", "MSc",
//                 "PhD", "Prof", "Prof Dr", "Dr Hc", "BSc", "BA", "BEng", "MBA", "LLB", "LLM", "MPhil", "DPhil", "EdD",
//                 "DDS", "DMD", "DO", "DVM", "MD", "MFA", "JD", "PsyD", "ThD", "DMin", "BBA", "AB", "BS", "BM", "BFA",
//                 "MLIS", "MSW", "MPH", "MEd", "MEng", "MArch", "MDes", "MSN", "DSc", "DHEd", "DMus", "DPT", "OTD",
//                 "PharmD", "RN", "NP", "CFA", "CPA", "Esq", "PGDip", "DipHE", "CertHE", "PGCE", "BEd", "MSt", "MMus",
//                 "MAEd", "MChem", "MBiol", "MSocSci", "BAA", "BAppSc", "MComp", "MAcc", "AMd", "SAg", "SPd", "SE",
//                 "SKom", "SHum", "SIKom", "SPt", "SFarm", "SKed", "SKes", "SSi", "SKH", "ST", "STP", "SPsi", "SPt",
//                 "SSos", "SH", "SHut", "SPi", "SGz", "SStat", "STrT", "STrKes", "MAg", "MPd", "MSi", "MKom", "MHum",
//                 "MIKom", "MFarm", "MKed", "MKes", "MSi", "MKH", "MT", "MTP", "MPsi", "MSos", "MH", "MHut", "MPi",
//                 "MGz", "MStat", "MTrT", "MTrKes", "DS", "DAg", "DPd", "DSi", "DKom", "DHum", "DIKom", "DFarm", "DKed",
//                 "DKes", "DT", "DTP", "DPsi", "DSos", "DH", "DHut", "DPi", "DGz", "DStat", "DTrT", "DTrKes", "MTi",
//                 "Mak, msip", "Mak, msi"
//             };

//             var usedNames = new HashSet<string>();

//             // Define SQL connection string
//             string connectionString = "Server=Fairuz;Database=stima;Uid=root;Pwd=bismillah.33;";

//             // Connect to MySQL database
//             using (var connection = new MySqlConnection(connectionString))
//             {
//                 connection.Open();

//                 // Create a MySQL command to execute SQL queries
//                 using (var command = connection.CreateCommand())
//                 {
//                     // Define SQL statements to create tables
//                     string createBiodataTable = @"
//                         CREATE TABLE IF NOT EXISTS `biodata` (
//                             `NIK` varchar(16) NOT NULL,
//                             `nama` varchar(100) DEFAULT NULL,
//                             `tempat_lahir` varchar(50) DEFAULT NULL,
//                             `tanggal_lahir` date DEFAULT NULL,
//                             `jenis_kelamin` enum('Laki-Laki','Perempuan') DEFAULT NULL,
//                             `golongan_darah` varchar(5) DEFAULT NULL,
//                             `alamat` varchar(255) DEFAULT NULL,
//                             `agama` varchar(50) DEFAULT NULL,
//                             `status_perkawinan` enum('Belum Menikah','Menikah','Cerai') DEFAULT NULL,
//                             `pekerjaan` varchar(100) DEFAULT NULL,
//                             `kewarganegaraan` varchar(50) DEFAULT NULL,
//                             PRIMARY KEY (`NIK`)
//                         ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
//                     ";

//                     string createSidikJariTable = @"
//                         CREATE TABLE IF NOT EXISTS `sidik_jari` (
//                             `berkas_citra` LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
//                             `nama` varchar @ (100) DEFAULT NULL,
//                             `path` text NOT NULL,
//                             PRIMARY KEY (`path`(255))
//                         ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
//                     ";

//                     // Execute the SQL create table statements using the command
//                     command.CommandText = createBiodataTable;
//                     command.ExecuteNonQuery();

//                     command.CommandText = createSidikJariTable;
//                     command.ExecuteNonQuery();

//                     // Define SQL insert statements for biodata and sidik_jari
//                     string insertBiodata = @"
//                         INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
//                         VALUES (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)
//                     ";

//                     string insertSidikJari = @"
//                         INSERT INTO sidik_jari (berkas_citra, nama, path) VALUES (@berkas_citra, @nama, @path)
//                     ";

//                     command.CommandText = insertBiodata;
//                     command.Parameters.AddWithValue("@NIK", "");
//                     command.Parameters.AddWithValue("@nama", "");
//                     command.Parameters.AddWithValue("@tempat_lahir", "");
//                     command.Parameters.AddWithValue("@tanggal_lahir", "");
//                     command.Parameters.AddWithValue("@jenis_kelamin", "");
//                     command.Parameters.AddWithValue("@golongan_darah", "");
//                     command.Parameters.AddWithValue("@alamat", "");
//                     command.Parameters.AddWithValue("@agama", "");
//                     command.Parameters.AddWithValue("@status_perkawinan", "");
//                     command.Parameters.AddWithValue("@pekerjaan", "");
//                     command.Parameters.AddWithValue("@kewarganegaraan", "");

//                     // Path to the folder containing images
//                     string imageFolderPath = "test";
//                     var files = Directory.GetFiles(imageFolderPath);
//                     var bmpFiles = files.Where(file => Path.GetExtension(file) == ".BMP").ToList();
//                     int totalData = bmpFiles.Count;

//                     // Collect data first
//                     var biodataRecords = new List<(string, string, string, string, string, string, string, string, string, string, string)>();
//                     var sidikJariRecords = new List<(string, string, string)>();
//                     var uniquePaths = new HashSet<string>();
//                     var uniqueNames = new HashSet<string>();
//                     var random = new Random();
//                     string[] genders = { "Laki-Laki", "Perempuan" };
//                     string[] bloodTypes = { "A", "B", "AB", "O" };
//                     string[] religions = { "Islam", "Kristen", "Katolik", "Hindu", "Buddha" };
//                     string[] maritalStatuses = { "Belum Menikah", "Menikah", "Cerai" };
//                     string[] nationalities = { "WNI", "WNA" };
//                     int i = 0;

//                     while (i < bmpFiles.Count)
//                     {
//                         string fakeName = faker.Name.FullName();
//                         string cleanedName = RemoveNonAlphabetic(fakeName);
//                         string realName = RemoveTitles(cleanedName);
//                         string NIK = GenerateRandomNIK(16);

//                         string nama = GenerateRandomCorruptName(realName);
//                         string tempatLahir = faker.Address.City();
//                         string tanggalLahir = faker.Date.Between(new DateTime(1950, 1, 1), new DateTime(2000, 1, 1)).ToString("yyyy-MM-dd");
//                         string jenisKelamin = genders[random.Next(genders.Length)];
//                         string golonganDarah = bloodTypes[random.Next(bloodTypes.Length)];

//                         string alamat = faker.Address.FullAddress();
//                         string agama = religions[random.Next(religions.Length)];
//                         string statusPerkawinan = maritalStatuses[random.Next(maritalStatuses.Length)];
//                         string pekerjaan = faker.Name.JobTitle();
//                         string kewarganegaraan = nationalities[random.Next(nationalities.Length)];

//                         var biodataData = (
//                             NIK, nama, tempatLahir, tanggalLahir, jenisKelamin, golonganDarah, alamat, agama, statusPerkawinan, pekerjaan, kewarganegaraan
//                         );
//                         biodataRecords.Add(biodataData);

//                         // Create one-to-many relationship
//                         int remainingRecords = totalData - i;
//                         int randomCount = Math.Min(random.Next(1, 6), remainingRecords);
//                         for (int j = 0; j < randomCount; j++)
//                         {
//                             string bmpFile = bmpFiles[i + j];
//                             string berkasCitra = ImageToAscii(Path.Combine(imageFolderPath, bmpFile));
//                             string path = bmpFile;

//                             if (!uniquePaths.Contains(path))
//                             {
//                                 var sidikJariData = (berkasCitra, realName, path);
//                                 sidikJariRecords.Add(sidikJariData);
//                                 uniquePaths.Add(path);
//                             }
//                             else
//                             {
//                                 Console.WriteLine($"Duplicate path detected: {path}");
//                             }
//                         }

//                         i += randomCount;
//                         Console.WriteLine(i);
//                     }

//                     // Execute the SQL insert statements using the command
//                     command.CommandText = insertBiodata;
//                     foreach (var biodataData in biodataRecords)
//                     {
//                         command.Parameters["@NIK"].Value = biodataData.Item1;
//                         command.Parameters["@nama"].Value = biodataData.Item2;
//                         command.Parameters["@tempat_lahir"].Value = biodataData.Item3;
//                         command.Parameters["@tanggal_lahir"].Value = biodataData.Item4;
//                         command.Parameters["@jenis_kelamin"].Value = biodataData.Item5;
//                         command.Parameters["@golongan_darah"].Value = biodataData.Item6;
//                         command.Parameters["@alamat"].Value = biodataData.Item7;
//                         command.Parameters["@agama"].Value = biodataData.Item8;
//                         command.Parameters["@status_perkawinan"].Value = biodataData.Item9;
//                         command.Parameters["@pekerjaan"].Value = biodataData.Item10;
//                         command.Parameters["@kewarganegaraan"].Value = biodataData.Item11;
//                         command.ExecuteNonQuery();
//                     }

//                     command.CommandText = insertSidikJari;
//                     foreach (var sidikJariData in sidikJariRecords)
//                     {
//                         command.Parameters["@berkas_citra"].Value = sidikJariData.Item1;
//                         command.Parameters["@nama"].Value = sidikJariData.Item2;
//                         command.Parameters["@path"].Value = sidikJariData.Item3;
//                         command.ExecuteNonQuery();
//                     }

//                     // Commit changes to the database
//                     connection.Close();
//                 }
//             }

//             Console.WriteLine("Data insertion completed.");
//         }

//         public static string RemoveNonAlphabetic(string name)
//         {
//             return new string(name.Where(char.IsLetterOrDigit).ToArray());
//         }

//         public static string GenerateRandomNIK(int minLength)
//         {
//             Random random = new Random();
//             StringBuilder nikBuilder = new StringBuilder();

//             // Ensure the NIK has at least minLength digits
//             while (nikBuilder.Length < minLength)
//             {
//                 nikBuilder.Append(random.Next(0, 10));
//             }

//             return nikBuilder.ToString();
//         }

//         public static string RemoveTitles(string name)
//         {
//             string[] titles = { /* list of titles */ };
//             string cleanedName = name.ToLower();
//             foreach (var title in titles)
//             {
//                 cleanedName = cleanedName.Replace(title.ToLower(), "");
//             }
//             cleanedName = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(cleanedName);
//             return cleanedName;
//         }

//         public static string GenerateRandomCorruptName(string name)
//         {
//             StringBuilder result = new StringBuilder();
//             Random random = new Random();
//             foreach (char c in name)
//             {
//                 bool change = random.Next(2) == 0;
//                 if (change)
//                 {
//                     result.Append(char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c));
//                 }
//                 else
//                 {
//                     result.Append(c);
//                 }
//             }
//             return result.ToString();
//         }

//         public static string ImageToAscii(string imagePath)
//         {
//             StringBuilder asciiData = new StringBuilder();
//             using (Bitmap img = new Bitmap(imagePath))
//             {
//                 int width = img.Width;
//                 int height = img.Height;
//                 for (int y = 0; y < height; y++)
//                 {
//                     for (int x = 0; x < width; x++)
//                     {
//                         Color pixel = img.GetPixel(x, y);
//                         int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
//                         asciiData.Append(grayValue < 128 ? "0" : "1");
//                     }
//                 }
//             }
//             return ConvertBinaryToString(asciiData.ToString());
//         }

//         public static string ConvertBinaryToString(string binaryData)
//         {
//             StringBuilder asciiData = new StringBuilder();
//             for (int i = 0; i < binaryData.Length; i += 8)
//             {
//                 string byteString = binaryData.Substring(i, 8);
//                 int asciiCode = Convert.ToInt32(byteString, 2);
//                 asciiData.Append((char)asciiCode);
//             }
//             return asciiData.ToString();
//         }
//     }
// }

