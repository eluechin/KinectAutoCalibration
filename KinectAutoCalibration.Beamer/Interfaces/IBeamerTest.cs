using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KinectAutoCalibration.Beamer.Interfaces
{
    public interface IBeamerTest
    {

        void DrawCircle();
        void DrawChessBoard1(Color c1, Color c2);
    }
}
