using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupManager
{
    internal class ExecutableSettings
    {
        /// <summary>
        /// A name to identify this preset
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Aditional arguments that will be parsed on program start.
        /// </summary>
        public string? Arguments { get; set; }

        /// <summary>
        /// Defines the process style in wich the the aplication will be started.
        /// </summary>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Setting this to true, activates advanced handling to the associated executable.
        /// It will use further enhanced methodes to applie stylings and adds more features.
        /// </summary>
        public bool AdvancedHandling { get; set; }

        /// <summary>
        /// Is the number of windows that will be skipped before the window style is applied.
        /// AdvancedHandling must be set to true.
        /// </summary>
        public uint SkipAmountOfWindows { get; set; }

        /// <summary>
        /// This defines if the prior windows to the main Window that gets modified should also be modified.
        /// AdvancedHandling must be set to true.
        /// </summary>
        public bool StyleSkipedWindows { get; set; }
    
        

        /// <summary>
        /// Default settings applied.
        /// </summary>
        public ExecutableSettings()
        {
            Name = null;
            Arguments = null;
            WindowStyle = ProcessWindowStyle.Normal;
            AdvancedHandling = false;
            SkipAmountOfWindows = 0;
            StyleSkipedWindows = false;
        }

        /// <summary>
        /// Define basic settigns.
        /// </summary>
        /// <param name="arguments">Arguments parsed at aplication start</param>
        /// <param name="windowStyle">Window style</param>
        public ExecutableSettings(string? name, string? arguments, ProcessWindowStyle windowStyle)
        {
            Name = name;
            Arguments = arguments;
            WindowStyle = windowStyle;
            AdvancedHandling = false;
            SkipAmountOfWindows = 0;
            StyleSkipedWindows = false;
        }

        /// <summary>
        /// Define advanced settigs.
        /// This will automaticli activate advancedHandling.
        /// </summary>
        /// <param name="arguments">Arguments parsed at aplication start</param>
        /// <param name="windowStyle">Window style</param>
        /// <param name="skipAmountOfWindows">Windows that will be skiped befor styles are applied</param>
        /// <param name="styleSkipedWindows">Defines if the style should also be set to prior windows</param>
        public ExecutableSettings(string? name, string? arguments, ProcessWindowStyle windowStyle, uint skipAmountOfWindows, bool styleSkipedWindows)
        {  
            Name = name;
            Arguments = arguments;
            WindowStyle = windowStyle;
            SkipAmountOfWindows = skipAmountOfWindows;
            StyleSkipedWindows = styleSkipedWindows;
        }
    }
}
