using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KinectAutoCalibration.Calibration;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Common.Algorithms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //int tolerance = 0x64;
            //Color color1 = Color.Black;
            //Color color2 = Color.White;

            /*
            MessageBox.Show(
                (
                (green.R + tolerance <= red.R || green.R - tolerance >= red.R) &&
                         
                (green.G + tolerance <= red.G || green.G - tolerance >= red.G) &&
                
                (green.B + tolerance <= red.B || green.B - tolerance >= red.B)
                ).ToString());
             */

            /*
            Vector3D vector1 = new Vector3D(color1.R, color1.G, color1.B);
            Vector3D vector2 = new Vector3D(color2.R, color2.G, color2.B);
            Vector3D diffVector = (Vector3D)vector1.Subtract(vector2);
            double length = diffVector.GetLength();

            MessageBox.Show((length > 350).ToString());
            */

            /*
            Vector3D vector1 = new Vector3D(13, -100, 17);
            Vector3D vector2 = new Vector3D(-21, 42, -119);
            Vector3D vector3 = vector1.CrossProduct(vector2);
            MessageBox.Show("CrossProduct: X:" + vector3.X + " /Y:" + vector3.Y + " /Z:" + vector3.Z);

            Vector3D vector4 = vector2.GetNormedVector();
            MessageBox.Show("NormedVector: X:" + vector4.X + " /Y:" + vector4.Y + " /Z:" + vector4.Z);
            */
            
            /*
            Vector3D vector1 = new Vector3D(13, 100, 17);
            Vector3D vector2 = new Vector3D(21, 42, 119);
            Vector3D vector3 = new Vector3D(17, 22, 30);

            Vector3D kinVector = new Vector3D(15, 30, 24);
            
            ChangeOfBasis.InitializeChangeOfBasis(vector1, vector2, vector3);
            Vector2D newBaseVector = ChangeOfBasis.GetVectorInNewBasis(vector2);

            MessageBox.Show("newBaseVector: X:" + newBaseVector.X + " /Y:" + newBaseVector.Y);
            */
            KinectAutoCalibration.Calibration.KinectCalibration test = new KinectCalibration();
            test.StartCalibration();


        }
    }
}
