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
using System.Collections.Concurrent;

namespace Tubes3_PuntangPanting
{

    /// Interaction logic for MainWindow.xaml

    public partial class MainWindow : Window
    {
        private Bitmap imgUpload; // Stores the uploaded image
        private Database db; // Database instance
        private DataTable biodataTable; // Table for storing biodata
        private DataTable sidikJariTable; // Table for storing fingerprint data

        public MainWindow()
        {
            InitializeComponent();

            // Custom database connection details
            string customServer = "Fairuz";
            string customUser = "root";
            string customDatabase = "stima";
            string customPassword = "bismillah.33";

            // Initialize database and create necessary tables
            db = new Database(customServer, customUser, customDatabase, customPassword);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();

            // Read data from database
            biodataTable = db.ReadBiodata();
            sidikJariTable = db.ReadSidikJari();

            // Display biodata table content
            DisplayBiodataTable();
        }


        /// Displays the biodata table content.
        private void DisplayBiodataTable()
        {
            DataTable dt = db.ReadBiodata();
            MessageBox.Show(dt.Rows.Count > 0 ? "Data masuk ke tabel biodata" : "Tidak ada data di tabel biodata");
        }


        /// Handles mouse down event on the grid for window drag.
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove(); // Enable window drag
            }
        }


        /// Handles the close button click event.
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the application
        }


        /// Handles the fullscreen button click event.
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between normal and maximized window states
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }


        /// Handles the upload image button click event.

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

                // Store the converted image for later processing
                if (convertedBitmap != null)
                {
                    imgUpload = convertedBitmap;
                }
                buttonUpload.Visibility = Visibility.Hidden;

            }
        }



        /// Handles the button click event to start image processing.
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingProgressBar.Visibility = Visibility.Visible; // Show loading indicator
                bool isKMPSelected = tbRight.IsChecked ?? false;
                if (imgUpload == null)
                {
                    return; // No image uploaded
                }

                // Convert the uploaded image to ASCII string pattern
                string pattern = AsciiConverter.MidOneBitmap(imgUpload);

                // Multithread lock mutex variable
                string method = isKMPSelected ? "kmp" : "bm";
                Stopwatch stopwatch = new Stopwatch();
                ConcurrentDictionary<string, double> similarities = new ConcurrentDictionary<string, double>();
                stopwatch.Start();

                await Task.Run(() =>
                {
                    Parallel.ForEach(sidikJariTable.AsEnumerable(), (datarow, state) =>
                    {
                        string text = datarow["berkas_citra"].ToString();
                        string path = datarow["path"].ToString();
                        double minPercentage = 70.0;
                        var result = Levenshtein.MatchWithLevenshtein(pattern, text, minPercentage, method == "kmp" ? 2 : 1);
                        similarities.TryAdd(path, result.similarity);

                        if (result.similarity == 100.0) 
                        {
                            state.Break();
                        }
                    });
                });

                stopwatch.Stop();
                var maxSimilarity = similarities.Values.Max();
                var imagePath = similarities.FirstOrDefault(kv => kv.Value == maxSimilarity).Key;

                durationLabel.Text = $"{stopwatch.ElapsedMilliseconds} ms";

                DataRow matchedData = db.ReadDataByBerkas(imagePath, sidikJariTable);
                if (matchedData == null)
                {
                    MessageBox.Show("No matching data found.");
                    PercentageLabel.Text = "0%";
                    return;
                }

                string sameName = matchedData["nama"].ToString();
                string foundName = "";
                List<string> arrName = db.ReadNameLeft();

                // Compare names to find a match
                foreach (string name in arrName)
                {
                    bool isSame = TextProcessing.CompareWord(sameName, name);
                    if (isSame)
                    {
                        foundName = name;
                        break;
                    }
                }

                DataTable resData = new DataTable();
                if (!string.IsNullOrEmpty(foundName))
                {
                    resData = db.ReadBiodataByName(foundName, biodataTable);
                }

                PercentageLabel.Text = $"{maxSimilarity}%";

                if (matchedData != null)
                {
                    string imgPath = matchedData["path"].ToString();
                    DisplayMatchedFingerprint(imgPath);
                }

                if (resData.Rows.Count > 0)
                {
                    DisplayBiodata(resData, sameName, matchedData["path"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Hidden; // Hide loading indicator
            }
        }


        /// Displays the matched fingerprint image.
        private void DisplayMatchedFingerprint(string imagePath)
        {
            var path = Path.Combine(Environment.CurrentDirectory, imagePath);
            var uri = new Uri(path);
            if (uri != null)
            {
                BitmapImage bitmap = new BitmapImage(uri);
                imgMatchedFingerprint.Source = bitmap;
            }
            else
            {
                MessageBox.Show("Image not found");
            }
        }

        /// Displays the biodata in a TextBlock control.
        private void DisplayBiodata(DataTable biodataTable, string realName, string imagePath)
        {
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
                    biodataText.AppendLine($"Image Path: {imagePath}");
                    biodataText.AppendLine($"NIK: {row["NIK"]}");
                    biodataText.AppendLine($"Nama: {row["nama"]}");
                    biodataText.AppendLine($"Real Name: {realName}");
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
                biodataLabel.TextAlignment = TextAlignment.Center;
                biodataLabel.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }


    }
}
