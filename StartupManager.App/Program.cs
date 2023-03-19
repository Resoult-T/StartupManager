using StartupManager.App.Model;

namespace StartupManager.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Defines whether the core.dll should be loaded dynamically or statically. 
            bool dynamicLoad = false;


            if (dynamicLoad)
                DynamicLoadCoreDll.Load();
            else
                StaticLoadCoreDll.Load();
        }
    }
}