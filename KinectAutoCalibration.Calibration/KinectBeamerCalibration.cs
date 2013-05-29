using System;
using System.Collections.Generic;
using System.Linq;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;

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

        public void RealWorldToAreaEdge()
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

            var coordinateSystemPoints = GetOriginPoint(vectorRealWorldA, vectorRealWorldB, vectorRealWorldC, vectorRealWorldD);
            ChangeOfBasis.InitializeChangeOfBasis(coordinateSystemPoints[1], coordinateSystemPoints[0], coordinateSystemPoints[2]);


            var vectorAreaA = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldA);
            var vectorAreaB = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldB);
            var vectorAreaC = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldC);
            var vectorAreaD = ChangeOfBasis.GetVectorInNewBasis(vectorRealWorldD);

            var areaPointA = new AreaPoint { X = (int)vectorAreaA.X, Y = (int)vectorAreaA.Y };
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

        private Vector3D[] GetOriginPoint(Vector3D rwvA, Vector3D rwvB, Vector3D rwvC, Vector3D rwvD)
        {
            Vector3D[] coordinateSystemPoints = new Vector3D[3];

            var dictVectorsA = new Dictionary<Vector3D, int>();
            dictVectorsA.Add(rwvB, (int)(rwvB.Subtract(rwvA)).GetLength());
            dictVectorsA.Add(rwvD, (int)(rwvD.Subtract(rwvA)).GetLength());
            int sumA = dictVectorsA.Values.Sum();

            var dictVectorsB = new Dictionary<Vector3D, int>();
            dictVectorsB.Add(rwvA, (int)(rwvA.Subtract(rwvB)).GetLength());
            dictVectorsB.Add(rwvC, (int)(rwvC.Subtract(rwvB)).GetLength());
            int sumB = dictVectorsB.Values.Sum();

            var dictVectorsC = new Dictionary<Vector3D, int>();
            dictVectorsC.Add(rwvB, (int)(rwvB.Subtract(rwvC)).GetLength());
            dictVectorsC.Add(rwvD, (int)(rwvD.Subtract(rwvC)).GetLength());
            int sumC = dictVectorsC.Values.Sum();

            var dictVectorsD = new Dictionary<Vector3D, int>();
            dictVectorsD.Add(rwvC, (int)(rwvC.Subtract(rwvD)).GetLength());
            dictVectorsD.Add(rwvA, (int)(rwvA.Subtract(rwvD)).GetLength());
            int sumD = dictVectorsD.Values.Sum();

            if (sumA < sumB && sumA < sumC && sumA < sumD)
            {
                coordinateSystemPoints[0] = rwvA;
                coordinateSystemPoints[1] = dictVectorsA.FirstOrDefault((x) => x.Value == dictVectorsA.Values.Max()).Key;
                coordinateSystemPoints[2] = dictVectorsA.FirstOrDefault((x) => x.Value == dictVectorsA.Values.Min()).Key;
            }
            else if (sumB < sumA && sumB < sumC && sumB < sumD)
            {
                coordinateSystemPoints[0] = rwvB;
                coordinateSystemPoints[1] = dictVectorsB.FirstOrDefault((x) => x.Value == dictVectorsB.Values.Max()).Key;
                coordinateSystemPoints[2] = dictVectorsB.FirstOrDefault((x) => x.Value == dictVectorsB.Values.Min()).Key;
            }
            else if (sumC < sumA && sumC < sumB && sumC < sumD)
            {
                coordinateSystemPoints[0] = rwvC;
                coordinateSystemPoints[1] = dictVectorsC.FirstOrDefault((x) => x.Value == dictVectorsC.Values.Max()).Key;
                coordinateSystemPoints[2] = dictVectorsC.FirstOrDefault((x) => x.Value == dictVectorsC.Values.Min()).Key;
            }
            else
            {
                coordinateSystemPoints[0] = rwvD;
                coordinateSystemPoints[1] = dictVectorsD.FirstOrDefault((x) => x.Value == dictVectorsD.Values.Max()).Key;
                coordinateSystemPoints[2] = dictVectorsD.FirstOrDefault((x) => x.Value == dictVectorsD.Values.Min()).Key;
            }

            var originPoint = Calibration.GetEdgePoints().Find((x) => x.RealWorldPoint.Equals(coordinateSystemPoints[0].ToRealWorldPoint()));
            var xAxisPoint = Calibration.GetEdgePoints().Find((x) => x.RealWorldPoint.Equals(coordinateSystemPoints[1].ToRealWorldPoint()));
            var yAxisPoint = Calibration.GetEdgePoints().Find((x) => x.RealWorldPoint.Equals(coordinateSystemPoints[2].ToRealWorldPoint()));

            originPoint.PointType = PointType.Origin;
            xAxisPoint.PointType = PointType.xAxis;
            yAxisPoint.PointType = PointType.yAxis;

            return coordinateSystemPoints;
        }

        public IKinectBeamerOperation CreateKinectBeamerOperation()
        {
            CalculateAllPoints();
            return new KinectBeamerOperation();
        }

        private void CalculateAllPoints()
        {
            var width = beamerWindow.GetWidth();
            var height = beamerWindow.GetHeight();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var point = new Point
                        {
                            BeamerPoint = new BeamerPoint { X = i, Y = j }
                        };
                    var kinectPoint = BeamerToKinect.CalculateKinectPoint(point.BeamerPoint);
                    if (Calibration.KinectSpace[kinectPoint.X, kinectPoint.Y] == null)
                        Calibration.KinectSpace[kinectPoint.X, kinectPoint.Y] = new List<BeamerPoint>();
                    Calibration.KinectSpace[kinectPoint.X, kinectPoint.Y].Add(new BeamerPoint { X = i, Y = j });

                    point.KinectPoint = kinectPoint;
                    point.RealWorldPoint = KinectToRealWorld.CalculateRealWorldPoint(point.KinectPoint);
                    point.AreaPoint = RealWorldToArea.CalculateAreaPoint(point.RealWorldPoint);
                    Points.Add(point);
                }
            }
        }
    }
}
