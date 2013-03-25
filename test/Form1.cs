using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KinectAutoCalibration.Common; 

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            int tolerance = 0x64;
            Color color1 = Color.Black;
            Color color2 = Color.White;

            /*
            MessageBox.Show(
                (
                (green.R + tolerance <= red.R || green.R - tolerance >= red.R) &&
                         
                (green.G + tolerance <= red.G || green.G - tolerance >= red.G) &&
                
                (green.B + tolerance <= red.B || green.B - tolerance >= red.B)
                ).ToString());
             */

            Vector3D vector1 = new Vector3D(color1.R, color1.G, color1.B);
            Vector3D vector2 = new Vector3D(color2.R, color2.G, color2.B);
            Vector3D diffVector = (Vector3D)vector1.Subtract(vector2);
            double length = diffVector.GetLength();

            MessageBox.Show((length > 350).ToString());

        }
    }
}
