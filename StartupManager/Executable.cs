using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Remoting.Lifetime;
using System.Threading;

namespace StartupManager
{
    [Serializable]
    class Executable
    {
        /// <summary>
        /// A unique id for this instace
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Represents the file name without extension
        /// </summary>
        public string Name { get; private set; }

        private string _pathToExe;
        /// <summary>
        /// Will Set the Path and the name will be sett to the filename without extension automaticly
        /// </summary>
        public string PathToExe
        {
            get { return _pathToExe; }

            set
            {
                _pathToExe = value;
                Name = Path.GetFileNameWithoutExtension(value);
            }
        }

        public string? Arguments { get; set; }

        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Is the number of windows that will be skipped before the window style is applied
        /// </summary>
        public uint SkipAmountOfWindows { get; set; }
        public bool StyleSkipedWindows { get; set; }

        /// <summary>
        /// Creates an object representation of an executable programm.
        /// Argumenrts are optional
        /// </summary>
        /// <param name="path">The path to the executable</param>
        /// <param name="arguments">Additional start arguments that will be parsed to the executable</param>
        public Executable(string path, string? arguments)
        {
            Id = GetNextId();
            PathToExe = path;
            Arguments = arguments;
            WindowStyle = ProcessWindowStyle.Normal;
            SkipAmountOfWindows = 0;
            StyleSkipedWindows = false;
        }


        /// <summary>
        /// Creates an object representation of an executable programm.
        /// Parameters are optional and the window style must be specified.
        /// </summary>
        /// <param name="path">The path to the executable</param>
        /// <param name="arguments">Additional start arguments that will be parsed to the executable</param>
        /// <param name="windowStyle">The style with which the program is started</param>
        public Executable(string path, string? arguments, ProcessWindowStyle windowStyle, uint skipAmountOfWindow, bool styleSkipedWindows)
        {
            Id = GetNextId();
            PathToExe = path;
            Arguments = arguments;
            WindowStyle = windowStyle;
            SkipAmountOfWindows = skipAmountOfWindow;
            StyleSkipedWindows = styleSkipedWindows;
        }



        /// <summary>
        /// Will run the executable with the specified parameterss
        /// </summary>
        /// <returns>A bool which will show if the program start was executed</returns>
        public bool Run()
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = PathToExe;
                    process.StartInfo.Arguments = Arguments;
                    process.StartInfo.RedirectStandardInput = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.WindowStyle = WindowStyle;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                }

                // Successively try to style th window
                WindowManager.WaitForWindowAndStyle(Name, WindowStyle, SkipAmountOfWindows, StyleSkipedWindows);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        // The next unique id that will be used for the next instance of this object
        public static int nextId = 1;

        /// <summary>
        /// Will create an id for a new instace
        /// </summary>
        /// <returns>Unique id</returns>
        private int GetNextId()
        {
            return nextId++;
        }
    }
}
