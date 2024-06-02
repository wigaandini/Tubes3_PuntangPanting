using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Multi
    {
        static void Main(string[] args)
        {
            string sourceImg = "Easy.BMP";
            string folderPath = "../src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/";

            Stopwatch stopwatch = Stopwatch.StartNew();
            TestAllImagesInFolder(folderPath, "kmp", sourceImg);
            stopwatch.Stop();

            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }

        public static (string, double) TestAllImagesInFolder(string folderPath, string method, string sourceImg)
        {
            string[] files = Directory.GetFiles(folderPath, "*.BMP");
            string pattern = MidOne(sourceImg);
            ConcurrentDictionary<string, double> similarities = new ConcurrentDictionary<string, double>();

            Parallel.ForEach(files, file =>
            {
                (string imagePath, double similarity) = ProcessImage(file, pattern, method, sourceImg);
                similarities.TryAdd(imagePath, similarity);
            });

            var maxSimilarity = similarities.Values.Max();
            var imagePath = similarities.FirstOrDefault(kv => kv.Value == maxSimilarity).Key;
            Console.WriteLine($"Most similar image to {sourceImg} is {Path.GetFileName(imagePath)} with similarity {maxSimilarity}%");
            return (imagePath, maxSimilarity);
        }

        public static string MidOne(string imagePath)
        {
            using Bitmap img = new Bitmap(imagePath);
            int width = img.Width;
            int height = img.Height;
            int midHeight = height / 2;
            int midWidthStart = Math.Max((width / 2) - 40, 0);
            int midWidthEnd = Math.Min(midWidthStart + 80, width);
            StringBuilder binaryData = new StringBuilder();

            for (int x = midWidthStart; x < midWidthEnd; x++)
            {
                Color pixel = img.GetPixel(x, midHeight);
                binaryData.Append(pixel.R < 128 ? "0" : "1");
            }

            return ConvertBinaryToString(binaryData.ToString());
        }

        public static (string imagePath, double similarity) ProcessImage(string imagePath, string pattern, string method, string sourceImg)
        {
            double minPercentage = 60.0;
            string text = ImageToAscii(imagePath);
            var result = Levenshtein.MatchWithLevenshtein(pattern, text, minPercentage, method == "kmp" ? 2 : 1);

            if (result.index != -1)
            {
                Console.WriteLine($"{Path.GetFileName(imagePath)} is similar to {sourceImg}");
                Console.WriteLine($"{(method == "kmp" ? "KMP" : "BM")}: Pattern found in string at index {result.textIndex} with similarity {result.similarity}% at position {result.index}.");
            }


            return (imagePath, result.similarity);
        }

        public static string ConvertBinaryToString(string binaryData)
        {
            StringBuilder asciiData = new StringBuilder();
            for (int i = 0; i < binaryData.Length; i += 8)
            {
                if (i + 8 <= binaryData.Length)
                {
                    string byteString = binaryData.Substring(i, 8);
                    int asciiCode = Convert.ToInt32(byteString, 2);
                    asciiData.Append((char)asciiCode);
                }
            }

            return asciiData.ToString();
        }

        public static string ImageToAscii(string imagePath)
        {
            using Bitmap img = new Bitmap(imagePath);
            int width = img.Width;
            int height = img.Height;
            StringBuilder binaryData = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    binaryData.Append(grayValue < 128 ? "0" : "1");
                }
            }

            return ConvertBinaryToString(binaryData.ToString());
        }
    }
}
