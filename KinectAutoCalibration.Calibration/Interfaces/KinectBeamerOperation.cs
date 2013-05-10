using System;
using System.Drawing;

namespace KinectAutoCalibration.Calibration
{
    public class KinectBeamerOperation : IKinectBeamerOperation
    {
        private int areaWidth;
        private int areaHeight;

        public KinectBeamerOperation()
        {
            CalculateAreaDimensions();
        }

        public void ColorizePoint(int x, int y, Color color)
        {
            throw new NotImplementedException();
        }

        public Color GetColorAtPoint(int x, int y)
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

        private void CalculateAreaDimensions()
        {
            var areaPointA = Calibration.Points.Find((e) => e.Name == "A").AreaPoint;
            var areaPointB = Calibration.Points.Find((e) => e.Name == "B").AreaPoint;
            var areaPointC = Calibration.Points.Find((e) => e.Name == "C").AreaPoint;

            areaWidth = (int) Math.Sqrt(Math.Pow(areaPointB.X - areaPointA.X, 2) + Math.Pow(areaPointB.Y - areaPointB.Y, 2));
            areaWidth = (int)Math.Sqrt(Math.Pow(areaPointB.X - areaPointC.X, 2) + Math.Pow(areaPointB.Y - areaPointC.Y, 2));
        }
    }
}
