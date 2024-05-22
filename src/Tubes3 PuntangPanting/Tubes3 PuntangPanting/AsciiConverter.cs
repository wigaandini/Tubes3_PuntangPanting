using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
namespace Tubes3_PuntangPanting
{
    class AsciiConverter
    {
        static string ImageToAscii(string imagePath)
        {
            using (Bitmap img = new Bitmap(imagePath))
            {
                int width = img.Width;
                int height = img.Height;

                // Grayscaling image
                using (Bitmap grayImg = new Bitmap(img))
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Color pixelColor = grayImg.GetPixel(x, y);
                            int grayValue = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11);
                            Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                            grayImg.SetPixel(x, y, grayColor);
                        }
                    }
                }

                // Get the center 30x30 box
                int startX = width / 2 - 3;
                int startY = height / 2 - 2;
                Rectangle cropRect = new Rectangle(startX, startY, 6, 5);
                using (Bitmap croppedImg = img.Clone(cropRect, img.PixelFormat))
                {
                    // Convert the 30x30 image to ASCII
                    StringBuilder binaryData = new StringBuilder();
                    for (int y = 0; y < 5; y++)
                    {
                        for (int x = 0; x < 6; x++)
                        {
                            int pixelValue = croppedImg.GetPixel(x, y).R;
                            // Create a binary string based on the pixel value
                            binaryData.Append(pixelValue < 128 ? '0' : '1');
                        }
                    }

                    // Convert the binary data to ASCII
                    StringBuilder asciiData = new StringBuilder();
                    for (int i = 0; i < binaryData.Length; i += 8)
                    {
                        string byteString = binaryData.ToString().Substring(i, Math.Min(8, binaryData.Length - i));
                        if (byteString.Length == 8)
                        {
                            char asciiChar = (char)Convert.ToInt32(byteString, 2);
                            asciiData.Append(asciiChar);
                        }
                    }

                    return asciiData.ToString();
                }
            }
        }
    }
}
