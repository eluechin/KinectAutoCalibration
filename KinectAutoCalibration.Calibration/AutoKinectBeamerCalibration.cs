﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class AutoKinectBeamerCalibration : IAutoKinectBeamerCalibration
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

        public AutoKinectBeamerCalibration()
        {
            beamerWindow = new BeamerWindow();
            kinect = new Kinect.Kinect();
        }

        public void InitialCalibration()
        {
            var beamerToKinect = new Dictionary<BeamerPoint, KinectPoint>();

            kinP = kinect.CreateKinectPointArray();

            for (var i = 0; i < CALIBRATION_POINTS; i++)
            {
                var beamerPoint = beamerWindow.DisplayCalibrationImageEdge(true, i);
                p1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImageEdge(false, i);
                Thread.Sleep(1000);
                p2 = kinect.GetColorImage();

                _differenceImage = kinect.GetDifferenceImage(p1, p2, THRESHOLD);
                var initPoints = new List<Vector2D>() { new Vector2D { X = 0, Y = 0 } };
                var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImage), initPoints);

                beamerToKinect.Add(beamerPoint, kinP[(int) centroids[0].X, (int) centroids[0].Y]);



                //rwCorners = GetCornerPoints(_differenceImage);
                //foreach (var rwCorner in rwCorners)
                //{
                //    var p = kinect.CreateRealWorldVector(rwCorner.Value);
                //    corners.Add(p);
                //}
            }
            //var realWorldArray = kinect.CreateRealWorldArray(kinArray);           

            /*int error = 0;
            foreach (var kinectPoint in kinP)
            {
                if (kinectPoint.Z == -1)
                {
                    ++error;
                }
            }*/

            var realWorldArray = kinect.CreateRealWorldArray(kinP, 640, 480);
            corners = new List<Vector3D>();
            foreach (var element in beamerToKinect)
            {
                var kinectPoint = element.Value;
                corners.Add(kinect.CreateRealWorldVector(realWorldArray[kinectPoint.X, kinectPoint.Y]));
            }
            corners.Sort((first, second) => first != null ? first.Z.CompareTo(second.Z) : 0);

            //// Punkt mit niedrigstem Abstand(z) als mittelpunkt (param2)
            //// Punkt mit höchstem Abstand(z) nicht nehmen!!!!
            ChangeOfBasis.InitializeChangeOfBasis(corners[1], corners[0], corners[2]);

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

            beamerWindow.DisplayBlank();
            Thread.Sleep(1000);
            _white = kinect.GetColorImage();
        }

        //private byte[] GetDifferenceImage()
        //{
        //    return kinect.GetDifferenceImage(p1, p2, 80);
        //}

        public void CalibrateBeamer()
        {
            for (var i = 1; i <= CALIBRATION_ROUNDS; i++)
            {
                beamerWindow.DisplayCalibrationImage(true, i);
                p1 = kinect.GetColorImage();
                Thread.Sleep(1000);
                beamerWindow.DisplayCalibrationImage(false, i);
                p2 = kinect.GetColorImage();
            }
            //    _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);

            //var centroidsInit = new List<Vector2D>
            //    {
            //        new Vector2D {X = 320, Y = 0},
            //        new Vector2D {X = 0, Y = 240},
            //        //new Vector2D {X = 320, Y = 240},
            //        new Vector2D {X = 640 - 1, Y = 240},
            //        new Vector2D {X = 320, Y = 480-1}
            //    };

            //var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImage), centroidsInit);

            //foreach (var c in centroids)
            //{
            //    var p = _differenceImage[(int) c.X, (int) c.Y];
            //    p.R = 255;
            //    p.G = 0;
            //    p.B = 0;
            //    _differenceImage[(int) c.X, (int) c.Y] = p;
            //}

            //foreach (var ci in centroidsInit)
            //{
            //    var p = _differenceImage[(int)ci.X, (int)ci.Y];
            //    p.R = 0;
            //    p.G = 255;
            //    p.B = 0;
            //    _differenceImage[(int)ci.X, (int)ci.Y] = p;
            //}

            //diffBitmap = kinect.ConvertKinectPointArrayToWritableBitmap(_differenceImage, 640, 480);

        }

        public int GetAreaWidth()
        {
            return _width;
        }

        public int GetAreaHeight()
        {
            return _height;
        }

        public int GetObstacleCentroidX()
        {
            return (int)_area2DVectors[0].X;
        }

        public int GetObstacleCentroidY()
        {
            return (int)_area2DVectors[0].Y;
        }

        public void GetObstacles(int c)
        {
            beamerWindow.DisplayCalibrationImage(true, c);
            Thread.Sleep(1000);
            p1 = kinect.GetColorImage();
            Thread.Sleep(500);
            beamerWindow.DisplayCalibrationImage(false, c);
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            _differenceImage = kinect.GetDifferenceImage(p1, p2, 80);
            var kinArray = kinect.CreateKinectPointArray();
            var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480);

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
            // 1400 = beamerWindow width
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
            beamerWindow.DisplayBlank();
            //MessageBox.Show("Display Obst");
            Thread.Sleep(1000);
            p2 = kinect.GetColorImage();
            _differenceImageObst = kinect.GetDifferenceImage(_white, p2, 80);
            var kinArray = kinect.CreateKinectPointArray();
            //var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480);
            var realWorldArray = kinect.CreateRealWorldArray(kinArray, 640, 480, rwCorners[0], rwCorners[1],
                                                             rwCorners[2]);

            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(_differenceImageObst), new List<Vector2D> { new Vector2D { X = 0, Y = 0 } });

            List<Vector3D> objs = KMeansHelper.ExtractBlackPointsAs3dVector(_differenceImageObst);
            _area2DVectors = new List<Vector2D>();
            foreach (var v in objs)
            {
                RealWorldPoint p = realWorldArray[(int)v.X, (int)v.Y];
                if (p != null)
                {
                    _area2DVectors.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(p)));
                }
            }
            RealWorldPoint kp = realWorldArray[(int)centroids[0].X, (int)centroids[0].Y];
            _area2DVectors.Add(ChangeOfBasis.GetVectorInNewBasis(kinect.CreateRealWorldVector(kp)));

            List<Vector2D> beamerCoordinates = new List<Vector2D>();
            foreach (var realVector in _area2DVectors)
            {
                beamerCoordinates.Add(CalculateBeamerCoordinate(realVector));
            }
            int widthPic = 640;
            int heightPic = 480;


            var stride = widthPic * 4; // bytes per row


            _areaArray = new byte[heightPic * stride];

            try
            {
                int counter = 0;
                foreach (var v2 in _area2DVectors)
                {
                    var x = (int)v2.X + 70;
                    var y = heightPic - (int)v2.Y - 70;
                    int index = y * widthPic * 4 + x * 4;
                    if (index > heightPic * stride || index < 0)
                        continue;

                    _areaArray[index + 2] = (byte)0xff;
                    _areaArray[index + 1] = (byte)0xff;
                    _areaArray[index] = (byte)0xff;
                    ++counter;
                }



            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }

            //diffBitmap = new WriteableBitmap(1600, 1200, 96, 96, PixelFormats.Bgr32, null);
            //diffBitmap.WritePixels(new Int32Rect(0, 0, 1600, 1200), _areaArray, 1600 * 4, 0);
        }

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
        }

        void IAutoKinectBeamerCalibration.GetObstacles(int c)
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

        public void RaiseKinect()
        {
            kinect.RaiseKinect();
        }

        public void LowerKinect()
        {
            kinect.LowerKinect();
        }

        public byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height)
        {
            return kinect.ConvertKinectPointArrayToByteArray(kinArray, width, height);
        }
    }
}