using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common.Interfaces
{
    public interface IVector
    {
        int X { get; set; }
        int Y { get; set; }

        IVector Add(IVector vector);
        IVector Multiply(double d);
        IVector Subtract(IVector vector);
        IVector Divide(double d);
        double GetLength();

    }
}
