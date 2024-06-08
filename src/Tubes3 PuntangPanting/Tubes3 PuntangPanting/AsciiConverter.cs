using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Tubes3_PuntangPanting
{
    class AsciiConverter
    {
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

        public static (int minX, int minY, int cropWidth, int cropHeight) CropImage(Bitmap img)
        {
            int width = img.Width;
            int height = img.Height;

            int minX = width, minY = height, maxX = 0, maxY = 0;

            int buffer = 4;

            // Loop through each pixel to find the bounding box
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

            if (minX > maxX || minY > maxY || minX == width || minY == height || maxX == 0 || maxY == 0 || maxX == width || maxY == height)
            {
                return (0, 0, width, height);
            }

            int cropWidth = maxX - minX + 1;
            int cropHeight = maxY - minY + 1;
            return (minX, minY, cropWidth, cropHeight);
        }

        public static string MidOneBitmap(Bitmap img)
        {
            var (minX, minY, cropWidth, cropHeight) = CropImage(img);

            int width = cropWidth - minX;
            int height = cropHeight - minY;
            int midHeight = 3 * height / 4;
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

    }

}
