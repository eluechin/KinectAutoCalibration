﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    class CalculateToRealWorldStrategy : IKinectToRealWorldStrategy
    {
        public Dictionary<KinectPoint, RealWorldPoint> TransformKinectToRealWorld(IKinect kinect, List<KinectPoint> kinectPoints)
        {
            throw new NotImplementedException();
        }
    }
}
