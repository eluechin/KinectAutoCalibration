using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KinectAutoCalibration.Common.Algorithms;
using KinectAutoCalibration.Common.Interfaces;
using KinectAutoCalibration.Common;

namespace KMeansTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Height = 720
            // Width = 1152
            var circleImage =
                Image.FromFile(
                    "..\\..\\Circle.png", true);

            var bitmap = new Bitmap(circleImage);
            var blackPixels = GetBlackPoints(bitmap);

            var centroid = new Point();
            foreach (var p in blackPixels)
            {
                centroid.X += p.X;
                centroid.Y += p.Y;
            }

            centroid.X = centroid.X / blackPixels.Count;
            centroid.Y = centroid.Y / blackPixels.Count;
            List<IVector> centroidsInit = new List<IVector>();

            //Set Points Random
            //centroidsInit = CreateCentroids(2, bitmap.Width, bitmap.Height);

            centroidsInit.Add(new Vector2D{X=0,Y=0});
            centroidsInit.Add(new Vector2D{X=bitmap.Width-1, Y=0});
            //centroidsInit.Add(new Vector2D{X=0, Y = bitmap});

            List<IPoint2D> centroids = KMeans.DoKMeans(ConvertPointToVector(blackPixels), centroidsInit);

            foreach (var v in centroidsInit)
            {
                bitmap.SetPixel((int)v.X,(int)v.Y, Color.Blue);
            }

            foreach (var point2D in centroids)
            {
                bitmap.SetPixel((int)point2D.X, (int)point2D.Y, Color.Red);
            }

            pictureBox1.Image = bitmap;
        }

        private static List<IPoint2D> GetBlackPoints(Bitmap bmp)
        {
            var blackPixel = new List<IPoint2D>();
            const int corr = 10;
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.R - corr < 0 && c.G - corr < 0 && c.B - corr < 0)
                    {
                        blackPixel.Add(new Point2D() {X = x, Y = y});
                    }
                }
            }
            return blackPixel;
        }

        private List<IVector> ConvertPointToVector(IEnumerable<IPoint2D> points)
        {
            var vectorPoints = new List<IVector>();
            foreach (var p in points)
            {
                vectorPoints.Add(new Vector2D { X = p.X, Y = p.Y });
            }

            return vectorPoints;
        }

        private static List<IVector> CreateCentroids(int centroidCount, int width, int height)
        {
            var vectorCentroids = new List<IVector>();
            var rnd = new Random();
            for (var i = 0; i < centroidCount; i++)
            {
                var x = rnd.Next(0, width - 1);
                var y = rnd.Next(0, height - 1);

                vectorCentroids.Add(new Vector2D { X = x, Y = y });
            }
            return vectorCentroids;
        }
    }
}
