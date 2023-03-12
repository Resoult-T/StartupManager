using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StartupManager.Properties;

namespace StartupManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Create a Test Setting
            var settings = new ExecutableSettings("Test", ProcessWindowStyle.Normal, 0, new WindowPlacementData(0, 0, 700, 400, 0), true);

            var exeManager = ExecutableManager.Instance();

            // Adding a new Executable to the manager
            exeManager.AddExe(new Executable("notepad.exe", null, settings));

            // Perform start
            exeManager.PerformStart();


            // Display all running processes
            var processes = Process.GetProcesses();
            var orederedProcesses = from process in processes
                                    orderby process.ProcessName ascending
                                    select process;

            lbProcesses.ItemsSource = orederedProcesses;
            lbProcesses.DisplayMemberPath = "ProcessName";

        }
    }
}
