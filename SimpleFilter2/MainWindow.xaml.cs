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
using Emgu.CV.Util;

namespace SimpleFilter2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Windows.Media.PointCollection GrayHistogramPoints = null;
        Mat OriginalMat = null;
        Mat CurrentMat = null;

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
                OriginalMat = img;
                CurrentMat = OriginalMat.Clone();

                menu_modi.IsEnabled = true;
                menu_look.IsEnabled = true;
                menu_filter.IsEnabled = true;
                menu_panel_control.Visibility = Visibility.Visible;

                manage_image_view_mode(1);
                
                toOriginal();
            }
        }

        private void init(int order)
        {
            switch(order)
            {
                case Constants.BGR:
                    min0c.Content = "B 최저값";
                    max0c.Content = "B 최고값";
                    min1c.Content = "G 최저값";
                    max1c.Content = "G 최고값";
                    min2c.Content = "R 최저값";
                    max2c.Content = "R 최고값";
                    break;
                case Constants.Luv:       
                    min0c.Content = "L 최저값";
                    max0c.Content = "L 최고값";
                    min1c.Content = "u 최저값";
                    max1c.Content = "u 최고값";
                    min2c.Content = "v 최저값";
                    max2c.Content = "v 최고값";
                    break;
                case Constants.HSV:
                    min0c.Content = "H 최저값";
                    max0c.Content = "H 최고값";
                    min1c.Content = "S 최저값";
                    max1c.Content = "S 최고값";
                    min2c.Content = "V 최저값";
                    max2c.Content = "V 최고값";
                    break;
                case Constants.YCrCb:
                    min0c.Content = "Y 최저값";
                    max0c.Content = "Y 최고값";
                    min1c.Content = "Cr 최저값";
                    max1c.Content = "Cr 최고값";
                    min2c.Content = "Cb 최저값";
                    max2c.Content = "Cb 최고값";
                    break;
                case Constants.XYZ:
                    min0c.Content = "X 최저값";
                    max0c.Content = "X 최고값";
                    min1c.Content = "Y 최저값";
                    max1c.Content = "Y 최고값";
                    min2c.Content = "Z 최저값";
                    max2c.Content = "Z 최고값";
                    break;
                case Constants.HLS:
                    min0c.Content = "H 최저값";
                    max0c.Content = "H 최고값";
                    min1c.Content = "L 최저값";
                    max1c.Content = "L 최고값";
                    min2c.Content = "S 최저값";
                    max2c.Content = "S 최고값";
                    break;
                case Constants.Lab:
                    min0c.Content = "L 최저값";
                    max0c.Content = "L 최고값";
                    min1c.Content = "a 최저값";
                    max1c.Content = "a 최고값";
                    min2c.Content = "b 최저값";
                    max2c.Content = "b 최고값";
                    break;
            }

            min0Cha.Minimum = 0;
            min0Cha.Maximum = 255;
            min0Cha.Value = 0;
            max0Cha.Minimum = 0;
            max0Cha.Maximum = 255;
            max0Cha.Value = 255;

            min1Cha.Minimum = 0;
            min1Cha.Maximum = 255;
            min1Cha.Value = 0;
            max1Cha.Minimum = 0;
            max1Cha.Maximum = 255;
            max1Cha.Value = 255;

            min2Cha.Minimum = 0;
            min2Cha.Maximum = 255;
            min2Cha.Value = 0;
            max2Cha.Minimum = 0;
            max2Cha.Maximum = 255;
            max2Cha.Value = 255;
        }

        #region 히스토그램
        internal void drawHistogram(Mat img)
        {
            using (System.Drawing.Bitmap bmp = img.Bitmap)
            {
                ImageStatistics Statistics = new ImageStatistics(bmp);

                if (Statistics.IsGrayscale == true) // 회색이라면
                {
                    this.LuminanceHistogramPoints = ConvertToPointCollection(Statistics.Gray.Values);
                }
                else
                {
                    // Luminance
                    ImageStatisticsHSL LStatistics = new ImageStatisticsHSL(bmp);
                    this.LuminanceHistogramPoints = ConvertToPointCollection(LStatistics.Luminance.Values);
                }
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

        internal System.Windows.Media.PointCollection ConvertToPointCollection(int[] values)
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
        #endregion

        private void viewOriginal_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
        }

        private void viewGray_Click(object sender, RoutedEventArgs e)
        {
            toGray(CurrentMat);
        }

        private void showImg(Mat img)
        {
            image.Source = BitmapSourceConvert.ToBitmapSource(img);
            drawHistogram(img);
        }

        private void toFlipLR_Click(object sender, RoutedEventArgs e)
        {
            flipImage(CurrentMat, 1);
        }

        private void toFlipUD_Click(object sender, RoutedEventArgs e)
        {
            flipImage(CurrentMat, 2);
        }

        private void rotate90_Click(object sender, RoutedEventArgs e)
        {
            rotateImage(CurrentMat, 1);
        }

        private void rotate270_Click(object sender, RoutedEventArgs e)
        {
            rotateImage(CurrentMat, 2);
        }

        private void rotate180_Click(object sender, RoutedEventArgs e)
        {
            rotateImage(CurrentMat, 3);
        }

        private void ProgramEnd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var response = MessageBox.Show("정말로 종료하시겠습니까?", "종료 안내", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (response == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        #region 히스토그램 균등화
        private void BgrEqualization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toBgrHistogramEqulization(OriginalMat);
        }

        private void YCrCbEqulization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toYCrCbHistogramEqulization(OriginalMat);
        }

        private void AdaptiveLightnessEqualization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toAdaptiveLightnessEqualization(OriginalMat);
        }

        private void AdaptiveBgrEqualization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toAdaptiveBgrEqualization(OriginalMat);
        }

        private void AdaptiveYCrCbEqualization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toAdaptiveYCrCbEqualization(OriginalMat);
        }

        private void AdaptiveSaturationEqualization_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toAdaptiveSaturationEqualization(OriginalMat);
        }

        #endregion

        #region 색상모델 변경
        private void LUV_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.Luv);
        }

        private void Lab_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.Lab);
        }

        private void YCrCb_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.YCrCb);
        }

        private void XYZ_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.XYZ);
        }

        private void HLS_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.HLS);
        }

        private void HSV_Click(object sender, RoutedEventArgs e)
        {
            toColorModel(OriginalMat, Constants.HSV);
        }

        private void BGR_B_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 1);
        }

        private void BGR_G_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 2);
        }

        private void BGR_R_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 3);
        }

        private void LUV_L_Click(object sender, RoutedEventArgs e)
        {
            //toSplitColor(CurrentMat, 4);
        }

        private void LUV_U_Click(object sender, RoutedEventArgs e)
        {
            //toSplitColor(CurrentMat, 5);
        }

        private void LUV_V_Click(object sender, RoutedEventArgs e)
        {
            //toSplitColor(CurrentMat, 6);
        }

        private void Lab_L_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 7);
        }

        private void Lab_a_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 8);
        }

        private void Lab_b_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 9);
        }

        private void YCrCb_Y_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 10);
        }

        private void YCrCb_Cr_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 11);
        }

        private void YCrCb_Cb_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 12);
        }

        private void HSV_H_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 13);
        }

        private void HSV_S_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 14);
        }

        private void HSV_V_Click(object sender, RoutedEventArgs e)
        {
            toSplitColor(OriginalMat, 15);
        }

        #endregion

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            toOriginal();
            menu_panel_control.Visibility = Visibility.Hidden;
            toSepia(OriginalMat);
        }

        #region 이미지 보기 방법      
        private void viewImgScreenSize_Click(object sender, RoutedEventArgs e)
        {
            manage_image_view_mode(1);
        }

        private void viewOriginalSize_Click(object sender, RoutedEventArgs e)
        {
            manage_image_view_mode(2);
        }

        private void viewImgWidthSize_Click(object sender, RoutedEventArgs e)
        {
            manage_image_view_mode(3);
        }

        private void viewImgHeightSize_Click(object sender, RoutedEventArgs e)
        {
            manage_image_view_mode(4);
        }

        #endregion
        

        private void ColorSlider_ValueChanged(object sender, MouseButtonEventArgs e)
        {
            //var obj = sender as Slider;
            //System.Diagnostics.Debug.WriteLine("==============" + obj.Name);
            toColorModel(OriginalMat, Constants.colorMode);
        }

        #region 채널 색상 조절바 제한
        private void min0Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimumB = min0Cha.Value;
            double maximumB = max0Cha.Value;

            if (minimumB > maximumB)
            {
                minimumB = maximumB;
                min0Cha.Value = maximumB;
            }
        }

        private void max0Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimumB = min0Cha.Value;
            double maximumB = max0Cha.Value;

            if (maximumB < minimumB)
            {
                maximumB = minimumB;
                max0Cha.Value = minimumB;
            }
        }

        private void min1Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimumG = min1Cha.Value;
            double maximumG = max1Cha.Value;

            if (minimumG > maximumG)
            {
                minimumG = maximumG;
                min1Cha.Value = maximumG;
            }
        }

        private void max1Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimumG = min1Cha.Value;
            double maximumG = max1Cha.Value;

            if (maximumG < minimumG)
            {
                maximumG = minimumG;
                max1Cha.Value = minimumG;
            }
        }

        private void min2Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimumR = min2Cha.Value;
            double maximumR = max2Cha.Value;

            if (minimumR > maximumR)
            {
                minimumR = maximumR;
                min2Cha.Value = maximumR;
            }
        }

        private void max2Channel(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double minimum2Channel = min2Cha.Value;
            double maximum2Channel = max2Cha.Value;

            if (maximum2Channel < minimum2Channel)
            {
                maximum2Channel = minimum2Channel;
                max2Cha.Value = minimum2Channel;
            }
        }
        #endregion

        
    }
}
