using System;
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
        public static IntPtr FindMainWindowHandle(Process process)
        {
            var root = Process.GetCurrentProcess();
            var thread = Thread.CurrentThread;

            var handle = IntPtr.Zero;
            var timeout = DateTime.UtcNow.AddSeconds(10);

            while (handle == IntPtr.Zero && DateTime.UtcNow < timeout)
            {
                allProcesses = Process.GetProcesses();
                if (!allProcesses.Any(p => p.Id == root.Id))
                {
                    // Root process is no longer running
                    break;
                }

                var childProcesses = GetChildProcesses(root);
                foreach (var child in childProcesses)
                {
                    if (child.MainWindowHandle != IntPtr.Zero)
                    {
                        handle = child.MainWindowHandle;
                        break;
                    }
                }

                if (handle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                }
            }

            return handle;
        }


        private static Process[] GetChildProcesses(Process root)
        {
            // Dictionary that stores a PerentProcess with key ChildProcess
            Dictionary<int, int> parentProcessIds = new Dictionary<int, int>();
            // Returnees
            Process[] childProcesses = new Process[allProcesses.Length];

            int count = 0;

            // Store the parent process ID for each process in a dictionary
            foreach (Process process in allProcesses)
            {
                parentProcessIds[process.Id] = process.ParentProcessId();
                Console.WriteLine(process.ProcessName);
            }

            // Use a breadth-first search algorithm to find the child processes of the root process
            Queue<Process> queue = new Queue<Process>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Process current = queue.Dequeue();

                foreach (Process process in allProcesses)
                {
                    if (process.Id != root.Id && parentProcessIds[process.Id] == current.Id && Array.IndexOf(childProcesses, process, 0, count) < 0)
                    {
                        childProcesses[count++] = process;
                        queue.Enqueue(process);
                    }
                }
            }

            Array.Resize(ref childProcesses, count);
            return childProcesses;
        }


        private static Process Parent(this Process process)
        {
            int parentProcessId = process?.ParentProcessId() ?? -1;
            if (parentProcessId <= 0)
            {
                return null; // Prozess ist der Systemprozess oder hat keinen gültigen Parent-Prozess
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
