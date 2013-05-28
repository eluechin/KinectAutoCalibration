using System.Windows.Controls;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerWindow
    {
        BeamerPoint DisplayCalibrationImageEdge(bool isInverted, int position);
        void DisplayCalibrationImage(bool isInverted, int depth);
        void DisplayBlank();
        int GetWidth();
        int GetHeight();

        void DisplayContent(Canvas imageCanvas);
        void DisplayContent(WriteableBitmap bmp);
    }
}
