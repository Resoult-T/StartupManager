﻿using System;
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
using static StartupManager.VirtualScreenHelper;
using System.Windows.Media.Media3D;

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

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hwnd, out RECT rect);

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
        internal static void WaitForWindowAndStyle(string processName, ExecutableSettings settings)
        {
            IntPtr mainWindow = IntPtr.Zero;

            // A list of all matches found
            List<IntPtr> ignoredWindows = new List<IntPtr>();

            // While no mailWindowHandle was fount or the amount of already found mainWindowHandle is below the skiped value...
            while (mainWindow == IntPtr.Zero || ignoredWindows.Count <= settings.SkipAmountOfWindows)
            {
                // Get all current processes
                Process[] processes = Process.GetProcesses();

                // Select all processes that matches the procesName case insensitive
                var thisProcess = from process in processes
                                  where process.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase)
                                  select process;

                foreach (var process in thisProcess )
                {
                    IntPtr hWnd = process.MainWindowHandle;
                    // Continue if the current process does not have a mainWindowHandle
                    if (hWnd == IntPtr.Zero)
                        continue;
                    // Continue if the current handle was already found
                    else if (ignoredWindows.Contains(hWnd))
                        continue;

                    // Style this window if enabled
                    if (settings.StyleSkipedWindows)
                        ShowWindow(hWnd, GetStyleFlag(settings.WindowStyle));

                    mainWindow = hWnd;
                    ignoredWindows.Add(hWnd);
                }

                Thread.Sleep(10);
            }

            // This check is to prevent multiple calls of ShowWindow to the same windowHandle
            if (!settings.StyleSkipedWindows)
                ShowWindow(mainWindow, GetStyleFlag(settings.WindowStyle));


            StyleWindow(mainWindow, settings.PlacementData);
        }

        /// <summary>
        /// Sets the position and size of a window considering the invisible border of some windows
        /// </summary>
        /// <param name="hWnd">MainWindowHandle</param>
        /// <param name="placementData"></param>
        private static void StyleWindow(IntPtr hWnd, WindowPlacementData placementData)
        {
            // TODO: Get information about the margin/border of this window and substrakt is from the VirtualWindowPosition.

            // Get the window's position and size
            RECT windowRect = new RECT();
            GetWindowRect(hWnd, out windowRect);

            // Get the window's client area size
            RECT clientRect = new RECT();
            GetClientRect(hWnd, out clientRect);

            // Calculate the size of the window's border
            int borderWidth = ((windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left));
            // Correction value to subtract from X positioning value
            int xCorrection;
            if (borderWidth > 1)
                // The correction value results from a single edge width minus 2. The reason for this is unknown
                xCorrection = (borderWidth/2) - 2;
            else
                // If no boder is present, the correction value must not fall below 0
                xCorrection = 0;

            // Set window position
            SetWindowPos(hWnd, HWND_TOP,
                (int)placementData.VirtualWindowPosition.X - xCorrection, 
                (int)placementData.VirtualWindowPosition.Y,
                placementData.CX + borderWidth, 
                placementData.CY + (borderWidth / 2), 
                SWP_NOZORDER);
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