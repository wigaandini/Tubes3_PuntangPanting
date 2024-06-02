using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;

namespace Tubes3_PuntangPanting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private Bitmap imgUpload;

        private Database db;
        private DataTable biodataTable ;
        private DataTable sidikJariTable ;

        public MainWindow()
        {
            string customServer = "Fairuz";
            string customUser = "root";
            string customDatabase = "test";
            string customPassword = "bismillah.33";

            db = new Database(customServer, customUser, customDatabase, customPassword);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();
            biodataTable =db.ReadBiodata();
            sidikJariTable= db.ReadSidikJari();

            DisplayBiodataTable();
        }

        private void DisplayBiodataTable()
        {
            DataTable dt = db.ReadBiodata();
        

            MessageBox.Show(dt.Rows.Count > 0 ? "Data masuk ke tabel biodata" : "Tidak ada data di tabel biodata");
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

    private void UploadImage1_Click(object sender, RoutedEventArgs e)
{
    OpenFileDialog openFileDialog = new OpenFileDialog();
    if (openFileDialog.ShowDialog() == true)
    {
        string filePath = openFileDialog.FileName;
        BitmapImage bitmap = new BitmapImage(new Uri(filePath));
        Bitmap bp = new Bitmap(filePath);

        // Convert the image to a format that supports direct pixel manipulation
        Bitmap convertedBitmap = new Bitmap(bp.Width, bp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        using (Graphics g = Graphics.FromImage(convertedBitmap))
        {
            g.DrawImage(bp, new System.Drawing.Rectangle(0, 0, bp.Width, bp.Height));
        }

        image1.Source = bitmap;

        if (convertedBitmap != null)
        {
            imgUpload = convertedBitmap;
        }
    }
}


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isKMPSelected = tbRight.IsChecked ?? false;
                string ascii = AsciiConverter.HighestDarkAscii(imgUpload);
                double minPercentage = 70.0;
                Stopwatch stopwatch = new Stopwatch();

                // Convert DataTable to List<string> for asciiData
                List<string> asciiData = new List<string>();
                foreach (DataRow row in sidikJariTable.Rows)
                {
                    string asciiString = row["berkas_citra"].ToString();

                    asciiData.Add(asciiString);
                }   

                (int textIndex, int matchIndex, double similarity) result;
                if (isKMPSelected)
                {
                    Console.WriteLine("Using KMP algorithm");
                    stopwatch.Restart();
                    result = Levenshtein.MatchWithLevenshtein(ascii, asciiData, minPercentage, 1);
                    stopwatch.Stop();
                    MessageBox.Show("KMP algorithm selected");
                }
                else
                {
                    Console.WriteLine("Using BM algorithm");
                    stopwatch.Restart();
                    result = Levenshtein.MatchWithLevenshtein(ascii, asciiData, minPercentage, 2);
                    stopwatch.Stop();
                    MessageBox.Show("BM algorithm selected");
                }

                Console.WriteLine("Matching process completed");

                // Update duration label
                durationLabel.Text = $"{stopwatch.ElapsedMilliseconds} ms";

                // Check if a valid match was found
                if (result.textIndex == -1 || result.matchIndex == -1)
                {
                    Console.WriteLine("No match found with the required similarity percentage.");
                    MessageBox.Show("No match found.");
                    PercentageLabel.Text = "0%";
                    return;
                }

                DataRow matchedData = ReadDataByBerkas(asciiData[result.textIndex], sidikJariTable);
                if (matchedData == null)
                {
                    Console.WriteLine("No matching data found in sidik_jari table.");
                    MessageBox.Show("No matching data found.");
                    PercentageLabel.Text = "0%";
                    return;
                }

                string sameName = matchedData["nama"].ToString();
                MessageBox.Show(sameName);
                string foundName = "";
                List<string> arrName = db.ReadNameLeft();

                foreach (string name in arrName)
                {
                    bool isSame = TextProcessing.CompareWord(sameName, name);
                    if (isSame)
                    {
                        foundName = name;
                        break;
                    }
                }
                MessageBox.Show(foundName);

                Console.WriteLine("Name comparison process completed");

                DataTable resData = new DataTable();
                if (!string.IsNullOrEmpty(foundName))
                {
                    resData = ReadBiodataByName(foundName, biodataTable);
                }

                Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Text Index: {result.textIndex}, Match Index: {result.matchIndex}, Similarity: {result.similarity}");
                Console.WriteLine($"Found Name: {foundName}");


                // Update percentage label
                PercentageLabel.Text = $"{result.similarity}%";

                // Display the matched fingerprint image and biodata
                if (matchedData != null)
                {
                    string imagePath = matchedData["path"].ToString();
                    DisplayMatchedFingerprint(imagePath);
                }

                if (resData.Rows.Count > 0)
                {
                    DisplayBiodata(resData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private DataTable ReadBiodataByName(string name, DataTable biodataTable)
        {
            DataTable filteredTable = new DataTable();
            if (!string.IsNullOrEmpty(name) && biodataTable != null && biodataTable.Rows.Count > 0)
            {
                // Clone the structure of the original table
                filteredTable = biodataTable.Clone();

                // Select rows with the specified name
                DataRow[] matchingRows = biodataTable.Select($"nama = '{name}'");
                foreach (DataRow row in matchingRows)
                {
                    filteredTable.ImportRow(row);
                }
            }
            return filteredTable;
        }
       
        private void DisplayMatchedFingerprint(string imagePath)
        {
            // Assuming you have an Image control named imgMatchedFingerprint
            var path = Path.Combine(Environment.CurrentDirectory,imagePath);
            var uri = new Uri(path);
            if(uri !=null) {
                imgMatchedFingerprint.Source = new BitmapImage(uri);
            }
            {
                Console.WriteLine("Image path is invalid or file does not exist.");
                MessageBox.Show("Ga masuk");
            }
        }

        private void DisplayBiodata(DataTable biodataTable)
        {
            // Assuming you have a TextBlock or other control named biodataLabel to display the biodata
            if (biodataTable.Rows.Count == 0)
            {
                biodataLabel.Text = "Biodata";
                biodataLabel.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                StringBuilder biodataText = new StringBuilder();
                foreach (DataRow row in biodataTable.Rows)
                {
                    biodataText.AppendLine($"NIK: {row["NIK"]}");
                    biodataText.AppendLine($"Nama: {row["nama"]}");
                    biodataText.AppendLine($"Tempat Lahir: {row["tempat_lahir"]}");
                    biodataText.AppendLine($"Tanggal Lahir: {row["tanggal_lahir"]}");
                    biodataText.AppendLine($"Jenis Kelamin: {row["jenis_kelamin"]}");
                    biodataText.AppendLine($"Golongan Darah: {row["golongan_darah"]}");
                    biodataText.AppendLine($"Alamat: {row["alamat"]}");
                    biodataText.AppendLine($"Agama: {row["agama"]}");
                    biodataText.AppendLine($"Status Perkawinan: {row["status_perkawinan"]}");
                    biodataText.AppendLine($"Pekerjaan: {row["pekerjaan"]}");
                    biodataText.AppendLine($"Kewarganegaraan: {row["kewarganegaraan"]}");
                    biodataText.AppendLine();
                }
                biodataLabel.Text = biodataText.ToString();
                biodataLabel.Foreground = new SolidColorBrush(Colors.White);
                biodataLabel.TextAlignment= TextAlignment.Center;
                biodataLabel.HorizontalAlignment= HorizontalAlignment.Center;
                biodataLabel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }


        private DataRow ReadDataByBerkas(string berkas, DataTable sidikJariTable)
        {
            // Filter the sidikJariTable based on the berkas and return the DataRow
            DataRow[] result = sidikJariTable.Select($"berkas_citra = '{berkas}'");
            if (result.Length > 0)
            {
                return result[0];
            }
            return null;
        }

    }


}