using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartupManager
{
    /// <summary>
    /// Contains all settings for an object type of Executable which are not related specific to an Executable Object and can be reused for others.
    /// </summary>
    [Serializable]
    public class ExecutableSettings
    {
        /// <summary>
        /// A name to identify this preset
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Defines the process style in wich the the application will be started.
        /// </summary>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Setting this to true, activates advanced handling to the associated executable.
        /// It will use further enhanced methods to apply stylings and adds more features.
        /// </summary>
        public bool AdvancedHandling { get; set; }

        /// <summary>
        /// Is the number of windows that will be skipped before the window style is applied.
        /// AdvancedHandling must be set to true.
        /// </summary>
        public uint SkipAmountOfWindows { get; set; }


        /// <summary>
        /// Contains placment data of the window.
        /// </summary>
        public WindowPlacementData PlacementData { get; set; }

        /// <summary>
        /// Enables custom positioning. This Only works when AdvancedHandling is enabled and PlacementData is set.
        /// </summary>
        public bool CustomPositioning { get; set; }
    
        /// <summary>
        /// Default settings applied.
        /// </summary>
        public ExecutableSettings()
        {
            Name = null;
            WindowStyle = ProcessWindowStyle.Normal;
            AdvancedHandling = false;
            SkipAmountOfWindows = 0;
            CustomPositioning = false;
        }

        /// <summary>
        /// Define basic settigns.
        /// </summary>
        /// <param name="arguments">Arguments parsed at application start</param>
        /// <param name="windowStyle">Window style</param>
        public ExecutableSettings(string? name, ProcessWindowStyle windowStyle)
        {
            Name = name;
            WindowStyle = windowStyle;
            AdvancedHandling = false;
            SkipAmountOfWindows = 0;
            CustomPositioning = false;
        }

        /// <summary>
        /// Define advanced settings.
        /// This will automatically activate advancedHandling.
        /// </summary>
        /// <param name="arguments">Arguments parsed at application start</param>
        /// <param name="windowStyle">Window style</param>
        /// <param name="skipAmountOfWindows">Windows that will be skipped before styles are applied</param>
        /// <param name="styleSkipedWindows">Defines if the style should also be set to prior windows</param>
        public ExecutableSettings(string? name, ProcessWindowStyle windowStyle, uint skipAmountOfWindows, WindowPlacementData placementData, bool customPositioning)
        {  
            Name = name;
            WindowStyle = windowStyle;
            AdvancedHandling = true;
            SkipAmountOfWindows = skipAmountOfWindows;
            PlacementData = placementData;
            CustomPositioning = customPositioning;
        }
    }
}
