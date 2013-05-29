using System;
using System.Linq;
using System.Windows.Forms;

namespace KinectAutoCalibration.Beamer
{
    /// <summary>
    /// This class is a representation of the beamer (usually on the second screen).
    /// </summary>
    internal static class Beamer
    {
        private static Screen screen;

        static Beamer()
        {
            GetBeamerScreen();
        }

        /// <summary>
        /// Get the width of the beamer depending on the resolution.
        /// </summary>
        /// <returns>integer with the width of the beamer</returns>
        public static int GetBeamerWidth()
        {
            return screen.Bounds.Size.Width;
        }

        /// <summary>
        /// Get the height of the beamer depending on the resolution.
        /// </summary>
        /// <returns>integer with the height of the beamer</returns>
        public static int GetBeamerHeight()
        {
            return screen.Bounds.Size.Height;
        }

        /// <summary>
        /// Get the first coordinate where the beamer window starts.
        /// </summary>
        /// <returns>integer with the width coordinate of the beamer window</returns>
        public static int GetBeamerTopPosition()
        {
            return screen.Bounds.Top;
        }

        /// <summary>
        /// Get the first coordinate where the beamer window starts.
        /// </summary>
        /// <returns>integer with the height coordinate of the beamer window</returns>
        public static int GetBeamerLeftPosition()
        {
            return screen.Bounds.Left;
        }

        /// <summary>
        /// Checks first if there is a second Screen. Throws an exception if there is no one found.
        /// </summary>
        private static void GetBeamerScreen()
        {
            if (Screen.AllScreens.Count() > 1)
            {
                screen = Screen.AllScreens[1];
            }
            else
            {
                throw new Exception("No Beamer found");
            }
        }
    }
}
