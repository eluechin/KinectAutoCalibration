using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Beamer
{
    public interface IBeamer
    {
        void DisplayCalibrationImage(bool isInverted);
        void DisplayBitmap(Bitmap bmp);
    }
}
