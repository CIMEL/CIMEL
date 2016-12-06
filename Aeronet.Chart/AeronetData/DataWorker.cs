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
using System.Threading;
using System.Threading.Tasks;
using ThreadState = System.Threading.ThreadState;

namespace Aeronet.Chart.AeronetData
{
    public class DataWorker
    {
        private Thread _threadWorker;
        private static bool _isStopped = false;
        private static object _processlocker=new object();
        private static object _stateLocker=new object();
        private static string _currentProcess;

        private string LOG_H_SUCCESS = @"[COMPLETED]";
        private string LOG_H_ABORTED = @"[ABORTED]";

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

        public DataWorker()
        {
        }

        private ProcessStartInfo NewStartInfo(string command, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(command, args)
            {
                RedirectStandardOutput = true, // display output in current screen
                RedirectStandardError = true, // display error in current screen
                CreateNoWindow = true, // don't launch another command line windows
                UseShellExecute = false, // perform in current command line windows
                ErrorDialog = false, // the error will be displayed in current windows,
                WorkingDirectory = Path.GetDirectoryName(command)
            };

            return startInfo;
        }

        private bool Run(ProcessStartInfo startInfo)
        {
            bool success = true;
            // initial creator process
            using (Process process = new Process() { StartInfo = startInfo })
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

                OnInformed("***************************************************************");
                // Run creator
                process.Start();

                lock (_processlocker)
                {
                    _currentProcess = process.ProcessName;
                }

                // read error
                process.BeginErrorReadLine();
                // read output
                process.BeginOutputReadLine();

                // waiting until exit
                process.WaitForExit();

                OnInformed("***************************************************************");
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

        public void Exit(bool success)
        {
            this.OnCompleted(success ? new EventMessage(LOG_H_SUCCESS, true) : new EventMessage(LOG_H_ABORTED, true));
        }

        /// <summary>
        /// Stop the data process
        /// </summary>
        public void Stop()
        {
            Task.Factory.StartNew(() =>
            {
                // anounce shut down
                lock (_stateLocker)
                {
                    _isStopped = true;
                }

                // abort the running process
                lock (_processlocker)
                {
                    var processors = Process.GetProcessesByName(_currentProcess);
                    if (processors.Length > 0)
                    {
                        processors[0].Kill();
                    }
                }

                // check the main thread state till it realy stopped
                while (this._threadWorker.ThreadState != ThreadState.Stopped)
                {
                    Thread.Sleep(0);
                }
                // release resource
                this._threadWorker = null;
            });
        }

        public void Start(string stns_fn,string stns_id,string fdata)
        {
            lock (_stateLocker)
            {
                _isStopped = false;
            }
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(this.Work);
            this._threadWorker = new Thread(threadStart);
            this._threadWorker.Start(new WorkParameters(stns_fn,stns_id,fdata));
        }

        /// <summary>
        /// Start the data process
        /// </summary>
        private void Work(object state)
        {
            try
            {
                var paras = state as WorkParameters;
                this.OnStarted(new EventMessage("Started", true));

                OnInformed(string.Format("Aeronet Inversion VER: {0}", Assembly.GetExecutingAssembly().GetName().Version));

                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }
                // Run process of Creator
                bool sucess = RunCreator(paras);

                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }
                if (!sucess)
                    throw new WorkFailedException();
                // Run process of Outputor
                sucess = RunOutputor(paras);
                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }
                if (!sucess)
                    throw new WorkFailedException();

                string strOutputRoot = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN);
                if (!Directory.Exists(strOutputRoot))
                    Directory.CreateDirectory(strOutputRoot);

