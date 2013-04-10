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
        private IBeamer beamer;
        private IKinect kinect;
        private Bitmap area;
        private static WriteableBitmap diffBitmap;
        private static WriteableBitmap pic1;
        private static WriteableBitmap pic2;


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
            KinectPoint[,] p1 = kinect.GetColorImage();
            pic1 = kinect.ConvertKinectPointArrayToWritableBitmap(p1, 640, 480);
            Thread.Sleep(1000);
            beamer.DisplayCalibrationImage(false);
            Thread.Sleep(1000);
            KinectPoint[,] p2 = kinect.GetColorImage();
            pic2 = kinect.ConvertKinectPointArrayToWritableBitmap(p2, 640, 480);

            KinectPoint[,] diff = kinect.GetDifferenceImage(p1, p2, 80);
            diffBitmap = kinect.ConvertKinectPointArrayToWritableBitmap(diff, 640, 480);
            
            List<Vector3D> corners = GetCornerPoints(diff);
            corners.Sort((first, second) => first != null ? first.Z.CompareTo(second.Z) : 0);

            //// Punkt mit niedrigstem Abstand(z) als mittelpunkt (param2)
            //// Punkt mit höchstem Abstand(z) nicht nehmen!!!!
            ChangeOfBasis.InitializeChangeOfBasis(corners[2], corners[0], corners[1]);

            MessageBox.Show("Start Object scanning");
            beamer.DisplayBlank();
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            Thread.Sleep(2000);
            p2 = kinect.GetColorImage();
            diff = kinect.GetDifferenceImage(p1, p2, 80);
            pic1 = kinect.ConvertKinectPointArrayToWritableBitmap(diff, 640, 480);

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

            int height = myList[1].Value;
            int width = myList[2].Value;



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

        private Dictionary<Vector2D, int> calculateLength(List<Vector2D> corners)
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

        public List<Vector2D> GetObstacles()
        {
            throw new NotImplementedException();
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
