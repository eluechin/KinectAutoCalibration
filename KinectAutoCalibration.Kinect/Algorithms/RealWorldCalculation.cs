﻿using System.Collections.Generic;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Kinect.Algorithms
{
    public class RealWorldCalculation
    {
        public RealWorldCalculation()
        {
        }

        /// <summary>
        /// This method is used to convert a list of KinectPoints to a mapping-dictionary.
        /// When to use: Use this method only to calculate the RealWorld-coordinates of the corner points. 
        /// </summary>
        /// <param name="kinectPoints">a list of KinectPoints which should be converted</param>
        /// <returns>Returns a dictionary which contains KinectPoints and their mapped RealWorldPoints</returns>
        public Dictionary<KinectPoint, RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints)
        {
            var dict_Kin2RW = new Dictionary<KinectPoint, RealWorldPoint>();

            foreach (var kinectPoint in kinectPoints)
            {
                dict_Kin2RW.Add(kinectPoint, CalculateRealWorldPoint(kinectPoint));
            }
                    
           
            return dict_Kin2RW;
        }

        /// <summary>
        /// This method is used to convert a list of KinectPoints to a mapping-dictionary.
        /// The difference to the other CreateRealWorldCoordinates-method is, that this method uses 3 known RealWorldPoints to calculate the 
        /// millimeter-coordinates of the new RealWorldPoints.
        /// When to use: As soon as the corner points are given in RealWorldPoints use this method to calculate the RealWorld-coordinates of 
        /// additional points needed.
        /// How to use: The parameters rwA, rwB and rwC represent the corner points of the detected field. rwB is the RealWorld-point which is a direct
        /// of the other RealWorld-points rwA and rwC. Direct means, that those point can be reached from rwB by following the adjacent outlines of the field.
        /// Important: The list must not contain only NULL-Points!
        /// </summary>
        /// <param name="kinectPoints">a list of KinectPoints which should be converted</param>
        /// <param name="rwA">RealWorldPoint A which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <param name="rwB">RealWorldPoint B which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <param name="rwC">RealWorldPoint C which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints</param>
        /// <returns>>Returns a dictionary which contains KinectPoints and their mapped RealWorldPoints</returns>
        public Dictionary<KinectPoint, RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints , RealWorldPoint rwA, RealWorldPoint rwB, RealWorldPoint rwC)
        {
            var dict_Kin2RW = new Dictionary<KinectPoint, RealWorldPoint>();

            foreach (var kinectPoint in kinectPoints)
            {
                dict_Kin2RW.Add(kinectPoint, CalculateRealWorldPoint(kinectPoint, rwA, rwB, rwC));
            }
       
            return dict_Kin2RW;
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
        /// Important: The passed KinectPoint must not be NULL!
        /// </summary>
        /// <param name="p">the KinectPoint which should be transformed to a RealWorldPoint </param>
        /// <returns>Returns the RealWorldPoint corresponding to the passed KinectPoint</returns>
        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p)
        {
            var rwPoint = new RealWorldPoint();

            rwPoint.Z = p.Z;
            rwPoint.X = (int) ((p.X - (Kinect.KINECT_IMAGE_WIDTH / 2))* rwPoint.Z/Kinect.WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z /Kinect.HEIGHT_CONST);

            return rwPoint;
        }

        /// <summary>
        /// This method is used to calculate the RealWorldCalculation-coordinates for a given KinectPoint
        /// When to use: use only to calculate the corner points' RealWorldCalculation-coordinates. For other KinectPoints use the other "CalculateRealWorldVector" 
        /// method.
        /// How to use: The parameters rwA, rwB and rwC represent the corner points of the detected field. rwB is the RealWorld-point which is a direct
        /// of the other RealWorld-points rwA and rwC. Direct means, that those point can be reached from rwB by following the adjacent outlines of the field.
        /// Important: The passed KinectPoint must not be NULL!
        /// </summary>
        /// <param name="p">the KinectPoint which should be transformed to a RealWorldPoint </param>
        /// <param name="rA">the RealWorldPoint A which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <param name="rB">the RealWorldPoint B which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <param name="rC">the RealWorldPoint C which represents a corner point and 
        ///     is used to calculate the coordinates for the new RealWorldPoints </param>
        /// <returns>Returns the RealWorldPoint corresponding to the passed KinectPoint</returns>
        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p, RealWorldPoint rA, RealWorldPoint rB, RealWorldPoint rC)
        {
            var rwPoint = new RealWorldPoint();
            var vec_rA = CreateRealWorldVector(rA);
            var vec_rB = CreateRealWorldVector(rB);
            var vec_rC = CreateRealWorldVector(rC);

            var vec_N = (vec_rA.Subtract(vec_rB)).CrossProduct(vec_rC.Subtract(vec_rB));
            var q = vec_N.ScalarProduct(vec_rB);

            var x = (int)((p.X - (Kinect.KINECT_IMAGE_WIDTH / 2)) /Kinect.WIDTH_CONST);
            var y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) /Kinect.HEIGHT_CONST);

            var vec_rP = new Vector3D(x,y,1);

            rwPoint.Z = (int)(q / (vec_N.ScalarProduct(vec_rP)));
            
            rwPoint.X = (int) ((p.X - (Kinect.KINECT_IMAGE_WIDTH / 2))*rwPoint.Z/Kinect.WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (Kinect.KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z /Kinect.HEIGHT_CONST);

            return rwPoint;
        }
    }
}