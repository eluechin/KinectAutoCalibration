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
            var beamerToKinect = simpleStrategy.CalibrateBeamterToKinect(beamerWindow, kinect);

            var diffImages = new Dictionary<int, KinectPoint[,]>();

            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImage(false, i);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);

                var initPoints = new List<Vector2D>() { new Vector2D { X = 0, Y = 0 } };
                diffImages.Add(i, diffKinectPoints);
            }
            //TODO Implement
            throw new NotImplementedException();
        }

        private List<Vector2D> CalculateInitVectors()
        {
            return new List<Vector2D>();
        } 
    }
}
