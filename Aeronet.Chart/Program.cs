using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Aeronet.Core;
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
            // handle the unHandle the exception
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            /*
            // do release dog when the application exit
            Application.ApplicationExit += Application_ApplicationExit;
             * */

            // config and initial log4net
            Utility.InitialLogger();

            // check superdog
            if(!AeronetDog.Default.IsAlive(true)) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fmMain());
        }

        /*
        static void Application_ApplicationExit(object sender, EventArgs e)
        {
            // logout super dog
            var slept = AeronetDog.Default.Sleep(true);
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
