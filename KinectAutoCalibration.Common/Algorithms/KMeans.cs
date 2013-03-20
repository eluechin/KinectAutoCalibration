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

        public static List<IPoint2D> DoKMeans(List<IVector> vectorPoints, List<IVector> vectorCentroids)
        {
            HashSet<IVector> newVectorCentroids = null;
            var finish = true;
            var count = 1;

            while (finish)
            {
                var pointsToCentroids = MapPointsToNearestCentroids(vectorPoints, vectorCentroids);
                newVectorCentroids = CalculateNewCentroids(pointsToCentroids);

                for (var i = 0; i < newVectorCentroids.Count; i++)
                {
                    var v1 = vectorCentroids.ElementAt(i);
                    var v2 = newVectorCentroids.ElementAt(i);
                    if (v1 != v2)
                    {
                        finish = false;
                    }
                }
                //if (count == 50)
                //{
                //    finish = false;
                //}
                //count++;
            }

            return VectorToPoint(newVectorCentroids.ToList());
        }

        private static List<IPoint2D> VectorToPoint(List<IVector> toList)
        {
            var newList = new List<IPoint2D>();
            foreach (var vector in toList)
            {
                newList.Add(new Point2D { X = (int) vector.X, Y = (int) vector.Y });
            }
            return newList;
        }

        private static HashSet<IVector> CalculateNewCentroids(Dictionary<IVector, IVector> pointsToCentroids)
        {
            // Centroid to point
            var centroidsToPoint = new Dictionary<IVector, List<IVector>>();
            foreach (var e in pointsToCentroids)
            {
                List<IVector> pointList;

                if (centroidsToPoint.TryGetValue(e.Value, out pointList))
                {
                    pointList.Add(e.Key);
                    centroidsToPoint[e.Value] = pointList;
                }
                else
                {
                    pointList = new List<IVector> {e.Value};
                    centroidsToPoint.Add(e.Value, pointList);
                }
            }

            //var newCentroidsPoints = new Dictionary<IVector, List<IVector>>();
            var newVectorCentroids = new HashSet<IVector>();

            foreach (var centroid in centroidsToPoint)
            {
                var pointList = centroid.Value;
                IVector sum = null;
                foreach (var p in pointList)
                {
                    sum = p.Add(sum);
                }
                sum = sum.Divide(pointList.Count);

                newVectorCentroids.Add(sum);
            }

            return newVectorCentroids;

        }

        private static Dictionary<IVector, IVector> MapPointsToNearestCentroids(List<IVector> vectorPoints, List<IVector> vectorCentroids)
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

        
    }
}
