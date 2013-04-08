using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public KinectCalibration()
        {
            
            //worker.DoWork += worker_DoWork;
            //kinect = new Kinect.Kinect();
            Thread newWindowThread = new Thread(new ThreadStart(ThreadStartingPoint));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            //area = new Bitmap(WIDTH, HEIGHT);
            //area.SetPixel(100, 100, System.Drawing.Color.Red);
            //beamer.DrawChessBoard1(Colors.Red, Colors.Blue);
            //beamer.DrawCircle();
        }

        private void ThreadStartingPoint()
        {
            Window beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            };

            beamer = new Beamer.Beamer(beamerWindow);
            beamer.DisplayCalibrationImage(true);
            Thread.Sleep(3000);
            beamer.DisplayCalibrationImage(false);

            System.Windows.Threading.Dispatcher.Run();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            beamer.DisplayCalibrationImage(false);
            Thread.Sleep(3000);
            beamer.DisplayCalibrationImage(true);
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
            //worker.RunWorkerAsync();

            //KinectPoint[,] p1 = kinect.GetColorImage();
            ////Verzögerung
            //Thread.Sleep(3000);
            
            //Console.Read();
            //beamer.DisplayCalibrationImage(true);
            //KinectPoint[,] p2 = kinect.GetColorImage();

            //KinectPoint[,] diff = kinect.GetDifferenceImage(p1, p2, 200);
            //List<Vector3D> corners = GetCornerPoints(diff);
            //corners.Sort((first, second) => first != null ? first.X.CompareTo(second.X) : 0);
            //// Punkt mit niedrigstem Abstand(z) als mittelpunkt (param2)
            //// Punkt mit höchstem Abstand(z) nicht nehmen!!!!
            //ChangeOfBasis.InitializeChangeOfBasis(corners[1], corners[0], corners[2]);

            //List<Vector2D> corners2d = new List<Vector2D>();
            //foreach (var vector3D in corners)
            //{
            //    corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(vector3D));
            //}

            //Dictionary<Vector2D, int> lengthDic = calculateLength(corners2d);
            //List<KeyValuePair<Vector2D, int>> myList = lengthDic.ToList();
            //myList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

            //int height = myList[1].Value;
            //int width = myList[2].Value;

            //Bitmap area = new Bitmap(width, height);
            //beamer.DisplayBitmap(area);
        }

        public Bitmap GetColorBitmap()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetDifferenceBitmap()
        {
            throw new NotImplementedException();
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
            KinectPoint[,] realDiffImage = kinect.CreateRealWorldArray(diffImage);
            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(realDiffImage), KMeansHelper.CreateInitialCentroids(640, 480));

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


    }
}
