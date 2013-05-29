namespace KinectAutoCalibration.Calibration
{
    public interface IAutoKinectBeamerCalibration : ICalibration
    {
        IKinectBeamerOperation StartAutoCalibration();
    }
}
