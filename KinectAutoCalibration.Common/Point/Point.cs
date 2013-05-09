namespace KinectAutoCalibration.Common
{
    public class Point
    {
        public string Name { get; set; }
        public BeamerPoint BeamerPoint { get; set; }
        public KinectPoint KinectPoint { get; set; }
        public RealWorldPoint RealWorldPoint { get; set; }
        public AreaPoint AreaPoint { get; set; }
    }
}
