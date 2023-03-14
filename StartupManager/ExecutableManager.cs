using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace StartupManager
{
    /// <summary>
    /// Provides tools to handle object of type Executable. 
    /// </summary>
    public sealed class ExecutableManager : IEnumerable<Executable>
    {
        //################# Singelton design pettern ###################
        // When an instance is created the saved objects will be automaticly loaded
        private ExecutableManager() { Executables = new List<Executable>(); Load(); }

        private static ExecutableManager _instance;
        // Thread save implementation using Lock
        private static readonly object _lock = new object();

        /// <summary>
        /// Creates an instace if it not already exists.
        /// </summary>
        /// <returns>Object instance</returns>
        public static ExecutableManager Instance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ExecutableManager();
                    }
                }
            }
            return _instance;
        }

        //################# Singelton design pettern ###################

        private string _savedExecutablePath = "executable.smso";

        /// <summary>
        /// This list contains all executables that will be runned on Startup.
        /// </summary>
        public List<Executable> Executables { get; private set; }

        public void AddExe(params Executable[] executables)
        {

            var existingPaths = from exe in Executables
                                select exe.PathToExe;


            foreach (var executable in executables)
            {
                if (existingPaths.Contains(executable.PathToExe)) continue;
                Executables.Add(executable);
            }
        }

        /// <summary>
        /// Remove an Executable by id.
        /// </summary>
        /// <param name="id">The id of an Executable object</param>
        public void RemoveExe(int id)
        {
            Executables.RemoveAll(x => x.Id == id);
        }

        /// <summary>
        /// Remove an Executable by path.
        /// </summary>
        /// <param name="path">Path to the executable</param>
        public void RemoveExe(string path)
        {
            Executables.RemoveAll(x => x.PathToExe == path);
        }

        /// <summary>
        /// Remove an Executable by object.
        /// </summary>
        /// <param name="exe">Executable that should be remove</param>
        public void RemoveExe(Executable exe)
        {
            Executables.Remove(exe);
        }

        /// <summary>
        /// Starts every Program within the ExecutableManager.
        /// </summary>
        public void PerformStart()
        {
            foreach (Executable exe in Executables)
            {
                exe.Run();
            }
        }


        /// <summary>
        /// Load saved executable settings.
        /// </summary>
        private void Load()
        {
            // If the file does not exists yet, return
            if (!File.Exists(_savedExecutablePath)) return;

            // DeSerialize all saved Object information
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(_savedExecutablePath, FileMode.Open))
            {
                // Will load the next id to ensure no dublicates
                Executable.nextId = (int)formatter.Deserialize(stream);
                // Will load the objects
                Executables = (List<Executable>)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Saves all executable objects added to the manager.
        /// This methode will be called automaticly when this instance is out of scope. Normaly there is not need to call this methode manualy.
        /// </summary>
        public void Save()
        {
            // Serialize curently loaded objects
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(_savedExecutablePath, FileMode.Create))
            {
                // Serialize the nextId that it can be restored afterwarts
                formatter.Serialize(stream, Executable.nextId);
                // Serialize all curently added Executable objects added to this class
                formatter.Serialize(stream, Executables);
            }
        }


        public override string ToString()
        {
            string classString = String.Empty;
            foreach (var exe in Executables)
            {
                classString +=
                    $"Id: --------- {exe.Id}\n" +
                    $"Name: ------- {exe.Name}\n";

            }

            return classString;
        }

        public IEnumerator<Executable> GetEnumerator()
        {
            return ((IEnumerable<Executable>)Executables).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Executables).GetEnumerator();
        }

        ~ExecutableManager()
        {
            Save();
        }



    }
}
