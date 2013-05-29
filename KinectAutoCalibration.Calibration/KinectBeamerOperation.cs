using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;
using Color = System.Drawing.Color;
using Point = KinectAutoCalibration.Common.Point;

namespace KinectAutoCalibration.Calibration
{
    public class KinectBeamerOperation : IKinectBeamerOperation
    {
        protected readonly IBeamerWindow beamerWindow;
        protected readonly IKinect kinect;
        private int areaWidth;
        private int areaHeight;
        private KinectPoint[,] kinectPoints;
        private KinectPoint[,] blankImage;

        private AreaPoint[,] areaSpace;
        private AreaPoint areaPointObstCentroid;

        public KinectBeamerOperation()
        {
            CalculateAreaDimensions();
            areaSpace = new AreaPoint[areaWidth, areaHeight];

            try
            {
                beamerWindow = new BeamerWindow();
                kinect = new Kinect.Kinect();
            }
            catch (Exception ex)
            {
                // TODO Exception Handling
            }

            beamerWindow.DisplayBlank();
            Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
            blankImage = kinect.GetColorImage();

            kinectPoints = kinect.CreateKinectPointArray();
        }

        public void ColorizePoint(int x, int y, Color color)
        {
            areaSpace[x, y] = new AreaPoint { Color = color, X = x, Y = y };
        }

        public Color GetColorAtPoint(int x, int y)
        {
            return areaSpace[x, y].Color;
        }

        public void ColorizeObstacle()
        {
            beamerWindow.DisplayBlank();
            Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
            var obstacleImage = kinect.GetColorImage();
            var diffImage = kinect.GetDifferenceImage(obstacleImage, blankImage,
                                                      KinectBeamerCalibration.THRESHOLD);

            var kinectPoints = KinectPointArrayHelper.ExtractBlackPoints(diffImage);

            var image = new OperationImage();
            var beamerPoints = new List<BeamerPoint>();

            foreach (var kinectPoint in kinectPoints)
            {
                var x = kinectPoint.X;
                if (Calibration.KinectSpace[x, kinectPoint.Y] != null)
                {
                    var tbeamerPoints = Calibration.KinectSpace[x, kinectPoint.Y];
                    beamerPoints.AddRange(tbeamerPoints);
                }
            }

            foreach (var beamerPoint in beamerPoints)
            {
                image.ColorizePoint(beamerPoint);
            }
            beamerWindow.DisplayContent(image.BeamerImage);
        }

        public WriteableBitmap ObstacleToArea()
        {
            beamerWindow.DisplayBlank();
            Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
            var obstacleImage = kinect.GetColorImage();
            var diffImage = kinect.GetDifferenceImage(obstacleImage, blankImage,
                                                      KinectBeamerCalibration.THRESHOLD);

            var kinectPoints = KinectPointArrayHelper.ExtractBlackPoints(diffImage);

            var realWorldPoints = new List<RealWorldPoint>();
            var realWorldStrategy = new CalculateToRealWorldStrategy();
            var kinToReal = realWorldStrategy.TransformKinectToRealWorld(kinect, kinectPoints.ToList());

            var areaPoints = new List<AreaPoint>();
            foreach (var kinToRealPair in kinToReal)
            {
                var realWorldPoint = kinToRealPair.Value;
                var areaPoint = ChangeOfBasis.GetVectorInNewBasis(realWorldPoint.ToVector3D()).ToAreaPoint();
            }

            foreach (var areaPoint in areaPoints)
            {
                areaSpace[areaPoint.X, areaPoint.Y] = areaPoint;
            }

            return GetAreaSpace();
        }

        public void DisplayBlank()
        {
            beamerWindow.DisplayBlank();
        }

        public int GetAreaWidth()
        {
            return areaWidth;
        }

        public int GetAreaHeight()
        {
            return areaHeight;
        }

        public int GetObstacleCentroidX()
        {
            return areaPointObstCentroid.X;
        }

        public int GetObstacleCentroidY()
        {
            return areaPointObstCentroid.Y;
        }

        public void CalculateObstacleCentroid()
        {
            var obstacleImage = kinect.GetColorImage();
            var diffImage = kinect.GetDifferenceImage(obstacleImage, blankImage, KinectBeamerCalibration.THRESHOLD);

            var initPoints = new List<Vector2D>() { new Vector2D { X = 0, Y = 0 } };
            var centroids = KMeans.DoKMeans(KinectPointArrayHelper.ExtractBlackPointsAs2dVector(diffImage), initPoints);

            var kinectPointCentroids = new List<KinectPoint>();
            kinectPointCentroids.Add(kinectPoints[(int)centroids[0].X, (int)centroids[0].Y]);

            ConvertKinectToRealWorld(new CalculateToRealWorldStrategy(), kinectPointCentroids);

            var realWorldObstCentroid = Calibration.Points.Find((x) => x.Name == "ObstacleCentroid").RealWorldPoint;
            var vectorRealWorldObstCentroid = realWorldObstCentroid.ToVector3D();

            var vectorAreaObstCentroid = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldObstCentroid);
            areaPointObstCentroid = new AreaPoint { X = (int)vectorAreaObstCentroid.X, Y = (int)vectorAreaObstCentroid.Y };

            Calibration.Points.Find((x) => x.Name == "ObstacleCentroid").AreaPoint = areaPointObstCentroid;
        }

