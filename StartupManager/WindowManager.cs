using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace StartupManager
{

    /// <summary>
    /// Make additionaly changes to Programs using windows api
    /// </summary>
    static class WindowManager
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        // Flags for ShowWindow methode
        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// Can be used to modify the window show behavior after a process is run.
        /// </summary>
        /// <param name="processName">The name of teh Process to be modified</param>
        /// <param name="show">A bool that represents if it should be whown or hide</param>
        /// <exception cref="ArgumentException">If the Process name isnt in running processes</exception>
        public static void ShowWindowByProcessName(string processName, bool show)
        {
            // Get the process by name
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                throw new ArgumentException($"Process with name '{processName}' not found");
            }

            // Find the main window handler
            foreach (Process process in processes)
            {
                IntPtr hWnd = process.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                {
                    continue;  // Process does not have a main window
                }

                ShowWindow(hWnd, show ? SW_SHOWNORMAL : SW_SHOWMINIMIZED);
            }
        }


        // Flags for SetWindowPos methode
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// Can be used to change the size and position of a running process window.
        /// The top left corner of a window will be at the specified position
        /// </summary>
        /// <param name="processName">The name of the process to modify</param>
        /// <param name="x">Window Position x as pixels starting from top</param>
        /// <param name="y">Window Position y as pixels starting from left</param>
        /// <param name="cx">Window size in x-axis in pixel</param>
        /// <param name="cy">Window size in y-axis in pixel</param>
        /// <exception cref="ArgumentException">If the Process name isnt in running processes</exception>
        public static void MoveAndResizeWindowByProcessName(string processName, int x, int y, int cx, int cy)
        {
            // Get the process by name
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                throw new ArgumentException($"Process with name '{processName}' not found");
            }

            // Find the mein Window
            foreach (Process process in processes)
            {
                IntPtr hWnd = process.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                {
                    continue;  // process does not have a main window
                }

                SetWindowPos(hWnd, HWND_TOP, x, y, cx, cy, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
            }
        }
    }
}
