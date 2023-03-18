using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace StartupManager
{
    internal class VirtualScreenHelper
    {
        
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        private delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Provides information about each monitor that is physically connected to the PC.
        /// </summary>
        /// <returns>A list of MonitorInfo objects</returns>
        /// <exception cref="ApplicationException"></exception>
        public static MonitorInfo[] GetMonitorsInfo()
        {
            var monitorInfoList = new List<MonitorInfo>();
            var callback = new MonitorEnumDelegate((IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
            {
                var monitorInfo = new MonitorInfo();
                var monitorInfoEx = new MONITORINFOEX();
                monitorInfoEx.cbSize = Marshal.SizeOf(monitorInfoEx);
                if (GetMonitorInfo(hMonitor, ref monitorInfoEx))
                {
                    monitorInfo.Index = monitorInfoList.Count;
                    monitorInfo.Bounds = new Rect(monitorInfoEx.rcMonitor.Left, monitorInfoEx.rcMonitor.Top,
                        monitorInfoEx.rcMonitor.Right - monitorInfoEx.rcMonitor.Left,
                        monitorInfoEx.rcMonitor.Bottom - monitorInfoEx.rcMonitor.Top);
                    monitorInfo.WorkingArea = new Rect(monitorInfoEx.rcWork.Left, monitorInfoEx.rcWork.Top,
                        monitorInfoEx.rcWork.Right - monitorInfoEx.rcWork.Left,
                        monitorInfoEx.rcWork.Bottom - monitorInfoEx.rcWork.Top);
                    monitorInfoList.Add(monitorInfo);
                }

                return true;
            });

            if (!EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero))
            {
                throw new ApplicationException("Failed to enumerate display monitors.");
            }

            return monitorInfoList.ToArray();
        }
        
    }
}
