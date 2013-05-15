﻿using System;
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

    }
}
