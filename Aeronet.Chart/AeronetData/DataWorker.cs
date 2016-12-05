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
using ThreadState = System.Threading.ThreadState;

namespace Aeronet.Chart.AeronetData
{
    public class DataWorker
    {
        private Thread _threadWorker;
        private static bool _isStopped = false;

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
            string program = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, command);
            ProcessStartInfo startInfo = new ProcessStartInfo(program, args)
            {
                RedirectStandardOutput = true, // display output in current screen
                RedirectStandardError = true, // display error in current screen
                CreateNoWindow = true, // don't launch another command line windows
                UseShellExecute = false, // perform in current command line windows
                ErrorDialog = false, // the error will be displayed in current windows,
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

                OnInformed("***************************************************************");
                // Run creator
                process.Start();

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

        public void Exit()
        {
            this.OnCompleted(new EventMessage("Completed", false));
        }

        /// <summary>
        /// Stop the data process
        /// </summary>
        public void Stop()
        {
            _isStopped = true;

            while (this._threadWorker.ThreadState!=ThreadState.Stopped)
            {
                Thread.Sleep(0);
            }
            // release resource
            this._threadWorker = null;
        }

        public void Start(string stns_fn,string stns_id,string fdata)
        {
            _isStopped = false;
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
                this.OnStarted(new EventMessage("Started", false));

                OnInformed(string.Format("Aeronet Inversion VER: {0}", Assembly.GetExecutingAssembly().GetName().Version));

                if (_isStopped)
                    throw new WorkCancelException();

                //OnInformed("Initial arguments");
                string ipt = ConfigOptions.Singleton.INS_PARA_Dir;
                string @out = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir,"input");
                string brdf = ConfigOptions.Singleton.MODIS_BRDF_Dir;
                string dat = ConfigOptions.Singleton.DATA_Dir;

                // move the creator program to the working folder(metadata)
                string metaData = ConfigOptions.Singleton.METADATA_Dir;
                string creator = ConfigOptions.Singleton.PROGRAM_CREATOR;
                string creatorName = Path.GetFileName(creator);
                string nCreator = Path.Combine(metaData, creatorName);
                File.Copy(creator, nCreator, true);

//#if !DEBUGMATLAB
                // initial creator command arguments
                string commandArgs = string.Format("{0} {1} {2} {3} {4} {5} {6}", paras.STNS_FN, paras.STNS_ID, paras.FDATA, ipt, @out, brdf, dat);
                ProcessStartInfo startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_CREATOR, commandArgs);
                // show command line and args
                OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_CREATOR, commandArgs));
                OnInformed(string.Format("{0} = {1}", "STNS_FN", paras.STNS_FN));
                OnInformed(string.Format("{0} = {1}", "STNS_ID", paras.STNS_ID));
                OnInformed(string.Format("{0} = {1}", "FDATA", paras.FDATA));
                OnInformed(string.Format("{0} = {1}", "FIPT", ipt));
                OnInformed(string.Format("{0} = {1}", "FOUT", @out));
                OnInformed(string.Format("{0} = {1}", "FBRDF", brdf));
                OnInformed(string.Format("{0} = {1}", "FDAT", dat));
                // perform creator process
                bool sucess = Run(startInfo);
                if (!sucess)
                    throw new WorkFailedException();

                if (_isStopped)
                    throw new WorkCancelException();

//#if DEMON
//            // only keep a few of testing files for next step
//            // only keep 130119 files, FNAME.txt and FNAME
//            LogInfo("For demo presentation, only keeps the 130119 testing files");
//            Cleanup(STNS_FN);
//#endif
                // move the creator program to the working folder(metadata)
                string outputor = ConfigOptions.Singleton.PROGRAM_CREATOR;
                string outputorName = Path.GetFileName(outputor);
                string nOutputor = Path.Combine(metaData, outputorName);
                File.Copy(outputor, nOutputor, true);

                // initial outputor command arguments
                commandArgs = paras.STNS_FN;
                startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_OUTPUTOR, commandArgs);
                // show command line and args
                OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_OUTPUTOR, commandArgs));
                OnInformed(string.Format("{0} = {1}", "STNSSTR", paras.STNS_FN));
                // perform outputor process
                sucess = Run(startInfo);
                if (!sucess)
                    throw new WorkFailedException();

                //#endif
                string inputbase = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN) + Path.DirectorySeparatorChar;
                string outputbase = ConfigOptions.Singleton.OUTPUT_Dir;// Path.DirectorySeparatorChar;
                if (!outputbase.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    outputbase += Path.DirectorySeparatorChar;

                // get lat and lon of region
                Region region = RegionStore.Singleton.FindRegion(paras.STNS_FN);
                commandArgs = string.Format("{0} {1} {2} {3}", region.Lat, region.Lon, inputbase, outputbase);
                startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_DRAWER, commandArgs);
                // show command line and args
                OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_DRAWER, commandArgs));
                OnInformed(String.Format("\t{0} : {1}", "LAT", region.Lat));
                OnInformed(String.Format("\t{0} : {1}", "LON", region.Lon));
                OnInformed(String.Format("\t{0} : {1}", "INPUT", inputbase));
                OnInformed(String.Format("\t{0} : {1}", "OUTPUT", outputbase));
                // perform outputor process
                sucess = Run(startInfo);
                if (!sucess)
                    throw new WorkFailedException();

                string outputfile = Path.Combine(outputbase,
                    String.Format("Dubovik_stats_{0}_{1}_{2:yyyyMMdd}.dat", paras.STNS_FN, paras.STNS_ID, DateTime.Now));
                commandArgs = outputfile;
                startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_SPLITTER, commandArgs);
                // show command line and args
                OnInformed(string.Format("{0} {1}", ConfigOptions.Singleton.PROGRAM_SPLITTER, commandArgs));
                OnInformed(String.Format("\t{0} : {1}", "OUTPUTFILE", outputfile));
                // perform outputor process
                sucess = Run(startInfo);
                if (!sucess)
                    throw new WorkFailedException();

                OnInformed("All jobs are complete!");
                Exit();
            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
                Exit();
            }
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