using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    public class Vector2D : IVector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public IVector Add(IVector vector)
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

        public IVector Multiply(double d)
        {
            return new Vector2D()
            {
                X = (int)(X * d),
                Y = (int)(Y * d)
            };
        }

        public IVector Multiply(IVector vector)
        {
            throw new NotImplementedException();
        }

        public IVector Subtract(IVector vector)
        {
            return new Vector2D()
            {
                X = this.X - vector.X,
                Y = this.Y - vector.Y
            };
        }

        public IVector Divide(double d)
        {
            return new Vector2D()
            {
                X = (int)(X / d),
                Y = (int)(Y / d)
            };
        }

        public IVector Divide(IVector vector)
        {
            throw new NotImplementedException();
        }

        public double GetLength()
        {
            return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public int CompareTo(Object o)
        {
            var other = (Vector2D)o;
            if (X == other.X && Y == other.Y)
            {
                return 0;
            }
            return 1;
        }

    }
}
