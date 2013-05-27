using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalculateToRealWorldStrategy : IKinectToRealWorldStrategy
    {
        public Dictionary<KinectPoint, RealWorldPoint> TransformKinectToRealWorld(IKinect kinect, List<KinectPoint> kinectPoints)
        {
            
            RealWorldPoint rOrigin = Calibration.Points.Find(x => x.PointType == PointType.Origin).RealWorldPoint;
            RealWorldPoint rXAxisPoint = Calibration.Points.Find(x => x.PointType == PointType.xAxis).RealWorldPoint;
            RealWorldPoint rYAxisPoint = Calibration.Points.Find(x => x.PointType == PointType.yAxis).RealWorldPoint;

            return kinect.CreateRealWorldCoordinates(kinectPoints, rXAxisPoint, rOrigin, rYAxisPoint);
        }
    }
}
