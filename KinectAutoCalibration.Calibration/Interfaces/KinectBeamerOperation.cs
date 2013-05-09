using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Calibration.Interfaces
{
    public class KinectBeamerOperation : IKinectBeamerOperation
    {
        public void ColorizePoint(int x, int y, Color color)
        {
            throw new NotImplementedException();
        }

        public Color GetColorAtPoint(int x, int y)
        {
            throw new NotImplementedException();
        }

        public int GetAreaWidth()
        {
            throw new NotImplementedException();
        }

        public int GetAreaHeight()
        {
            throw new NotImplementedException();
        }
    }
}
