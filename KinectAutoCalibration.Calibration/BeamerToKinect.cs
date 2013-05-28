using System;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Calibration
{
    public static class BeamerToKinect
    {
        private static readonly Point pointA;
        private static readonly Point pointB;
        private static readonly Point pointC;
        private static readonly Point pointD;

        static BeamerToKinect()
        {
            var edgePoints = Calibration.GetEdgePoints();
            pointA = edgePoints.Find(e => e.Name == "A");
            pointB = edgePoints.Find(e => e.Name == "B");
            pointC = edgePoints.Find(e => e.Name == "C");
            pointD = edgePoints.Find(e => e.Name == "D");
        }

        public static KinectPoint CalculateKinectPoint(BeamerPoint beamerPoint)
        {
            var kinectVectorA = new Vector2D { X = 639 - pointA.KinectPoint.X, Y = pointA.KinectPoint.Y };
            var kinectVectorB = new Vector2D { X = 639 - pointB.KinectPoint.X, Y = pointB.KinectPoint.Y };
            var kinectVectorC = new Vector2D { X = 639 - pointC.KinectPoint.X, Y = pointC.KinectPoint.Y };
            var kinectVectorD = new Vector2D { X = 639 - pointD.KinectPoint.X, Y = pointD.KinectPoint.Y };

            var beamerVectorA = pointA.BeamerPoint.ToVector2D();
            var beamerVectorB = pointB.BeamerPoint.ToVector2D();
            var beamerVectorC = pointC.BeamerPoint.ToVector2D();
            var beamerVectorD = pointD.BeamerPoint.ToVector2D();

            // ReSharper disable InconsistentNaming
            var detDA = beamerVectorD.Determinant(beamerVectorA);

            var detDB = beamerVectorD.Determinant(beamerVectorB);
            var detCA = beamerVectorC.Determinant(beamerVectorA);
            var detCB = beamerVectorC.Determinant(beamerVectorB);
            var detDC = beamerVectorD.Determinant(beamerVectorC);
            var detAB = beamerVectorA.Determinant(beamerVectorB);
            var detAC = beamerVectorA.Determinant(beamerVectorC);

            var beamerVectorP = beamerPoint.ToVector2D();

            var detDP = beamerVectorD.Determinant(beamerVectorP);
            var detCP = beamerVectorC.Determinant(beamerVectorP);
            var detPA = beamerVectorP.Determinant(beamerVectorA);
            var detPB = beamerVectorP.Determinant(beamerVectorB);
            var detAP = beamerVectorA.Determinant(beamerVectorP);
            var detPC = beamerVectorP.Determinant(beamerVectorC);
            // ReSharper restore InconsistentNaming

            var alphaMy = detDA - detDB - detCA + detCB;
            var betaMy = -2 * detDA + detDB + detDP + detCA - detCP + detPA - detPB;
            var gammaMy = detDA - detDP - detPA;

            var detMy = Math.Sqrt(betaMy * betaMy - 4 * alphaMy * gammaMy);
            var my1 = (-(betaMy / 2) + detMy) / (2 * alphaMy);
            var my2 = (-(betaMy / 2) - detMy) / (2 * alphaMy);

            var tmy = -gammaMy / betaMy;
            var my = tmy;

            var alphaLambda = detDC - detDB - detAC + detAB;
            var betaLambda = -2 * detDC + detDB + detDP + detAC - detAP + detPC - detPB;
            var gammaLambda = detDC - detDP - detPC;

            var detLambda = Math.Sqrt(betaLambda * betaLambda - 4 * alphaLambda * gammaLambda);
            var lambda1 = (-(betaLambda / 2) + detLambda) / (2 * alphaLambda);
            var lambda2 = (-(betaLambda / 2) - detLambda) / (2 * alphaLambda);

            var tlambda = -gammaLambda / betaLambda;
            var lambda = tlambda;

            var kinectVectorP =
                kinectVectorD.Multiply((1 - lambda) * (1 - my))
                             .Add(kinectVectorC.Multiply(((1 - lambda) * my)))
                             .Add(kinectVectorA.Multiply(lambda * (1 - my)))
                             .Add(kinectVectorB.Multiply(my * lambda));

            return kinectVectorP.ToKinectPoint();
        }
    }
}
