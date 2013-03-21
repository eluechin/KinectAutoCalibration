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
using Microsoft.Kinect;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect;
using KinectAutoCalibration.Kinect.Interfaces;

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

            DiscoverKinectSensor();

            KinectPoint[,] pic1 = _iKinect.GetColorImage(); // 1st Pic: strange area
            pic1 = _iKinect.GetColorImage(); //2nd Pic: too dark
            pic1 = _iKinect.GetColorImage(); //3rd Pic: not the same brightness as "real" image
            pic1 = _iKinect.GetColorImage(); //4th Pic: okay!
            MessageBox.Show("Please display the playground", "Display Playground", MessageBoxButton.OK,
                            MessageBoxImage.Information);

            KinectPoint[,] pic2 = _iKinect.GetColorImage();

            KinectPoint[,] newPic = _iKinect.GetDifferenceImage(pic2, pic1, 0x64); //0x64, 0x1E, 0x32


            short[] depthPic = _iKinect.GetDepthImage();


            KinectPoint[,] depthAndColorPic = _iKinect.GetDepthAndColorImage();


            //this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, newPic, this._colorImageStride, 0);
            //this._colorImageBitmap3.WritePixels(this._colorImageBitmapRect, pic1, this._colorImageStride, 0);
            //this._colorImageBitmap4.WritePixels(this._colorImageBitmapRect, pic2, this._colorImageStride, 0);


            this.ColorImageElement.Source = _iKinect.PrintKinectPointArray(newPic, 640, 480);
            this.ColorImageElement2.Source = _iKinect.PrintKinectPointArray(depthAndColorPic, 640, 480);
            this.ColorImageElement3.Source = _iKinect.PrintKinectPointArray(pic1, 640, 480);
            this.ColorImageElement4.Source = _iKinect.PrintKinectPointArray(pic2, 640, 480);
            this.DepthImageElement.Source = _iKinect.PrintKinectPointArray(depthAndColorPic, 640, 480);
            this._rawDepthImage2.WritePixels(this._rawDepthImageRect, depthPic, this._rawDepthImageStride, 0);

            
        }


        private void DiscoverKinectSensor()
        {
            if (this._kinect != null && this._kinect.Status != KinectStatus.Connected)
            {
                this._kinect = null;
            }


            if (this._kinect == null)
            {
                this._kinect = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

                if (this._kinect != null)
                {
                    this._kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    this._kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    this._kinect.Start();

                    ColorImageStream colorStream = this._kinect.ColorStream;
                    DepthImageStream depthStream = this._kinect.DepthStream;

                    this._colorImageBitmap = new WriteableBitmap(colorStream.FrameWidth, colorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null);
                    this._colorImageBitmapRect = new Int32Rect(0, 0, colorStream.FrameWidth, colorStream.FrameHeight);
                    this._colorImageStride = colorStream.FrameWidth * colorStream.FrameBytesPerPixel;
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
                    this._rawDepthImageStride = depthStream.FrameBytesPerPixel * depthStream.FrameWidth;
                    this.DepthImageElement.Source = this._rawDepthImage;

                    this._rawDepthImage2 = new WriteableBitmap(depthStream.FrameWidth, depthStream.FrameHeight, 96, 96, PixelFormats.Gray16, null);
                    this.DepthImageElement2.Source = this._rawDepthImage2;
                }
            }
        }
    }
}


      