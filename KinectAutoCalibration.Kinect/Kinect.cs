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

namespace KinectAutoCalibration.Kinect
{
    public class Kinect : IKinect
    {
        private KinectSensor _kinect;
        private WriteableBitmap _colorImageBitmap;
        private Int32Rect _colorImageBitmapRect;
        private int _colorImageStride;
        private const int MIN_ELEVATION_ANGLE = -27;
        private const int MAX_ELEVATION_ANGLE = 27;
        private const int KINECT_IMAGE_WIDTH = 640;
        private const int KINECT_IMAGE_HEIGHT = 480;
        private const double WIDTH_CONST = 544.945;
        private const double HEIGHT_CONST = 585.258;
        private const int BYTES_PER_PIXEL = 4;

        public Kinect()
        {
            DiscoverKinectSensor();
        }

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

                for (int x = (width - 1); x >= 0; --x)
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
            if ((x >= 0 || x < KINECT_IMAGE_WIDTH) &&
                (y >= 0 || y < KINECT_IMAGE_HEIGHT))
            {
                KinectPoint kinPoint = new KinectPoint();
                kinPoint = kinArray[x, y];

                return kinPoint;
            }
            else
            {
                return null;
            }
        }


        public byte[] GetColorImageAsByteArray()
        {
            try
            {

                using (ColorImageFrame frame = this._kinect.ColorStream.OpenNextFrame(10))
                {
                    if (frame != null)
                    {
                        byte[] pixelData = new byte[frame.PixelDataLength];
                        frame.CopyPixelDataTo(pixelData);


                        return pixelData;
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
        /// This method requests an image from the Color Stream of a connected kinect</summary>
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
                //kinArray = RecoverDepthInformationOfKinectPointArray(kinArray);
                return kinArray;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// This method is used to convert an array of KinectPoints to a WriteableBitmap.</summary>
        /// <param name="kinArray">the array that should be printed</param>
        /// <param name="width">the width of the passed array, e.g. 640</param>
        /// <param name="height">the height of the passed array, e.g. 480</param>
        /// <returns>Returns the passed array written to a Bitmap ready to use it in a WPF- or WinForms-Project</returns>
        public WriteableBitmap ConvertKinectPointArrayToWritableBitmap(KinectPoint[,] kinArray, int width, int height)
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
                    if (kinArray[x, y].Z != -1)
                    {
                        pixelData[index + 2] = (byte)kinArray[x, y].R;
                        pixelData[index + 1] = (byte)kinArray[x, y].G;
                        pixelData[index] = (byte)kinArray[x, y].B;
                        //pixelData[index + 3] = color.A; // color.A;

                    }
                    else
                    {
                        pixelData[index + 2] = 0xFF;
                        pixelData[index + 1] = 0x00;
                        pixelData[index] = 0x00;
                    }
                    index += BYTES_PER_PIXEL;
                }
            }

            this._colorImageBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
            return this._colorImageBitmap;
        }


        public byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height)
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
                    if (kinArray[x, y].Z != -1)
                    {
                        pixelData[index + 2] = (byte)kinArray[x, y].R;
                        pixelData[index + 1] = (byte)kinArray[x, y].G;
                        pixelData[index] = (byte)kinArray[x, y].B;
                        //pixelData[index + 3] = color.A; // color.A;

                    } else {
                        pixelData[index + 2] = 0xFF;
                        pixelData[index + 1] = 0x00;
                        pixelData[index] = 0x00;
                    }
                    
