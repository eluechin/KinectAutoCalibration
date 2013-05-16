using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;
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
        //private KinectPoint[,] diffImage;

        private AreaPoint[,] area;
        private AreaPoint areaPointObstCentroid;

        public KinectBeamerOperation()
        {
            CalculateAreaDimensions();
            area = new AreaPoint[areaWidth,areaHeight];

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
            area[x,y] = new AreaPoint{Color = color, X = x, Y = y};
        }

        public Color GetColorAtPoint(int x, int y)
        {
            return area[x, y].Color;
        }

        public void DrawAreaToBeamer()
        {
            throw new NotImplementedException();
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

        public void ConvertKinectToRealWorld(IKinectToRealWorldStrategy kinectToRealWorldStrategy, List<KinectPoint> kinectPoints )
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

        private void CalculateAreaDimensions()
        {
            var areaPointA = Calibration.Points.Find((e) => e.Name == "A").AreaPoint;
            var areaPointB = Calibration.Points.Find((e) => e.Name == "B").AreaPoint;
            var areaPointC = Calibration.Points.Find((e) => e.Name == "C").AreaPoint;

            areaWidth = (int) Math.Sqrt(Math.Pow(areaPointB.X - areaPointA.X, 2) + Math.Pow(areaPointB.Y - areaPointB.Y, 2));
            areaHeight = (int)Math.Sqrt(Math.Pow(areaPointB.X - areaPointC.X, 2) + Math.Pow(areaPointB.Y - areaPointC.Y, 2));
        }

        public byte[] CompareZCalcStrategies(IKinectToRealWorldStrategy kinectToRealWorldStrategy)
        {
            var kinectPoints = kinect.CreateKinectPointArray();
            var kinectPointsList = new List<KinectPoint>();

            for (int y = 0; y < Kinect.Kinect.KINECT_IMAGE_HEIGHT ; ++y)
            {
                for (int x = 0; x < Kinect.Kinect.KINECT_IMAGE_WIDTH; ++x)
                    kinectPointsList.Add(kinectPoints[x,y]);
            }

            var dictPoints = kinectToRealWorldStrategy.TransformKinectToRealWorld(kinect, kinectPointsList);

            var diffPoints = new KinectPoint[Kinect.Kinect.KINECT_IMAGE_WIDTH,Kinect.Kinect.KINECT_IMAGE_HEIGHT];
            var differences = new List<int>();

            foreach (var p in dictPoints)
            {
                differences.Add(p.Key.Z - p.Value.Z);
            }
            differences.Sort();
            var maxDiff = differences.Max();
            var minDiff = differences.Min();
            var rangeDiff = maxDiff - minDiff;
            //var levels = 20;

            //Grey : 128, 128, 128
            var r = 0x80;
            var g = 0x80;
            var b = 0x80;

            foreach (var p in dictPoints)
            {
                int diff = p.Key.Z - p.Value.Z;
                //diff positiv --> gegen weiss, diff negativ --> gegen schwarz
                //r = 0x80 + diff / 1;
                //g = 0x80 + diff / 1;
                //b = 0x80 + diff / 1;

                r = 0x80 + (diff % 127);
                g = 0x80 + (diff % 127);
                b = 0x80 + (diff % 127);

                diffPoints[p.Key.X, p.Key.Y] = new KinectPoint(p.Key.X, p.Key.Y, diff, r, g, b);
            }

            return kinect.ConvertKinectPointArrayToByteArray(diffPoints, Kinect.Kinect.KINECT_IMAGE_WIDTH, Kinect.Kinect.KINECT_IMAGE_HEIGHT);
        }
        
        /*
        public byte[] GetObstacleDiffImage()
        {
            return kinect.ConvertKinectPointArrayToByteArray(diffImage, Kinect.Kinect.KINECT_IMAGE_WIDTH, Kinect.Kinect.KINECT_IMAGE_HEIGHT);
        }*/

    }
}
