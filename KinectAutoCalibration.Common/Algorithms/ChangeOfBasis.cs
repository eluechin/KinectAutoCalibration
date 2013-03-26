using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectAutoCalibration.Common.Algorithms
{
    class ChangeOfBasis
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        public Vector3D GetNormalVector(Vector3D point1, Vector3D point2, Vector3D point3)
        {
            Vector3D e1 = point1.Subtract(point2);
            Vector3D temp = point3.Subtract(point2);
            Vector3D n = e1.CrossProduct(temp);
            Vector3D e2 = n.CrossProduct(e1);

            e1 = e1.GetNormedVector();
            e2 = e2.GetNormedVector();
            n = n.GetNormedVector();
           




            return new Vector3D(1,1,1);
        }

    }
}
