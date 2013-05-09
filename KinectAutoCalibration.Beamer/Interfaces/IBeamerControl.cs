using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerControl
    {
        BeamerPoint DisplayCalibrationImageEdge(bool isInverted, int position);
        void DisplayCalibrationImage(bool isInverted, int depth);
        void DisplayBlank();
    }
}
