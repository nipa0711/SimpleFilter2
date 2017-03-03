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
using System.Net;

namespace SimpleFilter2
{
    public partial class MainWindow
    {
        private void toOriginal()
        {
            CurrentMat = OriginalMat.Clone();
            showImg(CurrentMat);
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
    }
}
