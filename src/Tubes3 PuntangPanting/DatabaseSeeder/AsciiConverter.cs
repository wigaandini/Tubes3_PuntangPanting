using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace DatabaseSeeder
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

    }

}
