using CIMEL.Chart.Options;
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
using System.Windows.Forms;
using CIMEL.Core;
using CIMEL.Dog;
using ThreadState = System.Threading.ThreadState;

namespace CIMEL.Chart.CIMELData
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
        private string LOG_H_DOG = @"[DOG]";

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

        protected virtual void OnFailed(string message, bool external = true,bool showDlg=false)
        {
            var handler = Failed;
            if (handler != null) handler(this, new EventMessage(message, external,showDlg));
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

        private bool RunDrawer(ProcessStartInfo startInfo)
        {
            bool success = true;
            // initial creator process
            using (Process process = new Process() {StartInfo = startInfo})
            {
                process.OutputDataReceived += (s, e) => { this.OnInformed(e.Data, false); };

                OnInformed("***************************************************************");
                // Run creator
                process.Start();

                lock (_processlocker)
                {
                    _currentProcess = process.ProcessName;
                }

                // process.BeginErrorReadLine();
                // read output
                process.BeginOutputReadLine();

                // waiting until exit
                process.WaitForExit();

                // read error
                var error = process.StandardError.ReadToEnd();
                success = string.IsNullOrEmpty(error);
                if (!success)
                {
                    Logger.Default.Error(error);

                    error = InterceptError(error);

                    this.OnFailed(error, false,true);
                }

                OnInformed("***************************************************************");
            }

            return success;
        }

        private string InterceptError(string message)
        {
            // intercept the errors from matlab program
            string pattern = "index\\sout\\sof\\sbounds";
            Regex regex = new Regex(pattern,RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.Multiline);
            if (regex.IsMatch(message))
                return "由于测量条件不佳，无法为您计算出有效结果。请耐心等待无云天气并检查仪器工作状态是否正常";
            else
                return "数据处理失败，请检查仪器工作状态和原始数据是否正确";
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

        public void Start(string stns_fn, string stns_id, string fdata, WorkType workType = WorkType.Split)
        {
            lock (_stateLocker)
            {
                _isStopped = false;
            }
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(this.Work);
            this._threadWorker = new Thread(threadStart);
            this._threadWorker.Start(new WorkParameters(stns_fn, stns_id, fdata,workType));
        }

        /// <summary>
        /// Start the data process
        /// </summary>
        private void Work(object state)
        {
            try
            {
                // checks if the state is active
                var isActived = ActiveChecker.Singleton.IsActive(true);
                if(!isActived)
                    throw new DogException(isActived.Message);

                var paras = state as WorkParameters;
                this.OnStarted(new EventMessage("Started", true));

                OnInformed(string.Format("VER: {0}", Assembly.GetExecutingAssembly().GetName().Version));

                lock (_stateLocker)
                {
                    if (_isStopped)
                        throw new WorkCancelException();
                }

                bool sucess = false;

                // checks if the state is active
                isActived = ActiveChecker.Singleton.IsActive(true);
                if (!isActived)
                    throw new DogException(isActived.Message);

                string strInputRoot = Path.Combine(ConfigOptions.Singleton.METADATA_Dir,"input", paras.STNS_FN);
                if (!Directory.Exists(strInputRoot))
                    Directory.CreateDirectory(strInputRoot);

                string strArchiveName = string.Format("{0}_{1}_{2:yyyyMMdd}", paras.STNS_FN, paras.STNS_ID, DateTime.Now);
                string strArchiveRoot = Path.Combine(strInputRoot, strArchiveName);

                if ((paras.WorkType & WorkType.CleanOnly) == WorkType.CleanOnly)
                {
                    // clean up input folder at the begining
                    sucess = RunInputCleaner(paras, strInputRoot, strArchiveRoot);
                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();
                }

                if ((paras.WorkType & WorkType.CreateOnly) == WorkType.CreateOnly)
                {
                    // checks if the super dog is still working
                    isActived = ActiveChecker.Singleton.IsActive(true);
                    if (!isActived)
                        throw new DogException(isActived.Message);

                    // Run process of Creator
                    sucess = RunCreator(paras);

                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();
                }

                if ((paras.WorkType & WorkType.MainOnly) == WorkType.MainOnly)
                {
                    // checks if the super dog is still working
                    isActived = ActiveChecker.Singleton.IsActive(true);
                    if (!isActived)
                        throw new DogException(isActived.Message);

                    // Run process of Outputor
                    sucess = RunOutputor(paras);
                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();
                }

                // checks if the super dog is still working
                isActived = ActiveChecker.Singleton.IsActive(true);
                if (!isActived)
                    throw new DogException(isActived.Message);

                string strOutputRoot = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN);
                if (!Directory.Exists(strOutputRoot))
                    Directory.CreateDirectory(strOutputRoot);

                // string strArchiveName = string.Format("{0}_{1}_{2:yyyyMMdd}",paras.STNS_FN, paras.STNS_ID, DateTime.Now);
                strArchiveRoot = Path.Combine(strOutputRoot, strArchiveName);

                string outputfile = Path.Combine(strArchiveRoot,
                    string.Format("{0}.dat", strArchiveName));

                if ((paras.WorkType & WorkType.DrawOnly) == WorkType.DrawOnly)
                {
                    if(!Directory.Exists(strArchiveRoot))
                        Directory.CreateDirectory(strArchiveRoot);

                    // Run process of Drawer
                    sucess = RunDrawer(paras, outputfile);
                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();

                    // checks if the super dog is still working
                    isActived = ActiveChecker.Singleton.IsActive(true);
                    if (!isActived)
                        throw new DogException(isActived.Message);

                    // clean up output folder, move all day-data file ("_\d{6}_\d{6}\.dat$") to archive and just remains the final year-data file (_\d{8}.dat)
                    sucess = RunCleaner(paras, strOutputRoot, strArchiveRoot);
                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();
                }

                if ((paras.WorkType & WorkType.SplitOnly) == WorkType.SplitOnly)
                {
                    // checks if the super dog is still working
                    isActived = ActiveChecker.Singleton.IsActive(true);
                    if (!isActived)
                        throw new DogException(isActived.Message);
                    // Run process of Splitter
                    sucess = RunSplitter(paras, outputfile);

                    lock (_stateLocker)
                    {
                        if (_isStopped)
                            throw new WorkCancelException();
                    }
                    if (!sucess)
                        throw new WorkFailedException();
                }

                // checks if the super dog is still working
                isActived = ActiveChecker.Singleton.IsActive(true);
                if (!isActived)
                    throw new DogException(isActived.Message);

                OnInformed("All jobs are complete!");
                Exit(true);
            }
            catch (WorkFailedException)
            {
                Exit(false);
            }
            catch (DogException ex)
            {
                this.OnCompleted(new EventMessage(string.Format("{0}{1}{2}", LOG_H_DOG, LOG_H_ABORTED, ex.Message), true));
            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
                Exit(false);
            }
        }

        /// <summary>
        /// Check if there are output from Main.exe
        /// </summary>
        /// <param name="strOutputRoot"></param>
        /// <returns></returns>
        private bool OutputExists(string strOutputRoot)
        {
            int count = 0;
            Regex regex = new Regex("_\\d{6}_\\d{6}\\.dat$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var files = Directory.EnumerateFiles(strOutputRoot, "*.dat", SearchOption.TopDirectoryOnly);
            Parallel.ForEach(files, f =>
            {
                if (regex.IsMatch(f))
                {
                    count++;
                }
            });

            return count > 0;
        }

        // clean up output folder, move all day-data file ("_\d{6}_\d{6}\.dat$") to archive and just remains the final year-data file (_\d{8}.dat)
        private bool RunCleaner(WorkParameters paras, string strOutputRoot, string strArchiveRoot)
        {
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "CLEANER", "<INTERNAL>"));
            OnInformed(String.Format("{0} = {1}", "OUTPUTROOT", strOutputRoot));
            OnInformed(String.Format("{0} = {1}", "ARCHIVEROOT", strArchiveRoot));
            // perform outputor process
            OnInformed("***************************************************************");

            OnInformed("Clean up the output directory");
            try
            {
                if (!Directory.Exists(strOutputRoot))
                    throw new ArgumentException(string.Format("The output directory doesn't exist <- {0}",
                        strOutputRoot));

                try
                {
                    // initial the archive folder
                    if (!Directory.Exists(strArchiveRoot))
                        Directory.CreateDirectory(strArchiveRoot);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Cannot initial archive folder <- {0}, {1}",
                        strArchiveRoot, ex.Message));
                }

                Regex regex = new Regex("_\\d{6}_\\d{6}\\.dat$",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                var files = Directory.EnumerateFiles(strOutputRoot, "*.dat", SearchOption.TopDirectoryOnly);
                Parallel.ForEach(files, f =>
                {
                    if (regex.IsMatch(f))
                    {
                        this.MoveFile(f,strArchiveRoot);
                    }
                });
                OnInformed("Done. Archive folder: " + strArchiveRoot);
            }
            catch (ArgumentException ex)
            {
                OnInformed(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex);
            }

            OnInformed("***************************************************************");

            return true;
        }

        // clean up input folder, move all preparing files to archive folder at the begining
        private bool RunInputCleaner(WorkParameters paras, string strInputRoot, string strArchiveRoot)
        {
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "CLEANER", "<INTERNAL>"));
            OnInformed(String.Format("{0} = {1}", "INPUTROOT", strInputRoot));
            OnInformed(String.Format("{0} = {1}", "ARCHIVEROOT", strArchiveRoot));
            // perform process
            OnInformed("***************************************************************");

            OnInformed("Clean up the input directory");
            try
            {
                if (!Directory.Exists(strInputRoot))
                    throw new ArgumentException(string.Format("The input directory doesn't exist <- {0}",
                        strInputRoot));

                try
                {
                    // initial the archive folder
                    if (!Directory.Exists(strArchiveRoot))
                        Directory.CreateDirectory(strArchiveRoot);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Cannot initial archive folder <- {0}, {1}",
                        strArchiveRoot, ex.Message));
                }

                var files = Directory.EnumerateFiles(strInputRoot, "*", SearchOption.TopDirectoryOnly);
                Parallel.ForEach(files, f =>
                {
                    this.MoveFile(f, strArchiveRoot);
                });
                OnInformed("Done. Archive folder: " + strArchiveRoot);
            }
            catch (ArgumentException ex)
            {
                OnInformed(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Default.Error(ex);
            }

            OnInformed("***************************************************************");

            return true;
        }

        private void MoveFile(string file, string destDir)
        {
            try
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Logger.Default.Error(string.Format("Cannot move to archive folder, {0}", file), ex);
            }
        }

        private bool RunSplitter(WorkParameters paras,string outputfile)
        {
            string strRoot = Path.Combine(ConfigOptions.Singleton.CHARTSET_Dir, paras.STNS_FN);
            if (!Directory.Exists(strRoot))
                throw new ArgumentException(string.Format("The driectory not existing <- {0}", strRoot));

            if (!File.Exists(outputfile))
                throw new ArgumentException(string.Format("Not found the data file <- {0}", outputfile));

            var commandArgs = String.Format("{0} {1}", outputfile, strRoot);
            var startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_SPLITTER, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}","SPLITTER", ConfigOptions.Singleton.PROGRAM_SPLITTER));
            OnInformed(String.Format("{0} = {1}", "OUTPUTFILE", outputfile));
            OnInformed(String.Format("{0} = {1}", "CHARTSETROOT", strRoot));
            // perform outputor process
            var sucess = Run(startInfo);
            return sucess;
        }

        private bool RunDrawer(WorkParameters paras,string outputfile)
        {
            string inputbase = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN) +
                               Path.DirectorySeparatorChar;
            if (!Directory.Exists(inputbase))
                throw new ArgumentException(string.Format("The driectory not existing <- {0}", inputbase));

            // get lat and lon of region
            Region region = RegionStore.Singleton.FindRegion(paras.STNS_FN);
            if(region==null)
                throw new ArgumentException(string.Format("Not found lat & lon of the STNS <- {0}", paras.STNS_FN));

            var commandArgs = string.Format("{0} {1} {2}|{3}", inputbase, outputfile, region.Lat, region.Lon);
            var startInfo = NewStartInfo(ConfigOptions.Singleton.PROGRAM_DRAWER, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "DRAWER", ConfigOptions.Singleton.PROGRAM_DRAWER));
            OnInformed(String.Format("{0} = {1}", "INPUT", inputbase));
            OnInformed(String.Format("{0} = {1}", "OUTPUT", outputfile));
            OnInformed(String.Format("{0} = {1}", "LAT", region.Lat));
            OnInformed(String.Format("{0} = {1}", "LON", region.Lon));

            // checks if there are output from Main.exe
            if (!OutputExists(inputbase))
            {
                this.OnFailed("没有找到反演算法输出文件(.dat), 请执行[开始]重新运算", true, true);
                throw new WorkFailedException();
            }

            // perform outputor process
            var sucess = RunDrawer(startInfo);
            return sucess;
        }

        private bool RunOutputor(WorkParameters paras)
        {
            // string @out = Path.Combine(ConfigOptions.Singleton.METADATA_Dir, "input", paras.STNS_FN) + Path.DirectorySeparatorChar;
            // move the creator program to the working folder(metadata)
            string metaData = ConfigOptions.Singleton.METADATA_Dir;
            string outputor = ConfigOptions.Singleton.PROGRAM_OUTPUTOR;
            if (!File.Exists(outputor))
                throw new ArgumentException(string.Format("Not found the program <- {0}", outputor));

            string outputorName = Path.GetFileName(outputor);
            string nOutputor = Path.Combine(metaData, outputorName);
            File.Copy(outputor, nOutputor, true);
            if (!File.Exists(nOutputor))
                throw new ArgumentException(string.Format("Cannot initial the program <- {0}", nOutputor));

            // initial output path
            string @out = Path.Combine(ConfigOptions.Singleton.OUTPUT_Dir, paras.STNS_FN) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(@out))
                Directory.CreateDirectory(@out);
            if (!Directory.Exists(@out))
                throw new ArgumentException(string.Format("Cannot initial the directory <- {0}", @out));

            // initial outputor command arguments
            var commandArgs = paras.STNS_FN;
            var startInfo = NewStartInfo(nOutputor, commandArgs);
            // show command line and args
            OnInformed(string.Format("{0} = {1}", "OUTPUTOR", nOutputor));
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
            if(!Directory.Exists(ipt))
                throw new ArgumentException(string.Format("The driectory not existing <- {0}",ipt));

            string @out = Path.Combine(ConfigOptions.Singleton.METADATA_Dir, "input", paras.STNS_FN) + Path.DirectorySeparatorChar;
            if (!Directory.Exists(@out))
                Directory.CreateDirectory(@out);
            if (!Directory.Exists(@out))
                throw new ArgumentException(string.Format("Cannot initial driectory <- {0}", @out));

            string brdf = ConfigOptions.Singleton.MODIS_BRDF_Dir;
            if (brdf[brdf.Length - 1] != Path.DirectorySeparatorChar)
                brdf += Path.DirectorySeparatorChar;
            if (!Directory.Exists(brdf))
                throw new ArgumentException(string.Format("The driectory not existing <- {0}", brdf));

            string dat = Path.Combine(ConfigOptions.Singleton.DATA_Dir,paras.STNS_FN);
            if(!Directory.Exists(dat))
                Directory.CreateDirectory(dat);
            if (!Directory.Exists(dat))
                throw new ArgumentException(string.Format("Cannot initial driectory <- {0}", dat));

            // move the creator program to the working folder(metadata)
            string metaData = ConfigOptions.Singleton.METADATA_Dir;
            if (!Directory.Exists(metaData))
                throw new ArgumentException(string.Format("The driectory not existing <- {0}", metaData));

            string creator = ConfigOptions.Singleton.PROGRAM_CREATOR;
            if (!File.Exists(creator))
                throw new ArgumentException(string.Format("Not found the program <- {0}", creator));

            string creatorName = Path.GetFileName(creator);
            string nCreator = Path.Combine(metaData, creatorName);
            File.Copy(creator, nCreator, true);
            if (!File.Exists(nCreator))
                throw new ArgumentException(string.Format("Cannot initial the program <- {0}", creator));

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
        public WorkParameters(string stnsFn, string stnsId, string fdata,WorkType workType)
        {
            STNS_FN = stnsFn;
            STNS_ID = stnsId;
            FDATA = fdata;
            WorkType = workType;
        }

        public string STNS_FN { get; private set; }
        public string STNS_ID { get; private set; }
        public string FDATA { get; private set; }
        public WorkType WorkType { get; private set; }
    }

    [Flags]
    public enum WorkType
    {
        CleanOnly = 1,
        CreateOnly = 2,
        MainOnly = 4,
        DrawOnly = 8,
        SplitOnly = 16,
        Create = CleanOnly | CreateOnly,
        Main = CleanOnly | CreateOnly | MainOnly,
        Draw = CleanOnly | CreateOnly | MainOnly | DrawOnly,
        Split = CleanOnly | CreateOnly | MainOnly | DrawOnly | SplitOnly
    }
}