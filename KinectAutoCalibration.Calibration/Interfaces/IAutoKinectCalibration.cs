using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;
//using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace KinectAutoCalibration.Calibration
{
    public interface IAutoKinectCalibration
    {
        void InitialCalibration();

        WriteableBitmap GetDifferenceBitmap();
        byte[] GetDifferenceImage();
        byte[] GetDifferenceImageObst();
        WriteableBitmap GetPic1Bitmap();
        WriteableBitmap GetPic2Bitmap();
        byte[] GetPicKinP();
        WriteableBitmap PollLiveColorImage();
        void RaiseKinect();
        void LowerKinect();
        byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height);

        List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage);
        void GetObstacles(int c);
        void GetObstacles();
        void DisplayArea();
        void DisplayBlank();
        void CalibrateBeamer();

        int GetAreaWidth();
        int GetAreaHeight();
        int GetObstacleCentroidX();
        int GetObstacleCentroidY();
        byte[] GetAreaArray();
    }
}
