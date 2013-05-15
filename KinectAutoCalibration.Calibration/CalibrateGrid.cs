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
        private Point pointA;
        private Point pointB;
        private Point pointC;
        private Point pointD;
        //TODO XML Configuration?
        private const int CALIBRATION_ROUNDS = 2;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamerToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            RunSimpleStrategy(beamerWindow, kinect);

            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                beamerWindow.DisplayCalibrationImage(false, i);
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);
                var diffKinectVectors = KinectPointArrayHelper.ExtractBlackPointsAs2dVector(diffKinectPoints);

                //var newPoints = CalculateNewPoints(i, diffKinectVectors);

                //var initPoints = CalculateNewPoints(beamerToKinect.Values.GetEnumerator(), i);
            }

            return new Dictionary<BeamerPoint, KinectPoint>();
            //TODO Implement
            //throw new NotImplementedException();
        }

        private void RunSimpleStrategy(IBeamerWindow beamerWindow, IKinect kinect)
        {
            var simpleStrategy = new CalibrateEdgePoints();
            simpleStrategy.CalibrateBeamerToKinect(beamerWindow, kinect);

            var edgePoints = Calibration.GetEdgePoints();
            pointA = edgePoints.Find((e) => e.Name == "A");
            pointB = edgePoints.Find((e) => e.Name == "B");
            pointC = edgePoints.Find((e) => e.Name == "C");
            pointD = edgePoints.Find((e) => e.Name == "D");
        }

        private List<Vector2D> CalculateNewPoints(int round, List<Vector2D> diffKinectVectors)
        {
            var divisor = (int)Math.Pow(2, round);
            var numberOfAreaHorizontal = divisor;
            var numberOfAreaVertical = numberOfAreaHorizontal;

            var beamerWidth = (int)Math.Sqrt(Math.Pow(pointA.BeamerPoint.X, 2) + Math.Pow(pointB.BeamerPoint.X, 2)) / 2;
            var beamerHeight = (int)Math.Sqrt(Math.Pow(pointA.BeamerPoint.Y, 2) + Math.Pow(pointD.BeamerPoint.Y, 2)) / 2;

            var beameAreaWidth = beamerWidth / divisor;
            var beamerAreaHeight = beamerHeight / divisor;

            var kinectVectorA = pointA.KinectPoint.ToVector2D();
            var kinectVectorB = pointA.KinectPoint.ToVector2D();
            var kinectVectorC = pointA.KinectPoint.ToVector2D();
            var kinectVectorD = pointA.KinectPoint.ToVector2D();

            for (var i = 0; i <= numberOfAreaHorizontal; i++)
            {
                for (var j = 0; j <= numberOfAreaVertical; j++)
                {
                    if (i == 0 && j == 0 || i == numberOfAreaHorizontal && j == 0 || i == 0 && j == numberOfAreaVertical || i == numberOfAreaHorizontal && j == numberOfAreaVertical)
                    {
                        continue;
                    }

                    var beamerPoint = new BeamerPoint
                        {
                            X = pointA.BeamerPoint.X + i * beameAreaWidth,
                            Y = pointA.BeamerPoint.Y + j * beamerAreaHeight
                        };

                    var kinectPointPredicted = new KinectPoint();


                }
            }

            throw new NotImplementedException();
        }

        //private void CalculateInnerArea(int widthFactor, int heightFactor, int round)
        //{
        //    var topPoint = new Point();
        //    var middleLeftPoint = new Point();
        //    var middlePoint = new Point();
        //    var middleRightPoint = new Point();
        //    var bottomPoint = new Point();

        //    topPoint.BeamerPoint = new BeamerPoint { X = widthFactor * (pointA.X + pointB.X) / round, Y = (pointA.Y + pointB.Y) / 2 };
        //    middleLeftPoint.BeamerPoint = new BeamerPoint { X = (pointA.X + pointD.X) / 2, Y = heightFactor * (pointA.Y + pointD.Y) / heightFactor };



        //}
    }
}
