using System.Collections.Generic;
using System.Threading;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateEdgePoints : IBeamerToKinectStrategy
    {
        // TODO Put in XML Configuration File?
        private const int CALIBRATION_POINTS = 4;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamerToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            var beamerToKinect = new Dictionary<BeamerPoint, KinectPoint>();
            var kinectPoints = kinect.CreateKinectPointArray();
            

            for (var i = 0; i < CALIBRATION_POINTS; i++)
            {
                var beamerPoint = beamerWindow.DisplayCalibrationImageEdge(true, i);
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                beamerWindow.DisplayCalibrationImageEdge(false, i);
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);

                //TODO Refactoring
                var initPoints = new List<Vector2D>() { new Vector2D { X = 0, Y = 0 } };
                var centroids = KMeans.DoKMeans(KinectPointArrayHelper.ExtractBlackPointsAs2dVector(diffKinectPoints), initPoints);
                var kinectPoint = kinectPoints[(int)centroids[0].X, (int)centroids[0].Y];
                beamerToKinect.Add(beamerPoint, kinectPoint);

                var name = "";
                switch (i + 1)
                {
                    case 1:
                        name = "A";
                        break;
                    case 2:
                        name = "B";
                        break;
                    case 3:
                        name = "C";
                        break;
                    case 4:
                        name = "D";
                        break;
                }

                Calibration.Points.Add(new Point { Name = name, BeamerPoint = beamerPoint, KinectPoint = kinectPoint });
            }
            return beamerToKinect;
        }
    }
}
