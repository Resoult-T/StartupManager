﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace StartupManager.Core.Model.Executable
{
    /// <summary>
    /// Provides tools to handle object of type Executable. 
    /// </summary>
    public sealed class ExecutableManager : IEnumerable<Executable>
    {
        //################# Singleton design pattern ###################
        // When an instance is created the saved objects will be automatically loaded
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

        //################# Singleton design pattern ###################

        /// <summary>
        /// This list contains all executables that will be runned on Startup.
        /// </summary>
        public List<Executable> Executables { get; private set; }

        public void AddExe(Executable[] executables)
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
            if (!File.Exists(getPathToSave())) return;

            // DeSerialize all saved Object information
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(getPathToSave(), FileMode.Open))
            {
                // Will load the next id to ensure no dublicates
                Executable.nextId = (int)formatter.Deserialize(stream);
                // Will load the objects
                Executables = (List<Executable>)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Saves all executable objects added to the manager.
        /// This method will be called automatically when this instance is out of scope. 
        /// Usually there is no need to call this method manually.
        /// </summary>
        public void Save()
        {
            // Serialize curently loaded objects
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(getPathToSave(), FileMode.Create))
            {
                // Serialize the nextId that it can be restored afterwarts
                formatter.Serialize(stream, Executable.nextId);
                // Serialize all curently added Executable objects added to this class
                formatter.Serialize(stream, Executables);
            }
        }

        public string getPathToSave()
        {
            string fileName = "executable.smso";
            string outputDirectory = AppDomain.CurrentDomain.BaseDirectory; // Gibt das Ausführungsverzeichnis der Hauptanwendung zurück
            return Path.Combine(outputDirectory, fileName);
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