                string outputfile = Path.Combine(strOutputRoot,
                    string.Format("Dubovik_stats_{0}_{1}_{2:yyyyMMdd}.dat", paras.STNS_FN, paras.STNS_ID, DateTime.Now));
                // Run process of Drawer
                sucess = RunDrawer(paras,outputfile);
                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }
                if (!sucess)
                    throw new WorkFailedException();
                // Run process of Splitter
                sucess = RunSplitter(paras,outputfile);

                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }
                if (!sucess)
                    throw new WorkFailedException();

                OnInformed("All jobs are complete!");
                Exit(true);
            }
            catch (WorkFailedException)
            {
                Exit(false);
            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
                Exit(false);
            }
        }

        private bool RunSplitter(WorkParameters paras,string outputfile)
        {
            var commandArgs = String.Format("{0} {1}", outputfile, ConfigOptions.Singleton.CHARTSET_Dir);
            var startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_SPLITTER, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}","SPLITTER", ConfigOptions.Singleton.PROGRAM_SPLITTER));
            OnInformed(String.Format("{0} = {1}", "OUTPUTFILE", outputfile));
            OnInformed(String.Format("{0} = {1}", "CHARTSETROOT", ConfigOptions.Singleton.CHARTSET_Dir));
            // perform outputor process
            var sucess = Run(startInfo);
            return sucess;
        }

        private bool RunDrawer(WorkParameters paras,string outputfile)
        {
            string inputbase = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN) +
                               Path.DirectorySeparatorChar;

            // get lat and lon of region
            Region region = RegionStore.Singleton.FindRegion(paras.STNS_FN);
            var commandArgs = string.Format("{0} {1} {2}|{3}", inputbase, outputfile, region.Lat, region.Lon);
            var startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_DRAWER, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "DRAWER", ConfigOptions.Singleton.PROGRAM_DRAWER));
            OnInformed(String.Format("{0} = {1}", "INPUT", inputbase));
            OnInformed(String.Format("{0} = {1}", "OUTPUT", outputfile));
            OnInformed(String.Format("{0} = {1}", "LAT", region.Lat));
            OnInformed(String.Format("{0} = {1}", "LON", region.Lon));
            // perform outputor process
            var sucess = Run(startInfo);
            return sucess;
        }

        private bool RunOutputor(WorkParameters paras)
        {
            // string @out = Path.Combine(ConfigOptions.Singleton.METADATA_Dir, "input", paras.STNS_FN) + Path.DirectorySeparatorChar;
            // move the creator program to the working folder(metadata)
            string metaData = ConfigOptions.Singleton.METADATA_Dir;
            string outputor = ConfigOptions.Singleton.PROGRAM_OUTPUTOR;
            string outputorName = Path.GetFileName(outputor);
            string nOutputor = Path.Combine(metaData, outputorName);
            File.Copy(outputor, nOutputor, true);

            // initial output path
            string @out = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(@out))
                Directory.CreateDirectory(@out);

            // initial outputor command arguments
            var commandArgs = paras.STNS_FN;
            var startInfo = NewStartInfo(nOutputor, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "OUTPUTOR",ConfigOptions.Singleton.PROGRAM_OUTPUTOR));
            OnInformed(string.Format("{0} = {1}", "STNSSTR", paras.STNS_FN));
            // perform outputor process
            var sucess = Run(startInfo);
            return sucess;
        }

        private bool RunCreator(WorkParameters paras)
        {
            //OnInformed("Initial arguments");
            string ipt = ConfigOptions.Singleton.INS_PARA_Dir;
            if (ipt[ipt.Length - 1] != Path.DirectorySeparatorChar)
                ipt += Path.DirectorySeparatorChar;

            string @out = Path.Combine(ConfigOptions.Singleton.METADATA_Dir, "input", paras.STNS_FN) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(@out))
                Directory.CreateDirectory(@out);

            string brdf = ConfigOptions.Singleton.MODIS_BRDF_Dir;
            if (brdf[brdf.Length - 1] != Path.DirectorySeparatorChar)
                brdf += Path.DirectorySeparatorChar;
            string dat = ConfigOptions.Singleton.DATA_Dir;

            // move the creator program to the working folder(metadata)
            string metaData = ConfigOptions.Singleton.METADATA_Dir;
            string creator = ConfigOptions.Singleton.PROGRAM_CREATOR;
            string creatorName = Path.GetFileName(creator);
            string nCreator = Path.Combine(metaData, creatorName);
            File.Copy(creator, nCreator, true);

            // initial creator command arguments
            string commandArgs = string.Format("{0} {1} {2} {3} {4} {5} {6}", paras.STNS_FN, paras.STNS_ID, paras.FDATA, ipt,
                @out, brdf, dat);
            ProcessStartInfo startInfo = NewStartInfo(nCreator, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "CREATOR", ConfigOptions.Singleton.PROGRAM_CREATOR));
            OnInformed(string.Format("{0} = {1}", "STNS_FN", paras.STNS_FN));
            OnInformed(string.Format("{0} = {1}", "STNS_ID", paras.STNS_ID));
            OnInformed(string.Format("{0} = {1}", "FDATA", paras.FDATA));
            OnInformed(string.Format("{0} = {1}", "FIPT", ipt));
            OnInformed(string.Format("{0} = {1}", "FOUT", @out));
            OnInformed(string.Format("{0} = {1}", "FBRDF", brdf));
            OnInformed(string.Format("{0} = {1}", "FDAT", dat));
            // perform creator process
            bool sucess = Run(startInfo);
            return sucess;
        }
    }

    public class WorkParameters
    {
        public WorkParameters(string stnsFn, string stnsId, string fdata)
        {
            STNS_FN = stnsFn;
            STNS_ID = stnsId;
            FDATA = fdata;
        }

        public string STNS_FN { get; private set; }
        public string STNS_ID { get; private set; }
        public string FDATA { get; private set; }
    }
}