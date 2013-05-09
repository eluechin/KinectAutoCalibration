using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateGrid : IBeamerToKinectStrategy
    {
        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamterToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            throw new NotImplementedException();
        }
    }
}
