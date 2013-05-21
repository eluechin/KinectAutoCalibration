using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            pointA = edgePoints.Find((e) => e.Name == "A");
            pointB = edgePoints.Find((e) => e.Name == "B");
            pointC = edgePoints.Find((e) => e.Name == "C");
            pointD = edgePoints.Find((e) => e.Name == "D");
        }

        public static KinectPoint CalculateKinectPoint(BeamerPoint beamerPoint)
        {
            var kinectVectorA = pointA.KinectPoint.ToVector2D();
            var kinectVectorB = pointA.KinectPoint.ToVector2D();
            var kinectVectorC = pointA.KinectPoint.ToVector2D();
            var kinectVectorD = pointA.KinectPoint.ToVector2D();

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
            var detPP = beamerVectorP.Determinant(beamerVectorP);
            var detAP = beamerVectorA.Determinant(beamerVectorP);
            var detPC = beamerVectorP.Determinant(beamerVectorC);
            // ReSharper restore InconsistentNaming

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
                kinectVectorD.Multiply((1 - lambda) * (1 - my))
                             .Add(kinectVectorC.Multiply(((1 - lambda) * my)))
                             .Add(kinectVectorA.Multiply(lambda * (1 - my)))
                             .Add(kinectVectorB.Multiply(my * lambda));

            return kinectVectorP.ToKinectPoint();
        }
    }
}
