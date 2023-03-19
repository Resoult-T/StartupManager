using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StartupManager.App.Model
{
    public static class DynamicLoadCoreDll
    {
        public static void Load()
        {
            try
            {
                // Load the StartupManager.Core.dll assembly dynamically
                string assemblyPath = @"../../../../StartupManager.Core/bin/Debug/net6.0/StartupManager.Core.dll";
                Assembly coreAssembly = Assembly.LoadFrom(assemblyPath);

                // Create a WindowPlacementData instance
                Type windowPlacementDataType = coreAssembly.GetType("StartupManager.Core.Model.Executable.WindowPlacementData");
                var windowPlacementData1 = Activator.CreateInstance(windowPlacementDataType, 0, 0, 1000, 600, 0);

                // Create settings for executable
                Type settingsType = coreAssembly.GetType("StartupManager.Core.Model.Executable.ExecutableSettings");
                var constructors = settingsType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                ConstructorInfo targetConstructor = null;

                foreach (var constructor in constructors)
                {
                    var parameters = constructor.GetParameters();
                    if (parameters.Length == 5)
                    {
                        targetConstructor = constructor;
                        break;
                    }
                }

                if (targetConstructor != null)
                {
                    var settings1 = targetConstructor.Invoke(new object[] { "Notepad", ProcessWindowStyle.Normal, (uint)0, windowPlacementData1, true });

                    Type exeManagerType = coreAssembly.GetType("StartupManager.Core.Model.Executable.ExecutableManager");
                    var instanceMethod = exeManagerType.GetMethod("Instance", BindingFlags.Static | BindingFlags.Public);

                    // Call ExecutableManager.Instance()
                    var exeManager = instanceMethod.Invoke(null, null);

                    // Create a new Executable
                    var executableType = coreAssembly.GetType("StartupManager.Core.Model.Executable.Executable");
                    var executable1 = Activator.CreateInstance(executableType, "notepad.exe", null, settings1);

                    // Add an executable object to the manager
                    var addExeMethod = exeManagerType.GetMethod("AddExe");
                    Array exeArray = Array.CreateInstance(executableType, 1);
                    exeArray.SetValue(executable1, 0);
                    addExeMethod.Invoke(exeManager, new object[] { exeArray });

                    // Call exeManager.PerformStart method
                    var performStartMethod = exeManagerType.GetMethod("PerformStart");
                    performStartMethod.Invoke(exeManager, null);

                }
                else
                {
                    Console.WriteLine("Constructor not found");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Program.Main(): Error while loading dll: " + ex.Message);
            }
        }
    }
}
