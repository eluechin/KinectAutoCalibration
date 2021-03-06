﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common
{
    public class RealWorldPoint : IEquatable<RealWorldPoint>
    {
         public RealWorldPoint()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.R = 0;
            this.G = 0;
            this.B = 0;
            this.Type = 0;
        }

        public RealWorldPoint(int x, int y, int r, int g, int b) : this()
        {
            this.X = x;
            this.Y = y;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public RealWorldPoint(int x, int y, int z, int r, int g, int b) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public Vector3D ToVector3D()
        {
            return new Vector3D { X = X, Y = Y, Z = Z };
        }


        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Type { get; set; }

        public bool Equals(RealWorldPoint other)
        {
            if (this.X == other.X &&
                this.Y == other.Y &&
                this.Z == other.Z)
            {
                return true;
            }
            return false;
        }
    }
}
