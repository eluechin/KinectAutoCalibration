using System.Drawing;

namespace KinectAutoCalibration.Calibration
{
    public interface IKinectBeamerOperation
    {
        void ColorizePoint(int x, int y, Color color);
        Color GetColorAtPoint(int x, int y);
        void DrawAreaToBeamer();

        int GetAreaWidth();
        int GetAreaHeight();
    }
}
