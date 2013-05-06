﻿using System.Collections.Generic;
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

        private static int TopOffset(int beamerHeight)
        {
            return beamerHeight - 2 * TILE_HEIGHT;
        }

        private static int RightOffset(int beamerWidth)
        {
            return beamerWidth - 2 * TILE_WIDTH;
        }

        public static Canvas CreateCalibImage(int beamerWidth, int beamerHeight, bool isInverted, int depth)
        {
            var imageCanvas = new Canvas
            {
                Height = beamerHeight,
                Width = beamerWidth,
                Background = new SolidColorBrush(Colors.Black)
            };

            var allElements = CreateCalibRectangles(beamerWidth / depth, beamerHeight / depth, isInverted);
            foreach (var e in allElements)
            {
                imageCanvas.Children.Add(e);
            }

            return imageCanvas;
        }

        private static IEnumerable<Rectangle> CreateCalibRectangles(int width, int height, bool isInverted)
        {
            var top = CreateRectangleGroup(width / 2 - TILE_WIDTH, 0, isInverted);
            var left = CreateRectangleGroup(0, height / 2 - TILE_WIDTH, isInverted);
            var middle = CreateRectangleGroup(width / 2 - TILE_WIDTH, height / 2 - TILE_WIDTH, isInverted);
            var right = CreateRectangleGroup(width - 2 * TILE_WIDTH, height / 2 - TILE_WIDTH, isInverted);
            var bottom = CreateRectangleGroup(width / 2 - TILE_WIDTH, height - 2 * TILE_WIDTH, isInverted);

            return top.Union(left).Union(middle).Union(right).Union(bottom).ToList();
        }
    }
}
