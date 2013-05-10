using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class ConvertToRealWorldStrategy : IKinectToRealWorldStrategy
    {
        public Dictionary<KinectPoint, RealWorldPoint> TransformKinectToRealWorld(IKinect kinect, List<KinectPoint> kinectPoints)
        {
            var rwCornerPoints = kinect.CreateRealWorldCoordinates(kinectPoints);
            foreach (var point in Calibration.Points)
            {
                foreach (var realWorldPoint in rwCornerPoints)
                {
                    if (point.KinectPoint.Equals(realWorldPoint.Key))
                    {
                        point.RealWorldPoint = realWorldPoint.Value;
                    }
                }
            }

            return rwCornerPoints;
        }
    }
}
