namespace KinectAutoCalibration.Common
{
    public class BeamerPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Convert the BeamerPoint into a Vector 2D
        /// </summary>
        /// <returns>Vector 2D Object with the values of this point.</returns>
        public Vector2D ToVector2D()
        {
            return new Vector2D { X = X, Y = Y };
        }
    }
}