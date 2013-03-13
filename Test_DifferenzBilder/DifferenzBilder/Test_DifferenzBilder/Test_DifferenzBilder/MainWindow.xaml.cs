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

namespace Test_DifferenzBilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            InitializeComponent();

            //CompositionTarget.Rendering += CompositionTarget_Rendering;
            DiscoverKinectSensor();
            byte[] pic1 = GetColorImage(); // 1st Pic: strange area
            pic1 = GetColorImage(); //2nd Pic: too dark
            pic1 = GetColorImage(); //3rd Pic: not the same brightness as "real" image
            pic1 = GetColorImage(); //4th Pic: okay!
            MessageBox.Show("Please display the playground", "Display Playground", MessageBoxButton.OK, MessageBoxImage.Information);

            byte[] pic2 = GetColorImage();

            byte[] newPic = GetDifferenceImage(pic2, pic1, 0x64); //0x64, 0x1E, 0x32
            KinectPoint[,] newPicKin = ConvertToKinectPoint(newPic, 640, 480);
            KinectPoint[,] pic2Kin = ConvertToKinectPoint(pic2, 640, 480);

            short[] depthPic = GetDepthImage();


            KinectPoint[,] depthAndColorPic = GetDepthAndColorImage();


            this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, newPic, this._colorImageStride, 0);
            this._colorImageBitmap3.WritePixels(this._colorImageBitmapRect, pic1, this._colorImageStride, 0);
            this._colorImageBitmap4.WritePixels(this._colorImageBitmapRect, pic2, this._colorImageStride, 0);
            PrintKinectPointArray(newPicKin, 640, 480, this._colorImageBitmap2);
            PrintKinectPointArray(depthAndColorPic, 640, 480, this._rawDepthImage );
            
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
                    this._kinect.ColorStream.Enable();
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


        private void PollColorImageStream()
        {
            if (this._kinect == null)
            {
                MessageBox.Show("Please connect your Kinect.", "Kinect not found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                try
                {
                    Text.Text = "picture 1";

                    using (ColorImageFrame frame = this._kinect.ColorStream.OpenNextFrame(1000))
                    {
                        if (frame != null)
                        {
                            byte[] pixelData1 = new byte[frame.PixelDataLength];
                            frame.CopyPixelDataTo(pixelData1);

                            System.Threading.Thread.Sleep(1000);

                            using (ColorImageFrame frame2 = this._kinect.ColorStream.OpenNextFrame(1000))
                            {
                                if (frame2 != null)
                                {
                                    Text2.Text = "picture 2";
                                    byte[] pixelData2 = new byte[frame2.PixelDataLength];
                                    frame2.CopyPixelDataTo(pixelData2);


                                    byte[] newimage = new byte[pixelData1.Length];
                                    byte[] newimage2 = new byte[pixelData1.Length];


                                    //ManipulateImageGetNegative(pixelData2, frame2, newimage2);


                                    GetDifferenceImage(pixelData2, pixelData1, newimage, 0x1E, frame2);
                                    GetDifferenceImage(pixelData2, pixelData1, newimage2, 0x64, frame2);

                                    //KinectPoint[,] kinArray = ConvertToKinectPoint(pixelData2, frame2);
                                    //var point0_0 = kinArray[0, 0];

                                    //frame.CopyPixelDataTo(newimage);
                                    this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, pixelData1, this._colorImageStride, 0);
                                    this._colorImageBitmap2.WritePixels(this._colorImageBitmapRect, pixelData2, this._colorImageStride, 0);
                                    this._colorImageBitmap3.WritePixels(this._colorImageBitmapRect, newimage, this._colorImageStride, 0);
                                    this._colorImageBitmap4.WritePixels(this._colorImageBitmapRect, newimage2, this._colorImageStride, 0);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured. Please restart. \n" + ex.StackTrace, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// DIFF Color Image - Thresholding
        /// </summary>
        private void GetDifferenceImage(byte[] pixelData2, byte[] pixelData1, byte[] newimage, int tolerance, ColorImageFrame frame2)
        {
            //tolerance = 0x1E; //0x1E = 30, 0x32 = 50, 0xA = 10, 0x28 = 40, 0x14 = 20
            //Text3.Text = "tolerance: 0x1E --> 30";

            for (int i = 0; i < pixelData2.Length; i += frame2.BytesPerPixel)
            {
                if (pixelData2[i] + tolerance <= pixelData1[i] || pixelData2[i] - tolerance >= pixelData1[i] &&
                    pixelData2[i + 1] + tolerance <= pixelData1[i] || pixelData2[i + 1] - tolerance >= pixelData1[i] &&
                    pixelData2[i + 2] + tolerance <= pixelData1[i] || pixelData2[i + 2] - tolerance >= pixelData1[i])
                {
                    newimage[i] = 0x00; //Blue
                    newimage[i + 1] = 0x00; //Green
                    newimage[i + 2] = 0x00; //Red
                }
                else
                {
                    newimage[i] = 0xFF;
                    newimage[i + 1] = 0xFF;
                    newimage[i + 2] = 0xFF;
                }
            }
        }

        private byte[] GetDifferenceImage(byte[] pixelData2, byte[] pixelData1, int tolerance)
        {
            //tolerance = 0x1E; //0x1E = 30, 0x32 = 50, 0xA = 10, 0x28 = 40, 0x14 = 20
            //Text3.Text = "tolerance: 0x1E --> 30";
            const int bytesPerPixel = 4;


            byte[] newimage = new byte[pixelData1.Length];

            for (int i = 0; i < pixelData2.Length; i += bytesPerPixel)
            {
                if (pixelData2[i] + tolerance <= pixelData1[i] || pixelData2[i] - tolerance >= pixelData1[i] &&
                    pixelData2[i + 1] + tolerance <= pixelData1[i] || pixelData2[i + 1] - tolerance >= pixelData1[i] &&
                    pixelData2[i + 2] + tolerance <= pixelData1[i] || pixelData2[i + 2] - tolerance >= pixelData1[i])
                {
                    newimage[i] = 0x00; //Blue
                    newimage[i + 1] = 0x00; //Green
                    newimage[i + 2] = 0x00; //Red
                }
                else
                {
                    newimage[i] = 0xFF;
                    newimage[i + 1] = 0xFF;
                    newimage[i + 2] = 0xFF;
                }
            }

            return newimage;
        }

        private static void ManipulateImageGetNegative(byte[] pixelData2, ColorImageFrame frame2, byte[] newimage2)
        {
            for (int i = 0; i < pixelData2.Length; i += frame2.BytesPerPixel)
            {
                newimage2[i] = (byte)~pixelData2[i]; //Blue
                newimage2[i + 1] = (byte)~pixelData2[i + 1]; //Green
                newimage2[i + 2] = (byte)~pixelData2[i + 2]; //Red
            }
        }



        private static KinectPoint[,] ConvertToKinectPoint(byte[] bytearray, ColorImageFrame frame)
        {
            int width = frame.Width;
            int height = frame.Height;
            int bitPositionByteArray = 0;
            KinectPoint[,] kinectArray = new KinectPoint[width, height];



            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    kinectArray[x, y] = new KinectPoint(
                                            x,
                                            y,
                                            (byte)bytearray[bitPositionByteArray],
                                            (byte)bytearray[bitPositionByteArray + 1],
                                            (byte)bytearray[bitPositionByteArray + 2]);
                    bitPositionByteArray += frame.BytesPerPixel;
                }
            }


            return kinectArray;
        }

        private KinectPoint[,] ConvertToKinectPoint(byte[] bytearray, int width, int height)
        {
            int bitPositionByteArray = 0;
            const int BytesPerPixel = 4;
            KinectPoint[,] kinectArray = new KinectPoint[width, height];



            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    kinectArray[x, y] = new KinectPoint(
                                            x,
                                            y,
                                            (byte)bytearray[bitPositionByteArray],
                                            (byte)bytearray[bitPositionByteArray + 1],
                                            (byte)bytearray[bitPositionByteArray + 2]);
                    bitPositionByteArray += BytesPerPixel;
                }
            }
            return kinectArray;
        }


        private static KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y)
        {
            KinectPoint kinPoint = new KinectPoint();

            kinPoint = kinArray[x, y];

            return kinPoint;
        }


        private byte[] GetColorImage()
        {

            try
            {

                using (ColorImageFrame frame = this._kinect.ColorStream.OpenNextFrame(1000))
                {
                    if (frame != null)
                    {
                        byte[] pixelData1 = new byte[frame.PixelDataLength];
                        frame.CopyPixelDataTo(pixelData1);

                        KinectPoint[,] kinArray = ConvertToKinectPoint(pixelData1, frame);

                        return pixelData1;
                    }

                    return null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured. Please restart. \n" + ex.StackTrace, "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

        }

        private short[] GetDepthImage()
        {
            try
            {

                using (DepthImageFrame depthFrame = this._kinect.DepthStream.OpenNextFrame(1000))
                {
                    if (depthFrame != null)
                    {
                        short[] depthPixelData = new short[depthFrame.PixelDataLength];
                        depthFrame.CopyPixelDataTo(depthPixelData);

                        return depthPixelData;
                    }

                    return null;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An error occured. Please restart. \n" + ex.StackTrace, "Error occured",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

       /* private KinectPoint[,] GetDepthAndColorImage()
        {
            KinectPoint[,] kinArray = new KinectPoint[640,480];

            using (ColorImageFrame colorFrame = this._kinect.ColorStream.OpenNextFrame(100))
            {
                using (DepthImageFrame depthFrame = this._kinect.DepthStream.OpenNextFrame(100))
                {
                    short[] depthPixelData = new short[1228800];
                    byte[] colorPixelData = new byte[1228800];
                    int depthPixelIndex;
                    int colorPixelIndex;
                    ColorImagePoint colorPoint;
                    DepthImagePoint depthPoint;
                    int colorStride = colorFrame.BytesPerPixel * colorFrame.Width;
                    int bytesPerPixel = 4;
                    byte[] newImage = new byte[depthFrame.Height * depthFrame.Width*4];
                    int newImageIndex = 0;


                    depthFrame.CopyPixelDataTo(depthPixelData);
                    colorFrame.CopyPixelDataTo(colorPixelData);

                    
                    


                    for (int depthY = 0; depthY < depthFrame.Height; depthY++)
                    {
                        for (int depthX = 0; depthX < depthFrame.Width; depthX++, newImageIndex += bytesPerPixel)
                        {
                            depthPixelIndex = depthX + (depthY * depthFrame.Width);
                            depthPoint =;

                                colorPoint = _kinect.CoordinateMapper.MapDepthPointToColorPoint(depthFrame.Format, depthPixelData[depthPixelIndex], colorFrame.Format);
                                colorPixelIndex = (colorPoint.X * colorFrame.BytesPerPixel) + (colorPoint.Y * colorStride);

                                newImage[newImageIndex] = colorPixelData[colorPixelIndex];         //Blue    
                                newImage[newImageIndex + 1] = colorPixelData[colorPixelIndex + 1];     //Green
                                newImage[newImageIndex + 2] = colorPixelData[colorPixelIndex + 2];     //Red
                                //playerImage[playerImageIndex + 3] = 0xFF;                                          //Alpha
                        }
                    }
                }
            }

            return kinArray;
        }
        */

        private KinectPoint[,] GetDepthAndColorImage()
        {
            KinectPoint[,] kinArray = new KinectPoint[_kinect.DepthStream.FrameWidth, _kinect.DepthStream.FrameHeight];
            short[] depthPixelData = new short[_kinect.DepthStream.FramePixelDataLength];
            byte[] colorPixelData = new byte[_kinect.ColorStream.FramePixelDataLength];
           

            using (DepthImageFrame depthFrame = _kinect.DepthStream.OpenNextFrame(0))
            {
                DepthImagePixel[] depthImagePixelData = new DepthImagePixel[depthPixelData.Length];
                ColorImagePoint[] colorImagePixelData = new ColorImagePoint[depthFrame.Height * depthFrame.Width];

                if (depthFrame != null)
                {
                    byte[] newImage = new byte[depthFrame.Height * depthFrame.Width * 4];
                    int newImageIndex = 0;

                    depthFrame.CopyPixelDataTo(depthPixelData);

                    depthFrame.CopyDepthImagePixelDataTo(depthImagePixelData);

                    _kinect.CoordinateMapper.MapDepthFrameToColorFrame(depthFrame.Format, depthImagePixelData, _kinect.ColorStream.Format, colorImagePixelData);

                    using (ColorImageFrame colorFrame = _kinect.ColorStream.OpenNextFrame(0))
                    {
                        if (colorFrame != null)
                        {
                            colorFrame.CopyPixelDataTo(colorPixelData);
                        }
                    }
                }

                int index = 0;

                for (int y = 0; y < depthFrame.Height; ++y)
                {
                    for (int x = 0; x < depthFrame.Width; ++x)
                    {
                         if (colorImagePixelData[index].X < _kinect.ColorStream.FrameWidth
                            && colorImagePixelData[index].Y < _kinect.ColorStream.FrameHeight)
                        {
                            kinArray[x, y] = new KinectPoint(colorImagePixelData[index].X,
                                                                colorImagePixelData[index].Y,
                                                                depthImagePixelData[index].Depth,
                                                                colorPixelData[colorImagePixelData[index].Y * depthFrame.Width * 4 + (colorImagePixelData[index].X + 1) * 4],
                                                                colorPixelData[colorImagePixelData[index].Y * depthFrame.Width * 4 + (colorImagePixelData[index].X + 1) * 4 + 1],
                                                                colorPixelData[colorImagePixelData[index].Y * depthFrame.Width * 4 + (colorImagePixelData[index].X + 1) * 4 + 2]);
                            ++index;
                        }
                    }
                       
                }
                        
                //}
            }
                return kinArray;
        }

        private void PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height)
        {
            var stride = width * 4; // bytes per row

            byte[] pixelData = new byte[height * stride];
            int index = 0;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //var color = colorArray[y, x];
                    //var index = (y * stride) + (x * 4);

                    pixelData[index] = (byte)newPicKin[x, y].R;
                    pixelData[index + 1] = (byte)newPicKin[x, y].G;
                    pixelData[index + 2] = (byte)newPicKin[x, y].B;
                    //pixelData[index + 3] = color.A; // color.A;
                    index += 4;
                }
            }

            //var bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixelData, stride);
            this._colorImageBitmap2.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
        }

        private void PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height, WriteableBitmap wrBitmap)
        {
            var stride = width * 4; // bytes per row

            byte[] pixelData = new byte[height * stride];
            int index = 0;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //var color = colorArray[y, x];
                    //var index = (y * stride) + (x * 4);

                    pixelData[index] = (byte)newPicKin[x, y].R;
                    pixelData[index + 1] = (byte)newPicKin[x, y].G;
                    pixelData[index + 2] = (byte)newPicKin[x, y].B;
                    //pixelData[index + 3] = color.A; // color.A;
                    index += 4;
                }
            }

            //var bitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixelData, stride);
            wrBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            DiscoverKinectSensor();
            PollColorImageStream();
        }
    }
}
