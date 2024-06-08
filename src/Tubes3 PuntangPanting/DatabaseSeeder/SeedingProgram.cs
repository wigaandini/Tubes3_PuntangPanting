using System.Text;

//USE THIS IF YOU ARE GENERATING A RANDOM ENCRYPTED DATABASE
namespace DatabaseSeeder
{
    class SeedingProgram
    {
        static void Main(string[] args)
        {
            var customServer = "localhost";
            var customUser = "root";
            var customDatabase = "datastimaencrypted";
            var customPassword = "root";
            var connectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={customDatabase}";
            string imageFolder = @"../Tubes3 PuntangPanting/data";

            DataSeederEncrypted db = new DataSeederEncrypted(connectionString);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();
            db.SeedFakeData(imageFolder);
        }
    }
}
