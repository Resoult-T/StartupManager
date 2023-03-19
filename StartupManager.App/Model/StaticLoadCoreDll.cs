using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartupManager.Core.Model.Executable;


namespace StartupManager.App.Model
{
    public static class StaticLoadCoreDll
    {
        public static void Load()
        {
            // Call ExecutableManager.Instance()
            var exeManager = ExecutableManager.Instance();

            // Create settings for executable
            var settings = new ExecutableSettings("notepad", ProcessWindowStyle.Normal, 0, new WindowPlacementData(0, 0, 1000, 600, 0), true);
            // Add an executable object to the manager
            exeManager.AddExe(new[] { new Executable("notepad.exe", null, settings) });

            exeManager.PerformStart();
        }

    }
}
