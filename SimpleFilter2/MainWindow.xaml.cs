using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Emgu.CV;
using Emgu.Util;
using Microsoft.Win32;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using System.ComponentModel;
using System.IO;
using System.Net;
using AForge.Imaging;

namespace SimpleFilter2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Media.PointCollection GrayHistogramPoints = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\"; // 기본경로
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.tif)|*.jpg;*.jpeg;*.png;*.bmp;*.tif";

            if (openFileDialog.ShowDialog() == true)
            {               
                Mat img = CvInvoke.Imread(openFileDialog.FileName, Emgu.CV.CvEnum.LoadImageType.AnyColor);
                image.Source = BitmapSourceConvert.ToBitmapSource(img);
                
                drawHistogram(img);
            }
        }

        private void drawHistogram(Mat img)
        {
            using (System.Drawing.Bitmap bmp = img.Bitmap)
            {
                // Luminance
                ImageStatisticsHSL Statistics = new ImageStatisticsHSL(bmp);
                this.LuminanceHistogramPoints = ConvertToPointCollection(Statistics.Luminance.Values);
            }
        }

        public System.Windows.Media.PointCollection LuminanceHistogramPoints
        {
            get
            {
                return this.GrayHistogramPoints;
            }
            set
            {
                if (this.GrayHistogramPoints != value)
                {
                    this.GrayHistogramPoints = value;
                    if (this.PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("LuminanceHistogramPoints"));
                    }
                }
            }
        }

        private System.Windows.Media.PointCollection ConvertToPointCollection(int[] values)
        {
            int max = values.Max();

            System.Windows.Media.PointCollection points = new System.Windows.Media.PointCollection();
            // first point (lower-left corner)
            points.Add(new Point(0, max));
            // middle points
            for (int i = 0; i < values.Length; i++)
            {
                points.Add(new Point(i, max - values[i]));
            }
            // last point (lower-right corner)
            points.Add(new Point(values.Length - 1, max));

            return points;
        }
    }
}
