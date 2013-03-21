using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            int tolerance = 0x64;
            Color red = Color.Red;
            Color green = Color.Lime;


            MessageBox.Show(
                (
                (green.R + tolerance <= red.R || green.R - tolerance >= red.R) &&
                         
                (green.G + tolerance <= red.G || green.G - tolerance >= red.G) &&
                
                (green.B + tolerance <= red.B || green.B - tolerance >= red.B)
                ).ToString());
          
        }
    }
}
