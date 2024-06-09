using System.Drawing;
using System.IO;
using System.Text;

namespace Tubes3_PuntangPanting
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
            using var img = new Bitmap(imagePath);
            int minY, minX, maxY, maxX;

            (minX, minY, maxX, maxY) = (0, 0, img.Width, img.Height);
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
            const int buffer = 4;
            int minX = width, minY = height, maxX = 0, maxY = 0;
            const int threshold = 20;

            for (int y = buffer; y < height - buffer; y++)
            {
                int count = 0;
                for (int x = buffer; x < width - buffer; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);

                    if (grayValue < 128)
                    {
                        count++;
                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);
                    }
                }
                if (count >= threshold)
                {
                    minY = Math.Min(minY, y);
                    maxY = Math.Max(maxY, y);
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
            var (minX, minY, maxX, maxY) = (0, 0, img.Width, img.Height);

            int width = maxX - minX;
            int height = maxY - minY;
            int midHeight = 5 * height / 8;

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
