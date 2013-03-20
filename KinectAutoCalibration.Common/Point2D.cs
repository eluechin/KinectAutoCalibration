using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    public class Point2D : IPoint2D
    {

        public int X { get; set; }
        public int Y { get; set; }
    }
}
