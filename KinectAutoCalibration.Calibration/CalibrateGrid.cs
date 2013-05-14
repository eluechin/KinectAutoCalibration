using System;
using System.Collections.Generic;
using System.Threading;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;
using System.Linq;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateGrid : IBeamerToKinectStrategy
    {
        private List<Point> edgePoints;
        //TODO XML Configuration?
        private const int CALIBRATION_ROUNDS = 2;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamerToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            var newPoints = new Dictionary<BeamerPoint, KinectPoint>();

            var simpleStrategy = new CalibrateEdgePoints();
            var beamerToKinect = simpleStrategy.CalibrateBeamerToKinect(beamerWindow, kinect);

            edgePoints = Calibration.GetEdgePoints();

            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImage(false, i);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);

                var initVectors = CalculateInitVectors(i);
                var diffKinectVectors = KinectPointArrayHelper.ExtractBlackPointsAs2dVector(diffKinectPoints);

                //var initPoints = CalculateInitVectors(beamerToKinect.Values.GetEnumerator(), i);
            }
            //TODO Implement
            throw new NotImplementedException();
        }

        private List<Vector2D> CalculateInitVectors(int round)
        {

            var initVectors = new List<Vector2D>();
            var widthDivisor = (int) Math.Pow(2, round);

            for (var i = 1; i <= round; i++)
            {
                for (int j = 0; j < round; j++)
                {
                    
                }
                initVectors.AddRange(CalculateTopLine(i, widthDivisor));
            }

            
          
            return initVectors;
        }

        private IEnumerable<Vector2D> CalculateTopLine(int numberOfPoints, int divisor)
        {
            var topVectors = new List<Vector2D>();

            var kinectPointA = edgePoints.Find(d => d.Name == "A").KinectPoint;
            var kinectPointB = edgePoints.Find(d => d.Name == "B").KinectPoint;
            var vectorA = kinectPointA.ToVector2D();
            var vectorB = kinectPointB.ToVector2D();

            var localDivisor = divisor;

            for (var i = 1; i <= numberOfPoints; i++)
            {
                topVectors.Add(vectorA.Add(vectorB.Subtract(vectorA).Divide(localDivisor)));
                localDivisor += divisor;
            }

            return topVectors;
        }
    }
}
