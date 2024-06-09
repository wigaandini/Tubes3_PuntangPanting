// using System.Text;

// //USE THIS IF YOU ARE GENERATING A RANDOM ENCRYPTED DATABASE
// namespace DatabaseSeeder
// {
//     class SeedingProgram
//     {
//         static void Main(string[] args)
//         {
//             var customServer = "Fairuz";
//             var customUser = "root";
//             var customDatabase = "dummy";
//             var customPassword = "bismillah.33";
//             var connectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={customDatabase}";
//             string imageFolder = @"data";

//             DataSeeder db = new DataSeeder(connectionString);
//             db.CreateBiodataTable();
//             db.CreateSidikJariTable();
//             db.SeedFakeData(imageFolder);
//         }
//     }
// }
