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
using Emgu.CV.Util;
using System.ComponentModel;
using System.IO;
using System.Drawing;

namespace SimpleFilter2
{
    public partial class MainWindow
    {
        private void toOriginal()
        {
            Constants.colorMode = Constants.BGR;
            init(Constants.BGR);
            menu_panel_control.Visibility = Visibility.Visible;
            showImg(OriginalMat);
        }

        private void toGray(Mat img)
        {
            //System.Diagnostics.Debug.WriteLine("==============" + CurrentMat.NumberOfChannels);
            if (img.NumberOfChannels >= 3)
            {
                CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Gray);
                showImg(CurrentMat);
            }
        }

        private void toBgrHistogramEqulization(Mat img)
        {
            if (img.NumberOfChannels >= 3)
            {
                var channels = img.Split(); // 이미지 채널 분리
                CvInvoke.EqualizeHist(channels[0], channels[0]);
                CvInvoke.EqualizeHist(channels[1], channels[1]);
                CvInvoke.EqualizeHist(channels[2], channels[2]);

                VectorOfMat temp = new VectorOfMat();
                temp.Push(channels[0]);
                temp.Push(channels[1]);
                temp.Push(channels[2]);

                CvInvoke.Merge(temp, CurrentMat);
            }
            else
            {
                CvInvoke.EqualizeHist(img, CurrentMat);
            }

            showImg(CurrentMat);
        }

