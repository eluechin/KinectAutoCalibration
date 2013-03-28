﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Beamer.Interfaces;
using KinectAutoCalibration.Kinect.Interfaces;


namespace KinectAutoCalibration.Calibration
{
    public class KinectCalibration : IKinectCalibration
    {
        private const int WIDTH = 640;
        private const int HEIGHT = 480;
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