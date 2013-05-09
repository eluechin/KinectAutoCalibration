using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateGrid : IBeamerToKinectStrategy
    {
        //TODO XML Configuration?
        private const int CALIBRATION_ROUNDS = 2;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamterToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImage(false, i);
                var picture2 = kinect.GetColorImage();
                
            }
            //TODO Implement
            throw new NotImplementedException();
        }
    }
}
