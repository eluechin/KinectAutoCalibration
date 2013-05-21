using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Beamer
{
    public class OperationImage
    {
        public Canvas OperationCanvas { get; set; }

        public OperationImage()
        {
            OperationCanvas = new Canvas
            {
                Height = Beamer.GetBeamerHeight(),
                Width = Beamer.GetBeamerWidth(),
                Background = new SolidColorBrush(Colors.White)
            };
        }

        public void ColorizePoint(BeamerPoint point)
        {
            var line = new Line
            {
                Stroke = Brushes.Red,
                X1 = point.X,
                X2 = point.X,
                Y1 = point.Y,
                Y2 = point.Y,
                StrokeThickness = 1
            };
            OperationCanvas.Children.Add(line);
        }
    }
}
