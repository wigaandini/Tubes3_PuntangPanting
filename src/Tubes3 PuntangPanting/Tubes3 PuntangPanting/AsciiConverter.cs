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
        public static (int, int) FindDominantDarkArea(Bitmap img, int windowSize = 8)
        {
            int width = img.Width;
            int height = img.Height;
            int minIntensitySum = int.MaxValue;
            (int, int) dominantCoords = (0, 0);

            for (int y = 0; y <= height - windowSize; y++)
            {
                for (int x = 0; x <= width - windowSize; x++)
                {
                    int intensitySum = 0;
                    for (int dy = 0; dy < windowSize; dy++)
                    {
                        for (int dx = 0; dx < windowSize; dx++)
                        {
                            Color pixel = img.GetPixel(x + dx, y + dy);
                            intensitySum += pixel.R + pixel.G + pixel.B;
                        }
                    }
                    if (intensitySum < minIntensitySum)
                    {
                        minIntensitySum = intensitySum;
                        dominantCoords = (x, y);
                    }
                }
            }

            return dominantCoords;
        }

        public static string HighestDarkAscii(Bitmap img)
        {
                (int x, int y) = FindDominantDarkArea(img);
                Rectangle cropBox = new Rectangle(x, y, 8, 4);
                Bitmap croppedImg = img.Clone(cropBox, img.PixelFormat);

                // Convert cropped image to grayscale
                for (int i = 0; i < croppedImg.Width; i++)
                {
                    for (int j = 0; j < croppedImg.Height; j++)
                    {
                        Color originalColor = croppedImg.GetPixel(i, j);
                        int grayscale = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                        Color grayColor = Color.FromArgb(grayscale, grayscale, grayscale);
                        croppedImg.SetPixel(i, j, grayColor);
                    }
                }

                // Evaluation the 8x4 image to Binary 
                StringBuilder binaryData = new StringBuilder();
                for (int y1 = 0; y1 < 4; y1++)
                {
                    for (int x1 = 0; x1 < 8; x1++)
                    {
                        int pixelValue = croppedImg.GetPixel(x1, y1).R;
                        binaryData.Append(pixelValue < 128 ? '0' : '1');
                    }
                }

                // Convert the binary data to ASCII
                StringBuilder asciiData = new StringBuilder();
                for (int i = 0; i < binaryData.Length; i += 8)
                {
                    string byteString = binaryData.ToString(i, 8);
                    if (byteString.Length == 8)
                    {
                        int byteValue = Convert.ToInt32(byteString, 2);
                        asciiData.Append((char)byteValue);
                    }
                }

                return asciiData.ToString();
            }

        public static string ImageAscii(string imagePath, int targetWidth = 8, int targetHeight = 4)
        {
            using (Bitmap img = new Bitmap(imagePath))
            {
                // Grayscaling image
                for (int i = 0; i < img.Width; i++)
                {
                    for (int j = 0; j < img.Height; j++)
                    {
                        Color originalColor = img.GetPixel(i, j);
                        int grayscale = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);
                        Color grayColor = Color.FromArgb(grayscale, grayscale, grayscale);
                        img.SetPixel(i, j, grayColor);
                    }
                }

                StringBuilder asciiData = new StringBuilder();

                for (int startY = 0; startY <= img.Height - targetHeight; startY++)
                {
                    for (int startX = 0; startX <= img.Width - targetWidth; startX++)
                    {
                        StringBuilder binaryData = new StringBuilder();
                        for (int y = 0; y < targetHeight; y++)
                        {
                            for (int x = 0; x < targetWidth; x++)
                            {
                                int pixelValue = img.GetPixel(startX + x, startY + y).R;
                                binaryData.Append(pixelValue < 128 ? '0' : '1');
                            }
                        }

                        for (int i = 0; i < binaryData.Length; i += 8)
                        {
                            string byteString = binaryData.ToString(i, 8);
                            if (byteString.Length == 8)
                            {
                                int byteValue = Convert.ToInt32(byteString, 2);
                                asciiData.Append((char)byteValue);
                            }
                        }
                    }
                }

                return asciiData.ToString();
            }
        }
    }

}
