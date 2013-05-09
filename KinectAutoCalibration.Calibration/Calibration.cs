using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public abstract class Calibration : ICalibration
    {
        protected readonly IBeamerWindow beamerWindow;
        protected readonly IKinect kinect;

        protected readonly Dictionary<BeamerPoint, KinectPoint> beamerToKinect;
        protected readonly Dictionary<KinectPoint, RealWorldPoint> kinectToRealWorld;
        protected readonly Dictionary<RealWorldPoint, KinectPoint> realWorldToArea; 

        protected Calibration()
        {
            beamerToKinect = new Dictionary<BeamerPoint, KinectPoint>();

            try
            {
                beamerWindow = new BeamerWindow();
                kinect = new Kinect.Kinect();
            }
            catch (Exception ex)
            {
                // TODO Exception Handling
            }
        }

        public void DrawPointOnArea(int x, int y, Color c)
        {
            throw new NotImplementedException();
        }

        public List<AreaPoint> GetAllObstaclePoints()
        {
            throw new NotImplementedException();
        }

        public AreaPoint GetCentroidOfObstacle()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetDifferenceImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetKinectImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetAreaImage()
        {
            throw new NotImplementedException();
        }

        public WriteableBitmap GetColorImage()
        {
            throw new NotImplementedException();
        }

        public void RaiseKinect()
        {
            kinect.RaiseKinect();
        }

        public void LowerKinect()
        {
            kinect.LowerKinect();
        }
    }
}
