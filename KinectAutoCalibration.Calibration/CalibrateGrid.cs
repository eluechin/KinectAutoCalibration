using System;
using System.Collections.Generic;
using System.Threading;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateGrid : IBeamerToKinectStrategy
    {
        //TODO XML Configuration?
        private const int CALIBRATION_ROUNDS = 2;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamerToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            var newPoints = new Dictionary<BeamerPoint, KinectPoint>();
            var simpleStrategy = new CalibrateEdgePoints();
            var beamerToKinect = simpleStrategy.CalibrateBeamerToKinect(beamerWindow, kinect);

            var diffImages = new Dictionary<int, KinectPoint[,]>();

            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImage(false, i);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);
                //var initPoints = CalculateInitVectors(beamerToKinect.Values.GetEnumerator(), i);
                diffImages.Add(i, diffKinectPoints);
            }
            //TODO Implement
            throw new NotImplementedException();
        }

        private List<Vector2D> CalculateInitVectors(List<Vector2D> points, int round)
        {
            var initVectors = new List<Vector2D>();
            if (round == 1)
            {
                initVectors.Add(new Vector2D {X = 0, Y = 0});
            }
            else
            {
                
            }
            return initVectors;
        }


    }
}
