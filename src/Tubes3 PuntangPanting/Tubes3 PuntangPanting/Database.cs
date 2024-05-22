using MySql.Data.MySqlClient;
using System.Data;

namespace Tubes3_PuntangPanting
{
    public class Database
    {
        private string server;
        private string user;
        private string databaseName;
        private string password;
        private MySqlConnection connection;

        public Database(string server, string user, string databaseName, string password)
        {
            this.server = server;
            this.user = user;
            this.databaseName = databaseName;
            this.password = password;
            Initialize();
        }

        private void Initialize()
        {
            string connectionString = $"server={server};user={user};database={databaseName};password={password};";
            connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                Console.WriteLine("Database connection successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                Console.WriteLine("Database connection closed.");
            }
        }

        public void CreateBiodataTable()
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS `biodata` (
                  `NIK` VARCHAR(16) NOT NULL,
                  `nama` VARCHAR(100) NULL,
                  `tempat_lahir` VARCHAR(50) NULL,
                  `tanggal_lahir` DATE NULL,
                  `jenis_kelamin` ENUM('Laki-Laki','Perempuan') NULL,
                  `golongan_darah` VARCHAR(5) NULL,
                  `alamat` VARCHAR(255) NULL,
                  `agama` VARCHAR(50) NULL,
                  `status_perkawinan` ENUM('Belum Menikah','Menikah','Cerai') NULL,
                  `pekerjaan` VARCHAR(100) NULL,
                  `kewarganegaraan` VARCHAR(50) NULL,
                  PRIMARY KEY (`NIK`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";

            ExecuteNonQuery(query);
        }

        public void CreateSidikJariTable()
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS `sidik_jari` (
                  `berkas_citra` TEXT NULL,
                  `nama` VARCHAR(100) NULL,
                  `path` TEXT NOT NULL,
                  PRIMARY KEY (`path`(255))
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
            ";

            ExecuteNonQuery(query);
        }

        private void ExecuteNonQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Query executed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }
        }

        public DataTable ReadBiodata()
        {
            string query = "SELECT * FROM biodata";
            return ExecuteQuery(query);
        }

        public DataTable ReadSidikJari()
        {
            string query = "SELECT * FROM sidik_jari";
            return ExecuteQuery(query);
        }

        public List<string> ReadAsciiData()
        {
            string query = "SELECT berkas_citra FROM sidik_jari";
            DataTable dataTable = ExecuteQuery(query);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return new List<string>();
            }

            List<string> dataList = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                string value = row["berkas_citra"].ToString();
                dataList.Add(value);
            }

            return dataList;
        }

        public List<string> ReadNameLeft()
        {
            string query = "SELECT nama FROM biodata";
            DataTable dataTable = ExecuteQuery(query);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return new List<string>();
            }

            List<string> dataList = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                string value = row["nama"].ToString();
                dataList.Add(value);
            }

            return dataList;
        }
        public string ReadNameByBerkas(string berkas)
        {
            try
            {
                string query = "SELECT nama FROM sidik_jari WHERE berkas_citra = @berkas";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@berkas", berkas);

                DataTable result = new DataTable();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }

                if (result.Rows.Count > 0)
                {
                    object nameObj = result.Rows[0]["nama"];
                    return nameObj?.ToString() ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
                return string.Empty;
            }
        }

        public DataTable ReadBiodataByName(string nama)
        {
            try
            {
                string query = "SELECT * FROM biodata WHERE nama = @nama";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@nama", nama);

                DataTable result = new DataTable();
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(result);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
                return new DataTable(); // Or handle the exception as needed
            }
        }



        private DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            try
            {
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing query: {ex.Message}");
            }

            return dt;
        }

        public void UpdateBiodata(string nik, string newNama)
        {
            string query = $"UPDATE biodata SET nama='{newNama}' WHERE NIK='{nik}'";
            ExecuteNonQuery(query);
        }

        public void DeleteBiodata(string nik)
        {
            string query = $"DELETE FROM biodata WHERE NIK='{nik}'";
            ExecuteNonQuery(query);
        }
    }
}