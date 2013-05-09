using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public class LinearCorrection : IBeamerCorrectionStrategy
    {
        public BeamerPoint CalculateBeamerCoordinate(AreaPoint areaCoordinate)
        {
            var width = Beamer.GetBeamerWidth();
            var height = Beamer.GetBeamerHeight();

            var px = areaCoordinate.X * (width - 2 * BeamerImage.TILE_WIDTH) / width;
            var py = (areaCoordinate.Y) * (height - 2 * BeamerImage.TILE_HEIGHT) / height;

            return new BeamerPoint { X = px, Y = py };
        }
    }
}
