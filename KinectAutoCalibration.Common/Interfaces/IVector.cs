using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common.Interfaces
{
    public interface IVector : IEquatable<IVector>
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }

        IVector Add(IVector vector);
        IVector Multiply(double d);
        IVector Subtract(IVector vector);
        IVector Divide(double d);
        double GetLength();

    }
}
