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
    /// <summary>
    /// Contains all information needed to launch a program with a certain behavior.
    /// </summary>
    [Serializable]
    class Executable
    {
        /// <summary>
        /// A unique id for this instace.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Represents the file name without extension.
        /// </summary>
        public string Name { get; private set; }

        private string _pathToExe;
        /// <summary>
        /// Will Set the Path and the name will be sett to the filename without extension automaticly.
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

        /// <summary>
        /// Aditional arguments that will be parsed on program start.
        /// </summary>
        public string? Arguments { get; set; }

        public ExecutableSettings Settings { get; set; }

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
            Settings = new ExecutableSettings();
        }


        /// <summary>
        /// Creates an object representation of an executable programm.
        /// Parameters are optional and the window style must be specified.
        /// </summary>
        /// <param name="path">The path to the executable</param>
        /// <param name="arguments">Additional start arguments that will be parsed to the executable</param>
        /// <param name="settings">An ExecutableSettings object that defince additional settings</param>
        public Executable(string path, string? arguments, ExecutableSettings settings)
        {
            Id = GetNextId();
            PathToExe = path;
            Arguments = arguments;
            Settings = settings;
        }



        /// <summary>
        /// Will run the executable with the specified parameterss.
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
                    process.StartInfo.WindowStyle = Settings.WindowStyle;
                    process.StartInfo.CreateNoWindow = false;
                    process.Start();
                }
                if (Settings.AdvancedHandling)
                {
                    // Successively try to style th window
                    WindowManager.WaitForWindowAndStyle(Name, Settings);
                }


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
        /// Updates the nextId each time a new instance was created.
        /// </summary>
        /// <returns>Unique id</returns>
        private int GetNextId()
        {
            return nextId++;
        }
    }
}
