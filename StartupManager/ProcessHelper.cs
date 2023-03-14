using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StartupManager
{
    public static class ProcessHelper
    {
        private static Process[] allProcesses;

        public static IntPtr FindMainWindowHandle(Process process, ref ExecutableSettings settings)
        {
            // Set this process to the root proces
            var root = Process.GetCurrentProcess();

            // Later when processes are started in paralell it must be found out to which thread which mainWindowHandle belongs to
            var thread = Thread.CurrentThread;


            var handle = IntPtr.Zero;

            // Timeout after which it will return a zero ptr
            var timeout = DateTime.UtcNow.AddSeconds(10);

            var foundWindowHandles = new List<IntPtr>(); 



            // Get the mainWindowHandle of the process 
            while (handle == IntPtr.Zero || foundWindowHandles.Count <= settings.SkipAmountOfWindows && DateTime.UtcNow < timeout)
            {
                
                allProcesses = Process.GetProcesses();
                
                
                if (!allProcesses.Any(p => p.Id == root.Id))
                {
                    // Root process is no longer running
                    break;
                }
                
                var childProcesses = GetChildProcesses(root);

                // Search for the MainWindowHandle in child processes
                foreach (var child in childProcesses)
                {
                    IntPtr hWnd = child.MainWindowHandle;

                    if (hWnd != IntPtr.Zero)
                    {
                        if (foundWindowHandles.Contains(hWnd))
                            continue;

                        handle = hWnd;
                        foundWindowHandles.Add(hWnd);
                        break;
                    }
                }
            }

            return handle;
        }

        /// <summary>
        /// Searches for all child processes of the root process
        /// </summary>
        /// <param name="root">The Parent proces</param>
        /// <returns>Alls child processes related to the given parent process</returns>
        private static Process[] GetChildProcesses(Process root)
        {
            // Dictionary that stores a PerentProcess with key ChildProcess
            ConcurrentDictionary<int, int> parentProcessIds = new ConcurrentDictionary<int, int>();
            // Contains all found child proceses related to the root Process
            Process[] childProcesses = new Process[allProcesses.Length];

            // Store the parent process ID for each process ID in a dictionary
            Parallel.ForEach(allProcesses, process =>
            {
                parentProcessIds[process.Id] = process.ParentProcessId();
                Console.WriteLine(process.ProcessName);
            });


            // Use a breadth-first search algorithm to find the child processes of the root process

            // All object in this queue are under the Parent(root) Process
            Queue<Process> queue = new Queue<Process>();

            // Set root as the dirst Parent to search for child processes
            queue.Enqueue(root);

            int count = 0;

            // While queue is not emty...
            while (queue.Count > 0)
            {
                // Dequeue the next Process. This will define the Parent Process for the next child search
                Process current = queue.Dequeue();

                // Check all available Processes... 
                foreach (Process process in allProcesses)
                {
                    // If the Parent of the current Process is the same as the current Parent and the Proces is not already in childProcesses...
                    if (process.Id != root.Id && parentProcessIds[process.Id] == current.Id && Array.IndexOf(childProcesses, process, 0, count) < 0)
                    {
                        // Add this Process to childProcesses and Queue it to be the next Parent
                        childProcesses[count++] = process;
                        queue.Enqueue(process);
                    }
                }
            }

            // Rezise the childProcesses array and return it
            Array.Resize(ref childProcesses, count);
            return childProcesses;
        }

        /// <summary>
        /// Gets the Perent of the current process
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private static Process Parent(this Process process)
        {
            int parentProcessId = process?.ParentProcessId() ?? -1;
            if (parentProcessId <= 0)
            {
                return null;
            }

            foreach (Process p in allProcesses)
            {
                if (p.Id == parentProcessId)
                {
                    return p;
                }
            }

            return null;
        }


        // TODO: Find a methd that is faster than this one 
        public static int ParentProcessId(this Process process)
        {
            int parentProcessId;
            try
            {
                using (ManagementObject managementObject = new ManagementObject($"win32_process.handle='{process.Id}'"))
                {
                    managementObject.Get();
                    return Convert.ToInt32(managementObject["ParentProcessId"]);
                }
            }
            catch (Exception)
            {
                parentProcessId = 0;
            }
            return parentProcessId;
        }
    }
}
