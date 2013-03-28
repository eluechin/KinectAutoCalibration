using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common.Algorithms;
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

        /// <summary>
        /// This method must be used to discover a already connected kinect.</summary>
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
            return null;
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


                        //Variante 1: (Hacking the Kinect) 
                        /*
                        if ((image2[x, y].R + threshold <= image1[x, y].R || image2[x, y].R - threshold >= image1[x, y].R) &&
                            (image2[x, y].G + threshold <= image1[x, y].G || image2[x, y].G - threshold >= image1[x, y].G) &&
                            (image2[x, y].B + threshold <= image1[x, y].B || image2[x, y].B - threshold >= image1[x, y].B))
                        { */

                        //Variante 2: Vektor-Differenz
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
        /// This method converts a byte array into an array of KinectPoints</summary>
        /// <param name="bytearray">the array which should be converted</param>
        /// <param name="width">the width of the new 2D-array, e.g. 640</param>
        /// <param name="height">the height of the new 2D-array, e.g. 480</param>
        /// <returns>Returns the converted byte array as an array of KinectPoints</returns>
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


        /// <summary>
        /// This method is used to get a specific x/y-point out of a KinectPoint-array.</summary>
        /// <param name="kinArray">the array where the needed point is contained in.</param>
        /// <param name="x">x-coordinate of the needed point</param>
        /// <param name="y">y-coordinate of the needed point</param>
        /// <returns>Returns the requested KinectPoint</returns>
        public KinectPoint GetKinectPoint(KinectPoint[,] kinArray, int x, int y)
        {
            //TODO: Exception Handling

            KinectPoint kinPoint = new KinectPoint();

            kinPoint = kinArray[x, y];

            return kinPoint;
        }


        /// <summary>
        /// This method requests an image from the Color Stream of a connected kinect</summary>
        /// <returns>
        /// returns the retrieved color data as an array of KinectPoints</returns>
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
                    ColorImagePoint[] colorImagePixelData = new ColorImagePoint[depthFrame.Height * depthFrame.Width];

                    if (depthFrame != null)
                    {
                        byte[] newImage = new byte[depthFrame.Height * depthFrame.Width * 4];
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
                            int depthIndex = x + (y * this._kinect.DepthStream.FrameWidth);
                            ColorImagePoint colorImagePoint = colorImagePixelData[depthIndex];

                            int colorInDepthX = colorImagePoint.X /
                                                (this._kinect.ColorStream.FrameWidth / this._kinect.DepthStream.FrameWidth);
                            int colorInDepthY = colorImagePoint.Y /
                                                (this._kinect.ColorStream.FrameWidth / this._kinect.DepthStream.FrameWidth);

                            if (colorInDepthX > 0 && colorInDepthX < this._kinect.DepthStream.FrameWidth &&
                                colorInDepthY >= 0 && colorInDepthY < this._kinect.DepthStream.FrameHeight)
                            {
                                kinArray[colorInDepthX, colorInDepthY] =
                                    new KinectPoint(colorImagePixelData[depthIndex].X,
                                                    colorImagePixelData[depthIndex].Y,
                                                    depthImagePixelData[depthIndex].Depth,
                                                    colorPixelData[colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 + (colorImagePixelData[depthIndex].X + 1) * 4 + 2],
                                                    colorPixelData[colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 + (colorImagePixelData[depthIndex].X + 1) * 4 + 1],
                                                    colorPixelData[colorImagePixelData[depthIndex].Y * depthFrame.Width * 4 + (colorImagePixelData[depthIndex].X + 1) * 4]);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
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

        public List<Vector3D> GetCornerPoints(KinectPoint[,] diffImage)
        {
            List<Vector3D> corners = new List<Vector3D>();
            var centroids = KMeans.DoKMeans(KMeansHelper.ExtractBlackPointsAs2dVector(diffImage), KMeansHelper.CreateInitialCentroids(640, 480));

            foreach (var vectorCentroid in centroids)
            {
                var p = diffImage[(int)vectorCentroid.X, (int)vectorCentroid.Y];
                corners.Add(new Vector3D{X = p.X, Y = p.Y, Z = p.Z});
            }

            return corners;
        }
    }
}
