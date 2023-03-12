using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        
        private static HashSet<int> visitedProcessesIds;
        private static Process[] GetChildProcesses(Process root)
        {
            List<Process> childProcesses = new List<Process>();
            visitedProcessesIds = new HashSet<int>();

            Console.WriteLine(allProcesses.Length);
            int count = 0;
            foreach (Process process in allProcesses)
            {
                count++;
                Console.WriteLine(count);
                if (process.Id != root.Id && IsChildProcess(root, process))
                {
                    childProcesses.Add(process);
                }
            }

            return childProcesses.ToArray();

            //var processes = Process.GetProcesses();

            //var childProcesses = from process in processes
            //                     where process.Id != root.Id && IsChildProcess(root, process)
            //                     select process;

            //return childProcesses.ToArray();
        }
        
        private static bool IsChildProcess(Process root, Process child)
        {
            var parent = child.Parent();
            while (parent != null)
            {
                if (parent.Id == root.Id)
                {
                    return true;
                }

                if (!visitedProcessesIds.Add(parent.Id))
                {
                    return false;
                }

                parent = parent.Parent();
            }

            return false;
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
