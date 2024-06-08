using System.Text;

namespace Tubes3_PuntangPanting
{
    class SeedingProgram
    {
        static void Main(string[] args)
        {
            //string plaintextString = Encoding.UTF8.GetString(plaintext);
            // Console.WriteLine("Plaintext: " + BitConverter.ToString(plaintext));

            // byte[] ciphertext = aes.Encrypt(plaintext);
            // Console.WriteLine("Ciphertext: " + BitConverter.ToString(ciphertext));

            // byte[] decrypted = aes.Decrypt(ciphertext);
            // Console.WriteLine("Decrypted: " + BitConverter.ToString(decrypted));

            // Console.WriteLine("Plaintext: ");
            // string normalString = Console.ReadLine() ?? string.Empty; // Your normal string
            // byte[] byteArray = new byte[256];

            // Convert the string to a byte array using UTF-8 encoding
            // byte[] stringBytes = Encoding.UTF8.GetBytes(normalString);

            // Copy the string bytes to the byte array
            // Array.Copy(stringBytes, byteArray, Math.Min(stringBytes.Length, byteArray.Length));

            // Calculate padding size
            // int paddingSize = byteArray.Length - stringBytes.Length;
            // byte padValue = (byte)paddingSize;

            // Apply PKCS#7 padding
            // for (int i = stringBytes.Length; i < byteArray.Length; i++)
            // {
            //     byteArray[i] = padValue;
            // }

            //string plaintextString = Encoding.UTF8.GetString(plaintext);
            //Console.WriteLine("\nInput: " + BitConverter.ToString(byteArray));
            //Æ7ÞäaóÖg1ç|þÜ`[b#3I¦|H$Áe²+l%òÓ)¶KL¤Û)I$ $ÌRfIi{#f­¤Éµ
            // Console.WriteLine("\nInput: " + Encoding.UTF8.GetString(byteArray));

            // byte[] ciphertext = aes.Encrypt(byteArray);
            // Console.WriteLine("Ciphertext: " + BitConverter.ToString(ciphertext));

            // byte[] decrypted = aes.Decrypt(ciphertext);
            // Console.WriteLine("Decrypted: " + BitConverter.ToString(decrypted));
            // Console.WriteLine("Decrypted: " + Encoding.UTF8.GetString(decrypted));

            var customServer = "localhost";
            var customUser = "root";
            var customDatabase = "datastima";
            var customPassword = "root";
            var connectionString = $"Server={customServer};User Id={customUser};Password={customPassword};Database={customDatabase}";
            string imageFolder = @"../Test/data";

            DataSeeder db = new DataSeeder(connectionString);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();
            db.SeedFakeData(imageFolder);
        }
    }
}
