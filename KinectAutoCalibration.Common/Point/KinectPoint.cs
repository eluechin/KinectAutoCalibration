using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common
{
    public class KinectPoint
    {
        public KinectPoint()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.R = 0;
            this.G = 0;
            this.B = 0;
            this.Type = 0;
        }

        public KinectPoint(int x, int y, int r, int g, int b) : this()
        {
            this.X = x;
            this.Y = y;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public KinectPoint(int x, int y, int z, int r, int g, int b) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Type { get; set; }

        public bool Equals(KinectPoint kinectPoint)
        {
            if (this.X == kinectPoint.X &&
                this.Y == kinectPoint.Y &&
                this.Z == kinectPoint.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
