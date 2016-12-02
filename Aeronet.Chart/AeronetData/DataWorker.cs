using Aeronet.Chart.Options;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Aeronet.Chart.AeronetData
{
    public class DataWorker
    {
        #region Events

        public event MessageHandler Informed;

        public event MessageHandler Failed;

        public event MessageHandler Started;

        /// <summary>
        /// The completed event message which will be triggered as either finishing successfully or faital error occurs
        /// </summary>
        public event MessageHandler Completed;

        protected virtual void OnInformed(string message, bool external = true)
        {
            var handler = Informed;
            if (handler != null) handler(this, new EventMessage(message, external));
        }

        protected virtual void OnFailed(string message, bool external = true)
        {
            var handler = Failed;
            if (handler != null) handler(this, new EventMessage(message, external));
        }

        protected virtual void OnStarted(EventMessage message)
        {
            var handler = Started;
            if (handler != null) handler(this, message);
        }

        protected virtual void OnCompleted(EventMessage message)
        {
            var handler = Completed;
            if (handler != null) handler(this, message);
        }

        #endregion Events

        private ProcessStartInfo NewStartInfo(string command, string args)
        {
            string program = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, command);
            ProcessStartInfo startInfo = new ProcessStartInfo(program, args)
            {
                RedirectStandardOutput = true, // display output in current screen
                RedirectStandardError = true, // display error in current screen
                CreateNoWindow = true, // don't launch another command line windows
                UseShellExecute = false, // perform in current command line windows
                ErrorDialog = false // the error will be displayed in current windows
            };

            return startInfo;
        }

        private bool Run(ProcessStartInfo startInfo)
        {
            bool success = true;
            // initial creator process
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (s, e) => { this.OnInformed(e.Data, false); };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        success = false;
                        OnFailed(e.Data, false);
                    }
                };

                // Run creator
                process.Start();

                // read error
                process.BeginErrorReadLine();
                // read output
                process.BeginOutputReadLine();

                // waiting until exit
                process.WaitForExit();
            }

            return success;
        }

        private void Cleanup(string region)
        {
            string input = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigOptions.Singleton.OUTPUT_Dir, region);
            string[] files = Directory.GetFiles(input, "*.*", SearchOption.TopDirectoryOnly);
            // delete all resource excepts 130119
            string[] reserveds = new string[] { "hangzhou_808_130119" };

            foreach (string f in files)
            {
                bool reserved = reserveds.Any(reservedFile => Regex.IsMatch(f, reservedFile, RegexOptions.IgnoreCase));
                if (!reserved)
                    File.Delete(f);
                else
                {
                    this.OnInformed(string.Format("{0} - normal", f));
                }
            }

            // revert FNAME
            string fname = Path.Combine(input, "FNAME");
            string content = @" 130119";
            File.WriteAllText(fname, content);
            this.OnInformed("Rewrite FNAME");

            // revert FNAME.txt
            content =
@"    hangzhou_808_130119_011148
    hangzhou_808_130119_021201
    hangzhou_808_130119_031202";
            string fnametxt = Path.Combine(input, "FNAME.txt");
            File.WriteAllText(fnametxt, content);
            this.OnInformed("Rewrite FNAME.txt");
        }

        public void Exit()
        {
            this.OnCompleted(new EventMessage("Completed", false));
        }

        /// <summary>
        /// Stop the data process
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException("Hasn't been implemented");
        }

        /// <summary>
        /// Start the data process
        /// </summary>
        public void Start()
        {
            this.OnStarted(new EventMessage("Started", false));

            OnInformed(string.Format("Aeronet Inversion VER: {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            // check if the data files are valid.
            OnInformed("Initial arguments");

            // todo: get FN, ID and Data from data file
            string STNS_FN = "";//Utility.GetAppSettingValue("ARG_STNS_FN", @default: "hangzhou");
            OnInformed(string.Format("STNS_FN : {0}", STNS_FN));

            string STNS_ID = "";//Utility.GetAppSettingValue("ARG_STNS_ID", @default: "808");
            OnInformed(string.Format("STNS_FN : {0}", STNS_ID));

            string FDATA = "";//Utility.GetAppSettingValue("ARG_FDATA", @default: "hangzhou-808-1");
            OnInformed(string.Format("STNS_FN : {0}", FDATA));

            string ipt = ConfigOptions.Singleton.INS_PARA_Dir;
            string @out = ConfigOptions.Singleton.OUTPUT_Dir;
            string brdf = ConfigOptions.Singleton.MODIS_BRDF_Dir;
            string dat = ConfigOptions.Singleton.DATA_Dir;

#if !DEBUGMATLAB
            // initial creator command arguments
            string commandArgs = string.Format("{0} {1} {2} {3} {4} {5} {6}", STNS_FN, STNS_ID, FDATA, ipt, @out, brdf, dat);
            ProcessStartInfo creatorProInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_CREATOR, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_CREATOR, commandArgs));
            OnInformed(string.Format("{0} = {1}", "STNS_FN", STNS_FN));
            OnInformed(string.Format("{0} = {1}", "STNS_ID", STNS_ID));
            OnInformed(string.Format("{0} = {1}", "FDATA", FDATA));
            OnInformed(string.Format("{0} = {1}", "FIPT", ipt));
            OnInformed(string.Format("{0} = {1}", "FOUT", @out));
            OnInformed(string.Format("{0} = {1}", "FBRDF", brdf));
            OnInformed(string.Format("{0} = {1}", "FDAT", dat));
            // perform creator process
            OnInformed("***************************************************************");
            bool sucess = Run(creatorProInfo);
            OnInformed("***************************************************************");
            if (!sucess)
            {
                Exit();
                return;
            }

#if DEMON
            // only keep a few of testing files for next step
            // only keep 130119 files, FNAME.txt and FNAME
            LogInfo("For demo presentation, only keeps the 130119 testing files");
            Cleanup(STNS_FN);
#endif
            // initial outputor command arguments
            ProcessStartInfo outputorProInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_OUTPUTOR, string.Format("{0}", STNS_FN));
            // show command line and args
            OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_OUTPUTOR, STNS_FN));
            OnInformed(string.Format("{0} = {1}", "STNSSTR", STNS_FN));
            // perform outputor process
            OnInformed("***************************************************************");
            sucess = Run(outputorProInfo);
            OnInformed("***************************************************************");
            if (!sucess)
            {
                Exit();
                return;
            }
#endif
            ProcessStartInfo drawProInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_OUTPUTOR, string.Format("{0}", STNS_FN));
            // show command line and args
            OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_OUTPUTOR, STNS_FN));
            OnInformed(string.Format("{0} = {1}", "STNSSTR", STNS_FN));
            // perform outputor process
            OnInformed("***************************************************************");
            sucess = Run(drawProInfo);
            OnInformed("***************************************************************");
            if (!sucess)
            {
                Exit();
                return;
            }

            OnInformed("All jobs are complete!");
            Exit();
        }
    }
}