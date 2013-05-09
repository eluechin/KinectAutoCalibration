using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    class TempCalibration
    {

        //private readonly int WIDTH = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        //private readonly int HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        private readonly IBeamerWindow beamerWindow;
        private readonly IKinect kinect;
        private WriteableBitmap diffBitmap;
        private WriteableBitmap pic1;
        private WriteableBitmap pic2;
        private WriteableBitmap pic3;
        private WriteableBitmap picKinP;
        private int _height;
        private int _width;
        private KinectPoint[,] _differenceImage;
        private KinectPoint[,] _differenceImageObst;
        private KinectPoint[,] p1;
        private KinectPoint[,] p2;
        private KinectPoint[,] p3;
        private KinectPoint[,] kinP;
        private List<Vector3D> corners;
        private Dictionary<int, RealWorldPoint> rwCorners;
        private List<Vector2D> _corners2D;
        private KinectPoint[,] _white;
        private List<Vector2D> _area2DVectors;
        private byte[] _areaArray;

        private const int CALIBRATION_ROUNDS = 2;
        private const int CALIBRATION_POINTS = 4;
        private const int THRESHOLD = 80;

        //private byte[] GetDifferenceImage()
        //{
        //    return kinect.GetDifferenceImage(p1, p2, 80);
        //}

        //public void GetObstacles()
        //{
        //    beamerWindow.DisplayBlank();
        //    //MessageBox.Show("Display Obst");
        //    Thread.Sleep(1000);
        //    p2 = kinect.GetColorImage();
        //    _differenceImageObst = kinect.GetDifferenceImage(_white, p2, 80);
        //    var kinArray = kinect.CreateKinectPointArray();
        //    //var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480);
        //    var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480, rwCorners[0], rwCorners[1],
        //                                                     rwCorners[2]);

        //    var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImageObst), new List<Vector2D> { new Vector2D { X = 0, Y = 0 } });

        //    List<Vector3D> objs = KMeansHelper.ExtractBlackPointsAs3dVector(_differenceImageObst);
        //    _area2DVectors = new List<Vector2D>();
        //    foreach (var v in objs)
        //    {
        //        RealWorldPoint p = realWorldArray[(int)v.X, (int)v.Y];
        //        if (p != null)
        //        {
        //            _area2DVectors.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(p)));
        //        }
        //    }
        //    RealWorldPoint kp = realWorldArray[(int)centroids[0].X, (int)centroids[0].Y];
        //    _area2DVectors.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(kp)));

        //    List<Vector2D> beamerCoordinates = new List<Vector2D>();
        //    foreach (var realVector in _area2DVectors)
        //    {
        //        beamerCoordinates.Add(CalculateBeamerCoordinate(realVector));
        //    }
        //    int widthPic = 640;
        //    int heightPic = 480;


        //    var stride = widthPic * 4; // bytes per row


        //    _areaArray = new byte[heightPic * stride];

        //    try
        //    {
        //        int counter = 0;
        //        foreach (var v2 in _area2DVectors)
        //        {
        //            var x = (int)v2.X + 70;
        //            var y = heightPic - (int)v2.Y - 70;
        //            int index = y * widthPic * 4 + x * 4;
        //            if (index > heightPic * stride || index < 0)
        //                continue;

        //            _areaArray[index + 2] = (byte)0xff;
        //            _areaArray[index + 1] = (byte)0xff;
        //            _areaArray[index] = (byte)0xff;
        //            ++counter;
        //        }



        //    }
        //    catch (Exception e)
        //    {
        //        Console.Error.WriteLine(e.StackTrace);
        //    }

        //    //diffBitmap = new WriteableBitmap(1600, 1200, 96, 96, PixelFormats.Bgr32, null);
        //    //diffBitmap.WritePixels(new Int32Rect(0, 0, 1600, 1200), _areaArray, 1600 * 4, 0);
        //}

        public byte[] GetAreaArray()
        {
            return _areaArray;
        }

        public void DisplayBlank()
        {
            beamerWindow.DisplayBlank();
        }

        public void DisplayArea()
        {
            //beamerWindow.DisplayBitmap(diffBitmap);
        }

        public Bitmap GetColorBitmap()
        {
            throw new NotImplementedException();
        }

        private static Dictionary<Vector2D, int> CalculateLength(IEnumerable<Vector2D> corners)
        {
            Dictionary<Vector2D, int> lengthDict = new Dictionary<Vector2D, int>();

            foreach (var vector2D in corners)
            {
                int length = (int)vector2D.GetLength();
                lengthDict.Add(vector2D, length);
            }
            return lengthDict;
        }
        /*
        public Dictionary<int, RealWorldPoint> GetCornerPoints(KinectPoint[,] diffImage)
        {
            var kinArray = kinect.CreateKinectPointArray();
            var rwCornersList = new List<RealWorldPoint>();
            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(diffImage), KMeansHelper.CreateInitialCentroids(640, 480));
            var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480);
            foreach (var vectorCentroid in centroids)
            {
                //corners.Add(kinect.CreateRealWorldVector(realWorldArray[(int)vectorCentroid.X, (int)vectorCentroid.Y]));
                rwCornersList.Add(realWorldArray[(int)vectorCentroid.X, (int)vectorCentroid.Y]);
            }

            rwCornersList.Sort((first, second) => first != null ? first.Z.CompareTo(second.Z) : 0);

            var rwCorners = new Dictionary<int, RealWorldPoint>();
            rwCorners.Add(0, rwCornersList[0]);
            rwCorners.Add(1, rwCornersList[1]);
            rwCorners.Add(2, rwCornersList[2]);
            rwCorners.Add(3, rwCornersList[3]);

            return rwCorners;
        }*/

        public WriteableBitmap GetPic1Bitmap()
        {
            pic1 = kinect.ConvertKinectPointArrayToWritableBitmap(p1, 640, 480);
            return pic1;
        }

        public WriteableBitmap GetPic2Bitmap()
        {
            pic2 = kinect.ConvertKinectPointArrayToWritableBitmap(p2, 640, 480);
            return pic2;
        }

        public WriteableBitmap GetDifferenceBitmap()
        {

            return diffBitmap;
        }

        public byte[] GetDifferenceImage()
        {
            //return kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);
            return kinect.ConvertKinectPointArrayToByteArray(_differenceImage, 640, 480);
        }

        public byte[] GetDifferenceImageObst()
        {
            return kinect.ConvertKinectPointArrayToByteArray(_differenceImageObst, 640, 480);
        }

        public byte[] GetPicKinP()
        {
            //return kinect.ConvertKinectPointArrayToWritableBitmap(kinP, 640, 480);
            return kinect.ConvertKinectPointArrayToByteArray(kinP, 640, 480);
        }

        public WriteableBitmap PollLiveColorImage()
        {
            p3 = kinect.GetColorImage();
            if (p3 != null)
            {
                return kinect.ConvertKinectPointArrayToWritableBitmap(p3, 640, 480);
            }
            return null;

        }

        public byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height)
        {
            return kinect.ConvertKinectPointArrayToByteArray(kinArray, width, height);
        }
    }
}

