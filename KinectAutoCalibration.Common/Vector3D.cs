﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    public class Vector3D : IVector
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public IVector Add(IVector vector)
        {
            if (vector == null)
            {
                return this;
            }
            return new Vector3D(
                this.X + vector.X,
                this.Y + vector.Y,
                this.Z + vector.Z
            );
        }

        public IVector Multiply(double d)
        {
            return new Vector3D(
                (int)(X * d),
                (int)(Y * d),
                (int)(Z * d)
            );
        }

        public IVector Subtract(IVector vector)
        {
            return new Vector3D(
                this.X - vector.X,
                this.Y - vector.Y,
                this.Z - vector.Z
            );
        }

        public IVector Divide(double d)
        {
            return new Vector3D(
                
                (int)(X / d),
                (int)(Y / d),
                (int)(Z / d)
            );
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

    }
}
