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
        1) Bestimme n --> normieren 
        2) Bestimme e1 <-- 	e1 aus der Differenz der Eckpunkte 1 und 2 berechnen
					        e1 muss senkrecht auf n stehen (also rechter Winkel)
			        e2 <-- 	(e1 x n)
			
        Methoden:
	    1. 3 Punkte entgegennehmen und n davon liefern
	    2. e1 berechnen
    	3. e2 berechnen (Input: e1 und n)
	    4. Basiswechsel --> a,b liefern		

        --> Nachtrag SW04: Normalenvektor bestimmen
            1) Differenz Eckpunkt P4 und Eckpunkt P1 --> e1
            2) Differenz Eckpunkt P2 und Eckpunkt P1
            3) Kreuzprodukt aus den Vektoren der Resultate von 1) und 2) --> n
            4) Kreuzprodukt aus n und Vektor e1 --> e2
            5) alle Vektoren normalisieren
         */

        private static Vector3D point1;
        private static Vector3D point2;
        private static Vector3D point3;
        private static Vector3D e1;
        private static Vector3D e2;
        private static Vector3D n;
        private static Vector3D e1normed;
        private static Vector3D e2normed;
        private static Vector3D nnormed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        private static Vector3D GetNormalVector()
        {
            Vector3D e1 = point1.Subtract(point2);
            Vector3D temp = point3.Subtract(point2);

            Vector3D n = e1.CrossProduct(temp);
            nnormed = n.GetNormedVector();
            return n;
        }

        private static Vector3D GetE1Vector()
        {
            Vector3D e1 = point1.Subtract(point2);
            e1normed = e1.GetNormedVector();
            return e1;
        }

        private static Vector3D GetE2Vector()
        {
            e2 = n.CrossProduct(e1);
            e2normed = e2.GetNormedVector();
            return e2;
        }

        public static Vector2D GetVectorInNewBasis(Vector3D pointInKinSpace)
        {
            return new Vector2D{X = e1normed.ScalarProduct(pointInKinSpace.Subtract(point1)), 
                                Y = e2normed.ScalarProduct(pointInKinSpace.Subtract(point1))};
        }

        public static void InitializeChangeOfBasis(Vector3D point1, Vector3D point2, Vector3D point3)
        {
            ChangeOfBasis.point1 = point1;
            ChangeOfBasis.point2 = point2;
            ChangeOfBasis.point3 = point3;

            n = GetNormalVector(); //evtl umschreiben und anstatt point1 gerade e1 übergeben!
            e1 = GetE1Vector();
            e2 = GetE2Vector();

        }


    }
}
