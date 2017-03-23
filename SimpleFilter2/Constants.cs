using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFilter2
{
    public static class Constants
    {
        public const int BGR = 0;
        public const int Luv = 1;
        public const int Lab = 2;
        public const int YCrCb = 3;
        public const int XYZ = 4;
        public const int HLS = 5;
        public const int HSV = 6;

        public static int colorMode;
        public static int blurMode;

        public static string fileAddr;

        public const int Sobel = 0;
        public const int Laplace = 1;
        public const int Median = 2;
        public const int Gaussian = 3;
    }
}
