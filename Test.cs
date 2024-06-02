
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        string sourceImg = "src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/1__M_Left_little_finger.BMP";
        string folderPath = "src/Tubes3 PuntangPanting/Tubes3 PuntangPanting/data/";
        
        DateTime start = DateTime.Now;
        TestAllImagesInFolder(folderPath, "kmp", sourceImg);
        DateTime end = DateTime.Now;
        Console.WriteLine($"Time taken: {(end - start).TotalSeconds} seconds");
    }

    public static string ImageToAscii(string imagePath)
    {
        using (Bitmap img = new Bitmap(imagePath))
        {
            int width = img.Width;
            int height = img.Height;

            // Convert image to grayscale
            Bitmap grayImg = new Bitmap(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = img.GetPixel(x, y);
                    int grayValue = (int)(pixel.R * 0.3 + pixel.G * 0.59 + pixel.B * 0.11);
                    grayImg.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }

            if (width % 8 != 0)
            {
                width -= width % 8;
                grayImg = new Bitmap(grayImg, new Size(width, height));
            }

            string binaryData = "";
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pixelValue = grayImg.GetPixel(x, y).R;
                    binaryData += pixelValue < 128 ? "0" : "1";
                }
            }

            string asciiData = "";
            for (int i = 0; i < binaryData.Length; i += 8)
            {
                if (i + 8 <= binaryData.Length)
                {
                    string byteString = binaryData.Substring(i, 8);
                    int asciiCode = Convert.ToInt32(byteString, 2);
                    asciiData += (char)asciiCode;
                }
            }

            return asciiData;
        }
    }

    public static string MidOne(string imagePath)
    {
        using (Bitmap img = new Bitmap(imagePath))
        {
            int width = img.Width;
            int height = img.Height;

            int midHeight = height / 2;
            int midWidthStart = Math.Max((width / 2) - 40, 0);
            int midWidthEnd = Math.Min(midWidthStart + 80, width);

            Bitmap cropImg = img.Clone(new Rectangle(midWidthStart, midHeight, midWidthEnd - midWidthStart, 1), img.PixelFormat);

            string binaryData = "";
            for (int x = 0; x < cropImg.Width; x++)
            {
                int pixelValue = cropImg.GetPixel(x, 0).R;
                binaryData += pixelValue < 128 ? "0" : "1";
            }

            string asciiData = "";
            for (int i = 0; i < binaryData.Length; i += 8)
            {
                if (i + 8 <= binaryData.Length)
                {
                    string byteString = binaryData.Substring(i, 8);
                    int asciiCode = Convert.ToInt32(byteString, 2);
                    asciiData += (char)asciiCode;
                }
            }

            return asciiData;
        }
    }

    public static void ProcessImage(string imagePath, string pattern, string method, string sourceImg)
    {
        string asciiData = ImageToAscii(imagePath);

        if (method == "kmp")
        {
            KMP kmp = new KMP(pattern);
            int result = kmp.Search(asciiData);
            if (result != -1)
            {
                Console.WriteLine($"{Path.GetFileName(imagePath)} is similar to {sourceImg} at index {result}");
            }
        }
        else
        {
            BM bm = new BM(pattern);
            int result = bm.Search(asciiData);
            if (result != -1)
            {
                Console.WriteLine($"{Path.GetFileName(imagePath)} is similar to {sourceImg} at index {result}");
            }
        }
    }

    public static void TestAllImagesInFolder(string folderPath, string method, string sourceImg)
    {
        string[] files = Directory.GetFiles(folderPath, "*.BMP");
        string pattern = MidOne(sourceImg);

        Parallel.ForEach(files, (file) =>
        {
            ProcessImage(file, pattern, method, sourceImg);
        });
    }
}

// Implementasi KMP dan BM perlu ditambahkan sesuai dengan algoritma yang dimiliki pada modul 'algo' dalam Python
public class KMP
{
    private string pattern;

    public KMP(string pattern)
    {
        this.pattern = pattern;
    }

    public int Search(string text)
    {
        int[] lps = ComputeLPSArray();
        int i = 0;
        int j = 0;
        while (i < text.Length)
        {
            if (pattern[j] == text[i])
            {
                j++;
                i++;
            }

            if (j == pattern.Length)
            {
                return i - j;
            }
            else if (i < text.Length && pattern[j] != text[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }
        return -1;
    }

    private int[] ComputeLPSArray()
    {
        int[] lps = new int[pattern.Length];
        int length = 0;
        int i = 1;
        lps[0] = 0;

        while (i < pattern.Length)
        {
            if (pattern[i] == pattern[length])
            {
                length++;
                lps[i] = length;
                i++;
            }
            else
            {
                if (length != 0)
                {
                    length = lps[length - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }
        return lps;
    }
}

public class BM
{
    private string pattern;
    private int[] badChar;

    public BM(string pattern)
    {
        this.pattern = pattern;
        badChar = new int[256];
        PreprocessBadChar();
    }

    public int Search(string text)
    {
        int m = pattern.Length;
        int n = text.Length;

        int s = 0;
        while (s <= (n - m))
        {
            int j = m - 1;

            while (j >= 0 && pattern[j] == text[s + j])
            {
                j--;
            }

            if (j < 0)
            {
                return s;
            }
            else
            {
                s += Math.Max(1, j - badChar[text[s + j]]);
            }
        }
        return -1;
    }

    private void PreprocessBadChar()
    {
        for (int i = 0; i < 256; i++)
        {
            badChar[i] = -1;
        }

        for (int i = 0; i < pattern.Length; i++)
        {
            badChar[(int)pattern[i]] = i;
        }
    }
}