        private void toYCrCbHistogramEqulization(Mat img)
        {
            if (img.NumberOfChannels >= 3)
            {
                CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2YCrCb);
                var channels = CurrentMat.Split(); // 이미지 채널 분리
                CvInvoke.EqualizeHist(channels[0], channels[0]); // Y

                VectorOfMat temp = new VectorOfMat();
                temp.Push(channels[0]);
                temp.Push(channels[1]);
                temp.Push(channels[2]);

                CvInvoke.Merge(temp, CurrentMat);
                CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.YCrCb2Bgr);
            }
            else
            {
                CvInvoke.EqualizeHist(img, CurrentMat);
            }

            showImg(CurrentMat);
        }

        private void toColorModel(Mat img, int order)
        {
            if(order!=Constants.colorMode)
            {
                init(order);
            }

            switch (order)
            {
                case Constants.Luv:
                    Constants.colorMode = Constants.Luv;
                    break;
                case Constants.Lab:
                    Constants.colorMode = Constants.Lab;
                    break;
                case Constants.YCrCb:
                    Constants.colorMode = Constants.YCrCb;
                    break;
                case Constants.XYZ:
                    Constants.colorMode = Constants.XYZ;
                    break;
                case Constants.HLS:
                    Constants.colorMode = Constants.HLS;
                    break;
                case Constants.HSV:
                    Constants.colorMode = Constants.HSV;
                    break;
            }
            manageColorModel(OriginalMat,Constants.colorMode);
        }

        private void toAdaptiveLightnessEqualization(Mat img)
        {
            if (img.NumberOfChannels < 3)
            {
                MessageBoxResult result = MessageBox.Show("흑백 영상은 변환할 수 없습니다.", "경고");
                return;
            }

            CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Lab);
            var channels = CurrentMat.Split(); // 이미지 채널 분리

            CvInvoke.CLAHE(channels[0], 2, new System.Drawing.Size(8, 8), channels[0]);

            VectorOfMat temp = new VectorOfMat();
            temp.Push(channels[0]);
            temp.Push(channels[1]);
            temp.Push(channels[2]);

            CvInvoke.Merge(temp, CurrentMat);
            CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Lab2Bgr);
            showImg(CurrentMat);
        }

        private void toAdaptiveBgrEqualization(Mat img)
        {
            if (img.NumberOfChannels < 3)
            {
                MessageBoxResult result = MessageBox.Show("흑백 영상은 변환할 수 없습니다.", "경고");
                return;
            }

            var channels = img.Split(); // 이미지 채널 분리

            CvInvoke.CLAHE(channels[0], 2, new System.Drawing.Size(8, 8), channels[0]);
            CvInvoke.CLAHE(channels[1], 2, new System.Drawing.Size(8, 8), channels[1]);
            CvInvoke.CLAHE(channels[2], 2, new System.Drawing.Size(8, 8), channels[2]);

            VectorOfMat temp = new VectorOfMat();
            temp.Push(channels[0]);
            temp.Push(channels[1]);
            temp.Push(channels[2]);

            CvInvoke.Merge(temp, CurrentMat);
            showImg(CurrentMat);
        }

        private void toAdaptiveYCrCbEqualization(Mat img)
        {
            if (img.NumberOfChannels < 3)
            {
                MessageBoxResult result = MessageBox.Show("흑백 영상은 변환할 수 없습니다.", "경고");
                return;
            }

            CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2YCrCb);
            var channels = CurrentMat.Split(); // 이미지 채널 분리

            CvInvoke.CLAHE(channels[0], 2, new System.Drawing.Size(8, 8), channels[0]);
            CvInvoke.CLAHE(channels[1], 2, new System.Drawing.Size(8, 8), channels[1]);
            CvInvoke.CLAHE(channels[2], 2, new System.Drawing.Size(8, 8), channels[2]);

            VectorOfMat temp = new VectorOfMat();
            temp.Push(channels[0]);
            temp.Push(channels[1]);
            temp.Push(channels[2]);

            CvInvoke.Merge(temp, CurrentMat);
            CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.YCrCb2Bgr);
            showImg(CurrentMat);
        }

        private void toAdaptiveSaturationEqualization(Mat img)
        {
            if (img.NumberOfChannels < 3)
            {
                MessageBoxResult result = MessageBox.Show("흑백 영상은 변환할 수 없습니다.", "경고");
                return;
            }

            CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Hsv);
            var channels = CurrentMat.Split(); // 이미지 채널 분리

            CvInvoke.CLAHE(channels[1], 2, new System.Drawing.Size(8, 8), channels[1]);

            VectorOfMat temp = new VectorOfMat();
            temp.Push(channels[0]);
            temp.Push(channels[1]);
            temp.Push(channels[2]);

            CvInvoke.Merge(temp, CurrentMat);
            CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Hsv2Bgr);
            showImg(CurrentMat);
        }

        private void toSepia(Mat img)
        {
            Mat kern = new Mat(3, 3, DepthType.Cv8U, 3);
            Matrix<double> kernel = new Matrix<double>(new double[3, 3] { { 0.131, 0.534, 0.272 }, { 0.168, 0.686, 0.349 }, { 0.189, 0.769, 0.393 } });
            CvInvoke.Transform(img, CurrentMat, kernel);
            showImg(CurrentMat);
        }

        private void toSplitColor(Mat img, int selectColor)
        {
            switch (selectColor)
            {
                #region BGR 채널 보기
                case 1:
                    {
                        var channels = img.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(channels[0]); // B
                        temp.Push(empty); // G
                        temp.Push(empty); // R

                        CvInvoke.Merge(temp, CurrentMat);
                        showImg(CurrentMat);
                        break;
                    }
                case 2:
                    {
                        var channels = img.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // B
                        temp.Push(channels[0]); // G
                        temp.Push(empty); // R

                        CvInvoke.Merge(temp, CurrentMat);
                        showImg(CurrentMat);
                        break;
                    }
                case 3:
                    {
                        var channels = img.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // B
                        temp.Push(empty); // G
                        temp.Push(channels[0]); // R

                        CvInvoke.Merge(temp, CurrentMat);
                        showImg(CurrentMat);
                        break;
                    }
                #endregion
                #region LUV 채널 보기
                case 4:
                    {

                        break;
                    }
                case 5:
                    {

                        break;
                    }
                case 6:
                    {

                        break;
                    }
                #endregion
                #region Lab 채널 보기
                case 7:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Lab);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(channels[0]); // L
                        temp.Push(empty); // a
                        temp.Push(empty); // b

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Lab2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 8:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Lab);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(191.25));

                        Mat empty2 = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty2.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // L
                        temp.Push(channels[1]); // a
                        temp.Push(empty2); // b

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Lab2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 9:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Lab);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(191.25));

                        Mat empty2 = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty2.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // L
                        temp.Push(empty2); // a
                        temp.Push(channels[2]); // b

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Lab2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                #endregion
                #region YCrCb 채널 보기
                case 10:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2YCrCb);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(channels[0]); // Y
                        temp.Push(empty); // Cr
                        temp.Push(empty); // Cb      

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.YCrCb2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 11:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2YCrCb);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // Y
                        temp.Push(channels[1]); // Cr
                        temp.Push(empty); // Cb

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.YCrCb2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 12:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2YCrCb);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(127.5));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // Y
                        temp.Push(empty); // Cr
                        temp.Push(channels[2]); // Cb

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.YCrCb2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                #endregion
                #region HSV 채널 보기
                case 13:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Hsv);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(255));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(channels[0]); // H
                        temp.Push(empty); // S
                        temp.Push(empty); // V      

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Hsv2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 14:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Hsv);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(179));

                        Mat empty2 = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty2.SetTo(new MCvScalar(255));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // H
                        temp.Push(channels[1]); // S
                        temp.Push(empty2); // V

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Hsv2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                case 15:
                    {
                        CvInvoke.CvtColor(img, CurrentMat, ColorConversion.Bgr2Hsv);
                        var channels = CurrentMat.Split(); // 이미지 채널 분리
                        Mat empty = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty.SetTo(new MCvScalar(179));

                        Mat empty2 = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);
                        empty2.SetTo(new MCvScalar(0));

                        VectorOfMat temp = new VectorOfMat();
                        temp.Push(empty); // H
                        temp.Push(empty2); // S
                        temp.Push(channels[2]); // V

                        CvInvoke.Merge(temp, CurrentMat);
                        CvInvoke.CvtColor(CurrentMat, CurrentMat, ColorConversion.Hsv2Bgr);
                        showImg(CurrentMat);
                        break;
                    }
                    #endregion
            }
        }

        private void manageColorModel(Mat img, int colorMode)
        {
            if (img.NumberOfChannels < 3)
            {
                MessageBoxResult result = MessageBox.Show("흑백 영상은 변환할 수 없습니다.", "경고");
                return;
            }

            double minimum0c = min0Cha.Value;
            double maximum0c = max0Cha.Value;
            double minimum1c = min1Cha.Value;
            double maximum1c = max1Cha.Value;
            double minimum2c = min2Cha.Value;
            double maximum2c = max2Cha.Value;

            Mat store = OriginalMat.Clone();

            switch (colorMode)
            {
                case Constants.BGR:
                    {
                        Image<Bgr, Byte> temp = img.ToImage<Bgr, Byte>();
                        
                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        CurrentMat = temp.Mat;
                        break;
                    }
                case Constants.YCrCb:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2YCrCb);
                        Image<Ycc, Byte> temp = store.ToImage<Ycc, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.YCrCb2Bgr);
                        break;
                    }
                case Constants.Luv:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2Luv);
                        Image<Luv, Byte> temp = store.ToImage<Luv, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.Luv2Bgr);
                        break;
                    }
                case Constants.Lab:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2Lab);
                        Image<Lab, Byte> temp = store.ToImage<Lab, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.Lab2Bgr);
                        break;
                    }

                case Constants.HSV:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2Hsv);
                        Image<Hsv, Byte> temp = store.ToImage<Hsv, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.Hsv2Bgr);
                        break;
                    }

                case Constants.HLS:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2Hls);
                        Image<Hls, Byte> temp = store.ToImage<Hls, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.Hls2Bgr);
                        break;
                    }

                case Constants.XYZ:
                    {
                        CvInvoke.CvtColor(img, store, ColorConversion.Bgr2Xyz);
                        Image<Xyz, Byte> temp = store.ToImage<Xyz, Byte>();

                        for (int i = 0; i < img.Height; ++i)
                        {
                            for (int j = 0; j < img.Width; ++j)
                            {
                                if (temp.Data[i, j, 0] <= minimum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)minimum0c;
                                }

                                else if (temp.Data[i, j, 0] >= maximum0c)
                                {
                                    temp.Data[i, j, 0] = (byte)maximum0c;
                                }

                                else if (temp.Data[i, j, 1] < minimum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)minimum1c;
                                }

                                else if (temp.Data[i, j, 1] > maximum1c)
                                {
                                    temp.Data[i, j, 1] = (byte)maximum1c;
                                }

                                else if (temp.Data[i, j, 2] < minimum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)minimum2c;
                                }

                                else if (temp.Data[i, j, 2] > maximum2c)
                                {
                                    temp.Data[i, j, 2] = (byte)maximum2c;
                                }
                            }
                        }
                        store = temp.Mat;
                        CvInvoke.CvtColor(store, CurrentMat, ColorConversion.Xyz2Bgr);
                        break;
                    }
            }

            showImg(CurrentMat);
        }

        //private void toDotPattern(Mat img)
        //{
        //    // http://opencv-code.com/tutorials/photo-to-colored-dot-patterns-with-opencv/#more-754

        //    Mat dst = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 3);
        //    Mat cir = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 1);

        //    int bsize = 10;

        //    for (int i = 0; i < img.Rows; i += bsize)
        //    {
        //        for (int j = 0; j < img.Cols; j += bsize)
        //        {
        //            var rectangle1 = new System.Drawing.Rectangle(j, i, bsize, bsize);
        //            var rectangle2 = new System.Drawing.Rectangle(0, 0, img.Cols, img.Rows);
        //            //    Rectangle rect = Rectangle(j, i, bsize, bsize) &
        //            //Rect(0, 0, input.cols, input.rows);

        //            CvInvoke.rect

        //            var rect = rectangle1 & rectangle2;

        //            Mat sub_dst(dst, rect);
        //            sub_dst.setTo(mean(input(rect)));

        //            circle(
        //                cir,
        //                Point(j + bsize / 2, i + bsize / 2),
        //                bsize / 2 - 1,
        //                CV_RGB(255, 255, 255), -1, CV_AA
        //                );
        //        }
        //    }

        //    Mat cir_32f;
        //    cir.convertTo(cir_32f, CV_32F);
        //    normalize(cir_32f, cir_32f, 0, 1, NORM_MINMAX);

        //    Mat dst_32f;
        //    dst.convertTo(dst_32f, CV_32F);

        //    vector<Mat> channels;
        //    split(dst_32f, channels);
        //    for (int i = 0; i < channels.size(); ++i)
        //        channels[i] = channels[i].mul(cir_32f);

        //    merge(channels, dst_32f);
        //    dst_32f.convertTo(dst, CV_8U);

        //    showImg(dst);
        //}
    }
}
