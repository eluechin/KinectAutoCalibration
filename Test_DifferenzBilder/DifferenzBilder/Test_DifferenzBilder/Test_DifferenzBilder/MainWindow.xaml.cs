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
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Common.Interfaces;
using Microsoft.Kinect;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;

namespace Test_DifferenzBilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKinect _iKinect;
        private KinectSensor _kinect;
        private WriteableBitmap _colorImageBitmap;
        private WriteableBitmap _colorImageBitmap2;
        private WriteableBitmap _colorImageBitmap3;
        private WriteableBitmap _colorImageBitmap4;
        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;
        private byte[] _colorImagePixelData;
        private byte[] _colorImagePixelData2;
        private byte[] _colorImagePixelData3;
        private byte[] _colorImagePixelData4;

        private WriteableBitmap _rawDepthImage;
        private WriteableBitmap _rawDepthImage2;
        private Int32Rect _rawDepthImageRect;
        private int _rawDepthImageStride;


        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            _iKinect = new Kinect();
            InitializeComponent();
            this._kinect = _iKinect.DiscoverKinectSensor();
            Initialize();

            KinectPoint[,] pic1 = _iKinect.GetColorImage(); // 1st Pic: strange area
            pic1 = _iKinect.GetColorImage(); //2nd Pic: too dark
            pic1 = _iKinect.GetColorImage(); //3rd Pic: not the same brightness as "real" image
            pic1 = _iKinect.GetColorImage(); //4th Pic: okay!
            
            MessageBox.Show("Please display the playground", "Display Playground", MessageBoxButton.OK,
                            MessageBoxImage.Information);

            KinectPoint[,] pic2 = _iKinect.GetColorImage();

            KinectPoint[,] newPic = _iKinect.GetDifferenceImage(pic2, pic1, 200); //0x64, 0x1E, 0x32

            //newPic = CalculateKMeans(newPic);

            short[] depthPic = _iKinect.GetDepthImage();


            KinectPoint[,] depthAndColorPic = _iKinect.CreateKinectPointArray();


            this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, PrintKinectPointArray(newPic, 640, 480), this._colorImageStride, 0);
            this._colorImageBitmap2.WritePixels(this._colorImageBitmapRect, PrintKinectPointArray(depthAndColorPic, 640, 480), this._colorImageStride, 0);
            this._colorImageBitmap3.WritePixels(this._colorImageBitmapRect, PrintKinectPointArray(pic1, 640, 480), this._colorImageStride,0);
            this._colorImageBitmap4.WritePixels(this._colorImageBitmapRect, PrintKinectPointArray(pic2, 640, 480), this._colorImageStride, 0);
            this._rawDepthImage.WritePixels(this._rawDepthImageRect, PrintKinectPointArray(depthAndColorPic, 640, 480), this._rawDepthImageStride,0);
            this._rawDepthImage2.WritePixels(this._rawDepthImageRect, depthPic, this._rawDepthImageStride, 0);

        }

        private KinectPoint[,] CalculateKMeans(KinectPoint[,] newPic)
        {
            try
            {
                List<Vector2D> centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(newPic), KMeansHelper.CreateInitialCentroids(640, 480));

                foreach (var vectorCentroid in centroids)
                {
                    newPic[(int) vectorCentroid.X, (int) vectorCentroid.Y].R = 255;
                    newPic[(int) vectorCentroid.X, (int) vectorCentroid.Y].G = 0;
                    newPic[(int) vectorCentroid.X, (int) vectorCentroid.Y].B = 0;
                    //bitmap.SetPixel((int)vectorCentroid.X, (int)vectorCentroid.Y, Color.Red);
                    MessageBox.Show("X: " + vectorCentroid.X.ToString() + ", Y: " + vectorCentroid.Y.ToString());
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }

            return newPic;
        }


        private void Initialize()
        {

            if (this._kinect != null)
            {
                ColorImageStream colorStream = this._kinect.ColorStream;
                DepthImageStream depthStream = this._kinect.DepthStream;

                this._colorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
                this._colorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                this._colorImageStride = colorStream.FrameWidth*colorStream.FrameBytesPerPixel;
                this._colorImagePixelData = new byte[colorStream.FramePixelDataLength];
                this.ColorImageElement.Source = this._colorImageBitmap;

                this._colorImageBitmap2 = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
                this._colorImagePixelData2 = new byte[colorStream.FramePixelDataLength];
                this.ColorImageElement2.Source = this._colorImageBitmap2;

                this._colorImageBitmap3 = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
                this._colorImagePixelData3 = new byte[colorStream.FramePixelDataLength];
                this.ColorImageElement3.Source = this._colorImageBitmap3;

                this._colorImageBitmap4 = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
                this._colorImagePixelData4 = new byte[colorStream.FramePixelDataLength];
                this.ColorImageElement4.Source = this._colorImageBitmap4;

                this._rawDepthImage = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight, 96, 96, PixelFormats.Gray16, null);
                this._rawDepthImageRect = new Int32Rect(0, 0, depthStream.FrameWidth, depthStream.FrameHeight);
                this._rawDepthImageStride = depthStream.FrameBytesPerPixel*depthStream.FrameWidth;
                this.DepthImageElement.Source = this._rawDepthImage;

                this._rawDepthImage2 = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight, 96, 96, PixelFormats.Gray16, null);
                this.DepthImageElement2.Source = this._rawDepthImage2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPicKin"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public byte[] PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height)
        {
            var stride = width * 4; // bytes per row

            byte[] pixelData = new byte[height * stride];
            int index = 0;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (newPicKin[x, y] != null)
                    {
                        pixelData[index + 2] = (byte)newPicKin[x, y].R;
                        pixelData[index + 1] = (byte)newPicKin[x, y].G;
                        pixelData[index] = (byte)newPicKin[x, y].B;
                    }
                    else
                    {
                        pixelData[index + 2] = 0xFF;
                        pixelData[index + 1] = 0x00;
                        pixelData[index] = 0x00;
                    }
                    index += 4;
                }
            }
            return pixelData;
            //wrBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
        }
    }
}


      