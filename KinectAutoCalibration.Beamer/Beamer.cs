using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace KinectAutoCalibration.Beamer
{
    public class Beamer : IBeamer
    {
        protected Window beamerWindow;
        protected Screen screen;
        protected const int TILE_WIDTH = 70;
        protected const int TILE_HEIGHT = 70;

        public Beamer(Window win)
        {
            beamerWindow = win;
            GetBeamerScreen();
            SetWindowFullScreen();
            beamerWindow.Show();
            //System.Windows.Threading.Dispatcher.Run();
            
        }

        

        private void GetBeamerScreen()
        {
            if (Screen.AllScreens.Count() > 1)
            {
                screen = Screen.AllScreens[1];
            }
            else
            {
                screen = Screen.AllScreens[0];
            }
        }

        private void SetWindowFullScreen()
        {
            beamerWindow.Left = screen.Bounds.Left;
            beamerWindow.Top = screen.Bounds.Top;
            beamerWindow.Width = screen.Bounds.Size.Width;
            beamerWindow.Height = screen.Bounds.Size.Height;
        }

        public void DisplayCalibrationImage(bool isInverted)
        {
            var imageCanvas = new Canvas { Height = screen.Bounds.Height, Width = screen.Bounds.Width };
            imageCanvas.Background = new SolidColorBrush(Colors.Black);

            var width = screen.Bounds.Width;
            var height = screen.Bounds.Height;
            var rightOffset = width - 2 * TILE_WIDTH;
            var topOffset = height - 2 * TILE_HEIGHT;

            var topLeft = CreateRectangles(0, 0, isInverted);
            var topRight = CreateRectangles(rightOffset, 0, isInverted);
            var botRight = CreateRectangles(rightOffset, topOffset, isInverted);
            var botLeft = CreateRectangles(0, topOffset, isInverted);

            var recList = topLeft.Union(topRight).Union(botRight).Union(botLeft).ToList();
            foreach (var rectangle in recList)
            {
                imageCanvas.Children.Add(rectangle);
            }

            beamerWindow.Content = imageCanvas;

            //System.Windows.Threading.Dispatcher.Run();
        }

        public void DisplayBitmap(Bitmap bmp)
        {
            var resizedBmp = BitmapHelper.ResizeImage(bmp, screen.Bounds.Width, screen.Bounds.Height);

            beamerWindow.Background = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(),
                                                                            IntPtr.Zero,
                                                                            Int32Rect.Empty,
                                                                            BitmapSizeOptions.FromEmptyOptions()
            ));
        }

        private List<Rectangle> CreateRectangles(int leftOffset, int topOffset, bool isInverted)
        {
            var recList = new List<Rectangle>();

            Color c1;
            Color c2;
            if (isInverted)
            {
                c1 = Colors.Black;
                c2 = Colors.White;
            }
            else
            {
                c1 = Colors.White;
                c2 = Colors.Black;
            }

            var rectTopLeft = new Rectangle();
            Canvas.SetLeft(rectTopLeft, leftOffset);
            Canvas.SetTop(rectTopLeft, topOffset);
            rectTopLeft.Width = TILE_WIDTH;
            rectTopLeft.Height = TILE_HEIGHT;
            rectTopLeft.Fill = new SolidColorBrush() { Color = c1 };
            recList.Add(rectTopLeft);

            var rectTopRight = new Rectangle();
            Canvas.SetLeft(rectTopRight, leftOffset + TILE_WIDTH);
            Canvas.SetTop(rectTopRight, topOffset);
            rectTopRight.Width = TILE_WIDTH;
            rectTopRight.Height = TILE_HEIGHT;
            rectTopRight.Fill = new SolidColorBrush() { Color = c2 };
            recList.Add(rectTopRight);

            var rectBotRight = new Rectangle();
            Canvas.SetLeft(rectBotRight, leftOffset + TILE_WIDTH);
            Canvas.SetTop(rectBotRight, topOffset + TILE_HEIGHT);
            rectBotRight.Width = TILE_WIDTH;
            rectBotRight.Height = TILE_HEIGHT;
            rectBotRight.Fill = new SolidColorBrush() { Color = c1 };
            recList.Add(rectBotRight);

            var rectBotLeft = new Rectangle();
            Canvas.SetLeft(rectBotLeft, leftOffset);
            Canvas.SetTop(rectBotLeft, topOffset + TILE_HEIGHT);
            rectBotLeft.Width = TILE_WIDTH;
            rectBotLeft.Height = TILE_HEIGHT;
            rectBotLeft.Fill = new SolidColorBrush() { Color = c2 };
            recList.Add(rectBotLeft);

            return recList;
        }
    }
}
