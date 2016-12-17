using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aeronet.Core;
using Aeronet.Dog;
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
            try
            {
                //MessageBox.Show(string.Format("运行禁止: {0} (DogStatus::{1})\r\n",
                //               AeronetDog.Default.GetStatus((int)status),
                //               status), @"安全锁");
                //// check superdog
            }
            catch (Exception)
            {
                MessageBox.Show(@"运行禁止!", @"超级狗");
                return;
            }


            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            // config log4net
             
             // load stream of config file from embeded resource
             var thisExe = System.Reflection.Assembly.GetExecutingAssembly();
             Stream logConfigStream = thisExe.GetManifestResourceStream("Aeronet.Chart.log4net.config");
             Peach.Log.Configurator.Configurate(logConfigStream);
            
            // load config file from working folder
            /*
            try
            {
                string configfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Options", "log4net.config");
                if (File.Exists(configfile))
                {
                    using (FileStream fs = new FileStream(configfile, FileMode.Open))
                    {
                        Peach.Log.Configurator.Configurate(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */

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
