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
        KinectPoint[,] GetDifferenceImage(KinectPoint[,] image2, KinectPoint[,] image1, int threshold);
        // !Q: Wieso kinArray als Parameter
        KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y);
        KinectPoint[,] GetColorImage();
        short[] GetDepthImage();
        KinectPoint[,] CreateKinectPointArray();
        KinectPoint[,] CreateRealWorldArray(KinectPoint[,] kinArray, int width, int height);
        Bitmap ConvertKinectPointArrayToBitmap(KinectPoint[,] kinArray, int width, int height);
        WriteableBitmap ConvertKinectPointArrayToWritableBitmap(KinectPoint[,] kinArray, int width, int height);

        Vector3D CreateRealWorldVector(KinectPoint p);

        void RaiseKinect();
        void LowerKinect();
        byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height);
    }
}
