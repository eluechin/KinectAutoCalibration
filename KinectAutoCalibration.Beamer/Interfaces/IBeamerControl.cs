namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerControl
    {
        BeamerPoint2D DisplayCalibrationImageEdge(bool isInverted, int position);
        void DisplayCalibrationImage(bool isInverted, int depth);
        void DisplayBlank();
    }
}
