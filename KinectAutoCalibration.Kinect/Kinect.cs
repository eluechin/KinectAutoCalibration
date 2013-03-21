using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using KinectAutoCalibration.Common;
using KinectAutoCalibration.Kinect.Interfaces;

namespace KinectAutoCalibration.Kinect
{
    public class Kinect : IKinect
    {
        private KinectSensor _kinect;
        private WriteableBitmap _colorImageBitmap;
        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;

        public KinectSensor DiscoverKinectSensor()
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

                    return this._kinect;
                }
            }
            return null;
        }

        public KinectPoint[,] GetDifferenceImage(KinectPoint[,] picture2, KinectPoint[,] picture1, int tolerance)
        {
            if (picture1 != null || picture2 != null)
            {


                int width = _kinect.ColorStream.FrameWidth;
                int height = _kinect.ColorStream.FrameHeight;

                KinectPoint[,] diffImage = new KinectPoint[width,height];


                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        diffImage[x,y] = new KinectPoint();

                        //Variante 1: (Hacking the Kinect) 
                        /*
                        if ((picture2[x, y].R + tolerance <= picture1[x, y].R || picture2[x, y].R - tolerance >= picture1[x, y].R) &&
                            (picture2[x, y].G + tolerance <= picture1[x, y].G || picture2[x, y].G - tolerance >= picture1[x, y].G) &&
                            (picture2[x, y].B + tolerance <= picture1[x, y].B || picture2[x, y].B - tolerance >= picture1[x, y].B))
                        { */


                            diffImage[x, y].B = 0x00; //Blue
                            diffImage[x, y].G = 0x00; //Green
                            diffImage[x, y].R = 0x00; //Red
                        }
                        else
                        {
                            diffImage[x, y].B = 0xFF; //Blue
                            diffImage[x, y].G = 0xFF; //Green
                            diffImage[x, y].R = 0xFF; //Red
                        }
                    }
                }

                return diffImage;
            }
            else
            {
                return null;
            }
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
                                            (byte)bytearray[bitPositionByteArray + 2],
                                            (byte)bytearray[bitPositionByteArray + 1],
                                            (byte)bytearray[bitPositionByteArray]);
                    bitPositionByteArray += BytesPerPixel;
                }
            }
            return kinectArray;
        }


        public KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y)
        {
            KinectPoint kinPoint = new KinectPoint();

            kinPoint = kinArray[x, y];

            return kinPoint;
        }


        public KinectPoint[,] GetColorImage()
        {

            try
            {

                using (ColorImageFrame frame = this._kinect.ColorStream.OpenNextFrame(1000))
                {
                    if (frame != null)
                    {
                        byte[] pixelData1 = new byte[frame.PixelDataLength];
                        frame.CopyPixelDataTo(pixelData1);

                        //KinectPoint[,] kinArray = ConvertToKinectPoint(pixelData1, frame.Width, frame.Height);

                        KinectPoint[,] picKin = ConvertToKinectPoint(pixelData1, 640, 480);
                        return picKin;
                    }

                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }

        }

        public short[] GetDepthImage()
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

            catch (Exception)
            {
                return null;
            }
        }


        public KinectPoint[,] GetDepthAndColorImage()
        {
            KinectPoint[,] kinArray = new KinectPoint[_kinect.DepthStream.FrameWidth, _kinect.DepthStream.FrameHeight];
            short[] depthPixelData = new short[_kinect.DepthStream.FramePixelDataLength];
            byte[] colorPixelData = new byte[_kinect.ColorStream.FramePixelDataLength];

            try
            {

                using (DepthImageFrame depthFrame = _kinect.DepthStream.OpenNextFrame(0))
                {
                    DepthImagePixel[] depthImagePixelData = new DepthImagePixel[depthPixelData.Length];
                    ColorImagePoint[] colorImagePixelData = new ColorImagePoint[depthFrame.Height*depthFrame.Width];

                    if (depthFrame != null)
                    {
                        byte[] newImage = new byte[depthFrame.Height*depthFrame.Width*4];
                        //int newImageIndex = 0;

                        depthFrame.CopyPixelDataTo(depthPixelData);

                        depthFrame.CopyDepthImagePixelDataTo(depthImagePixelData);

                        _kinect.CoordinateMapper.MapDepthFrameToColorFrame(depthFrame.Format, depthImagePixelData,
                                                                           _kinect.ColorStream.Format,
                                                                           colorImagePixelData);

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
                            int depthIndex = x + (y*this._kinect.DepthStream.FrameWidth);
                            ColorImagePoint colorImagePoint = colorImagePixelData[depthIndex];

                            int colorInDepthX = colorImagePoint.X/
                                                (this._kinect.ColorStream.FrameWidth/this._kinect.DepthStream.FrameWidth);
                            int colorInDepthY = colorImagePoint.Y/
                                                (this._kinect.ColorStream.FrameWidth/this._kinect.DepthStream.FrameWidth);

                            if (colorInDepthX > 0 && colorInDepthX < this._kinect.DepthStream.FrameWidth &&
                                colorInDepthY >= 0 && colorInDepthY < this._kinect.DepthStream.FrameHeight)
                            {
                                kinArray[colorInDepthX, colorInDepthY] =
                                    new KinectPoint(colorImagePixelData[depthIndex].X,
                                                    colorImagePixelData[depthIndex].Y,
                                                    depthImagePixelData[depthIndex].Depth,
                                                    colorPixelData[colorImagePixelData[depthIndex].Y*depthFrame.Width*4 + (colorImagePixelData[depthIndex].X + 1)*4 + 2],
                                                    colorPixelData[colorImagePixelData[depthIndex].Y*depthFrame.Width*4 + (colorImagePixelData[depthIndex].X + 1)*4 + 1],
                                                    colorPixelData[colorImagePixelData[depthIndex].Y*depthFrame.Width*4 + (colorImagePixelData[depthIndex].X + 1)*4]);
                                ++index;
                            }
                        }

                    }
                }
                return kinArray;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public WriteableBitmap PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height)
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
                    if (newPicKin[x, y] != null)
                    {
                        pixelData[index + 2] = (byte)newPicKin[x, y].R;
                        pixelData[index + 1] = (byte)newPicKin[x, y].G;
                        pixelData[index] = (byte)newPicKin[x, y].B;
                        //pixelData[index + 3] = color.A; // color.A;

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

            this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
            return this._colorImageBitmap;
        }


        public void PrintKinectPointArray(KinectPoint[,] newPicKin, int width, int height, WriteableBitmap wrBitmap)
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

            wrBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
        }


    }
}
