using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KinectAutoCalibration.Beamer
{
    internal static class BeamerImage
    {
        // ReSharper disable InconsistentNaming
        public static readonly int TILE_WIDTH = 70;
        public static readonly int TILE_HEIGHT = 70;
        // ReSharper restore InconsistentNaming

        public static Canvas CreateCalibImageEdge(int beamerWidth, int beamerHeight, bool isInverted)
        {
            var imageCanvas = new Canvas
            {
                Height = beamerHeight,
                Width = beamerWidth,
                Background = new SolidColorBrush(Colors.Black)
            };
            var rightOffset = RightOffset(beamerWidth);
            var topOffset = TopOffset(beamerHeight);

            var topLeft = CreateRectangleGroup(0, 0, isInverted);
            var topRight = CreateRectangleGroup(rightOffset, 0, isInverted);
            var botRight = CreateRectangleGroup(rightOffset, topOffset, isInverted);
            var botLeft = CreateRectangleGroup(0, topOffset, isInverted);

            var allElements = topLeft.Union(topRight).Union(botRight).Union(botLeft).ToList();
            foreach (var e in allElements)
            {
                imageCanvas.Children.Add(e);
            }

            return imageCanvas;
        }

        public static Canvas CreateCalibImage(int beamerWidth, int beamerHeight, bool isInverted, int round)
        {
            var imageCanvas = new Canvas
            {
                Height = beamerHeight,
                Width = beamerWidth,
                Background = new SolidColorBrush(Colors.Black)
            };
            var divisorArray = new []{1,2,4,6};
            var divisor = divisorArray[round-1];
            var width = beamerWidth/divisor;
            var height = beamerHeight/divisor;
            for (var x = 0; x < round; x++)
            {
                for (var y = 0; y < round; y++)
                {
                    var allElements = new List<Rectangle>();
                    if (round == 1)
                    {
                         allElements.AddRange(CreateCalibRectangles(width , height, x, y, isInverted));
                    }
                    else
                    {
                        var specificWidth = width + TILE_WIDTH;
                        var specifcHeight = height + TILE_HEIGHT;
                        var specificOffsetLeft = x*specificWidth - 2*TILE_WIDTH;
                        var specificOffsetTop = y*specifcHeight - 2*TILE_HEIGHT;
                        if (specificOffsetLeft < 0)
                            specificOffsetLeft = 0;
                       if (specificOffsetTop < 0)
                            specificOffsetTop = 0;
                       allElements.AddRange(CreateCalibRectangles(specificWidth, specifcHeight, specificOffsetLeft, specificOffsetTop, isInverted));
                    }
                    
                    foreach (var e in allElements)
                    {
                        imageCanvas.Children.Add(e);
                    } 
                }
            }

            return imageCanvas;
        }

        public static Canvas CreateAreaImage(int beamerWidth, int beamerHeight, List<Point2D> objects)
        {
            var imageCanvas = new Canvas
            {
                Height = beamerHeight,
                Width = beamerWidth,
                Background = new SolidColorBrush(Colors.Black)
            };

            foreach (var element in objects)
            {
                var point = new Ellipse
                    {
                        Width = 1, 
                        Height = 1, 
                        Fill = new SolidColorBrush(Colors.White)
                    };
                Canvas.SetLeft(point, element.X);
                Canvas.SetTop(point, element.Y);
                imageCanvas.Children.Add(point);
            }

            return imageCanvas;
        }

        private static IEnumerable<Rectangle> CreateRectangleGroup(int leftPosition, int topPosition, bool isInverted)
        {
            var rectangleGroup = new List<Rectangle>();

            Color c1;
            Color c2;
            if (isInverted)
            {
                c1 = Colors.Black;
                c2 = Colors.White;
            }
            else
            {
                c1 = Colors.White;
                c2 = Colors.Black;
            }
            rectangleGroup.Add(CreateRectangle(leftPosition, topPosition, c1));
            rectangleGroup.Add(CreateRectangle(leftPosition + TILE_WIDTH, topPosition, c2));
            rectangleGroup.Add(CreateRectangle(leftPosition + TILE_WIDTH, topPosition + TILE_HEIGHT, c1));
            rectangleGroup.Add(CreateRectangle(leftPosition, topPosition + TILE_HEIGHT, c2));

            return rectangleGroup;
        }

        private static Rectangle CreateRectangle(int leftPosition, int topPosition, Color color)
        {
            var rectangle = new Rectangle
                {
                    Width = TILE_WIDTH,
                    Height = TILE_HEIGHT,
                    Fill = new SolidColorBrush { Color = color }
                };
            Canvas.SetLeft(rectangle, leftPosition);
            Canvas.SetTop(rectangle, topPosition);
            return rectangle;
        }

        private static int TopOffset(int beamerHeight)
        {
            return beamerHeight - 2 * TILE_HEIGHT;
        }

        private static int RightOffset(int beamerWidth)
        {
            return beamerWidth - 2 * TILE_WIDTH;
        }

        private static IEnumerable<Rectangle> CreateCalibRectangles(int width, int height, int leftOffset, int topOffset, bool isInverted)
        {
            var top = CreateRectangleGroup(leftOffset + (width / 2 - TILE_WIDTH), topOffset, isInverted);
            var left = CreateRectangleGroup(leftOffset, topOffset + (height / 2 - TILE_WIDTH), isInverted);
            var middle = CreateRectangleGroup(leftOffset + (width / 2 - TILE_WIDTH), topOffset + (height / 2 - TILE_WIDTH), isInverted);
            var right = CreateRectangleGroup(leftOffset + (width - 2 * TILE_WIDTH),topOffset + (height / 2 - TILE_WIDTH), isInverted);
            var bottom = CreateRectangleGroup(leftOffset + (width / 2 - TILE_WIDTH), topOffset + (height - 2 * TILE_WIDTH), isInverted);

            return top.Union(left).Union(middle).Union(right).Union(bottom).ToList();
        }


    }
}
