using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Calibration
{
    public interface IAutoKinectBeamerCalibration : ICalibration
    {
        void StartAutoCalibration();
    }
}
