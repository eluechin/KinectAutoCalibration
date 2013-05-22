using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common
{
    public static class KinectPointArrayHelper
    {
        public const int BLACK_TRESHOLD = 10;

        public static List<Vector2D> ExtractBlackPointsAs2dVector(KinectPoint[,] kinectPoints)
        {
            var blackPixel = new List<Vector2D>();
            
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

        public static List<KinectPoint> ExtractBlackPoints(KinectPoint[,] kinectPoints)
        {
            var blackPixel = new List<KinectPoint>();
            for (var i = 0; i < 480; i++)
            {
                for (var j = 0; j < 640; j++)
                {
                    var p = kinectPoints[j, i];
                    if (p.R - BLACK_TRESHOLD < 0 && p.G - BLACK_TRESHOLD < 0 && p.B - BLACK_TRESHOLD < 0)
                    {
                        blackPixel.Add(new KinectPoint{X = j, Y = i});
                    }
                }
            }
            return blackPixel;
        }

        private static Vector2D Get2dVectorFromKinectPoint(KinectPoint p)
        {
            return new Vector2D { X = p.X, Y = p.Y };
        }
    }
}
