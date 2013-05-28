using System.Windows.Media.Imaging;

namespace KinectAutoCalibration.Calibration
{
    public interface IKinectBeamerCalibration : ICalibration
    {
        void CalibrateBeamerToKinect(IBeamerToKinectStrategy beamerToKinectStrategy);
        void ConvertKinectToRealWorld(IKinectToRealWorldStrategy kinectToRealWorldStrategy);
        void RealWorldToAreaEdge();

        IKinectBeamerOperation CreateKinectBeamerOperation();
        
    }
}
