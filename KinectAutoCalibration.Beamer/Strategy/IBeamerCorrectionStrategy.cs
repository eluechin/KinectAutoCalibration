namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerCorrectionStrategy
    {
        Point2D CalculateBeamerCoordinate(AreaPoint2D areaCoordinate);
    }
}
