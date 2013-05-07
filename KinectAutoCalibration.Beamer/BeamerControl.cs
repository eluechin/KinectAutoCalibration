﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace KinectAutoCalibration.Beamer
{
    /// <summary>
    /// 
    /// </summary>
    public class BeamerControl : IBeamerControl
    {
        private readonly Window beamerWindow;

        public BeamerControl()
        {
            beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Left = Beamer.GetBeamerLeftPosition(),
                Top = Beamer.GetBeamerTopPosition(),
                Width = Beamer.GetBeamerWidth(),
                Height = Beamer.GetBeamerHeight()
            };
        }

        public void DisplayCalibrationImageEdge(bool isInverted)
        {
            var canvasImage = BeamerImage.CreateCalibImageEdge(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(),
                                                               isInverted);
            DisplayContent(canvasImage);
        }

        public void DisplayCalibrationImage(bool isInverted, int depth)
        {
            var canvasImage = BeamerImage.CreateCalibImage(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(), isInverted,
                                                           depth);
            DisplayContent(canvasImage);
        }

        public void DisplayBlank()
        {
            var imageCanvas = new Canvas { Height = Beamer.GetBeamerHeight(), Width = Beamer.GetBeamerWidth() };
            DisplayContent(imageCanvas);
        }

        private void DisplayContent(Canvas imageCanvas)
        {
            beamerWindow.Dispatcher.Invoke(
            DispatcherPriority.Render,
            new Action(() => beamerWindow.Content = imageCanvas));
            beamerWindow.WindowState = WindowState.Minimized;
            beamerWindow.WindowState = WindowState.Maximized;
        }
    }
}
