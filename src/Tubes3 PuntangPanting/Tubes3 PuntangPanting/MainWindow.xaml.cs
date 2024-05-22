﻿using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MySql.Data;
using System.Drawing;
using MySql.Data.MySqlClient;
<<<<<<< HEAD

=======
>>>>>>> 81fa4d29b77b32dc604504a888cfe35a96ee47ee

namespace Tubes3_PuntangPanting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private Database db;

        public MainWindow()
        {
            string customServer = "Fairuz";
            string customUser = "root";
            string customDatabase = "stima";
            string customPassword = "bismillah.33";

            db = new Database(customServer, customUser, customDatabase, customPassword);
            db.CreateBiodataTable();
            db.CreateSidikJariTable();

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
                image1.Source = bitmap;
<<<<<<< HEAD
=======
                if (bp != null)
                {
                imgUpload = bp;
                }
>>>>>>> 81fa4d29b77b32dc604504a888cfe35a96ee47ee
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isKMPSelected = tbRight.IsChecked ?? false;

            string ascii = AsciiConverter.ImageToAsciiImage(imgUpload);

            List<string> asciiData = db.ReadAsciiData();


            if (isKMPSelected)
            {
                // Looping to compare all ascii result to ascii berkas_citra in database
                
                // If already get the same exact value return and stop iterations
                // If dont get the result also calculate the precentage similarity
                MessageBox.Show("KMP is selected.");
            }
            else
            {
                // Looping to compare all ascii result to ascii berkas_citra in database
                // If already get the same exact value return and stop iterations
                // If dont get the result also calculate the precentage similarity
                MessageBox.Show("BM is selected.");
            }
               string sameName = db.ReadNameByBerkas("");
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
            DataTable resData = new DataTable();
                if (foundName != "")
            {
                resData = db.ReadBiodataByName(foundName);
                
            }


            // Add regex validation to compare the result name from database between table
            // Return all information biodata
        }

    }
}