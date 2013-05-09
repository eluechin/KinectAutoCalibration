using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Kinect
{
    public class RealWorldCalculation
    {
        public RealWorldCalculation()
        {
        }

        /// <summary>
        /// This method is used to convert a KinectPoint-array to a RealWorldCalculation-Array whose coordinates are all in millimeters.
        /// When to use: Use this method only to calculate the RealWorldCalculation-coordinates of the corner points. 
        /// </summary>
        /// <param name="kinArray">the KinectPoint-array which should be converted</param>
        /// <param name="width">the actual width of the two-dimensional KinectPoint-array</param>
        /// <param name="height">the actual height of the two-dimensional KinectPoint-array</param>
        /// <returns>Returns an array which contains RealWorldPoints</returns>
        public RealWorldPoint[,] CreateRealWorldArray(KinectPoint[,] kinArray, int width, int height)
        {
            RealWorldPoint[,] rwArray = new RealWorldPoint[640, 480];

            //var a = new KinectPoint(0, 0, 0, 0, 0);
            //var b = new KinectPoint(1, 1, 0, 0, 0);
            //var c = new KinectPoint(2, 2, 0, 0, 0);
            //var rwA = CalculateRealWorldPoint(a);
            //var rwB = CalculateRealWorldPoint(b);
            //var rwC = CalculateRealWorldPoint(c);
                
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var rwPoint = CalculateRealWorldPoint(kinArray[x, y]);
                    rwArray[x, y] = rwPoint;
                }
            }
            return rwArray;
        }

        /// <summary>
        /// This method is used to convert a KinectPoint-array to a RealWorldCalculation-Array whose coordinates are all in millimeters.
        /// The difference to the other CreateRealWorld-method is, that this method uses 3 known RealWorldPoints to calculate the millimeter-coordinates
        /// of the new RealWorldPoints.
        /// When to use: As soon as the corner points are given in RealWorldPoints use this method to calculate the RealWorldCalculation-coordinates of 
        /// additional points needed.
        /// </summary>
        /// <param name="kinArray">the KinectPoint-array which should be converted</param>
        /// <param name="width">the actual width of the two-dimensional KinectPoint-array</param>
        /// <param name="height">the actual height of the two-dimensional KinectPoint-array</param>
        /// <param name="rwA">RealWorldPoint A which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <param name="rwB">RealWorldPoint B which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <param name="rwC">RealWorldPoint C which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <returns>Returns an array which contains RealWorldPoints</returns>
        public RealWorldPoint[,] CreateRealWorldArray(KinectPoint[,] kinArray, int width, int height, RealWorldPoint rwA, RealWorldPoint rwB, RealWorldPoint rwC)
        {
            RealWorldPoint[,] rwArray = new RealWorldPoint[640, 480];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var rwPoint = CalculateRealWorldPoint(kinArray[x, y], rwA, rwB, rwC);
                    rwArray[x, y] = rwPoint;
                }
            }
            return rwArray;
        }

        /// <summary>
        /// This method is used to create a Vector3D-object for a specified RealWorldPoint
        /// </summary>
        /// <param name="p">the RealWorldPoint for which a Vector should be created</param>
        /// <returns>Returns the Vector3D corresponding to the passed RealWorldPoint </returns>
        public Vector3D CreateRealWorldVector(RealWorldPoint p)
        {
            return new Vector3D { X = p.X, Y = p.Y, Z = p.Z };
        }

        /// <summary>
        /// This method is used to calculate the RealWorldCalculation-coordinates for a given KinectPoint
        /// When to use: use only to calculate the corner points' RealWorldCalculation-coordinates. For other KinectPoints use the other "CalculateRealWorldVector" 
        /// method.
        /// </summary>
        /// <param name="p">the KinectPoint which should be transformed to a RealWorldPoint </param>
        /// <returns>Returns the RealWorldPoint corresponding to the passed KinectPoint</returns>
        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p)
        {
            var rwPoint = new RealWorldPoint();

            rwPoint.Z = p.Z;
            rwPoint.X = (int) ((p.X - (Kinect.KINECT_IMAGE_WIDTH/2))* rwPoint.Z/Kinect.WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z /Kinect.HEIGHT_CONST);

            return rwPoint;
        }

        /// <summary>
        /// This method is used to calculate the RealWorldCalculation-coordinates for a given KinectPoint
        /// When to use: use only to calculate the corner points' RealWorldCalculation-coordinates. For other KinectPoints use the other "CalculateRealWorldVector" 
        /// method.
        /// </summary>
        /// <param name="p">the KinectPoint which should be transformed to a RealWorldPoint </param>
        /// <param name="rA">the RealWorldPoint A which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <param name="rA">the RealWorldPoint B which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <param name="rA">the RealWorldPoint C which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <returns>Returns the RealWorldPoint corresponding to the passed KinectPoint</returns>
        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p, RealWorldPoint rA, RealWorldPoint rB, RealWorldPoint rC)
        {
            var rwPoint = new RealWorldPoint();
            var vec_rA = CreateRealWorldVector(rA);
            var vec_rB = CreateRealWorldVector(rB);
            var vec_rC = CreateRealWorldVector(rC);

            var vec_N = (vec_rB.Subtract(vec_rA)).CrossProduct(vec_rC.Subtract(vec_rA));
            var q = vec_N.ScalarProduct(vec_rA);

            var x = (int)((p.X - (Kinect.KINECT_IMAGE_WIDTH / 2)) /Kinect.WIDTH_CONST);
            var y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) /Kinect.HEIGHT_CONST);

            var vec_rP = new Vector3D(x,y,1);

            rwPoint.Z = (int)(q / (vec_N.ScalarProduct(vec_rP)));

            //RWPoint.Z = kinectPoint.Z;

            //var numerator = 2*(a.Z * b.Y * c.X - a.Y * b.Z * c.X - a.Z * b.X * c.Y + a.X * b.Z * c.Y + a.Y * b.X * c.Z - a.X * b.Y * c.Z) * HEIGHT_CONST * WIDTH_CONST;
            //var denominator = ((a.Z*(b.X - c.X) + b.Z*c.X - b.X*c.Z + a.X*(-b.Z + c.Z))*(KINECT_IMAGE_HEIGHT - 2*p.Y)*WIDTH_CONST + HEIGHT_CONST*((-a.Y*b.Z + a.Z*(b.Y-c.Y) + b.Z*c.Y + a.Y*c.Z*-b.Y*c.Z) * (2*p.X - KINECT_IMAGE_WIDTH) + 2*(-a.X*b.Y + a.Y*(b.X-c.X) + b.Y*c.X + a.X*c.Y - b.X*c.Y)*WIDTH_CONST));
            //var numerator = 1;
            //var denominator = 1;
            //rwPoint.Z = (int) (numerator/denominator);
            
            rwPoint.X = (int) ((p.X - (Kinect.KINECT_IMAGE_WIDTH/2))*rwPoint.Z/Kinect.WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z /Kinect.HEIGHT_CONST);

            return rwPoint;
        }
    }
}