using System;

namespace KinectAutoCalibration.Common
{
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D()
        {
            
        }

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
                (X * d),
                (Y * d),
                (Z * d)
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
                
                (X / d),
                (Y / d),
                (Z / d)
            );
        }

        public Vector3D CrossProduct(Vector3D vector)
        {
            return new Vector3D(
                    (this.Y * vector.Z - this.Z * vector.Y),
                    (this.Z * vector.X - this.X * vector.Z),
                    (this.X * vector.Y - this.Y * vector.X)
                
                );
        }

        public double ScalarProduct(Vector3D vector)
        {
            return (double) (this.X*vector.X + this.Y*vector.Y + this.Z*vector.Z);
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
        }

        public Vector3D GetNormedVector()
        {
            double magnitude = this.GetLength();

            return new Vector3D(
                    (this.X / magnitude),
                    (this.Y / magnitude),
                    (this.Z / magnitude)
                );
        }
    }
}
