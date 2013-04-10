using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public interface IBeamer
    {
        void DisplayCalibrationImage(bool isInverted);
        void DisplayBitmap(Bitmap bmp);
        void DisplayBitmap(WriteableBitmap bmp);
        void DisplayRectangle(List<Vector2D> list);
        void DisplayBlank();
    }
}
