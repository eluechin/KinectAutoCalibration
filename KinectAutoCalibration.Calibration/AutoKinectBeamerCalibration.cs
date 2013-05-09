using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;
using Color = System.Windows.Media.Color;

namespace KinectAutoCalibration.Calibration
{
    public class AutoKinectBeamerCalibration : Calibration, IAutoKinectBeamerCalibration
    {
        private IKinectBeamerCalibration kinectBeamerCalibration;

        public AutoKinectBeamerCalibration()
        {
            kinectBeamerCalibration = new KinectBeamerCalibration();
        }

        public void StartAutoCalibration()
        {
            throw new NotImplementedException();
        }
    }
}
