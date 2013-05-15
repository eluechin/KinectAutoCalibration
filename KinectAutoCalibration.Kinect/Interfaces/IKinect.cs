using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Kinect;
using KinectAutoCalibration.Common;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace KinectAutoCalibration.Kinect
{
    public interface IKinect
    {
        KinectSensor DiscoverKinectSensor();
        void RaiseKinect();
        void LowerKinect();

        KinectPoint[,] GetDifferenceImage(KinectPoint[,] image2, KinectPoint[,] image1, int threshold);
        KinectPoint[,] GetColorImage();
        short[] GetDepthImage();
        KinectPoint[,] CreateKinectPointArray();

        Dictionary<KinectPoint, RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints);
        Dictionary<KinectPoint, RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints, RealWorldPoint a, RealWorldPoint b, RealWorldPoint c);
        Vector3D CreateRealWorldVector(RealWorldPoint p);

        byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height);
        Bitmap ConvertKinectPointArrayToBitmap(KinectPoint[,] kinArray, int width, int height);
        WriteableBitmap ConvertKinectPointArrayToWritableBitmap(KinectPoint[,] kinArray, int width, int height);
    }
}
