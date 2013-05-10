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
        private WriteableBitmap _colorImageBitmap4;
        private WriteableBitmap _colorImageBitmap5;
        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;
        private byte[] _colorImagePixelData1;
        private byte[] _colorImagePixelData2;
        private byte[] _colorImagePixelData3;
        private IAutoKinectBeamerCalibration _kC;
        private IKinectBeamerOperation kBO;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                

                CompositionTarget.Rendering += CompositionTarget_Rendering;

                //_kC.InitialCalibration();
                this._colorImageBitmapRect = new Int32Rect(0, 0, 640, 480);
                this._colorImageStride = 640 * 4;
           
                this._colorImageBitmap1 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                this._colorImageBitmap2 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                this._colorImageBitmap3 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                this._colorImageBitmap4 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                this._colorImageBitmap5 = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

                this.ColorImageElement1.Source = this._colorImageBitmap1;
                this.ColorImageElement2.Source = this._colorImageBitmap2;
                this.ColorImageElement3.Source = this._colorImageBitmap3;
                this.ColorImageElement4.Source = this._colorImageBitmap4;
                this.ColorImageElement5.Source = this._colorImageBitmap5;

                


                //this.ColorImageElement1.Source = kC.GetDifferenceBitmap();
                
                //this.ColorImageElement3.Source = kC.GetPic2Bitmap();

            }
            catch (Exception e)
            { 
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
           //this.ColorImageElement6.Source = _kC.PollLiveColorImage();
        }

        private void InitialCalibration(object sender, RoutedEventArgs e)
        {
            IKinectBeamerCalibration kbc = new KinectBeamerCalibration();
            kbc.CalibrateBeamerToKinect(new CalibrateEdgePoints());
            kbc.ConvertKinectToRealWorld(new ConvertToRealWorldStrategy());
            kbc.RealWorldToArea();
            kBO = kbc.CreateKinectBeamerOperation();

            var h = "Area Height: ";
            var w = "Area Width: ";
            AreaHeight.Text = h + kBO.GetAreaHeight().ToString();
            AreaWidth.Text = w + kBO.GetAreaWidth().ToString();

            //_kC.InitialCalibration();
            ////this.ColorImageElement1.Source = _kC.GetPic1Bitmap();
            ////this.ColorImageElement2.Source = _kC.GetPic2Bitmap();
            ////this.ColorImageElement1.Source = _kC.GetPicKinP();
            ////this.ColorImageElement3.Source = _kC.GetDifferenceBitmap();

            //var pixelsKinP = _kC.GetPicKinP();
            //this._colorImageBitmap1.WritePixels(this._colorImageBitmapRect, pixelsKinP, this._colorImageStride, 0);


            //var pixels = _kC.GetDifferenceImage();
            //this._colorImageBitmap3.WritePixels(this._colorImageBitmapRect, pixels, this._colorImageStride, 0);

            //var h = "Area Height: ";
            //var w = "Area Width: ";
            //AreaHeight.Text = h + _kC.GetAreaHeight().ToString();
            //AreaWidth.Text = w + _kC.GetAreaWidth().ToString();

            //this.WindowState = WindowState.Minimized;
            //this.WindowState = WindowState.Maximized;
        }

        private void Obst(object sender, RoutedEventArgs e)
        {
            //_kC.GetObstacles();
            
            //var pixelsObst = _kC.GetDifferenceImageObst();
            //this._colorImageBitmap2.WritePixels(this._colorImageBitmapRect, pixelsObst, this._colorImageStride, 0);

            //var pixelsArea = _kC.GetAreaArray();
            //this._colorImageBitmap5.WritePixels(this._colorImageBitmapRect, pixelsArea, this._colorImageStride, 0);

            //var x = "Obstacle x: ";
            //var y = "Obstacle y: ";
            //ObstacleX.Text = x+_kC.GetObstacleCentroidX().ToString();
            //ObstacleY.Text = y+_kC.GetObstacleCentroidY().ToString();

            //this.WindowState = WindowState.Minimized;
            //this.WindowState = WindowState.Maximized;
        }

        private void dsplArea(object sender, RoutedEventArgs e)
        {
            //_kC.DisplayArea();
        }

        private void dsplBlank(object sender, RoutedEventArgs e)
        {
            //_kC.DisplayBlank();
        }

        private void CalibBeamer(object sender, RoutedEventArgs e)
        {
            //_kC.CalibrateBeamer();
            //this.ColorImageElement3.Source = _kC.GetDifferenceBitmap();
            //this.WindowState = WindowState.Minimized;
            //this.WindowState = WindowState.Maximized;
        }

        private void KinectUp(object sender, RoutedEventArgs e)
        {
            //_kC.RaiseKinect();

        }

        private void KinectDown(object sender, RoutedEventArgs e)
        {
            //_kC.LowerKinect();
        }
    }

}
