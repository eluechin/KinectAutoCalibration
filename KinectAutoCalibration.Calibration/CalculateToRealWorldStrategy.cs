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
            var cornerPoints = new List<KinectPoint>();
            foreach (var point in Calibration.GetEdgePoints())
            {
                cornerPoints.Add(point.KinectPoint);
            }

            var rwCornerPoints = kinect.CreateRealWorldCoordinates(cornerPoints);
            //TODO: necessary? strategy ConvertToRealWorldStrategy always needed?
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

            RealWorldPoint rA = Calibration.Points.Find(x => x.Name == "A").RealWorldPoint;
            RealWorldPoint rB = Calibration.Points.Find(x => x.Name == "B").RealWorldPoint;
            RealWorldPoint rC = Calibration.Points.Find(x => x.Name == "C").RealWorldPoint;
            RealWorldPoint rD = Calibration.Points.Find(x => x.Name == "D").RealWorldPoint;

            return kinect.CreateRealWorldCoordinates(kinectPoints, rA, rB, rC);
        }
    }
}
