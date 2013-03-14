using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common
{
    class Vector2D : IVector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IVector Add(IVector vector)
        {
            throw new NotImplementedException();
        }

        public IVector Multiply(double d)
        {
            throw new NotImplementedException();
        }

        public IVector Multiply(IVector vector)
        {
            throw new NotImplementedException();
        }

        public IVector Subtract(IVector vector)
        {
            throw new NotImplementedException();
        }

        public IVector Divide(double d)
        {
            throw new NotImplementedException();
        }

        public IVector Divide(IVector vector)
        {
            throw new NotImplementedException();
        }

        public double GetLength()
        {
            throw new NotImplementedException();
        }
    }
}
