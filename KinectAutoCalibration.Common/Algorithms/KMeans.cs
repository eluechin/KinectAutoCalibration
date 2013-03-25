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

        public static List<IVector> DoKMeans(List<IVector> vectorPoints, List<IVector> vectorCentroids)
        {
            List<IVector> newVectorCentroids = null;
            var oldVectorCentroids = new List<IVector>();
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

        private static List<IVector> CalculateNewCentroids(Dictionary<IVector, IVector> pointsToCentroids)
        {
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
                    pointList = new List<IVector> { e.Value };
                    centroidsToPoint.Add(e.Value, pointList);
                }
            }
            var newVectorCentroids = new List<IVector>();

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
            foreach (var vectorPoint in vectorPoints)
            {
                IVector nearestCentroid = null;
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
