﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class CalibrateEdgePoints : IBeamerToKinectStrategy
    {
        // TODO Put in XML Configuration File?
        private const int CALIBRATION_POINTS = 4;

        public Dictionary<BeamerPoint, KinectPoint> CalibrateBeamterToKinect(IBeamerWindow beamerWindow, IKinect kinect)
        {
            var beamerToKinect = new Dictionary<BeamerPoint, KinectPoint>();
            var kinectPoints = kinect.CreateKinectPointArray();

            for (var i = 0; i < CALIBRATION_POINTS; i++)
            {
                var beamerPoint = beamerWindow.DisplayCalibrationImageEdge(true, i);
                var picture1 = kinect.GetColorImage();
                Thread.Sleep(KinectBeamerCalibration.THREAD_SLEEP);
                beamerWindow.DisplayCalibrationImageEdge(false, i);
                var picture2 = kinect.GetColorImage();

                var diffKinectPoints = kinect.GetDifferenceImage(picture1, picture2, KinectBeamerCalibration.THRESHOLD);

                //TODO Refactoring
                var initPoints = new List<Vector2D>() { new Vector2D { X = 0, Y = 0 } };
                var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(diffKinectPoints), initPoints);
                var kinectPoint = kinectPoints[(int)centroids[0].X, (int)centroids[0].Y];
                beamerToKinect.Add(beamerPoint, kinectPoint);
            }
            return beamerToKinect;
        }
    }
}
