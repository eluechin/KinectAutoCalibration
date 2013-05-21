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
        private Point pointA;
        private Point pointB;
        private Point pointC;
        private Point pointD;
        //TODO XML Configuration?
        private const int CALIBRATION_ROUNDS = 2;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamerToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            RunSimpleStrategy(beamerWindow, kinect);

            CalculateNewPoints();


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

        private List<Vector2D> CalculateNewPoints()
        {
            var divisor = (int)Math.Pow(2, CALIBRATION_ROUNDS);
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

            var beamerVectorA = pointA.BeamerPoint.ToVector2D();
            var beamerVectorB = pointB.BeamerPoint.ToVector2D();
            var beamerVectorC = pointC.BeamerPoint.ToVector2D();
            var beamerVectorD = pointD.BeamerPoint.ToVector2D();

            var detDA = beamerVectorD.Determinant(beamerVectorA);
            var detDB = beamerVectorD.Determinant(beamerVectorB);
            var detCA = beamerVectorC.Determinant(beamerVectorA);
            var detCB = beamerVectorC.Determinant(beamerVectorB);

            var detDC = beamerVectorD.Determinant(beamerVectorC);
            var detAB = beamerVectorA.Determinant(beamerVectorB);
            var detAC = beamerVectorA.Determinant(beamerVectorC);

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
                    var beamerVectorP = beamerPoint.ToVector2D();

                    var detDP = beamerVectorD.Determinant(beamerVectorP);
                    var detCP = beamerVectorC.Determinant(beamerVectorP);
                    var detPA = beamerVectorP.Determinant(beamerVectorA);
                    var detPB = beamerVectorP.Determinant(beamerVectorB);
                    var detPP = beamerVectorP.Determinant(beamerVectorP);
                    var detAP = beamerVectorA.Determinant(beamerVectorP);
                    var detPC = beamerVectorP.Determinant(beamerVectorC);

                    var alphaMy = detDA - detDB - detCA + detCB;
                    var betaMy = -2 * detDA + detDB + detDP + detCA - detCP + detPA - detPB;
                    var gammaMy = detDA - detDP - detPA + detPP;

                    var my1 = (-(betaMy / 2) + Math.Sqrt(betaMy * betaMy - 4 * alphaMy * gammaMy)) / (2 * alphaMy);
                    var my2 = (-(betaMy / 2) - Math.Sqrt(betaMy * betaMy - 4 * alphaMy * gammaMy)) / (2 * alphaMy);

                    var my = my1 >= 0 && my1 <= 1 ? my1 : my2;

                    var alphaLambda = detDC - detDB - detAC + detAB;
                    var betaLambda = -2 * detDC + detDB + detDP + detAC - detAP + detPB;
                    var gammaLambda = detDC - detDP - detPC + detPP;

                    var lambda1 = (-(betaLambda / 2) + Math.Sqrt(betaLambda * betaLambda - 4 * alphaLambda * gammaLambda)) / (2 *
                                  alphaLambda);
                    var lambda2 = (-(betaLambda / 2) - Math.Sqrt(betaLambda * betaLambda - 4 * alphaLambda * gammaLambda)) / (2 *
                                  alphaLambda);

                    var lambda = lambda1 >= 0 && lambda1 <= 1 ? lambda1 : lambda2;

                    var kinectVectorP =
                        kinectVectorD.Multiply((1 - lambda)*(1 - my))
                                     .Add(kinectVectorC.Multiply(((1 - lambda)*my)))
                                     .Add(kinectVectorA.Multiply(lambda*(1 - my)))
                                     .Add(kinectVectorB.Multiply(my*lambda));

                    var kinectPointPredicted = kinectVectorP.ToKinectPoint();

                    var newPoint = new Point
                        {
                            BeamerPoint = beamerPoint,
                            KinectPoint = kinectPointPredicted
                        };
                    Calibration.Points.Add(newPoint);
                }
            }
            
            return new List<Vector2D>();
        }

        //private void CalculateInnerArea(int widthFactor, int heightFactor, int CALIBRATION_ROUNDS)
        //{
        //    var topPoint = new Point();
        //    var middleLeftPoint = new Point();
        //    var middlePoint = new Point();
        //    var middleRightPoint = new Point();
        //    var bottomPoint = new Point();

        //    topPoint.BeamerPoint = new BeamerPoint { X = widthFactor * (pointA.X + pointB.X) / CALIBRATION_ROUNDS, Y = (pointA.Y + pointB.Y) / 2 };
        //    middleLeftPoint.BeamerPoint = new BeamerPoint { X = (pointA.X + pointD.X) / 2, Y = heightFactor * (pointA.Y + pointD.Y) / heightFactor };



        //}
    }
}
