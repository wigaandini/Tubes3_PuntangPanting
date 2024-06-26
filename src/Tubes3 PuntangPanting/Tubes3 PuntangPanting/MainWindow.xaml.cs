﻿using System.Data;
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
    public partial class MainWindow : Window
    {
        private Bitmap? imgUpload;
        private readonly byte[] key = new byte[16] {
            0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6,
            0xab, 0xf7, 0x4e, 0x35, 0x0b, 0x34, 0x78, 0x55
        };

        private readonly byte[] iv = new byte[16] {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
            0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f
        };

        private CancellationTokenSource cts = new CancellationTokenSource();

        private readonly CustomAes aes;
        private Database? db;
        private DataTable biodataTable = new();
        private DataTable sidikJariTable = new();
        private bool isEncrypt;

        public MainWindow()
        {
            InitializeComponent();
            aes = new CustomAes(key, iv);

            InitializeDatabaseConnection();
            LoadDataFromDatabase();

        }

        private void InitializeDatabaseConnection()
        {
            string customServer, customUser, customDatabase, customPassword;
            if (AskForCustomization())
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("Enter database connection details separated by commas (Server, Username, Database, Password):", "Enter Connection Details", "");
                var inputParams = input.Split(',');

                if (inputParams.Length != 4)
                {
                    MessageBox.Show("Please enter all four parameters separated by commas.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                customServer = inputParams[0].Trim();
                customUser = inputParams[1].Trim();
                customDatabase = inputParams[2].Trim();
                customPassword = inputParams[3].Trim();
            }
            else
            {
                customServer = "Fairuz";
                customUser = "root";
                customDatabase = "stima";
                customPassword = "bismillah.33";
            }

            isEncrypt = AskForEncryption();
            db = new Database(customServer, customUser, customDatabase, customPassword);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();
        }

        private bool AskForCustomization()
        {
            return MessageBox.Show("Do you want to customize the database connection details?", "Customize Database", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private bool AskForEncryption()
        {
            return MessageBox.Show("Is the data encrypted?", "Encryption", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void LoadDataFromDatabase()
        {
            if (db != null)
            {
                biodataTable = db.ReadBiodata();
                sidikJariTable = db.ReadSidikJari();
            }
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
            Close();
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void UploadImage1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                imgUpload = new Bitmap(filePath);

                image1.Source = bitmap;
                buttonUpload.Visibility = Visibility.Hidden;
            }
        }

        public byte[] ConvertHexStringToByteArray(string? hexString)
        {
            if (hexString == null)
            {
                return new byte[0];
            }

            string cleanHexString = hexString.Replace("-", "");
            byte[] byteArray = Enumerable.Range(0, cleanHexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(cleanHexString.Substring(x, 2), 16))
                .ToArray();
            return byteArray;
        }

        private async Task ProcessDataRow(DataRow datarow, ConcurrentDictionary<string, double> similarities, string pattern, string method)
        {
            string? path = datarow["path"].ToString();
            if(path == null)
            {
                return;
            }
            string? text = await Task.Run(() => AsciiConverter.ImageToAscii(path));

            if (text == null || path == null)
                return;
            string? decryptedText = isEncrypt ? await Task.Run(() => Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(text)))) : text;

            const double minPercentage = 50.0;
            var result = await Task.Run(() => Levenshtein.MatchWithLevenshtein(pattern, decryptedText, minPercentage, method == "kmp" ? 2 : 1));
            if (result.similarity > minPercentage)
            {
                similarities.TryAdd(path, result.similarity);
                if (result.similarity == 100.0)
                {
                    cts.Cancel();
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingProgressBar.Visibility = Visibility.Visible;
                bool isKMPSelected = tbRight.IsChecked ?? false;

                if (imgUpload == null)
                {
                    return;
                }

                string pattern = AsciiConverter.MidOneBitmap(imgUpload);
                string method = isKMPSelected ? "kmp" : "bm";

                Stopwatch stopwatch = new Stopwatch();
                ConcurrentDictionary<string, double> similarities = new ConcurrentDictionary<string, double>();

                if (sidikJariTable.Rows.Count == 0)
                {
                    MessageBox.Show("No data in sidik jari table.");
                    return;
                }

                stopwatch.Start();

                if (tbRight2.IsChecked == true)
                {
                    var tasks = sidikJariTable.AsEnumerable().Select(datarow => ProcessDataRow(datarow, similarities, pattern, method));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var datarow in sidikJariTable.AsEnumerable())
                    {
                        await ProcessDataRow(datarow, similarities, pattern, method);
                    }
                }

                stopwatch.Stop();
                var maxSimilarity = similarities.Values.Max();
                if (maxSimilarity < 50)
                {
                    PercentageLabel.Text = $"{maxSimilarity:F2}%";
                    string fullPath = Path.Combine(Environment.CurrentDirectory, "no-data.jpg");

                    if (!File.Exists(fullPath))
                    {
                        MessageBox.Show($"File not found: {fullPath}");
                        return;
                    }

                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgMatchedFingerprint.Source = bitmap;
                    biodataLabel.Text = "No Data Match with > 50% Similarities Found";
                    durationLabel.Text = $"{stopwatch.ElapsedMilliseconds} ms";


                    return;
                }
                var imagePath = similarities.FirstOrDefault(kv => kv.Value == maxSimilarity).Key;
                durationLabel.Text = $"{stopwatch.ElapsedMilliseconds} ms";

                if (db == null)
                {
                    MessageBox.Show("Database is not initialized.");
                    return;
                }

                DataRow matchedData = db.ReadDataByBerkas(imagePath, sidikJariTable);
                if (matchedData == null)
                {
                    MessageBox.Show("No matching data found.");
                    PercentageLabel.Text = "0%";
                    return;
                }

                string? sameName = matchedData["nama"].ToString();
                string? decryptedSameName = isEncrypt ? await Task.Run(() => Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(sameName)))) : sameName;

                string foundName = "";
                try
                {
                    List<string> arrName = db.ReadNameLeft();
                    if (decryptedSameName != null && arrName != null && arrName.Count > 0)
                    {
                        foreach (string name in arrName)
                        {
                            string nameDecrypt = isEncrypt ? await Task.Run(() => Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(name)))) : name;
                            if (TextProcessing.CompareWord(decryptedSameName, nameDecrypt))
                            {
                                foundName = name;
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(foundName))
                        {
                            MessageBox.Show("No matching name found.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No names found in database.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred 2: {ex.Message}");
                }


                DataTable resData = new DataTable();
                if (!string.IsNullOrEmpty(foundName))
                {
                    resData = db.ReadBiodataByName(foundName, biodataTable);
                }


                PercentageLabel.Text = $"{maxSimilarity:F2}%";
                string decryptImgPath = isEncrypt ? await Task.Run(() => Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(imagePath)))) : imagePath;
                if (decryptImgPath == null)
                {
                    MessageBox.Show("Image path not found.");
                    return;
                }

                DisplayMatchedFingerprint(decryptImgPath);

                if (resData.Rows.Count > 0 && decryptedSameName != null)
                {
                    if (decryptImgPath != null)
                    {
                        await DisplayBiodata(resData, decryptedSameName, decryptImgPath);
                    }
                }
            }
            catch (Exception ex)
            {
                PercentageLabel.Text = $"{0:F2}%";
                string fullPath = Path.Combine(Environment.CurrentDirectory, "no-data.jpg");

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"File not found: {fullPath}");
                    return;
                }

                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgMatchedFingerprint.Source = bitmap;
                biodataLabel.Text = "No Data Match with > 50% Similarities Found";
                MessageBox.Show($"An error occurred: {ex.Message}");
                
            }
            finally
            {
                LoadingProgressBar.Visibility = Visibility.Hidden;
            }
        }

        private void DisplayMatchedFingerprint(string imagePath)
        {
            try
            {
                // Todo: Harusnya remove non valid gak dipake
                string fullPath = Path.Combine(Environment.CurrentDirectory, TextProcessing.RemoveNonValidPath(imagePath));

                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"File not found: {fullPath}");
                    return;
                }

                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgMatchedFingerprint.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while displaying the fingerprint image: {ex.Message}");
            }
        }

        private async Task DisplayBiodata(DataTable biodataTable, string realName, string imagePath)
        {
            if (biodataTable.Rows.Count == 0)
            {
                biodataLabel.Text = "Biodata";
                biodataLabel.Foreground = new SolidColorBrush(Colors.Gray);
                return;
            }

            StringBuilder biodataText = new();
            foreach (DataRow row in biodataTable.Rows)
            {
                string decryptedNIK = await DecryptFieldAsync(row["NIK"]);
                string decryptedNama = await DecryptFieldAsync(row["nama"]);
                string decryptedTempatLahir = await DecryptFieldAsync(row["tempat_lahir"]);
                string decryptedTanggalLahir = await DecryptFieldAsync(row["tanggal_lahir"]);
                string decryptedAlamat = await DecryptFieldAsync(row["alamat"]);
                string decryptedPekerjaan = await DecryptFieldAsync(row["pekerjaan"]);

                biodataText.AppendLine($"Image Path: {imagePath}");
                biodataText.AppendLine($"NIK: {decryptedNIK.Substring(0, 16)}");
                biodataText.AppendLine($"Nama: {decryptedNama}");
                biodataText.AppendLine($"Real Name: {realName}");
                biodataText.AppendLine($"Tempat Lahir: {decryptedTempatLahir}");
                biodataText.AppendLine($"Tanggal Lahir: {decryptedTanggalLahir}");
                biodataText.AppendLine($"Jenis Kelamin: {row["jenis_kelamin"]}");
                biodataText.AppendLine($"Golongan Darah: {row["golongan_darah"]}");
                biodataText.AppendLine($"Alamat: {decryptedAlamat}");
                biodataText.AppendLine($"Agama: {row["agama"]}");
                biodataText.AppendLine($"Status Perkawinan: {row["status_perkawinan"]}");
                biodataText.AppendLine($"Pekerjaan: {decryptedPekerjaan}");
                biodataText.AppendLine($"Kewarganegaraan: {row["kewarganegaraan"]}");
                biodataText.AppendLine();
            }

            biodataLabel.Text = biodataText.ToString();
            biodataLabel.Foreground = new SolidColorBrush(Colors.White);
            biodataLabel.TextAlignment = TextAlignment.Center;
            biodataLabel.HorizontalAlignment = HorizontalAlignment.Center;
        }

        private async Task<string> DecryptFieldAsync(object field)
        {
            string? fieldValue = field.ToString();
            return isEncrypt && fieldValue != null
                ? await Task.Run(() => Encoding.UTF8.GetString(aes.Decrypt(ConvertHexStringToByteArray(fieldValue))))
                : fieldValue ?? string.Empty;
        }
    }
}