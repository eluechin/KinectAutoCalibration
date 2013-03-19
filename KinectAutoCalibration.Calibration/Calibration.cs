using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KinectAutoCalibration.Beamer;

namespace KinectAutoCalibration.Calibration
{
    public class Calibration
    {
        private Beamer.Beamer beamer;

        public Calibration()
        {
            beamer = new Beamer.Beamer();
        }
    }
}
