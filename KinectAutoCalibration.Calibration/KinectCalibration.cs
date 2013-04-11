using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;
//using Vector3D = System.Windows.Media.Media3D.Vector3D;


namespace KinectAutoCalibration.Calibration
{
    public class KinectCalibration : IKinectCalibration
    {
        //private readonly int WIDTH = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        //private readonly int HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        private readonly int WIDTH = 1400;
        private readonly int HEIGHT = 1050;
        private static IBeamer beamer;
        private static IKinect kinect;
        private static Bitmap area;
        private static WriteableBitmap diffBitmap;
        private static WriteableBitmap pic1;
        private static WriteableBitmap pic2;
        private static int _height;
        private static int _width;
        private static KinectPoint[,] _differenceImage;
        private static KinectPoint[,] p1;
        private static KinectPoint[,] p2;
        private static List<Vector3D> corners;


        public KinectCalibration()
        {
            Window beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            };

            beamer = new Beamer.Beamer(beamerWindow);
            kinect = new Kinect.Kinect();
        }

        public KinectCalibration(Window beamerWindow)
        {
            beamer = new Beamer.Beamer(beamerWindow);
            kinect = new Kinect.Kinect();
        }

        public void ShowArea()
        {
            beamer.DisplayBitmap(area);
        }

        public void StartCalibration()
        {

            beamer.DisplayCalibrationImage(true);
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            //p1 = kinect.CreateKinectPointArray();
            //pic1 = kinect.ConvertKinectPointArrayToWritableBitmap(p1, 640, 480);
            Thread.Sleep(1000);
            beamer.DisplayCalibrationImage(false);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            //p2 = kinect.CreateKinectPointArray();
            pic2 = kinect.ConvertKinectPointArrayToWritableBitmap(p2, 640, 480);

            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            diffBitmap = kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);
            
            corners = GetCornerPoints(_differenceImage);
            corners.Sort((first, second) => first != null ? first.Z.CompareTo(second.Z) : 0);

            //// Punkt mit niedrigstem Abstand(z) als mittelpunkt (param2)
            //// Punkt mit höchstem Abstand(z) nicht nehmen!!!!
            ChangeOfBasis.InitializeChangeOfBasis(corners[2], corners[0], corners[1]);

            

        }

        public static void GetObstacles()
        {
            beamer.DisplayCalibrationImage(true,3);
            //beamer.DisplayBlank();
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            //MessageBox.Show("Start Object scanning");
            Thread.Sleep(2000);
            beamer.DisplayCalibrationImage(false,3);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            //pic1 = kinect.ConvertKinectPointArrayToWritableBitmap(diff, 640, 480);

            List<Vector2D> corners2d = new List<Vector2D>();
            corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(corners[0]));
            corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(corners[1]));
            corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(corners[3]));
            corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(corners[2]));
            //foreach (var vector3D in corners)
            //{
            //    corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(vector3D));
            //}

            Dictionary<Vector2D, int> lengthDic = calculateLength(corners2d);
            List<KeyValuePair<Vector2D, int>> myList = lengthDic.ToList();
            myList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

            _height = myList[1].Value;
            _width = myList[2].Value;

            KinectPoint[,] diff;
            int width = _width;
            int height = _height;
            List<Vector3D> objs = KMeansHelper.ExtractBlackPointsAs3dVector(_differenceImage);
            List<Vector2D> objs2D = new List<Vector2D>();
            foreach (var v in objs)
            {
                objs2D.Add(ChangeOfBasis.GetVectorInNewBasis(v));
            }

            var stride = width*4; // bytes per row

            byte[] pixelData = new byte[height*stride];
            int index = 0;
            //pixelData[2] = (byte) 0xFF;
            try
            {
                foreach (var v2 in objs2D)
                {
                    var x = (int) v2.X;
                    var y = (int) v2.Y;
                    index = y*width*4 + x*4;
                    if (index > height*stride || index < 0)
                        continue;

                    pixelData[index + 2] = (byte) 0xFF;
                    pixelData[index + 1] = (byte) 0;
                    pixelData[index] = (byte) 0;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            //pic1.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
            pic1 = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            pic1.WritePixels(new Int32Rect(0, 0, width, height), pixelData, width*4, 0);
            //beamer.DisplayRectangle(corners2d);

            //Bitmap area = new Bitmap(width, height);
            //beamer.DisplayBitmap(area);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            beamer.DisplayCalibrationImage(false);
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        public Bitmap GetColorBitmap()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetDifferenceBitmap()
        {
            return diffBitmap;
        }

        public Bitmap GetAreaBitmap()
        {
            return area;
        }

        private static Dictionary<Vector2D, int> calculateLength(List<Vector2D> corners)
        {
            Dictionary<Vector2D, int> lengthDict = new Dictionary<Vector2D, int>();

            foreach (var vector2D in corners)
            {
                int length = (int)vector2D.GetLength();
                lengthDict.Add(vector2D, length);
            }
            return lengthDict;
        }

        public List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage)
        {
            List<Vector3D> corners = new List<Vector3D>();
            //KinectPoint[,] realDiffImage = kinect.CreateRealWorldArray(diffImage);
            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(diffImage), KMeansHelper.CreateInitialCentroids(640, 480));

            foreach (var vectorCentroid in centroids)
            {
                var p = diffImage[(int)vectorCentroid.X, (int)vectorCentroid.Y];
                corners.Add(new Vector3D { X = p.X, Y = p.Y, Z = p.Z });
            }
            return corners;
        }

        void IKinectCalibration.GetObstacles()
        {
            GetObstacles();
        }


        public WriteableBitmap GetPic1Bitmap()
        {
            return pic1;
        }

        public WriteableBitmap GetPic2Bitmap()
        {
            return pic2;
        }
    }
}
