using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KinectAutoCalibration.Beamer;
using KinectAutoCalibration.Calibration;
using KinectAutoCalibration.Beamer.Interfaces;

namespace WindowsTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBeamerTest beamer;
        private IBeamer beamer2;
        
        public MainWindow()
        {
            InitializeComponent();
            Window beamerWindow = new Window
            {
                WindowStartupLocation = WindowStartupLocation.Manual,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true
            };
            beamer = new BeamerTest(beamerWindow);
            beamer2 = new Beamer(beamerWindow);
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            beamer.DrawCircle();
        }

        private void DrawChessboard1_Click(object sender, RoutedEventArgs e)
        {
            beamer.DrawChessBoard1(Colors.Black, Colors.White);
        }

        private void DrawChessboard2_Click(object sender, RoutedEventArgs e)
        {
            beamer.DrawChessBoard1(Colors.White, Colors.Black);
        }

        private void DrawChessboard3_Click(object sender, RoutedEventArgs e)
        {
            beamer.DrawChessBoard1(Colors.GreenYellow, Colors.Red);
            
        }

        private void CalibImg_Click(object sender, RoutedEventArgs e)
        {
           beamer2.DisplayCalibrationImage(false);
        }

        private void CalibImgInv_Click(object sender, RoutedEventArgs e)
        {
            beamer2.DisplayCalibrationImage(true);
        }
    }
}
