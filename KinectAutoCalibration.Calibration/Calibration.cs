using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using KinectAutoCalibration.Beamer;


namespace KinectAutoCalibration.Calibration
{
    public class Calibration
    {
        private Beamer.Interfaces.IBeamerTest beamer;

        public Calibration()
        {

            Window beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            };
            beamer = new BeamerTest(beamerWindow);
            beamer.DrawChessBoard1(Colors.Red, Colors.Blue);
            beamer.DrawCircle();
        }
    }
}
