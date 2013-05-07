namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerControl
    {
        void DisplayCalibrationImageEdge(bool isInverted);
        void DisplayCalibrationImage(bool isInverted, int depth);
        void DisplayBlank();
    }
}
