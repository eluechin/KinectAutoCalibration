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
        private WriteableBitmap diffBitmap;
        private WriteableBitmap pic1;
        private WriteableBitmap pic2;
        private WriteableBitmap picKinP;
        private int _height;
        private int _width;
        private KinectPoint[,] _differenceImage;
        private KinectPoint[,] p1;
        private KinectPoint[,] p2;
        private KinectPoint[,] kinP;
        private List<Vector3D> corners;
        private List<Vector2D> _corners2D;
        private KinectPoint[,] _white;

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

        public void InitialCalibration()
        {
            beamer.DisplayCalibrationImage(true);
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            Thread.Sleep(1000);
            beamer.DisplayCalibrationImage(false);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();

            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            diffBitmap = kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);
            
            //var realWorldArray = kinect.CreateRealWorldArray(kinArray);
            kinP = kinect.CreateKinectPointArray();
            corners = GetCornerPoints(_differenceImage);
            corners.Sort((first, second) => first != null ? first.Z.CompareTo(second.Z) : 0);
            
            //// Punkt mit niedrigstem Abstand(z) als mittelpunkt (param2)
            //// Punkt mit höchstem Abstand(z) nicht nehmen!!!!
            ChangeOfBasis.InitializeChangeOfBasis(corners[1], corners[0], corners[2]); //Corner Identification!!

            _corners2D = new List<Vector2D>();
            _corners2D.Add(ChangeOfBasis.GetVectorInNewBasis(corners[0]));
            _corners2D.Add(ChangeOfBasis.GetVectorInNewBasis(corners[1]));
            _corners2D.Add(ChangeOfBasis.GetVectorInNewBasis(corners[2]));
            //_corners2D.Add(ChangeOfBasis.GetVectorInNewBasis(corners[3]));
            //foreach (var vector3D in corners)
            //{
            //    corners2d.Add(ChangeOfBasis.GetVectorInNewBasis(vector3D));
            //}

            Dictionary<Vector2D, int> lengthDic = CalculateLength(_corners2D);
            List<KeyValuePair<Vector2D, int>> myList = lengthDic.ToList();
            myList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));

            var middle = ChangeOfBasis.GetVectorInNewBasis(corners[2]);
            _height = myList[1].Value;
            _width = myList[2].Value;

            //var lookupTable = new Dictionary<Vector2D, Vector2D>();
            //lookupTable.Add(new Vector2D{X=800,Y=600}, middle);

            //Thread.Sleep(1000);
            //beamer.DisplayCalibrationImage(true, 5);
            //Thread.Sleep(1000);
            //beamer.DisplayCalibrationImage(false, 5);
            
            beamer.DisplayBlank();
            Thread.Sleep(1000);
            _white = kinect.GetColorImage();
        }

        public void CalibrateBeamer()
        {
            beamer.DisplayGrid(true);
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            Thread.Sleep(1000);
            beamer.DisplayGrid(false);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();

            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            diffBitmap = kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);

            var centroidsInit = new List<Vector2D>
                {
                    new Vector2D {X = 320, Y = 0},
                    new Vector2D {X = 0, Y = 240},
                    new Vector2D {X = 320, Y = 240},
                    new Vector2D {X = 640 - 1, Y = 240},
                    new Vector2D {X = 320, Y = 480-1}
                };

            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImage), centroidsInit);

        }

        public void GetObstacles(int c)
        {
            beamer.DisplayCalibrationImage(true, c);
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            Thread.Sleep(500);
            beamer.DisplayCalibrationImage(false, c);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            var kinArray = kinect.CreateKinectPointArray();
            var realWorldArray = kinect.CreateRealWorldArray(kinArray);

            List<Vector3D> objs = KMeansHelper.ExtractBlackPointsAs3dVector(_differenceImage);
            var objs2D = new List<Vector2D>();
            foreach (var v in objs)
            {
                objs2D.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(realWorldArray[(int)v.X, (int)v.Y])));
            }

            List<Vector2D> beamerCoordinates = new List<Vector2D>();
            foreach (var realVector in objs2D)
            {
                beamerCoordinates.Add(CalculateBeamerCoordinate(realVector));
            }
            var gueltig = objs2D.Where(vector2D => vector2D.X > 0 && vector2D.Y > 0).ToList();

            var stride = _width * 4; // bytes per row

            var pixelData = new byte[_height * stride];
            try
            {
                foreach (var v2 in beamerCoordinates)
                {
                    var x = (int)v2.X;
                    var y = (int)v2.Y;
                    int index = y * _width * 4 + x * 4;
                    if (index > _height * stride || index < 0)
                        continue;

                    pixelData[index + 2] = (byte)0xff;
                    pixelData[index + 1] = (byte)0xff;
                    pixelData[index] = (byte)0xff;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            diffBitmap = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgr32, null);
            diffBitmap.WritePixels(new Int32Rect(0, 0, _width, _height), pixelData, _width * 4, 0);
        }

        private Vector2D CalculateBeamerCoordinate(Vector2D areaVector)
        {
            /*
            // 1400 = beamer width
            // 70 = tile width
            var s = new Vector2D{X = areaVector.X * (1400 - 2*70) / _width, Y = areaVector.Y + 70};
            var b = _corners2D[1];
            var t = s.Add(b);
            var u = new Vector2D{X = areaVector.X + 70, Y = areaVector.Y * (1050 - 2*70) / _height};
            var c = _corners2D[2];
            var v = c.Add(u);

            //var lambdaZaehler = t.Y*(u.X - v.X) - v.Y*(u.X - v.X) - t.X*(u.Y - v.Y) + v.X*(u.Y - v.Y);
            //var lambdaNenner = ((s.X - t.X)*(u.Y - v.Y)) - ((s.Y - t.Y)*(u.Y - v.Y));


            var lambdaZaehler = v.X*u.Y - v.Y*u.X;
            var lambdaNenner = -s.X*v.Y + s.X*u.Y + s.Y*v.X - s.Y*u.X;

            var lambda = lambdaZaehler/lambdaNenner;

            var temp = t.Subtract(s).Multiply(lambda);
            var P = s.Add(temp);
             * */
            var px = areaVector.X * 1760 / _width;
            var py = (areaVector.Y) * 1060 / _height;

            return new Vector2D { X = px, Y = py };
        }

        public void GetObstacles()
        {
            beamer.DisplayBlank();
            //MessageBox.Show("Display Obst");
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            _differenceImage = kinect.GetDifferenceImage(_white, p2, 80);
            var kinArray = kinect.CreateKinectPointArray();
            var realWorldArray = kinect.CreateRealWorldArray(kinArray);

            //var centroid = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImage), new List<Vector2D>{new Vector2D{X = 0, Y = 0}});
     
            List<Vector3D> objs = KMeansHelper.ExtractBlackPointsAs3dVector(_differenceImage);
            var objs2D = new List<Vector2D>();
            foreach (var v in objs)
            {
                KinectPoint p = realWorldArray[(int)v.X, (int)v.Y];
                if (p != null)
                {
                    objs2D.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(p)));
                }
            }
            //KinectPoint kp = realWorldArray[(int)centroid[0].X, (int)centroid[0].Y];
            //objs2D.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(kp)));

            List<Vector2D> beamerCoordinates = new List<Vector2D>();
            foreach (var realVector in objs2D)
            {
                beamerCoordinates.Add(CalculateBeamerCoordinate(realVector));
            }
            var stride = 1600 * 4; // bytes per row

            var pixelData = new byte[1200 * stride];
            try
            {
                foreach (var v2 in objs2D)
                {
                    var x = (int)v2.X + 70;
                    var y = 1200-(int)v2.Y -70;
                    int index = y * 1600 * 4 + x * 4;
                    if (index > 1200 * stride || index < 0)
                        continue;

                    pixelData[index + 2] = (byte)0xff;
                    pixelData[index + 1] = (byte)0xff;
                    pixelData[index] = (byte)0xff;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            diffBitmap = new WriteableBitmap(1600, 1200, 96, 96, PixelFormats.Bgr32, null);
            diffBitmap.WritePixels(new Int32Rect(0, 0, 1600, 1200), pixelData, 1600 * 4, 0);
        }

        public void DisplayBlank()
        {
            beamer.DisplayBlank();
        }

        public void DisplayArea()
        {
            beamer.DisplayBitmap(diffBitmap);
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

        public List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage)
        {
            var kinArray = kinect.CreateKinectPointArray();
            var realWorldArray = kinect.CreateRealWorldArray(kinArray);
            var corners = new List<Vector3D>();
            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(diffImage), KMeansHelper.CreateInitialCentroids(640, 480));
            foreach (var vectorCentroid in centroids)
            {
                corners.Add(kinect.CreateRealWorldVector(realWorldArray[(int)vectorCentroid.X, (int)vectorCentroid.Y]));
            }
            return corners;
        }

        void IKinectCalibration.GetObstacles(int c)
        {
            GetObstacles(c);
        }

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

        public WriteableBitmap GetDifferenceImage()
        {
            return kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);
        }

        public WriteableBitmap GetPicKinP()
        {
            return kinect.ConvertKinectPointArrayToWritableBitmap(kinP, 640, 480);
        }
    }
}
