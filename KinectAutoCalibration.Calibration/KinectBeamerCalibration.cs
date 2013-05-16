using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Kinect;

namespace KinectAutoCalibration.Calibration
{
    public class KinectBeamerCalibration : Calibration, IKinectBeamerCalibration
    {
        public const int THREAD_SLEEP = 1000;
        public const int THRESHOLD = 80;

        public void CalibrateBeamerToKinect(IBeamerToKinectStrategy beamerToKinectStrategy)
        {
            var newPoints = beamerToKinectStrategy.CalibrateBeamerToKinect(beamerWindow, kinect);
            foreach (var element in newPoints)
            {
                beamerToKinect.Add(element.Key, element.Value);
            }
        }

        public void ConvertKinectToRealWorld(IKinectToRealWorldStrategy kinectToRealWorldStrategy)
        {
            var kinectPoints = beamerToKinect.Values.ToList();
            var newPoints = kinectToRealWorldStrategy.TransformKinectToRealWorld(kinect, kinectPoints);
            foreach (var element in newPoints)
            {
                kinectToRealWorld.Add(element.Key, element.Value);
            }
        }

        public void RealWorldToArea()
        {

            var a = Points.Find((e) => e.Name == "A");
            var b = Points.Find((e) => e.Name == "B");
            var c = Points.Find((e) => e.Name == "C");
            var d = Points.Find((e) => e.Name == "D");

            var realWorldPointA = a.RealWorldPoint;
            var realWorldPointB = b.RealWorldPoint;
            var realWorldPointC = c.RealWorldPoint;
            var realWorldPointD = d.RealWorldPoint;

            var vectorRealWorldA = realWorldPointA.ToVector3D();
            var vectorRealWorldB = realWorldPointB.ToVector3D();
            var vectorRealWorldC = realWorldPointC.ToVector3D();
            var vectorRealWorldD = realWorldPointD.ToVector3D();

            //var coordinateSystemPoints = GetOriginPoint(vectorRealWorldA, vectorRealWorldB, vectorRealWorldC, vectorRealWorldD);
            ChangeOfBasis.InitializeChangeOfBasis(vectorRealWorldA, vectorRealWorldB, vectorRealWorldC);

            var vectorAreaA = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldA);
            var vectorAreaB = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldB);
            var vectorAreaC = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldC);
            var vectorAreaD = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldD);

            var areaPointA = new AreaPoint {X = (int) vectorAreaA.X, Y = (int) vectorAreaA.Y};
            var areaPointB = new AreaPoint { X = (int)vectorAreaB.X, Y = (int)vectorAreaB.Y };
            var areaPointC = new AreaPoint { X = (int)vectorAreaC.X, Y = (int)vectorAreaC.Y };
            var areaPointD = new AreaPoint { X = (int)vectorAreaD.X, Y = (int)vectorAreaD.Y };

            realWorldToArea.Add(realWorldPointA, areaPointA);
            realWorldToArea.Add(realWorldPointB, areaPointB);
            realWorldToArea.Add(realWorldPointC, areaPointC);
            realWorldToArea.Add(realWorldPointD, areaPointD);

            a.AreaPoint = areaPointA;
            b.AreaPoint = areaPointB;
            c.AreaPoint = areaPointC;
            d.AreaPoint = areaPointD;
        }

        private List<Vector3D> GetOriginPoint(Vector3D rwvA, Vector3D rwvB, Vector3D rwvC, Vector3D rwvD)
        {
            var dictVectors = new Dictionary<List<Vector3D>, double>();
            dictVectors.Add(new List<Vector3D> { rwvA, rwvB }, (rwvB.Subtract(rwvA)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvA, rwvD }, (rwvD.Subtract(rwvA)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvB, rwvA }, (rwvA.Subtract(rwvB)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvB, rwvC }, (rwvC.Subtract(rwvB)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvC, rwvB }, (rwvB.Subtract(rwvC)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvC, rwvD }, (rwvD.Subtract(rwvC)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvD, rwvC }, (rwvC.Subtract(rwvD)).GetLength());
            dictVectors.Add(new List<Vector3D> { rwvD, rwvA }, (rwvA.Subtract(rwvD)).GetLength());
            

            return null;
        }

        public IKinectBeamerOperation CreateKinectBeamerOperation()
        {
            return new KinectBeamerOperation();
        }
    }
}
