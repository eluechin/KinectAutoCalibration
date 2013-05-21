using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    /// <summary>
    /// Abstraction of the BeamerWindow
    /// </summary>
    public class BeamerWindow : IBeamerWindow
    {
        private readonly Window beamerWindow;

        public BeamerWindow()
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
            beamerWindow.Show();
        }

        /// <summary>
        /// Display a calibration image with chess patterns in the corners.
        /// </summary>
        public BeamerPoint DisplayCalibrationImageEdge(bool isInverted, int position)
        {
            var beamerPoint = CalibrationImage.CreateCalibImageEdge(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(),
                                                               isInverted, position);
            var canvas = CalibrationImage.GetImageCanvas();
            DisplayContent(canvas);
            return beamerPoint;
        }

        public void DisplayCalibrationImage(bool isInverted, int depth)
        {
            var canvasImage = CalibrationImage.CreateCalibImage(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(), isInverted,
                                                           depth);
            DisplayContent(canvasImage);
        }

        public void DisplayBlank()
        {
            var imageCanvas = new Canvas { Height = Beamer.GetBeamerHeight(), Width = Beamer.GetBeamerWidth() };
            DisplayContent(imageCanvas);
        }

        public void DisplayArea(List<AreaPoint> objects, IBeamerCorrectionStrategy correctionStrategy)
        {
            var beamerPointOfObjects = objects.Select(correctionStrategy.CalculateBeamerCoordinate).ToList();
            var canvasImage = CalibrationImage.CreateAreaImage(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(), beamerPointOfObjects);
            DisplayContent(canvasImage);
        }

        public int GetWidth()
        {
            return Beamer.GetBeamerWidth();
        }

        public int GetHeight()
        {
            return Beamer.GetBeamerHeight();
        }

        public void DisplayContent(Canvas imageCanvas)
        {
            beamerWindow.Dispatcher.Invoke(
            DispatcherPriority.Render,
            new Action(() => beamerWindow.Content = imageCanvas));
            beamerWindow.WindowState = WindowState.Minimized;
            beamerWindow.WindowState = WindowState.Maximized;
        }
    }
}
