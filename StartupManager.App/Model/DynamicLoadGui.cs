using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace StartupManager.App.Model
{
    public static class DynamicLoadGui
    {
        public static void Load()
        {
            // Load the StartupManager.GUI.dll assembly dynamically
            string relativeAssemblyPath = @"../../../../StartupManager.GUI/bin/Debug/net6.0-windows/StartupManager.GUI.dll";
            string absoluteAssemblyPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, relativeAssemblyPath));

            AssemblyLoadContext context = new AssemblyLoadContext("WpfAssemblyContext");
            Assembly assembly = context.LoadFromAssemblyPath(absoluteAssemblyPath);

            // Make sure the assembly is loaded
            if (assembly == null)
            {
                Debug.WriteLine("Failed to load the assembly.");
                return;
            }

            // Create an instance of the MainWindow
            Type mainWindowType = assembly.GetType("StartupManager.GUI.Model.MainWindow"); // Update the namespace
            if (mainWindowType == null)
            {
                Debug.WriteLine("Failed to get the MainWindow type.");
                return;
            }

            var mainWindow = Activator.CreateInstance(mainWindowType);

        }
    }
}
