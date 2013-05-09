using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Calibration
{
    public interface ICalibration
    {
        void DrawPointOnArea(int x, int y, Color c);

        List<AreaPoint> GetAllObstaclePoints();
        AreaPoint GetCentroidOfObstacle();

        WriteableBitmap GetDifferenceImage();
        WriteableBitmap GetKinectImage();
        WriteableBitmap GetAreaImage();

        WriteableBitmap GetColorImage();
        void RaiseKinect();
        void LowerKinect();
    }
}
