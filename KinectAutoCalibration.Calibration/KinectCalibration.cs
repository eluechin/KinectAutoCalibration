using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Kinect;


namespace KinectAutoCalibration.Calibration
{
    public class KinectCalibration : IKinectCalibration
    {
        private readonly int WIDTH = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        private readonly int HEIGHT = Convert.ToInt32(ConfigurationManager.AppSettings["AREA_WIDTH"]);
        private IBeamer beamer;
        private IKinect kinect;

        private Bitmap area;

        public KinectCalibration()
        {
            Window beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            };
            beamer = new Beamer.Beamer(beamerWindow);
            kinect = new Kinect.Kinect();
            area = new Bitmap(WIDTH, HEIGHT);
            area.SetPixel(100,100, System.Drawing.Color.Red);
            //beamer.DrawChessBoard1(Colors.Red, Colors.Blue);
            //beamer.DrawCircle();
        }

        public void ShowArea()
        {
            beamer.DisplayBitmap(area);
        }

        public void StartCalibration()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetColorBitmap()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetDifferenceBitmap()
        {
            throw new NotImplementedException();
        }

        public Bitmap GetAreaBitmap()
        {
            throw new NotImplementedException();
        }
    }
}
