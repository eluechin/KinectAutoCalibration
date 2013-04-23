using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common
{
    public static class KMeansHelper
    {

        private static List<Vector2D> Get2dVectorFromKinectPointArray(KinectPoint[,] kinectPoints)
        {
            var vector2dList = new List<Vector2D>();
            foreach (var kinectPoint in kinectPoints)
            {
                vector2dList.Add(Get2dVectorFromKinectPoint(kinectPoint));
            }
            return vector2dList;
        }

        private static Vector2D Get2dVectorFromKinectPoint(KinectPoint p)
        {
            return new Vector2D { X = p.X, Y = p.Y };
        }

        public static List<Vector2D> ExtractBlackPointsAs2dVector(KinectPoint[,] kinectPoints)
        {
            var blackPixel = new List<Vector2D>();
            const int BLACK_TRESHOLD = 10;
            for (var i = 0; i < 480; i++)
            {
                for (var j = 0; j < 640; j++)
                {
                    KinectPoint p = kinectPoints[j, i];
                    if (p.R - BLACK_TRESHOLD < 0 && p.G - BLACK_TRESHOLD < 0 && p.B - BLACK_TRESHOLD < 0)
                    {
                        blackPixel.Add(Get2dVectorFromKinectPoint(p));
                    }
                }
            }
            return blackPixel;
        }

        public static List<Vector2D> CreateInitialCentroids(int width, int height)
        {
            var centroidsInit = new List<Vector2D>
                {
                    new Vector2D {X = 0, Y = 0},
                    new Vector2D {X = width - 1, Y = 0},
                    new Vector2D {X = 0, Y = height - 1},
                    new Vector2D {X = width - 1, Y = height - 1},
                    new Vector2D {X = width/2 - 1, Y = height/2 - 1} 
                };
            return centroidsInit;
        }

        public static List<Vector3D> ExtractBlackPointsAs3dVector(KinectPoint[,] kinectPoints)
        {
            var blackPixel = new List<Vector3D>();
            const int BLACK_TRESHOLD = 10;
            for (var i = 0; i < 480; i++)
            {
                for (var j = 0; j < 640; j++)
                {
                    KinectPoint p = kinectPoints[j, i];
                    if (p.R - BLACK_TRESHOLD < 0 && p.G - BLACK_TRESHOLD < 0 && p.B - BLACK_TRESHOLD < 0)
                    {
                        blackPixel.Add(Get3dVectorFromKinectPoint(p));
                    }
                }
            }
            return blackPixel;
        }

        private static Vector3D Get3dVectorFromKinectPoint(KinectPoint p)
        {
             return new Vector3D { X = p.X, Y = p.Y, Z = p.Z};
        }
    }
}
