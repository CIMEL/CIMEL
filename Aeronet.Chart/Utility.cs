using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    public class Utility
    {
        public static bool IsEmpty(string dir)
        {
            return string.IsNullOrEmpty(dir);
        }

        public static bool IsExist(string dir)
        {
            return Directory.Exists(dir);
        }

        public static bool IsInit(string dir)
        {
            return !IsEmpty(dir) && IsExist(dir);
        }

        public static void ShowDogAlert(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, @"安全锁",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        public static void ShowDogAlert(string message)
        {
            MessageBox.Show(message, @"安全锁", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ExitApp()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // Use this since we are a WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Use this since we are a console app
                System.Environment.Exit(1);
            }
        }

        public static void InitialLogger()
        {
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
        }
    }
}
