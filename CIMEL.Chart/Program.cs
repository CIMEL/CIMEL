using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CIMEL.Core;
using CIMEL.Dog;
using Peach.Log;
namespace CIMEL.Chart
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread threadSplash =new Thread(new ThreadStart(SplashScreen));
            threadSplash.Start();
            Thread.Sleep(4000);
            // handle the unHandle the exception
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            /*
            // do release dog when the application exit
            Application.ApplicationExit += Application_ApplicationExit;
             * */

            // config and initial log4net
            Utility.InitialLogger();

            // check superdog
            if (!CIMELDog.Default.IsAlive(true)) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            threadSplash.Abort();
            Application.Run(new fmMain());

        }

        public static void SplashScreen()
        {
            Application.Run(new SplashScreen());
        }

        /*
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            // logout super dog
            var slept = CIMELDog.Default.Sleep(true);
            if (!slept)
            {
                string message = "关闭安全锁异常，请重新插拔安全锁";
                Utility.ShowDogAlert(message);
            }
        }
        */
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // log unexpected error
            Logger.Default.Error(e.ExceptionObject);
        }
    }
}
