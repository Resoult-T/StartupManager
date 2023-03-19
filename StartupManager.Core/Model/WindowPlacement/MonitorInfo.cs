using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartupManager.Core.Model.WindowPlacement
{
    /// <summary>
    /// Contains information about a physical monitor.
    /// </summary>
    internal class MonitorInfo
    {
        /// <summary>
        /// Index of this Monitor.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Physical dimensions.
        /// </summary>
        public Rectangle Bounds { get; set; }
        /// <summary>
        /// Dimensions excluding UI elements like Taskbar.
        /// </summary>
        public Rectangle WorkingArea { get; set; }
    }
}
