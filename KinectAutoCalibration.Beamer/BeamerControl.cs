using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            beamerWindow.Show();
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
