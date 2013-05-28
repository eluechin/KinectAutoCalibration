using System.Drawing;
using System.Windows.Media.Imaging;
using KinectAutoCalibration.Common;

namespace KinectAutoCalibration.Kinect
{
    public class KinectConverters
    {
        private Kinect _kinect;

        public KinectConverters(Kinect kinect)
        {
            _kinect = kinect;
        }

        /// <summary>
        /// This method is used to convert an array of KinectPoints to a WriteableBitmap.</summary>
        /// <param name="kinArray">the array that should be converted</param>
        /// <param name="width">the width of the passed two-dimensional array, e.g. 640</param>
        /// <param name="height">the height of the passed two-dimensional array, e.g. 480</param>
        /// <returns>Returns the passed array written to a Bitmap ready to use it in a WPF- or WinForms-Project</returns>
        public WriteableBitmap ConvertKinectPointArrayToWritableBitmap(KinectPoint[,] kinArray, int width, int height)
        {
            var stride = width * Kinect.BYTES_PER_PIXEL;

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
                    index += Kinect.BYTES_PER_PIXEL;
                }
            }

            _kinect._colorImageBitmap.WritePixels(_kinect._colorImageBitmapRect, pixelData, _kinect._colorImageStride, 0);
            return _kinect._colorImageBitmap;
        }

        /// <summary>
        /// This method is used to convert an array of KinectPoints to a ByteArray.</summary>
        /// <param name="kinArray">the array that should be converted</param>
        /// <param name="width">the width of the passed two-dimensional array, e.g. 640</param>
        /// <param name="height">the height of the passed two-dimensional array, e.g. 480</param>
        /// <returns>Returns the passed array as a one-dimensional array of bytes</returns>
        public byte[] ConvertKinectPointArrayToByteArray(KinectPoint[,] kinArray, int width, int height)
        {
            var stride = width * Kinect.BYTES_PER_PIXEL;

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
                       
                    } else {
                        pixelData[index + 2] = 0xFF;
                        pixelData[index + 1] = 0x00;
                        pixelData[index] = 0x00;
                    }
                    
                    index += Kinect.BYTES_PER_PIXEL;
                }
            }

            return pixelData;
        }

        /// <summary>
        /// This method is used to convert an array of KinectPoints to a Bitmap.</summary>
        /// <param name="kinArray">the array that should be converted</param>
        /// <param name="width">the width of the passed two-dimensional array, e.g. 640</param>
        /// <param name="height">the height of the passed two-dimensional array, e.g. 480</param>
        /// <returns>Returns the passed array as a bitmap ready to use for drawing</returns>
        public Bitmap ConvertKinectPointArrayToBitmap(KinectPoint[,] kinArray, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
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
        /// This method converts a byte array into an array of KinectPoints</summary>
        /// <param name="bytearray">the array which should be converted</param>
        /// <param name="width">the width of the new two-dimensional array, e.g. 640</param>
        /// <param name="height">the height of the new two-dimensional array, e.g. 480</param>
        /// <returns>Returns the converted byte array as an array of KinectPoints</returns>
        public KinectPoint[,] ConvertByteArrayToKinectPoint(byte[] bytearray, int width, int height)
        {
            int bitPositionByteArray = 0;
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
                    bitPositionByteArray += Kinect.BYTES_PER_PIXEL;
                }
            }
            return kinectArray;
        }

        /// <summary>
        /// This method is used to reflect the picture seen by the kinect on the y-axis.
        /// So for example the Point (0,0) becomes the Point (639,0).
        /// The y-values do not change. </summary>
        /// <param name="kinArray">the array which should be transformed</param>
        /// <param name="width">the width of the new two-dimensional array, e.g. 640</param>
        /// <param name="height">the height of the new two-dimensional array, e.g. 480</param>
        /// <returns>Returns the flipped KinectPoint-Array</returns>
        public KinectPoint[,] FlipArray(KinectPoint[,] kinArray, int width, int height)
        {
            var newKinArray = new KinectPoint[width,height];

            for (int y = 0; y < height; ++y)
            {
                for (int x = (width - 1); x >= 0; --x)
                {
                    newKinArray[x, y] = kinArray[((width - 1) - x), y];
                }
            }

            return newKinArray;
        } 
    }
}