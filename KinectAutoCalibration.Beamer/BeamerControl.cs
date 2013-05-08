using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace KinectAutoCalibration.Beamer
{
    /// <summary>
    /// Abstraction of the Beamer. Provides the basic funtionality to control the beamer.
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
            beamerWindow.Show();
        }

        /// <summary>
        /// Display a calibration image with chess patterns in the corners.
        /// </summary>
        public BeamerPoint2D DisplayCalibrationImageEdge(bool isInverted, int position)
        {
            var beamerPoint = BeamerImage.CreateCalibImageEdge(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(),
                                                               isInverted, position);
            var canvas = BeamerImage.GetImageCanvas();
            DisplayContent(canvas);
            return beamerPoint;
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

        public void DisplayArea(List<AreaPoint2D> objects, IBeamerCorrectionStrategy correctionStrategy)
        {
            var beamerPointOfObjects = objects.Select(correctionStrategy.CalculateBeamerCoordinate).ToList();
            var canvasImage = BeamerImage.CreateAreaImage(Beamer.GetBeamerWidth(), Beamer.GetBeamerHeight(), beamerPointOfObjects);
            DisplayContent(canvasImage);
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
