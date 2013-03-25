using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    public class Vector2D : IEquatable<Vector2D>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set;  }

        public Vector2D Add(Vector2D vector)
        {
            if (vector == null)
            {
                return this;
            }
            return new Vector2D()
                {
                    X = this.X + vector.X,
                    Y = this.Y + vector.Y
                };
        }

        public Vector2D Multiply(double d)
        {
            return new Vector2D()
            {
                X = (int)(X * d),
                Y = (int)(Y * d)
            };
        }

        public Vector2D Subtract(Vector2D vector)
        {
            return new Vector2D()
            {
                X = this.X - vector.X,
                Y = this.Y - vector.Y
            };
        }

        public Vector2D Divide(double d)
        {
            return new Vector2D()
            {
                X = (int)(X / d),
                Y = (int)(Y / d)
            };
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public bool Equals(Vector2D other)
        {
            if (other == null || GetType() != other.GetType())
                return false;
            return (int)X == (int)other.X && (int)Y == (int)other.Y;
        }
    }
}
