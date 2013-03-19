using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectAutoCalibration.Beamer
{
    public class Beamer
    {
        private Window beamerWindow;
        Screen screen;
        public Beamer()
        {
            beamerWindow = new Window();
            if (Screen.AllScreens.Count() > 1)
            {
                screen = Screen.AllScreens[1];
            }
            else
            {
                screen = Screen.AllScreens[0];
            }
            SetWindowLocation();
            DrawChessBoard1(screen.Bounds.Size.Width, screen.Bounds.Size.Height);
            beamerWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            beamerWindow.WindowStyle = WindowStyle.None;
            beamerWindow.AllowsTransparency = true;

            beamerWindow.Show();
        }

        private void SetWindowLocation()
        {
            beamerWindow.Left = screen.Bounds.Left;
            beamerWindow.Top = screen.Bounds.Top;
            beamerWindow.Width = screen.Bounds.Size.Width;
            beamerWindow.Height = screen.Bounds.Size.Height;
        }

        private void DrawChessBoard1(double width, double height)
        {
            const int tileWidth = 70;
            const int tileHeight = 70;

            var black = Colors.White;
            var white = Colors.Black;

            var chessCanvas = new Canvas { Height = height, Width = width };

            for (var i = 0; i < (int)width / tileWidth; i++)
            {
                for (var j = 0; j < (int)height / tileHeight; j++)
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
