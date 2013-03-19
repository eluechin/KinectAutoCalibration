using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KinectAutoCalibration.Beamer.Interfaces;

namespace KinectAutoCalibration.Beamer
{
    public class BeamerTest : Beamer, IBeamerTest
    {

        public void DrawCircle()
        {
            // Create a StackPanel to contain the shape.
            StackPanel myStackPanel = new StackPanel();

            // Create a red Ellipse.
            Ellipse myEllipse = new Ellipse();

            // Create a SolidColorBrush with a red color to fill the  
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = 500;
            myEllipse.Height = 500;

            // Add the Ellipse to the StackPanel.
            myStackPanel.Children.Add(myEllipse);

            beamerWindow.Content = myStackPanel;
        }

        public void DrawChessBoard1(Color c1, Color c2)
        {
            const int tileWidth = 700;
            const int tileHeight = 525;
            double width = screen.Bounds.Width;
            double height = screen.Bounds.Height;

            var black = c1;
            var white = c2;

            var chessCanvas = new Canvas { Height = height, Width = width };

            for (var i = 0; i < (int)width / tileWidth + 1; i++)
            {
                for (var j = 0; j < (int)height / tileHeight + 1; j++)
                {
                    var newRect = new Rectangle();
                    Canvas.SetLeft(newRect, i * tileWidth);
                    Canvas.SetTop(newRect, j * tileHeight);
                    newRect.Width = tileWidth;
                    newRect.Height = tileHeight;
                    Color c;
                    if (i % 2 == 0)
                    {
                        c = j % 2 != 0 ? black : white;
                    }
                    else
                    {
                        c = j % 2 != 0 ? white : black;
                    }
                    newRect.Fill = new SolidColorBrush() { Color = c, Opacity = 1f };

                    chessCanvas.Children.Add(newRect);
                }
            }
            beamerWindow.Content = chessCanvas;
        }
    }
}
