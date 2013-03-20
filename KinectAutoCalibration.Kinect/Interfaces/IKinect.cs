﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Kinect;
using KinectAutoCalibration.Common;
using System.Windows.Media.Imaging;

namespace KinectAutoCalibration.Kinect.Interfaces
{
    public interface IKinect
    {
        KinectPoint[,] GetDifferenceImage(KinectPoint[,] picture2, KinectPoint[,] picture1, int tolerance);
        KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y);
        KinectPoint[,] GetColorImage();
        short[] GetDepthImage();
        KinectPoint[,] GetDepthAndColorImage();
        WriteableBitmap PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height);


    }
}
