using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace SimpleFilter2
{
    public partial class MainWindow
    {
        private void rotateImage(Mat img, int order)
        {
            switch (order)
            {
                case 1: // 90
                    CvInvoke.Transpose(img, CurrentMat);
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.Horizontal); //transpose+flip(1)=CW
                    break;
                case 2: // 270
                    CvInvoke.Transpose(img, CurrentMat);
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.None); //transpose+flip(1)=CW
                    break;
                case 3: // 180
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.Vertical); //transpose+flip(1)=CW
                    break;

            }
            showImg(CurrentMat);
        }

        private void flipImage(Mat img, int order)
        {
            switch (order)
            {
                case 1:
                    CvInvoke.Flip(img, CurrentMat, Emgu.CV.CvEnum.FlipType.Horizontal);
                    break;
                case 2:
                    CvInvoke.Flip(img, CurrentMat, Emgu.CV.CvEnum.FlipType.Vertical);
                    break;
            }
            showImg(CurrentMat);
        }
    }
}
