using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public abstract class Calibration : ICalibration
    {
        //TODO: New Name...
        public static List<Point> Points;
        public static List<Point> EdgePoints; 

        public static List<BeamerPoint>[,] KinectSpace; 

        protected readonly IBeamerWindow beamerWindow;
        protected readonly IKinect kinect;

        protected readonly Dictionary<BeamerPoint, KinectPoint> beamerToKinect;
        protected readonly Dictionary<KinectPoint, RealWorldPoint> kinectToRealWorld;
        protected readonly Dictionary<RealWorldPoint, AreaPoint> realWorldToArea;

        protected Calibration()
        {
            Points = new List<Point>();
            beamerToKinect = new Dictionary<BeamerPoint, KinectPoint>();
            kinectToRealWorld = new Dictionary<KinectPoint, RealWorldPoint>();
            realWorldToArea = new Dictionary<RealWorldPoint, AreaPoint>();
            KinectSpace = new List<BeamerPoint>[640,480];

            try
            {
                beamerWindow = new BeamerWindow();
                kinect = new Kinect.Kinect();
            }
            catch (Exception ex)
            {
                // TODO Exception Handling
            }
        }

        public void DrawPointOnArea(int x, int y, Color c)
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetDifferenceImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetKinectImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetAreaImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetColorImage()
        {
            throw new NotImplementedException();
        }

        public void RaiseKinect()
        {
            kinect.RaiseKinect();
        }

        public void LowerKinect()
        {
            kinect.LowerKinect();
        }

        public static List<Point> GetEdgePoints()
        {
            var edgePoints = new List<Point>
                {
                    Points.Find(d => d.Name == "A"),
                    Points.Find(d => d.Name == "B"),
                    Points.Find(d => d.Name == "C"),
                    Points.Find(d => d.Name == "D")
                };

            return edgePoints;
        }
    }
}
