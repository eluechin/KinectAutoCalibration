using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Kinect;
using KinectAutoCalibration.Common;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KinectAutoCalibration.Kinect.Interfaces
{
    public interface IKinect
    {
        KinectSensor DiscoverKinectSensor();
        KinectPoint[,] GetDifferenceImage(KinectPoint[,] image2, KinectPoint[,] image1, int threshold);
        KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y);
        KinectPoint[,] GetColorImage();
        short[] GetDepthImage();
        KinectPoint[,] GetDepthAndColorImage();
        WriteableBitmap PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height);
        void PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height, WriteableBitmap wrBitmap);


    }
}
