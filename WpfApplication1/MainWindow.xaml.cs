using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap _colorImageBitmap1;
        private WriteableBitmap _colorImageBitmap2;
        private WriteableBitmap _colorImageBitmap3;
        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;
        private byte[] _colorImagePixelData1;
        private byte[] _colorImagePixelData2;
        private byte[] _colorImagePixelData3;
        private IKinectCalibration _kC;

        public MainWindow()
        {
            InitializeComponent();
            //var s = new KinectCalibration();
            //s.ShowArea();
            //MessageBox.Show("aaa");
            //var beamer2 = new Beamer(beamerWindow);
            //beamer2.DisplayCalibrationImage(true);
            //beamerWindow.Show();
            
           
            //MessageBox.Show("hallo");
            

            //Thread newThread = new Thread(new ThreadStart(() =>
            //    {
            //        new KinectCalibration().StartCalibration();
            //        System.Windows.Threading.Dispatcher.Run();
            //    }));
            //newThread.SetApartmentState(ApartmentState.STA);
            //newThread.IsBackground = true;
            //newThread.Start();
            //MessageBox.Show("test");


            try
            {
                _kC = new KinectCalibration();
                _kC.StartCalibration();

                //WriteableBitmap _colorImageBitmap1 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                //WriteableBitmap _colorImageBitmap2 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                //WriteableBitmap _colorImageBitmap3 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

                //this.ColorImageElement1.Source = _colorImageBitmap1;
                //this.ColorImageElement2.Source = _colorImageBitmap2;
                //this.ColorImageElement3.Source = _colorImageBitmap3;


                //this.ColorImageElement1.Source = kC.GetDifferenceBitmap();
                
                //this.ColorImageElement3.Source = kC.GetPic2Bitmap();

            }
            catch (Exception e)
            { 
                Console.Error.WriteLine(e.StackTrace);
            }
           

        }

        private void click(object sender, RoutedEventArgs e)
        {
            _kC.GetObstacles();
            this.ColorImageElement2.Source = _kC.GetPic1Bitmap();
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Maximized;
        }

    }

}
