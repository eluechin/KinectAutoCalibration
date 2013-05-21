using System.Collections.Generic;
using System.Linq;

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
            var movedCentroids = new List<Vector2D>();
            var oldCentroids = new List<Vector2D>();
            vectorCentroids.ForEach((e)=>oldCentroids.Add(e.Copy()));
            var finish = true;

            var loopCount = 0;
            
            //TODO Abbruchkriterium....
            while (finish)
            {
                var pointsToCentroids = MapPointsToNearestCentroids(vectorPoints, oldCentroids);
                movedCentroids = MoveCentroids(pointsToCentroids);

                finish = !movedCentroids.All(oldCentroids.Contains);

                oldCentroids.Clear();
                movedCentroids.ForEach((e)=>oldCentroids.Add(e.Copy()));
                loopCount++;
                finish = false;
            }

            return movedCentroids;
        }

        private static List<Vector2D> MoveCentroids(Dictionary<Vector2D, List<Vector2D>> centroidToPoint)
        {
            var movedCentroids = new List<Vector2D>();
            foreach (var pair in centroidToPoint)
            {
                var centroid = pair.Key;
                var points = pair.Value;

                var sumVector = new Vector2D();
                foreach (var vectorPoint in points)
                {
                    sumVector = sumVector.Add(vectorPoint);
                }
                sumVector=sumVector.Divide(points.Count);

                centroid.X = sumVector.X;
                centroid.Y = sumVector.Y;
                movedCentroids.Add(centroid);
            }
            return movedCentroids;
        }

        private static Dictionary<Vector2D, List<Vector2D>> MapPointsToNearestCentroids(IEnumerable<Vector2D> vectorPoints,
                                                                        IReadOnlyList<Vector2D> vectorCentroids)
        {
            var centroidToPoint = new Dictionary<Vector2D, List<Vector2D>>();
            foreach (var vectorPoint in vectorPoints)
            {
                var nearestCentroid = vectorCentroids[0];
                var nearestDistance = double.PositiveInfinity;

                foreach (var vectorCentroid in vectorCentroids)
                {
                    var distance = vectorPoint.Subtract(vectorCentroid).GetLength();
                    if (distance > nearestDistance) { continue; }
                    nearestCentroid = vectorCentroid;
                    nearestDistance = distance;
                }

                if (centroidToPoint.ContainsKey(nearestCentroid))
                {
                    centroidToPoint[nearestCentroid].Add(vectorPoint);
                }
                else
                {
                    centroidToPoint.Add(nearestCentroid, new List<Vector2D> {vectorPoint});    
                }
            }
            return centroidToPoint;
        }
    }
}