        public void ConvertKinectToRealWorld(IKinectToRealWorldStrategy kinectToRealWorldStrategy, List<KinectPoint> kinectPoints)
        {
            var newPoints = kinectToRealWorldStrategy.TransformKinectToRealWorld(kinect, kinectPoints);
            foreach (var element in newPoints)
            {
                var point = new Point();
                point.Name = "ObstacleCentroid";
                point.KinectPoint = element.Key;
                point.RealWorldPoint = element.Value;
                Calibration.Points.Add(point);
            }
        }

        public WriteableBitmap GetKinectSpace()
        {
            var kinectSpace = new WriteableBitmap(
                640,
                480,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            for (var i = 0; i < kinectSpace.Width; i++)
            {
                for (var j = 0; j < kinectSpace.Height; j++)
                {
                    byte[] ColorData;
                    if (Calibration.KinectSpace[i, j] == null)
                    {
                        ColorData = new byte[] { 255, 255, 255, 0 }; // B G R
                    }
                    else
                    {
                        ColorData = new byte[] { 255, 0, 0, 0 }; // B G R
                    }

                    Int32Rect rect = new Int32Rect(
                            i,
                            j,
                            1,
                            1);

                    kinectSpace.WritePixels(rect, ColorData, 4, 0);
                }
            }

            return kinectSpace;
        }

        public WriteableBitmap GetAreaSpace()
        {
            var areaSpaceBmp = new WriteableBitmap(
                640,
                480,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            for (var i = 0; i < GetAreaWidth(); i++)
            {
                for (var j = 0; j < GetAreaHeight(); j++)
                {
                    byte[] ColorData;
                    if (areaSpace[i, j] == null)
                    {
                        ColorData = new byte[] { 255, 255, 255, 0 }; // B G R
                    }
                    else
                    {
                        ColorData = new byte[] { 0, 255, 0, 0 }; // B G R
                    }

                    Int32Rect rect = new Int32Rect(
                            i,
                            j,
                            1,
                            1);

                    areaSpaceBmp.WritePixels(rect, ColorData, 4, 0);
                }
            }

            return areaSpaceBmp;
        }

        private void CalculateAreaDimensions()
        {
            var areaPointA = Calibration.Points.Find((e) => e.Name == "A").AreaPoint;
            var areaPointB = Calibration.Points.Find((e) => e.Name == "B").AreaPoint;
            var areaPointC = Calibration.Points.Find((e) => e.Name == "C").AreaPoint;

            areaWidth = (int)Math.Sqrt(Math.Pow(areaPointB.X - areaPointA.X, 2) + Math.Pow(areaPointB.Y - areaPointB.Y, 2));
            areaHeight = (int)Math.Sqrt(Math.Pow(areaPointB.X - areaPointC.X, 2) + Math.Pow(areaPointB.Y - areaPointC.Y, 2));
        }

        public byte[] CompareZCalcStrategies(IKinectToRealWorldStrategy kinectToRealWorldStrategy)
        {
            var kinectPoints = kinect.CreateKinectPointArray();
            var kinectPointsList = new List<KinectPoint>();

            for (int y = 0; y < Kinect.Kinect.KINECT_IMAGE_HEIGHT; ++y)
            {
                for (int x = 0; x < Kinect.Kinect.KINECT_IMAGE_WIDTH; ++x)
                    kinectPointsList.Add(kinectPoints[x, y]);
            }

            var dictPoints = kinectToRealWorldStrategy.TransformKinectToRealWorld(kinect, kinectPointsList);

            var diffPoints = new KinectPoint[Kinect.Kinect.KINECT_IMAGE_WIDTH, Kinect.Kinect.KINECT_IMAGE_HEIGHT];
            var differences = new List<int>();

            foreach (var p in dictPoints)
            {
                differences.Add(p.Key.Z - p.Value.Z);
            }
            differences.Sort();
            var maxDiff = differences.Max();
            var minDiff = differences.Min();
            var rangeDiff = maxDiff - minDiff;

            var r = 0x80;
            var g = 0x80;
            var b = 0x80;

            foreach (var p in dictPoints)
            {
                int diff = p.Key.Z - p.Value.Z;
                //diff positiv --> gegen weiss, diff negativ --> gegen schwarz

                if (diff == 0)
                {
                    r = 0x00;
                    b = 0x00;
                    g = 0xFF;
                }
                else
                {
                    r = 0x80 + (int)(diff * 127.0 / rangeDiff);
                    g = 0x80 + (int)(diff * 127.0 / rangeDiff);
                    b = 0x80 + (int)(diff * 127.0 / rangeDiff);

                }

                diffPoints[p.Key.X, p.Key.Y] = new KinectPoint(p.Key.X, p.Key.Y, diff, r, g, b);
            }

            var edgepoints = Calibration.GetEdgePoints();
            foreach (var edgepoint in edgepoints)
            {
                var x = edgepoint.KinectPoint.X;
                var y = edgepoint.KinectPoint.Y;
                diffPoints[x, y] = new KinectPoint(x, y, 0x00, 0x00, 0xFF);
            }

            return kinect.ConvertKinectPointArrayToByteArray(diffPoints, Kinect.Kinect.KINECT_IMAGE_WIDTH, Kinect.Kinect.KINECT_IMAGE_HEIGHT);
        }

    }
}
