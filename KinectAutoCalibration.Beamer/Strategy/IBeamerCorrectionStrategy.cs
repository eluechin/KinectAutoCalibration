namespace KinectAutoCalibration.Beamer
{
    public interface IBeamerCorrectionStrategy
    {
        BeamerPoint CalculateBeamerCoordinate(AreaPoint areaCoordinate);
    }
}
