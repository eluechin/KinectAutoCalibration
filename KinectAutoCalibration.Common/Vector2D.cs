using System;

namespace KinectAutoCalibration.Common
{
    public class Vector2D : IEquatable<Vector2D>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public BeamerPoint BeamerPoint { get; set; }

        public Vector2D Add(Vector2D vector)
        {
            if (vector == null)
            {
                return this;
            }
            return new Vector2D()
                {
                    X = X + vector.X,
                    Y = Y + vector.Y
                };
        }

        public Vector2D Multiply(double d)
        {
            return new Vector2D()
            {
                X = (X * d),
                Y = (Y * d)
            };
        }

        public Vector2D Subtract(Vector2D vector)
        {
            return new Vector2D()
            {
                X = X - vector.X,
                Y = Y - vector.Y
            };
        }

        public Vector2D Divide(double d)
        {
            return new Vector2D()
            {
                X = (X / d),
                Y = (Y / d)
            };
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public double Determinant(Vector2D vector)
        {
            return X * vector.Y - vector.X * Y;
        }

        public KinectPoint ToKinectPoint()
        {
            return new KinectPoint{X=(int) X, Y = (int) Y};
        }

        public bool Equals(Vector2D other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            return (int)X == (int)other.X && (int)Y == (int)other.Y;
        }

        public Vector2D Copy()
        {
            return new Vector2D{X = X, Y = Y};
        }
    }
}
