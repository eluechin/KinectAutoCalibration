using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    public class Vector3D
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

        public Vector3D Add(Vector3D vector)
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

        public Vector3D Multiply(double d)
        {
            return new Vector3D(
                (int)(X * d),
                (int)(Y * d),
                (int)(Z * d)
            );
        }

        public Vector3D Subtract(Vector3D vector)
        {
            return new Vector3D(
                this.X - vector.X,
                this.Y - vector.Y,
                this.Z - vector.Z
            );
        }

        public Vector3D Divide(double d)
        {
            return new Vector3D(
                
                (int)(X / d),
                (int)(Y / d),
                (int)(Z / d)
            );
        }

        public Vector3D CrossProduct(Vector3D vector)
        {
            return new Vector3D(
                    (int)(this.Y * vector.Z - this.Z * vector.Y),
                    (int)(this.Z * vector.X - this.X * vector.Z),
                    (int)(this.X * vector.Y - this.Y * vector.X)
                
                );
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }
    }
}
