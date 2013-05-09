using System.Windows.Controls;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerWindow
    {
        BeamerPoint DisplayCalibrationImageEdge(bool isInverted, int position);
        void DisplayCalibrationImage(bool isInverted, int depth);
        void DisplayBlank();

        void DisplayContent(Canvas imageCanvas);
    }
}
