namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerCorrectionStrategy
    {
        BeamerPoint2D CalculateBeamerCoordinate(AreaPoint2D areaCoordinate);
    }
}
