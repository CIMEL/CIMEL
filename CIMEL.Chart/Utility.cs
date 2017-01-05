using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CIMEL.Chart
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
            ShowAlertDlg(owner, message, @"安全锁");
        }

        public static void ShowAlertDlg(IWin32Window owner, string message, string title)
        {
            if (owner == null)
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ShowInfoDlg(IWin32Window owner, string message, string title)
        {
            if (owner == null)
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            Stream logConfigStream = thisExe.GetManifestResourceStream("CIMEL.Chart.log4net.config");
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

    public static class FormExt
    {
        public static void ShowAlert(this IWin32Window owner, string message, string title)
        {
            Utility.ShowAlertDlg(owner,message,title);
        }

        public static void ShowInfo(this IWin32Window owner, string message, string title)
        {
            Utility.ShowInfoDlg(owner, message, title);
        }
    }
}
