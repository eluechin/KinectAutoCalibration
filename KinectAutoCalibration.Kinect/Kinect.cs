using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Kinect
{
    public class Kinect : IKinect
    {
        internal KinectSensor _kinect;
        internal WriteableBitmap _colorImageBitmap;
        internal Int32Rect _colorImageBitmapRect;
        internal int _colorImageStride;
        internal readonly KinectConverters _kinectConverters;
        private readonly RecoverDepthInformation _recoverDepthInformation;
        private readonly RealWorldCalculation _realWorldCalculation;
        public const int MIN_ELEVATION_ANGLE = -27;
        public const int MAX_ELEVATION_ANGLE = 27;
        public const int KINECT_IMAGE_WIDTH = 640;
        public const int KINECT_IMAGE_HEIGHT = 480;
        public const double WIDTH_CONST = 544.945;
        public const double HEIGHT_CONST = 585.258;
        public const int BYTES_PER_PIXEL = 4;

        public Kinect()
        {
            DiscoverKinectSensor();
            _kinectConverters = new KinectConverters(this);
            _recoverDepthInformation = new RecoverDepthInformation();
            _realWorldCalculation = new RealWorldCalculation();
        }

        /// <summary>
        /// This method must be used to discover a already connected kinect. 
        /// Do not start calling methods which need the KinectCamera without calling this method first!</summary>
        /// <returns>
        /// Returns the discovered KinectSensor-Object if a Kinect is connected. Otherwise "null" will be returned.</returns>
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
            return this._kinect;
        }

        /// <summary>
        /// This method compares two images and generates a difference image.
        /// The difference is calculated only on the basis of the color difference.
        /// The pixels which change from the first image to the second will be black. All other will be white.
        /// So the black pixels are the difference between the two images.</summary>
        /// <param name="image2">second image</param>
        /// <param name="image1">first image</param>
        /// <param name="threshold">the threshold value defines how sensitive the difference between the two pictures are calculated. 
        ///     A small threshold detects pixels with a slightly color change. </param>
        /// <returns>
        /// Returns the difference image as a KinectPoint-array (2-dimensional array).
        /// Can also return "null" if at least one of the passed images is null.</returns>
        public KinectPoint[,] GetDifferenceImage(KinectPoint[,] image2, KinectPoint[,] image1, int threshold)
        {
            if (image1 != null || image2 != null)
            {
                int width = _kinect.ColorStream.FrameWidth;
                int height = _kinect.ColorStream.FrameHeight;

                KinectPoint[,] diffImage = new KinectPoint[width, height];

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        diffImage[x, y] = new KinectPoint { X = x, Y = y };

                        Vector3D vector1 = new Vector3D(image1[x, y].R, image1[x, y].G, image1[x, y].B);
                        Vector3D vector2 = new Vector3D(image2[x, y].R, image2[x, y].G, image2[x, y].B);
                        Vector3D diffVector = (Vector3D)vector1.Subtract(vector2);
                        double length = diffVector.GetLength();

                        if (length > threshold)
                        {
                            diffImage[x, y].B = 0x00;
                            diffImage[x, y].G = 0x00;
                            diffImage[x, y].R = 0x00;
                        }
                        else
                        {
                            diffImage[x, y].B = 0xFF;
                            diffImage[x, y].G = 0xFF;
                            diffImage[x, y].R = 0xFF;
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

        /// <summary>
        /// This method requests an image from the Color Stream of a connected kinect.</summary>
        /// <returns>
        /// returns the retrieved color data as an array of KinectPoints</returns>
        public KinectPoint[,] GetColorImage()
        {
            try
            {
                using (ColorImageFrame frame = this._kinect.ColorStream.OpenNextFrame(10))
                {
                    if (frame != null)
                    {
                        byte[] pixelData1 = new byte[frame.PixelDataLength];
                        frame.CopyPixelDataTo(pixelData1);

                        KinectPoint[,] picKin = _kinectConverters.ConvertByteArrayToKinectPoint(pixelData1, 640, 480);
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

        /// <summary>
        /// This method requests an image from the Depth Stream of a connected kinect</summary>
        /// <returns>
        /// returns the retrieved depth data as a short-array</returns>
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

        /// <summary>
        /// This method requests one image of the color stream and one of the depth stream.
        /// Then it maps the pixels of the depth data with the pixels of the color data</summary>
        /// <returns>
        /// Returns an array which contains the merged color and depth data</returns>
        public KinectPoint[,] CreateKinectPointArray()
        {
            KinectPoint[,] kinArray = new KinectPoint[_kinect.DepthStream.FrameWidth, _kinect.DepthStream.FrameHeight];
            short[] depthPixelData = new short[_kinect.DepthStream.FramePixelDataLength];
            byte[] colorPixelData = new byte[_kinect.ColorStream.FramePixelDataLength];

            try
            {

                using (DepthImageFrame depthFrame = _kinect.DepthStream.OpenNextFrame(0))
                {
                    DepthImagePixel[] depthImagePixelData = new DepthImagePixel[depthPixelData.Length];
                    ColorImagePoint[] colorImagePixelData = new ColorImagePoint[depthFrame.Height * depthFrame.Width];

                    if (depthFrame != null)
                    {
                        byte[] newImage = new byte[depthFrame.Height * depthFrame.Width * 4];
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

                    for (int y = 0; y < depthFrame.Height; ++y)
                    {
                        for (int x = 0; x < depthFrame.Width; ++x)
                        {
                            kinArray[x, y] = new KinectPoint(x, y, -1, 0, 0, 0);
                        }
                    }

                    for (int y = 0; y < depthFrame.Height; ++y)
                    {
                        //for (int x = 0; x < depthFrame.Width; ++x)
                        for (int x = depthFrame.Width - 1; x >= 0; --x)
                        {
                            int depthIndex = x + (y * this._kinect.DepthStream.FrameWidth);
                            ColorImagePoint colorImagePoint = colorImagePixelData[depthIndex];

                            int colorInDepthX = colorImagePoint.X /
                                                (this._kinect.ColorStream.FrameWidth / this._kinect.DepthStream.FrameWidth);
                            int colorInDepthY = colorImagePoint.Y /
                                                (this._kinect.ColorStream.FrameWidth / this._kinect.DepthStream.FrameWidth);

                            if (IsValidKinectPoint(colorInDepthX, colorInDepthY, depthImagePixelData[depthIndex].Depth))
                            {
                                kinArray[(KINECT_IMAGE_WIDTH - 1) - colorInDepthX, colorInDepthY] =
                                    new KinectPoint(colorImagePixelData[depthIndex].X,
                                                    colorImagePixelData[depthIndex].Y,
                                                    depthImagePixelData[depthIndex].Depth,
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4 + 2],
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4 + 1],
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4]);
   
                            }
                            else
                            {
                                kinArray[(KINECT_IMAGE_WIDTH - 1) - colorInDepthX, colorInDepthY] =
                                    new KinectPoint(colorImagePixelData[depthIndex].X,
                                                    colorImagePixelData[depthIndex].Y,
                                                    -1,
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4 + 2],
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4 + 1],
                                                    colorPixelData[
                                                        colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 +
                                                        (colorImagePixelData[depthIndex].X + 1) * 4]);
                            }
                        }
                    }
                }
                kinArray = _recoverDepthInformation.RecoverDepthInformationOfKinectPointArray(kinArray);
                return kinArray;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void RaiseKinect()
        {
            if (_kinect.ElevationAngle != MAX_ELEVATION_ANGLE)
            {
                _kinect.ElevationAngle += 1;
            }
        }

        public void LowerKinect()
        {
            if (_kinect.ElevationAngle != MIN_ELEVATION_ANGLE)
            {
                _kinect.ElevationAngle -= 1;
            }
        }

        public bool IsValidKinectPoint(int colorInDepthX, int colorInDepthY, short depthValue)
        {
            if (colorInDepthX > 0 && colorInDepthX < this._kinect.DepthStream.FrameWidth &&
                colorInDepthY >= 0 && colorInDepthY < this._kinect.DepthStream.FrameHeight)
            {

                if (depthValue != this._kinect.DepthStream.TooNearDepth &&
                    depthValue != this._kinect.DepthStream.TooFarDepth &&
                    depthValue != this._kinect.DepthStream.UnknownDepth)
                {
                    return true;
                }
            }
            return false;
        }

        public WriteableBitmap ConvertKinectPointArrayToWritableBitmap(KinectPoint[,] kinArray, int width, int height)
        {
            return _kinectConverters.ConvertKinectPointArrayToWritableBitmap(kinArray, width, height);
        }


        public byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height)
        {
            return _kinectConverters.ConvertKinectPointArrayToByteArray(kinArray, width, height);
        }


        public Bitmap ConvertKinectPointArrayToBitmap(KinectPoint[,] kinArray, int width, int height)
        {
            return _kinectConverters.ConvertKinectPointArrayToBitmap(kinArray, width, height);
        }

        public Dictionary<KinectPoint,RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints )
        {
            return _realWorldCalculation.CreateRealWorldCoordinates(kinectPoints);
        }

        public Dictionary<KinectPoint, RealWorldPoint> CreateRealWorldCoordinates(List<KinectPoint> kinectPoints, RealWorldPoint rwA, RealWorldPoint rwB, RealWorldPoint rwC)
        {
            return _realWorldCalculation.CreateRealWorldCoordinates(kinectPoints, rwA, rwB, rwC);
        }

        public Vector3D CreateRealWorldVector(RealWorldPoint p)
        {
            return _realWorldCalculation.CreateRealWorldVector(p);
        }
    }
}
