using System.Drawing;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Calibration
{
    public interface IKinectBeamerOperation
    {
        void ColorizePoint(int x, int y, Color color);
        Color GetColorAtPoint(int x, int y);

        int GetAreaWidth();
        int GetAreaHeight();
        void CalculateObstacleCentroid();
        int GetObstacleCentroidX();
        int GetObstacleCentroidY();
        byte[] CompareZCalcStrategies(IKinectToRealWorldStrategy kinectToRealWorldStrategy);
        void ColorizeObstacle();
        //byte[] GetObstacleDiffImage();
    }
}
