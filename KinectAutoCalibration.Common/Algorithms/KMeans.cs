using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Common.Interfaces;

namespace KinectAutoCalibration.Common.Algorithms
{
    /// <summary>
    /// Holds the KMeans cluster algorithm.
    /// </summary>
    public static class KMeans
    {
        /// <summary>
        /// Execute KMeans algorithm with given points and centroids.
        /// </summary>
        /// <param name="vectorPoints">List of all points (as Vectors) available</param>
        /// <param name="vectorCentroids">List of centroids</param>
        /// <returns>List of the calculated centroids</returns>
        public static List<Vector2D> DoKMeans(List<Vector2D> vectorPoints, List<Vector2D> vectorCentroids)
        {
            List<Vector2D> newVectorCentroids = null;
            var oldVectorCentroids = new List<Vector2D>();
            var finish = true;

            var loopCount = 0;
            
            while (finish)
            {
                var pointsToCentroids = MapPointsToNearestCentroids(vectorPoints, vectorCentroids);
                newVectorCentroids = CalculateNewCentroids(pointsToCentroids);

                foreach (var newVectorCentroid in newVectorCentroids)
                {
                    if (!oldVectorCentroids.Contains(newVectorCentroid))
                    {
                        break;
                    }
                    finish = false;
                }

                oldVectorCentroids = newVectorCentroids;
                loopCount++;
            }

            return newVectorCentroids.ToList();
        }

        private static List<Vector2D> CalculateNewCentroids(Dictionary<Vector2D, Vector2D> pointsToCentroids)
        {
            var centroidsToPoint = new Dictionary<Vector2D, List<Vector2D>>();
            foreach (var e in pointsToCentroids)
            {
                List<Vector2D> pointList;

                if (centroidsToPoint.TryGetValue(e.Value, out pointList))
                {
                    pointList.Add(e.Key);
                    centroidsToPoint[e.Value] = pointList;
                }
                else
                {
                    pointList = new List<Vector2D> { e.Value };
                    centroidsToPoint.Add(e.Value, pointList);
                }
            }
            var newVectorCentroids = new List<Vector2D>();

            foreach (var centroid in centroidsToPoint)
            {
                var pointList = centroid.Value;
                Vector2D sum = null;
                foreach (var p in pointList)
                {
                    sum = p.Add(sum);
                }
                sum = sum.Divide(pointList.Count);

                newVectorCentroids.Add(sum);
            }
            return newVectorCentroids;
        }

        private static Dictionary<Vector2D, Vector2D> MapPointsToNearestCentroids(List<Vector2D> vectorPoints, List<Vector2D> vectorCentroids)
        {
            var centroidToPoint = new Dictionary<Vector2D, Vector2D>();
            foreach (var vectorPoint in vectorPoints)
            {
                Vector2D nearestCentroid = new Vector2D{X=0, Y=0};

                var nearestDistance = double.PositiveInfinity;

                foreach (var vectorCentroid in vectorCentroids)
                {
                    var distance = vectorPoint.Subtract(vectorCentroid).GetLength();
                    if (distance > nearestDistance) { continue; }
                    nearestCentroid = vectorCentroid;
                    nearestDistance = distance;
                }
                centroidToPoint.Add(vectorPoint, nearestCentroid);
            }
            return centroidToPoint;
        }


    }
}
