using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Threading;

namespace StartupManager
{

    /// <summary>
    /// Make additionaly changes to Programs using windows APIs.
    /// </summary>
    static class WindowManager
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        /// <summary>
        /// Returns the First MainWindowHandle that is found. If there was no MainWindowHandle it will return IntPtr.Zero.
        /// </summary>
        /// <param name="processes">An Array of Processes to search for a mainWindowHandle</param>
        /// <returns>IntPtr to a MainWindowHandle</returns>
        private static IntPtr getMainWindowHandle(Process[] processes)
        {
            IntPtr returnValue = IntPtr.Zero;

            foreach (Process process in processes)
            {
                // Get the mainWindowHandle
                IntPtr hWnd = process.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                    continue; // Process dose not have a window handle

                // In case it is not IntPtr.Zero it will be the returnValue
                returnValue = hWnd;
                break;
            }

            // [Debugging] Show if the return value is still IntPtr.Zero
            if (returnValue == IntPtr.Zero)
                MessageBox.Show("None of the specified processes had a mainWindowHandle");

            return returnValue;
        }

        // Flags for ShowWindow methode
        const int SW_HIDE = 0;
        const int SW_SHOWNORMAL = 1;
        const int SW_SHOWMINIMIZED = 2;
        const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// Can be used to modify the window show behavior after a process is run.
        /// </summary>
        /// <param name="processName">The name of the Process to be modified</param>
        /// <param name="show">A bool that represents if it should be show or hide</param>
        /// <exception cref="ArgumentException">If the Process name isnt in running processes</exception>
        public static void StyleWindowByProcessName(string processName, ProcessWindowStyle style)
        {
            // Get the process by name
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                throw new ArgumentException($"Process with name '{processName}' not found");
            }

            // Find the main window handler
            IntPtr hWnd = getMainWindowHandle(processes);

            // Set the MainWindowHandle to the specified mode
            ShowWindow(hWnd, GetStyleFlag(style));
            
        }


        // Flags for SetWindowPos methode
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_SHOWWINDOW = 0x0040;
        const uint SWP_NOOWNERZORDER = 0x0200;
        const uint SWP_NOREDRAW = 0x0008;
        const uint SWP_NOZORDER = 0x0004;

        /// <summary>
        /// Can be used to change the size and position of a running process window.
        /// The top left corner of a window will be at the specified position.
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

            // Find the main window handler
            IntPtr hWnd = getMainWindowHandle(processes);

            // Set the MainWindowHandle to the specified mode
            SetWindowPos(hWnd, HWND_TOP, x, y, cx, cy, SWP_SHOWWINDOW | SWP_NOOWNERZORDER);
            
        }

        /// <summary>
        /// Waits until the wanted mainWondowHandle was detected ans then aplies the styling.
        /// </summary>
        /// <param name="skipAmountOfWindows">The amount of matches that will be skiped</param>
        /// <param name="processName">The name to search in Processes</param>
        internal static void WaitForWindowAndStyle(ref Process process,ref ExecutableSettings settings)
        {
            // Wait for Main window handle
            IntPtr mainWindow = WaitForMainWindowHandl(ref process, settings.SkipAmountOfWindows);


            ShowWindow(mainWindow, GetStyleFlag(settings.WindowStyle));

            SetWindowPos(mainWindow, HWND_TOP, 
                (int)settings.PlacementData.VirtualWindowPosition.X, (int)settings.PlacementData.VirtualWindowPosition.Y,
                settings.PlacementData.CX, settings.PlacementData.CY, SWP_NOZORDER);
        }

        private static IntPtr WaitForMainWindowHandl(ref Process process, uint skipAmountOfWindows) 
        {
            var hWnd = IntPtr.Zero;

            // TODO: Create a better way to find the MainWindowHandl even if the Process name is changed


            return hWnd;
        }

        /// <summary>
        /// Gets the style flag to pars it to ShowWindow function of user32.dll.
        /// </summary>
        /// <param name="style">Window Style</param>
        /// <returns>An integer representation of the parsed style</returns>
        private static int GetStyleFlag(ProcessWindowStyle style)
        {
            switch (style)
            {
                case ProcessWindowStyle.Hidden:
                    return SW_HIDE;

                case ProcessWindowStyle.Normal:
                    return SW_SHOWNORMAL;

                case ProcessWindowStyle.Minimized:
                    return SW_SHOWMINIMIZED;

                case ProcessWindowStyle.Maximized:
                    return SW_SHOWMAXIMIZED;
            }

            return SW_SHOWNORMAL;
        }
    }
}