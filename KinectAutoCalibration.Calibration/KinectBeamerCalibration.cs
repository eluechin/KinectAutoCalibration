using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class KinectBeamerCalibration : IKinectBeamerCalibration
    {
        public const int THREAD_SLEEP = 1000;
        public const int THRESHOLD = 80;

        private readonly IBeamerWindow beamerWindow;
        private readonly IKinect kinect;
        private readonly Dictionary<BeamerPoint, KinectPoint> beamerToKinect;
        private readonly Dictionary<KinectPoint, RealWorldPoint> kinectToRealWorld;
        private readonly Dictionary<RealWorldPoint, KinectPoint> realWorldToArea; 

        public KinectBeamerCalibration()
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

        public void CalibrateBeamerToKinect(IBeamerToKinectStrategy beamerToKinectStrategy)
        {
            var newPoints = beamerToKinectStrategy.CalibrateBeamterToKinect(beamerWindow, kinect);
            foreach (var element in newPoints)
            {
                beamerToKinect.Add(element.Key, element.Value);
            }
        }

        public void ConvertKinectToRealWorld(IKinectToRealWorldStrategy kinectToRealWorldStrategy)
        {
            
        }

        public void RealWorldToArea()
        {
            var a = new Vector3D();
            var b = new Vector3D();
            var c = new Vector3D();

            //TODO a,b und c ermittlen
            ChangeOfBasis.InitializeChangeOfBasis(a, b, c);
        }

    }

    
}
