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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KinectAutoCalibration.Beamer
{
    public class Beamer
    {
        protected Window beamerWindow;
        protected Screen screen;

        public Beamer()
        {
            beamerWindow = new Window
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    WindowStyle = WindowStyle.None,
                    AllowsTransparency = true
                };

            GetBeamerScreen();
            SetWindowFullScreen();
            beamerWindow.Show();
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
    }
}
