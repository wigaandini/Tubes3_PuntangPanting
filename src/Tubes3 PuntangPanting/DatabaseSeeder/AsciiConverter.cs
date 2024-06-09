using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace DatabaseSeeder
{
    class AsciiConverter
    {
        public static string ConvertBinaryToString(string binaryData)
        {
            StringBuilder asciiData = new StringBuilder();
            for (int i = 0; i <= binaryData.Length - 8; i += 8)
            {
                int asciiCode = Convert.ToInt32(binaryData.Substring(i, 8), 2);
                asciiData.Append((char)asciiCode);
            }
            return asciiData.ToString();
        }

        public static void ReturnImage(Bitmap img, int minX, int minY, int maxX, int maxY, string imagePath)
        {
            if (minX >= maxX || minY >= maxY)
            {
                Console.WriteLine($"Invalid cropping bounds, skipping cropping. MIN: {minX} {maxX} MAX: {minY} {maxY}");
                return;
            }

            var cropRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            using var croppedImage = img.Clone(cropRect, img.PixelFormat);

            string outputDirectory = "temp/";
            Directory.CreateDirectory(outputDirectory);
            string outputFileName = Path.GetFileNameWithoutExtension(imagePath) + "_cropped.png";
            string outputPath = Path.Combine(outputDirectory, outputFileName);

            croppedImage.Save(outputPath);
        }

        public static string ImageToAscii(string imagePath)
        {
            const bool isCropped = false;
            using var img = new Bitmap(imagePath);
            int minY, minX, maxY, maxX;
            if (isCropped)
            {
                (minX, minY, maxX, maxY) = CropImage(img);
                ReturnImage(img, minX, minY, maxX, maxY, imagePath);
            }
            else
            {
                (minX, minY, maxX, maxY) = (0, 0, img.Width, img.Height);
            }
            StringBuilder binaryData = new StringBuilder((maxX - minX) * (maxY - minY));

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    binaryData.Append(grayValue < 128 ? '0' : '1');
                }
            }

            return ConvertBinaryToString(binaryData.ToString());
        }

        public static (int minX, int minY, int maxX, int maxY) CropImage(Bitmap img)
        {
            int width = img.Width, height = img.Height;
            int minX = width, minY = height, maxX = 0, maxY = 0;
            const int buffer = 4;

            for (int y = buffer; y < height - buffer; y++)
            {
                for (int x = buffer; x < width - buffer; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);

                    if (grayValue < 128)
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            if (minX >= maxX || minY >= maxY || minX == width || minY == height || maxX == 0 || maxY == 0)
            {
                Console.WriteLine("No significant bounds found, using full image.");
                return (0, 0, width, height);
            }

            return (minX, minY, maxX, maxY);
        }

        public static string MidOneBitmap(Bitmap img)
        {
            var (minX, minY, maxX, maxY) = CropImage(img);

            int width = maxX - minX;
            int height = maxY - minY;
            int midHeight = 3 * height / 4;

            int midWidthStart = Math.Max((width / 2) - 40, 0);
            int midWidthEnd = Math.Min(midWidthStart + 80, width);


            StringBuilder binaryData = new StringBuilder(midWidthEnd - midWidthStart);

            for (int x = midWidthStart; x < midWidthEnd; x++)
            {
                Color pixel = img.GetPixel(x, midHeight);
                int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                binaryData.Append(grayValue < 128 ? '0' : '1');
            }

            return ConvertBinaryToString(binaryData.ToString());
        }
    }
}
