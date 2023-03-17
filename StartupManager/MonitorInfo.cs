using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StartupManager
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
        public Rect Bounds { get; set; }
        /// <summary>
        /// Dimensions excluding UI elements like Taskbar.
        /// </summary>
        public Rect WorkingArea { get; set; }
    }
}
