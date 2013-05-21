using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;

namespace KinectAutoCalibration.Calibration
{
    public static class RealWorldToArea
    {
        public static AreaPoint CalculateAreaPoint(RealWorldPoint realWorldPoint)
        {
            var areaPoint = new AreaPoint();
            var areaVector = ChangeOfBasis.GetVectorInNewBasis(realWorldPoint.ToVector3D());
            areaPoint.X = (int) areaVector.X;
            areaPoint.Y = (int) areaVector.Y;
            return areaPoint;
        }
    }
}
