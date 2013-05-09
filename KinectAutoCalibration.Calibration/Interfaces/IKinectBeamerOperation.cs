using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Calibration.Interfaces
{
    public interface IKinectBeamerOperation
    {
        void ColorizePoint(int x, int y, Color color);
        Color GetColorAtPoint(int x, int y);

        int GetAreaWidth();
        int GetAreaHeight();
    }
}
