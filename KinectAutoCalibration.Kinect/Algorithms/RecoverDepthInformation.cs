using System.Collections.Generic;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Kinect
{
    public class RecoverDepthInformation
    {
        public RecoverDepthInformation()
        {
        }

        /// <summary>
        /// This method is used to recover missing depth information of the KinectPoints delivered by the Kinect. 
        /// This solves the problem of having incorrect depth values or NULL-values in the KinectPoint-array.  
        /// </summary>
        /// <param name="kinArray">the array whose KinectPoint-values have to be corrected.</param>
        /// <returns>Returns a new KinectPoint-array containing valid depth information for every KinectPoint</returns>
        public KinectPoint[,] RecoverDepthInformationOfKinectPointArray(KinectPoint[,] kinArray)
        {
            var recoveredKinArray = new KinectPoint[Kinect.KINECT_IMAGE_WIDTH,Kinect.KINECT_IMAGE_HEIGHT];

            for (int y = 0; y < Kinect.KINECT_IMAGE_HEIGHT; ++y)
            {
                for (int x = 0; x < Kinect.KINECT_IMAGE_WIDTH; ++x)
                {
                    if (kinArray[x,y].Z == -1)
                    {
                        int correctionRadius = 1;
                        int z = -1;
                        int depthMeanValue = 0;

                        do
                        {
                            var neighbors = GetNeighborsOfKinectPoint(kinArray, kinArray[x,y], correctionRadius);
                            depthMeanValue = CalculateDepthMeanValue(neighbors);

                            ++correctionRadius;

                        } while (depthMeanValue == 0);

                        kinArray[x,y].Z = depthMeanValue;
                        recoveredKinArray[x,y] = kinArray[x,y];

                    }
                    else
                    {
                        recoveredKinArray[x, y] = kinArray[x, y];
                    }
                }
            }
   
            return recoveredKinArray;
        }

        /// <summary>
        /// This method searches all neighbors of a KinectPoint within a KinectPoint-array   
        /// </summary>
        /// <param name="kinArray">the array which contains the needed KinectPoints </param>
        /// /// <param name="kinPoint">the KinectPoint whose neighbors should be found </param>
        /// /// <param name="correctionRadius">the radius specifies how far away KinectPoints can be from the kinPoint to be considered as neighbor, 
        /// e.g. correctionRadius = 1 means that only direct neighbors of the kinPoint are found.</param>
        /// <returns>Returns a list containing all neighbors of the specified kinPoint within the correctionRadius</returns>
        private List<KinectPoint> GetNeighborsOfKinectPoint(KinectPoint[,] kinArray, KinectPoint kinPoint, int correctionRadius)
        {
            var neighbors = new List<KinectPoint>();

            for (int i = correctionRadius; i >= -correctionRadius; --i)
            {
                for (int j = correctionRadius; j >= -correctionRadius; --j)
                {
                    int x = kinPoint.X + i;
                    int y = kinPoint.Y + j;

                    if ((x >= 0 && x < Kinect.KINECT_IMAGE_WIDTH) &&
                        (y >= 0 && y < Kinect.KINECT_IMAGE_HEIGHT))
                    {
                        neighbors.Add(kinArray[x, y]);
                    }
                }
            }
            return neighbors;
        }

        /// <summary>
        /// This method calculates the mean value of the depth information of KinectPoints.  
        /// </summary>
        /// <param name="neighbors">a list of KinectPoints whose depth information should be processed</param>
        /// <returns>Returns the mean value of the depth information of all KinectPoints included.
        /// Returns 0 if every KinectPoint in the neigbors-list has an invalid depth value</returns>
        private int CalculateDepthMeanValue(List<KinectPoint> neighbors)
        {
            int sum = 0;
            int validNeighbors = 0;

            foreach (var kinPoint in neighbors)
            {
                if (kinPoint.Z != -1)
                {
                    sum += kinPoint.Z;
                    ++validNeighbors;
                }

            }
            if (validNeighbors != 0)
            {
                return (int) (sum/validNeighbors);
            }
            else
            {
                return 0;
            }
            
        }
    }
}