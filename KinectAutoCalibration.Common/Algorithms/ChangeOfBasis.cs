using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common.Algorithms
{
    public static class ChangeOfBasis
    {
        /*
        1) Bestimme _n --> normieren 
        2) Bestimme _e1 <-- 	_e1 aus der Differenz der Eckpunkte 1 und 2 berechnen
					        _e1 muss senkrecht auf _n stehen (also rechter Winkel)
			        _e2 <-- 	(_e1 x _n)
			
        Methoden:
	    1. 3 Punkte entgegennehmen und _n davon liefern
	    2. _e1 berechnen
    	3. _e2 berechnen (Input: _e1 und _n)
	    4. Basiswechsel --> a,b liefern		

        --> Nachtrag SW04: Normalenvektor bestimmen
            1) Differenz Eckpunkt P4 und Eckpunkt P1 --> _e1
            2) Differenz Eckpunkt P2 und Eckpunkt P1
            3) Kreuzprodukt aus den Vektoren der Resultate von 1) und 2) --> _n
            4) Kreuzprodukt aus _n und Vektor _e1 --> _e2
            5) alle Vektoren normalisieren
         */

        private static Vector3D _point1;
        private static Vector3D _point2;
        private static Vector3D _point3;
        private static Vector3D _e1;
        private static Vector3D _e2;
        private static Vector3D _n;
        private static Vector3D _e1Normed;
        private static Vector3D _e2Normed;
        private static Vector3D _nNormed;

        /// <summary>
        /// This method calculates the normal vector of 2 vectors with the same origin.</summary>
        /// <returns>Returns the normed normal vector</returns>
        private static Vector3D GetNormalVector()
        {
            _e1 = _point1.Subtract(_point2);
            Vector3D temp = _point3.Subtract(_point2);

            _n = _e1.CrossProduct(temp);
            _nNormed = _n.GetNormedVector();
            return _nNormed;
        }

        /// <summary>
        /// This method calculates the vector between two 3D points.</summary>
        /// <returns>Returns a normed vector</returns>
        private static Vector3D GetE1Vector()
        {
            _e1 = _point1.Subtract(_point2);
            _e1Normed = _e1.GetNormedVector();
            return _e1Normed;
        }

        /// <summary>
        /// This method calculates an orthogonal vector to e1 and n.</summary>
        /// <returns>Returns a normed vector</returns>
        private static Vector3D GetE2Vector()
        {
            _e2 = _n.CrossProduct(_e1);
            _e2Normed = _e2.GetNormedVector();
            return _e2Normed;
        }


        /// <summary>
        /// This method calculates the 2D coordinates in the new basis for a 3D point in the "old" kinect coordinate system</summary>
        /// This method must be used after calling the method "InitializeChangeOfBasis". Otherwise no new coordinate system is existing and this method will fail.
        /// <param name="pointInKinSpace">any 3D point which is located in the kinect space</param>
        /// <returns>Returns the calculated 2D point in the new coordinate system</returns>
        public static Vector2D GetVectorInNewBasis(Vector3D pointInKinSpace)
        {
            return new Vector2D{X = _e1Normed.ScalarProduct(pointInKinSpace.Subtract(_point1)), 
                                Y = _e2Normed.ScalarProduct(pointInKinSpace.Subtract(_point1))};
        }


        /// <summary>
        /// This method initializes a new coordinate system on the basis of the passed 3 points.
        /// These points are the corners of the coordinate system</summary>
        /// <param name="point1">first point.</param>
        /// <param name="point2">second point. It is important that this point "connects" to both other points, because this will be the origin of the new coordinate system</param>
        /// <param name="point3">third point.</param>
        /// <returns></returns>
        public static void InitializeChangeOfBasis(Vector3D point1, Vector3D point2, Vector3D point3)
        {
            ChangeOfBasis._point1 = point1;
            ChangeOfBasis._point2 = point2;
            ChangeOfBasis._point3 = point3;

            _n = GetNormalVector(); //evtl umschreiben und anstatt _point1 gerade _e1 übergeben!
            _e1 = GetE1Vector();
            _e2 = GetE2Vector();

        }


    }
}
