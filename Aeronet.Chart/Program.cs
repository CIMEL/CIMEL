using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Peach.Log;

namespace Aeronet.Chart
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // config log4net
            var thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            Stream logConfigStream = thisExe.GetManifestResourceStream("Aeronet.Chart.log4net.config");
            Peach.Log.Configurator.Configurate(logConfigStream);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fmMain());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.ExceptionObject);
        }
    }
}