                    index += BYTES_PER_PIXEL;
                }
            }

            return pixelData;
        }


        public Bitmap ConvertKinectPointArrayToBitmap(KinectPoint[,] kinArray, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //var color = colorArray[y, x];
                    //var index = (y * stride) + (x * 4);
                    KinectPoint p = kinArray[x, y];
                    if (p.Z != -1)
                    {
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(0, p.R, p.G, p.B);
                        bmp.SetPixel(p.X, p.Y, c);

                    }
                }
            }
            return bmp;
        }


        /// <summary>
        /// This method prints the image information of an array of KinectPoints directly to a bitmap.</summary>
        /// <param name="kinArray">the array that should be printed</param>
        /// <param name="width">the width of the passed array, e.g. 640</param>
        /// <param name="height">the height of the passed array, e.g. 480</param>
        /// <param name="wrBitmap">the bitmap to which the passed array should be printed to</param>
        public void PrintKinectPointArray(KinectPoint[,] kinArray, int width, int height, WriteableBitmap wrBitmap)
        {
            var stride = width * 4; // bytes per row

            byte[] pixelData = new byte[height * stride];
            int index = 0;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (kinArray[x, y].Z != -1)
                    {
                        pixelData[index + 2] = (byte)kinArray[x, y].R;
                        pixelData[index + 1] = (byte)kinArray[x, y].G;
                        pixelData[index] = (byte)kinArray[x, y].B;
                    }
                    else
                    {
                        pixelData[index + 2] = 0xFF;
                        pixelData[index + 1] = 0x00;
                        pixelData[index] = 0x00;
                    }
                    index += BYTES_PER_PIXEL;
                }
            }

            wrBitmap.WritePixels(this._colorImageBitmapRect, pixelData, this._colorImageStride, 0);
        }

        /// <summary>
        /// TO DO</summary>
        /// <param name="kinArray">TO DO</param>
        /// <returns>TO DO</returns>
        public RealWorldPoint[,] CreateRealWorldArray(KinectPoint[,] kinArray, int width, int height)
        {
            RealWorldPoint[,] rwArray = new RealWorldPoint[640, 480];
            var a = new KinectPoint(0, 0, 0, 0, 0);
            var b = new KinectPoint(1, 1, 0, 0, 0);
            var c = new KinectPoint(2, 2, 0, 0, 0);

            var rwA = CalculateRealWorldPoint(a);
            var rwB = CalculateRealWorldPoint(b);
            var rwC = CalculateRealWorldPoint(c);
                
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    //var rwPoint = CalculateRealWorldPoint(kinArray[x, y], rwA, rwB, rwC);
                    var rwPoint = CalculateRealWorldPoint(kinArray[x, y]);
                    rwArray[x, y] = rwPoint;
                }
            }
            return rwArray;
        }

        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p)
        {
            var rwPoint = new RealWorldPoint();

            rwPoint.Z = p.Z;
            rwPoint.X = (int) ((p.X - (KINECT_IMAGE_WIDTH/2))* rwPoint.Z/WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z / HEIGHT_CONST);

            return rwPoint;
        }

        private RealWorldPoint CalculateRealWorldPoint(KinectPoint p, RealWorldPoint a, RealWorldPoint b, RealWorldPoint c)
        {
            var rwPoint = new RealWorldPoint();

            //RWPoint.Z = kinectPoint.Z;

            var numerator = 2*(a.Z * b.Y * c.X - a.Y * b.Z * c.X - a.Z * b.X * c.Y + a.X * b.Z * c.Y + a.Y * b.X * c.Z - a.X * b.Y * c.Z) * HEIGHT_CONST * WIDTH_CONST;
            var denominator = ((a.Z*(b.X - c.X) + b.Z*c.X - b.X*c.Z + a.X*(-b.Z + c.Z))*(KINECT_IMAGE_HEIGHT - 2*p.Y)*WIDTH_CONST + HEIGHT_CONST*((-a.Y*b.Z + a.Z*(b.Y-c.Y) + b.Z*c.Y + a.Y*c.Z*-b.Y*c.Z) * (2*p.X - KINECT_IMAGE_WIDTH) + 2*(-a.X*b.Y + a.Y*(b.X-c.X) + b.Y*c.X + a.X*c.Y - b.X*c.Y)*WIDTH_CONST));
            //var numerator = 1;
            //var denominator = 1;

            rwPoint.Z = (int) (numerator/denominator);
            rwPoint.X = (int) ((p.X - (KINECT_IMAGE_WIDTH/2))*rwPoint.Z/WIDTH_CONST);
            rwPoint.Y = (int)((p.Y - (KINECT_IMAGE_HEIGHT / 2)) * rwPoint.Z / HEIGHT_CONST);

            return rwPoint;
        }

        public Vector3D CreateRealWorldVector(RealWorldPoint p)
        {
            return new Vector3D { X = p.X, Y = p.Y, Z = p.Z };
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

        public KinectPoint[,] RecoverDepthInformationOfKinectPointArray(KinectPoint[,] kinArray)
        {
            var recoveredKinArray = new KinectPoint[KINECT_IMAGE_WIDTH,KINECT_IMAGE_HEIGHT];

            for (int y = 0; y < KINECT_IMAGE_HEIGHT; ++y)
            {
                for (int x = 0; x < KINECT_IMAGE_WIDTH; ++x)
                {
                    if (kinArray[x,y].Z == -1)
                    {
                        int correctionRadius = 1;
                        int z = -1;
                        int depthMeanValue = 0;

                        do
                        {
                            var neighbors = GetNeighborsOfKinectPoint(kinArray, kinArray[x,y], correctionRadius);
                            depthMeanValue = CalculateDepthMeanValue(neighbors);

                            ++correctionRadius;

                        } while (depthMeanValue == 0);

                        kinArray[x,y].Z = depthMeanValue;
                        recoveredKinArray[x,y] = kinArray[x,y];

                    }
                    else
                    {
                        recoveredKinArray[x, y] = kinArray[x, y];
                    }
                }
            }
   
            return recoveredKinArray;
        } 

        public List<KinectPoint> GetNeighborsOfKinectPoint(KinectPoint[,] kinArray, KinectPoint kinPoint, int correctionRadius)
        {
            var neighbors = new List<KinectPoint>();

            for (int i = correctionRadius; i >= -correctionRadius; --i)
            {
                for (int j = correctionRadius; j >= -correctionRadius; --j)
                {
                    int x = kinPoint.X + i;
                    int y = kinPoint.Y + j;

                    if ((x >= 0 || x < KINECT_IMAGE_WIDTH) &&
                        (y >= 0 || y < KINECT_IMAGE_HEIGHT))
                    {
                        neighbors.Add(kinArray[x, y]);
                    }
                }
            }
            return neighbors;
        }

        public int CalculateDepthMeanValue(List<KinectPoint> neighbors)
        {
            int sum = 0;
            int validNeighbors = 0;

            foreach (var kinPoint in neighbors)
            {
                if (kinPoint.Z != -1)
                {
                    sum += kinPoint.Z;
                    ++validNeighbors;
                }

            }
            return (int)(sum / validNeighbors);
        }
    }
}
