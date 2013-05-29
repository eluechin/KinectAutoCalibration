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

        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;
        private byte[] _colorImagePixelData1;
        private byte[] _colorImagePixelData2;
        private byte[] _colorImagePixelData3;
        private IAutoKinectBeamerCalibration _kC;
        private IKinectBeamerOperation kinectBeamerOperation;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
                this._colorImageBitmapRect = new Int32Rect(0, 0, 640, 480);
                this._colorImageStride = 640 * 4;
           
                LiveImage.Source = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                DiffImage.Source = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);
                AreaImage.Source = new WriteableBitmap(640, 480, 96, 96, PixelFormats.Bgr32, null);

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

        private void AutoCalibration(object sender, RoutedEventArgs e)
        {
            kinectBeamerOperation = new AutoKinectBeamerCalibration().StartAutoCalibration();

            const string height = "Area Height: ";
            const string width = "Area Width: ";
            //AreaHeight.Text = height + kinectBeamerOperation.GetAreaHeight();
            //AreaWidth.Text = width + kinectBeamerOperation.GetAreaWidth();

            //Deprecated trololo
            //var kinectSpace = kinectBeamerOperation.GetKinectSpace();
            //ColorImageElement1.Source = kinectBeamerOperation.GetKinectSpace();

        }

        private void ObstacleToBeamer(object sender, RoutedEventArgs e)
        {
            kinectBeamerOperation.ColorizeObstacle();
        }

        private void ObstacleToArea(object sender, RoutedEventArgs e)
        {
            AreaImage.Source = kinectBeamerOperation.ObstacleToArea();
        }

        private void MeasureCentroid(object sender, RoutedEventArgs e)
        {
            kinectBeamerOperation.CalculateObstacleCentroid();

            var x = "Obstacle x: ";
            var y = "Obstacle y: ";
            //ObstacleX.Text = x + kinectBeamerOperation.GetObstacleCentroidX();
            //ObstacleY.Text = y + kinectBeamerOperation.GetObstacleCentroidY();
            
        }

        private void KinectUp(object sender, RoutedEventArgs e)
        {
            //_kC.RaiseKinect();

        }

        private void KinectDown(object sender, RoutedEventArgs e)
        {
            //_kC.LowerKinect();
        }

        //evtl. seperate??
        //private void CompareZCalc(object sender, RoutedEventArgs e)
        //{
        //    var diffPoints = kinectBeamerOperation.CompareZCalcStrategies(new CalculateToRealWorldStrategy());
        //    this._colorImageBitmap1.WritePixels(this._colorImageBitmapRect, diffPoints, this._colorImageStride, 0);
        //}
    }

}
