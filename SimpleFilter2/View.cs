using System.Windows.Controls;
using System.Windows.Media;
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
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.Horizontal);
                    break;
                case 2: // 270
                    CvInvoke.Transpose(img, CurrentMat);
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.None);
                    break;
                case 3: // 180
                    CvInvoke.Flip(CurrentMat, CurrentMat, Emgu.CV.CvEnum.FlipType.Vertical);
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

        private void manage_image_view_mode(int order)
        {
            switch(order)
            {
                case 1: // 화면에 맞춰 보기
                    image.Stretch = Stretch.Uniform;

                    viewOriginalSize.IsChecked = false;
                    viewImgScreenSize.IsChecked = true;
                    viewImgHeightSize.IsChecked = false;
                    viewImgWidthSize.IsChecked = false;

                    ImageScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled; // 가로 바
                    ImageScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled; // 세로 바
                    break;
                case 2: // 100% 보기
                    image.Stretch = Stretch.None;

                    viewOriginalSize.IsChecked = true;
                    viewImgScreenSize.IsChecked = false;
                    viewImgHeightSize.IsChecked = false;
                    viewImgWidthSize.IsChecked = false;

                    ImageScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto; // 가로 바
                    ImageScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; // 세로 바
                    break;
                case 3: // 가로폭에 맞춰 보기
                    image.Stretch = Stretch.Uniform;

                    viewOriginalSize.IsChecked = false;
                    viewImgScreenSize.IsChecked = false;
                    viewImgHeightSize.IsChecked = false;
                    viewImgWidthSize.IsChecked = true;

                    ImageScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled; // 가로 바
                    ImageScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto; // 세로 바
                    break;
                case 4: // 세로폭에 맞춰 보기
                    image.Stretch = Stretch.Uniform;

                    viewOriginalSize.IsChecked = false;
                    viewImgScreenSize.IsChecked = false;
                    viewImgHeightSize.IsChecked = true;
                    viewImgWidthSize.IsChecked = false;

                    ImageScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto; // 가로 바
                    ImageScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled; // 세로 바
                    break;
            }
        }
    }
}
