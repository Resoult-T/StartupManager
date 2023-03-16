using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace StartupManager
{
    public static class ProcessHelper
    {

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr hProcess, int processInformationClass, ref PROCESS_BASIC_INFORMATION processBasicInfo, int processInformationLength, out int returnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);


        // The only instance of a list with all processes
        private static Process[] allProcesses;

        /// <summary>
        /// Finds the mainWindowHandle of the passed process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="settings"></param>
        /// <returns>mainWindowHandle</returns>
        public static IntPtr FindMainWindowHandle(Process process, ref ExecutableSettings settings)
        {
            // Set this process to the root proces
            var root = Process.GetCurrentProcess();

            // Return value
            var handle = IntPtr.Zero;

            // Timeout after which it will return a zero ptr
            var timeout = DateTime.UtcNow.AddSeconds(20);

            // This list contains all found handles that should be ignored during the next run
            var foundWindowHandles = new List<IntPtr>();

            // Search for handle until found or timeout is triggered
            while (handle == IntPtr.Zero || foundWindowHandles.Count <= settings.SkipAmountOfWindows && DateTime.UtcNow < timeout)
            {
                // Initialize the processes only once per iteration
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

                if (handle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                }
            }

            return handle;
        }

        /// <summary>
        /// Searches for all child processes of the root process.
        /// </summary>
        /// <param name="root">The Parent proces</param>
        /// <returns>Alls child processes related to the given parent process.</returns>
        private static Process[] GetChildProcesses(Process root)
        {

            // Dictionary that stores a PerentProcess with key ChildProcess
            ConcurrentDictionary<int, int> parentProcessIds = new ConcurrentDictionary<int, int>();
            // Contains all found child proceses related to the root Process
            Process[] childProcesses = new Process[allProcesses.Length];


            Stopwatch stopwatch = Stopwatch.StartNew();

            // Store the parent process ID for each process ID in a dictionary
            Parallel.ForEach(allProcesses, process =>
            {
                parentProcessIds[process.Id] = process.ParentProcessId();
            });

            stopwatch.Stop();
            Console.WriteLine("Time: {0}", stopwatch.ElapsedMilliseconds);

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
        /// Gets the process ID of the parent process of a child
        /// </summary>
        /// <param name="process">The child process</param>
        /// <returns>ParentProcessId</returns>
        public static int ParentProcessId(this Process process)
        {
            // Return value
            int parentProcessId = 0;
            try
            {
                PROCESS_BASIC_INFORMATION processBasicInfo = new PROCESS_BASIC_INFORMATION();
                int processBasicInfoSize = Marshal.SizeOf(processBasicInfo);
                int bytesReturned = 0;

                // Get the handle of the child's proces
                IntPtr hProcess = OpenProcess(ProcessAccessFlags.QueryInformation, false, process.Id);
                if (hProcess != IntPtr.Zero)
                {
                    try
                    {
                        // Fill the processBasicInfo Struct with data
                        NtQueryInformationProcess(hProcess, 0, ref processBasicInfo, processBasicInfoSize, out bytesReturned);

                        // Get the ParentProcesId from processBasicInfo
                        parentProcessId = processBasicInfo.InheritedFromUniqueProcessId.ToInt32();
                    }
                    finally
                    {
                        CloseHandle(hProcess);
                    }
                }
            }
            catch (Exception)
            {
                parentProcessId = 0;
            }
            return parentProcessId;
        }

        // Buffer for the process basic information
        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public IntPtr[] Reserved2;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        // Flags for OpenProcess()
        [Flags]
        private enum ProcessAccessFlags : uint
        {
            QueryInformation = 0x0400
        }
    }
}
