using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Calibration
{
    interface IKinectCalibration
    {
        void StartCalibration();
        Bitmap GetColorBitmap();
        Bitmap GetDifferenceBitmap();
        Bitmap GetAreaBitmap();

    }
}
