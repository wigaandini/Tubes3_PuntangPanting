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
        private MySqlConnection connection = new MySqlConnection();
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
            string query = "SELECT * FROM `biodata`";
            return ExecuteQuery(query);
        }

        public DataTable ReadSidikJari()
        {
            string query = "SELECT * FROM `sidik_jari`";
            return ExecuteQuery(query);
        }


        public List<string> ReadNameLeft()
        {
            string query = "SELECT nama FROM `biodata`";
            DataTable? dataTable = ExecuteQuery(query);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return new List<string>();
            }

            List<string> dataList = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                string? value = row["nama"]?.ToString();
                if (value != null)
                {
                    dataList.Add(value);
                }
            }

            return dataList;
        }

        public DataRow ReadDataByBerkas(string berkas, DataTable sidikJariTable)
        {
            // Validate input to prevent SQL injection
            if (string.IsNullOrEmpty(berkas) || sidikJariTable == null || sidikJariTable.Rows.Count == 0)
            {
                return sidikJariTable?.NewRow() ?? new DataTable().NewRow();
            }

            // Enclose the column name in square brackets to handle spaces
            DataRow[] result = sidikJariTable.Select($"[path] = '{berkas.Replace("'", "''")}'");
            if (result.Length > 0)
            {
                return result[0];
            }
            return sidikJariTable.NewRow();
        }

        public DataTable ReadBiodataByName(string name, DataTable biodataTable)
        {
            DataTable filteredTable = new DataTable();
            // Validate input to prevent SQL injection
            if (!string.IsNullOrEmpty(name) && biodataTable != null && biodataTable.Rows.Count > 0)
            {
                filteredTable = biodataTable.Clone();
                // Enclose the column name in square brackets to handle spaces
                DataRow[] matchingRows = biodataTable.Select($"[nama] = '{name.Replace("'", "''")}'");
                foreach (DataRow row in matchingRows)
                {
                    filteredTable.ImportRow(row);
                }
            }
            return filteredTable;
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