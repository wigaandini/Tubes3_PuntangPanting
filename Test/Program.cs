// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Drawing;
// using System.IO;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace Test
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             string sourceImg = "data/29__F_Right_little_finger.BMP";
//             string folderPath = "data/";

//             Stopwatch stopwatch = new Stopwatch();
//             stopwatch.Start();
//             TestAllImagesInFolder(folderPath, "bm", sourceImg);
//             stopwatch.Stop();
//             Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
//         }

//         public static string ImageToAscii(string imagePath)
//         {
//             StringBuilder binaryData = new StringBuilder();
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
//                         binaryData.Append(grayValue < 128 ? "0" : "1");
//                     }
//                 }
//             }

//             for (int i = 0; i < binaryData.Length; i += 8)
//             {
//                 if (i + 8 <= binaryData.Length)
//                 {
//                     string byteString = binaryData.ToString().Substring(i, 8);
//                     int asciiCode = Convert.ToInt32(byteString, 2);
//                     asciiData.Append((char)asciiCode);
//                 }
//             }

//             return asciiData.ToString();
//         }

//         public static string MidOne(string imagePath)
//         {
//             StringBuilder binaryData = new StringBuilder();

//             using (Bitmap img = new Bitmap(imagePath))
//             {
//                 int width = img.Width;
//                 int height = img.Height;

//                 int midHeight = height / 2;
//                 int midWidthStart = Math.Max((width / 2) - 40, 0);
//                 int midWidthEnd = Math.Min(midWidthStart + 80, width);

//                 for (int x = midWidthStart; x < midWidthEnd; x++)
//                 {
//                     Color pixel = img.GetPixel(x, midHeight);
//                     int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
//                     binaryData.Append(grayValue < 128 ? "0" : "1");
//                 }
//             }

//             StringBuilder asciiData = new StringBuilder();

//             for (int i = 0; i < binaryData.Length; i += 8)
//             {
//                 if (i + 8 <= binaryData.Length)
//                 {
//                     string byteString = binaryData.ToString().Substring(i, 8);
//                     int asciiCode = Convert.ToInt32(byteString, 2);
//                     asciiData.Append((char)asciiCode);
//                 }
//             }

//             return asciiData.ToString();
//         }

//         public static double ProcessImage(string imagePath, string pattern, string method, string sourceImg)
//         {
//             double minPercentage = 70.0;
//             string text = ImageToAscii(imagePath);

//             var result = Levenshtein.MatchWithLevenshtein(pattern, text, minPercentage, method == "kmp" ? 2 : 1);

//             if (result.index != -1)
//             {
//                 Console.WriteLine($"{Path.GetFileName(imagePath)} is similar to {sourceImg}");
//                 Console.WriteLine($"{(method == "kmp" ? "KMP" : "BM")}: Pattern found in string at index {result.textIndex} with similarity {result.similarity}% at position {result.index}.");
//             }

//             return result.similarity;
//         }

//         public static void TestAllImagesInFolder(string folderPath, string method, string sourceImg)
//         {
//             string[] files = Directory.GetFiles(folderPath, "*.BMP");
//             string pattern = MidOne(sourceImg);
//             double maxSimilarity = 0.0;

//             foreach (string file in files)
//             {
//                 double similarity = ProcessImage(file, pattern, method, sourceImg);
//                 if (similarity > maxSimilarity)
//                 {
//                     maxSimilarity = similarity;
//                 }
//             }
//         }
//     }
// }
