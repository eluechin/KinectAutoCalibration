using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public class OperationImage
    {
        public WriteableBitmap BeamerImage { get; set; }

        public OperationImage()
        {
            BeamerImage = new WriteableBitmap(
                Beamer.GetBeamerWidth(),
                Beamer.GetBeamerHeight(),
                96,
                96,
                PixelFormats.Bgr32,
                null);
        }

        public void ColorizePoint(BeamerPoint point)
        {
            byte[] ColorData = { 255, 0, 0, 0 }; // B G R

            Int32Rect rect = new Int32Rect(
                    point.X,
                    point.Y,
                    1,
                    1);

            BeamerImage.WritePixels(rect, ColorData, 4, 0);
        }
    }
}
