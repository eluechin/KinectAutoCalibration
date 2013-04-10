﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;
//using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace KinectAutoCalibration.Calibration
{
    interface IKinectCalibration
    {
        void StartCalibration();
        Bitmap GetColorBitmap();
        WriteableBitmap GetDifferenceBitmap();
        Bitmap GetAreaBitmap();

        List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage);
        List<Vector2D> GetObstacles();
    }
}
