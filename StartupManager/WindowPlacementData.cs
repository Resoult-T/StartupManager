using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartupManager
{

    /// <summary>
    /// Contains information about the size and position of a window, including virtual positioning.
    /// </summary>
    [Serializable]
    internal class WindowPlacementData
    {
        /// <summary>
        /// Screen coordinate from left to right in pixels.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Screen coordinate from top to bottom in pixels.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Window size from left to right in pixels.
        /// </summary>
        public int CX { get; set; }
        /// <summary>
        /// Window size from top to bottom in pixels.
        /// </summary>
        public int CY { get; set; }

        /// <summary>
        /// Defines the screen on wich the UI parameeters are applied.
        /// </summary>
        public int ScreenIndex { get; set; }

        /// <summary>
        /// Gets the virtaul Point of the top left Window Position.
        /// </summary>
        public Point VirtualWindowPosition
        {
            get
            {
                return GetVirtualWindowPosition();
            }
        }

        /// <summary>
        /// Creates an object that contaisn informaton about a windows positioning.
        /// </summary>
        /// <param name="x">Window placement point in pixels counting from top</param>
        /// <param name="y">Window placement point in pixels counting from left</param>
        /// <param name="cX">Window size in X axis</param>
        /// <param name="cY">Window size in Y axis</param>
        /// <param name="screenIndex">The screen index on which these settings apply</param>
        public WindowPlacementData(int x, int y, int cX, int cY, int screenIndex)
        {
            X = x;
            Y = y;
            CX = cX;
            CY = cY;
            ScreenIndex = screenIndex;
        }


        /// <summary>
        /// Uses the ScreenIndex and positioning data to create a point containing the virtual position of this window.
        /// </summary>
        /// <returns></returns>
        private Point GetVirtualWindowPosition()
        {
            int screenNumber = ScreenIndex;

            MonitorInfo[] monitorInfo = VirtualScreenHelper.GetMonitorsInfo();

            if (monitorInfo.Length < ScreenIndex + 1)
                screenNumber = 0;

            var origin = monitorInfo[screenNumber].Bounds.TopLeft;

            origin.X += this.X;
            origin.Y += this.Y;

            return origin;
        }
    }
}
