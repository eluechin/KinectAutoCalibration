using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common.Algorithms
{
    public static class KMeans
    {

        public static List<IPoint2D> DoKMeans(List<IPoint2D> points, int centroidCount, int width, int height)
        {
            var vectorCentroids = CreateCentroids(centroidCount, width, height);
            var vectorPoints = ConvertPointToVector(points);

            var pointsToCentroids = MapPointsToCentroids(vectorPoints, vectorCentroids);
            AdjustCentroids(pointsToCentroids);

            return null;
        }

        private static void AdjustCentroids(Dictionary<IVector, IVector> pointsToCentroids)
        {
            var centroidsPoints = new Dictionary<IVector, List<IVector>>();
            foreach (var e in pointsToCentroids)
            {
                List<IVector> pointList;

                if (centroidsPoints.TryGetValue(e.Value, out pointList))
                {
                    pointList.Add(e.Key);
                    centroidsPoints[e.Value] = pointList;
                }
            }


            var newCentroidsPoints = new Dictionary<IVector, List<IVector>>();
            foreach (var centroid in centroidsPoints)
            {
                var pointList = centroid.Value;
                var sum = new Vector2D();
                foreach (var p in pointList)
                {
                    sum = (Vector2D) sum.Add(p);
                }
                sum.Divide(pointList.Count);

            }

        }

        private static Dictionary<IVector, IVector> MapPointsToCentroids(List<IVector> vectorPoints, List<IVector> vectorCentroids)
        {
            var centroidToPoint = new Dictionary<IVector, IVector>();
            foreach (var point in vectorPoints)
            {
                IVector nearestCentroid = new Vector2D();
                var nearestDistance = double.PositiveInfinity;

                foreach (var centroid in vectorCentroids)
                {
                    var distance = point.Subtract(centroid).GetLength();
                    if (distance <= nearestDistance)
                    {
                        nearestCentroid = centroid;
                        nearestDistance = distance;
                    }
                }
                centroidToPoint.Add(point, nearestCentroid);

            }
            return centroidToPoint;
        }

        private static List<IVector> CreateCentroids(int centroidCount, int width, int height)
        {
            var vectorCentroids = new List<IVector>();
            for (var i = 0; i < centroidCount; i++)
            {
                var rnd = new Random();
                var x = rnd.Next(0, width - 1);
                var y = rnd.Next(0, height - 1);

                vectorCentroids.Add(new Vector2D { X = x, Y = y });
            }
            return vectorCentroids;
        }

        private static List<IVector> ConvertPointToVector(List<IPoint2D> points)
        {
            var vectorPoints = new List<IVector>();
            foreach (var p in points)
            {
                vectorPoints.Add(new Vector2D { X = p.X, Y = p.Y });
            }

            return vectorPoints;
        }
    }
}
