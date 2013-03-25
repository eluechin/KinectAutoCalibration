using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common.Interfaces
{
    public interface IVector : IEquatable<IVector>
    {

        IVector Add(IVector vector);
        IVector Multiply(double d);
        IVector Subtract(IVector vector);
        IVector Divide(double d);
        double GetLength();

    }
}
