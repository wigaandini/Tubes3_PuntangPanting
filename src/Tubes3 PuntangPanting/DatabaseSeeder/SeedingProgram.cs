using System;

namespace DatabaseSeeder
{
    class SeedingProgram
    {
        static void Main(string[] args)
        {

            var customServer = "Fairuz";
            var customUser = "root";
            var customDatabase = "s";
            var customPassword = "bismillah.33";
            var connectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={customDatabase}";
            string imageFolder = @"../Tubes3 PuntangPanting/data";

            DataSeeder db = new DataSeeder(connectionString);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();
            db.SeedFakeData(imageFolder);
        }
    }
}
