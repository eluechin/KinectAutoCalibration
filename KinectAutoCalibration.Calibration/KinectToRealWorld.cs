using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public static class KinectToRealWorld
    {

        public static RealWorldPoint CalculateRealWorldPoint(KinectPoint p)
        {
            var rwPoint = new RealWorldPoint
            {
                Z = p.Z,
            };

            rwPoint.X = (int)((p.X - (Kinect.Kinect.KINECT_IMAGE_WIDTH / 2)) * rwPoint.Z / Kinect.Kinect.WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (Kinect.Kinect.KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z / Kinect.Kinect.HEIGHT_CONST);

            return rwPoint;
        }
    }
}
