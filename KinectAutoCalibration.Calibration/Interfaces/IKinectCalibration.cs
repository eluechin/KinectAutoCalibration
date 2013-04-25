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
    public interface IKinectCalibration
    {
        void InitialCalibration();

        WriteableBitmap GetDifferenceBitmap();
        WriteableBitmap GetDifferenceImage();
        WriteableBitmap GetPic1Bitmap();
        WriteableBitmap GetPic2Bitmap();
        WriteableBitmap GetPicKinP();

        List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage);
        void GetObstacles(int c);
        void GetObstacles();
        void DisplayArea();
        void DisplayBlank();
        void CalibrateBeamer();
    }
}
