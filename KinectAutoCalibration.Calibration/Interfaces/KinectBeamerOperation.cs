using System;
using System.Drawing;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Calibration
{
    public class KinectBeamerOperation : IKinectBeamerOperation
    {
        private int areaWidth;
        private int areaHeight;

        private readonly AreaPoint[,] area;
        

        public KinectBeamerOperation()
        {
            CalculateAreaDimensions();
            area = new AreaPoint[areaWidth,areaHeight];
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
