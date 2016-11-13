// #define DEBUGMATLAB
// #define DEMON
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
// using MathWorks.MATLAB.NET.Arrays;
using Microsoft.SqlServer.Server;
using Peach.Log;

namespace Peach.AeronetInversion
{
    class Program
    {
        private static void LogInfo(string info,bool external=true)
        {
            string msg = string.Format("{0} -> {1}", (external ? "EXT" : "INT"), info);
            Logger.Default.Info(msg);
            Console.WriteLine(msg);
        }

        private static void LogError(string error,bool external=true)
        {
            string msg = string.Format("{0} -> {1}", (external ? "EXT" : "INT"), error);
            Logger.Default.Error(msg);
            Console.WriteLine(msg);
        }

        private static ProcessStartInfo NewStartInfo(string command, string args)
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

        private static bool Execute(ProcessStartInfo startInfo)
        {
            bool success = true;
            // initial creator process
            using (Process process = new Process { StartInfo = startInfo })
            {
                process.OutputDataReceived += (s, e) => { LogInfo(e.Data,false); };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        success = false;
                        LogError(e.Data,false);
                    }
                };

                // Start creator
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

        private static void InitWorkingFolder(string[] folders)
        {
            if (folders == null || folders.Length == 0) return;

            foreach (string folder in folders)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        private static void InitRegionFolder(string region)
        {
            string[] workingfolders = new string[] {"output", ConfigOptions.FOUT};
            foreach (string wf in workingfolders)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,wf, region);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        private static void Cleanup(string region)
        {
            string input = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigOptions.FOUT, region);
            string[] files = Directory.GetFiles(input, "*.*", SearchOption.TopDirectoryOnly);
            // delete all resource excepts 130119
            string[] reserveds = new string[] {"hangzhou_808_130119"};

            foreach (string f in files)
            {
                bool reserved = reserveds.Any(reservedFile => Regex.IsMatch(f, reservedFile, RegexOptions.IgnoreCase));
                if (!reserved)
                    File.Delete(f);
                else
                {
                    LogInfo(string.Format("{0} - normal", f));
                }
            }

            // revert FNAME
            string fname = Path.Combine(input, "FNAME");
            string content = @" 130119";
            File.WriteAllText(fname,content);
            LogInfo("Rewrite FNAME");

            // revert FNAME.txt
            content = 
@"    hangzhou_808_130119_011148
    hangzhou_808_130119_021201
    hangzhou_808_130119_031202";
            string fnametxt = Path.Combine(input, "FNAME.txt");
            File.WriteAllText(fnametxt, content);
            LogInfo("Rewrite FNAME.txt");
        }

        private static void Exit()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            LogInfo("Aeronet Inversion VER: 1.0.0.1");

            // initial
            LogInfo("Initial arguments");

            string STNS_FN= Utility.GetAppSettingValue("ARG_STNS_FN", @default: "hangzhou");
            LogInfo(string.Format("STNS_FN : {0}", STNS_FN));

            string STNS_ID = Utility.GetAppSettingValue("ARG_STNS_ID", @default: "808");
            LogInfo(string.Format("STNS_FN : {0}", STNS_ID));

            string FDATA = Utility.GetAppSettingValue("ARG_FDATA", @default: "hangzhou-808-1");
            LogInfo(string.Format("STNS_FN : {0}", FDATA));

            string ipt = ConfigOptions.FIPT;
            string @out = ConfigOptions.FOUT;
            string brdf = ConfigOptions.FBRDF;
            string dat = ConfigOptions.FDAT;

            LogInfo("Initial working folders");
            InitWorkingFolder(new string[] { ipt, @out, brdf, dat });

            LogInfo("Initial input & output folders");
            InitRegionFolder(STNS_FN);

#if !DEBUGMATLAB
            // initial creator command arguments
            string commandArgs = string.Format("{0} {1} {2} {3} {4} {5} {6}", STNS_FN, STNS_ID, FDATA, ipt, @out, brdf, dat);
            ProcessStartInfo creatorProInfo = NewStartInfo(ConfigOptions.PROGRAM_CREATOR, commandArgs);
            // show command line and args
            LogInfo(string.Format("{0} {1}", ConfigOptions.PROGRAM_CREATOR, commandArgs));
            LogInfo(string.Format("{0} = {1}", "STNS_FN", STNS_FN));
            LogInfo(string.Format("{0} = {1}", "STNS_ID", STNS_ID));
            LogInfo(string.Format("{0} = {1}", "FDATA", FDATA));
            LogInfo(string.Format("{0} = {1}", "FIPT", ipt));
            LogInfo(string.Format("{0} = {1}", "FOUT", @out));
            LogInfo(string.Format("{0} = {1}", "FBRDF", brdf));
            LogInfo(string.Format("{0} = {1}", "FDAT", dat));
            // perform creator process
            LogInfo("***************************************************************");
            bool sucess = Execute(creatorProInfo);
            LogInfo("***************************************************************");
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
            ProcessStartInfo outputorProInfo = NewStartInfo(ConfigOptions.PROGRAM_OUTPUTOR, string.Format("{0}", STNS_FN));
            // show command line and args
            LogInfo(string.Format("{0} {1}", ConfigOptions.PROGRAM_OUTPUTOR, STNS_FN));
            LogInfo(string.Format("{0} = {1}", "STNSSTR", STNS_FN));
            // perform outputor process
            LogInfo("***************************************************************");
            sucess = Execute(outputorProInfo);
            LogInfo("***************************************************************");
            if (!sucess)
            {
                Exit();
                return;
            }
#endif
            bool success = true;
            // draw aeronent inversion
            AeronetDrawNative.Drawing drawing = new AeronetDrawNative.Drawing();
            try
            {
                LogInfo("***************************************************************");
                string inputbase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"output", STNS_FN)+System.IO.Path.DirectorySeparatorChar;
                string outputbase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "cimel_network", STNS_FN,
                        "dubovik") + System.IO.Path.DirectorySeparatorChar;
                string outputfile = Path.Combine(outputbase,
                    string.Format("Dubovik_stats_{0}_{1}_{2:yyyyMMdd}.dat", STNS_FN, STNS_ID, DateTime.Now));

                if (!Directory.Exists(outputbase))
                    Directory.CreateDirectory(outputbase);

                // 1 calculate Matrix of aeronent
                LogInfo("Calculating Aeronet inversion Matrix");
                string mwInput = inputbase;
                string mwOutput = outputfile;
                // get lat and lon of region
                Region region = RegionStore.Singleton.FindRegion(STNS_FN);

                double lat = region.Lat;
                double lon = region.Lon;
                LogInfo("\tARGUMENTS:");
                LogInfo(string.Format("\t{0} : {1}", "INPUT", mwInput));
                LogInfo(string.Format("\t{0} : {1}", "OUTPUT", mwOutput));
                object[] results = drawing.MatrixAeronet(2,lat,lon,mwInput, mwOutput);
                var stats_inversion = results[0];
                var r = results[1];

                LogInfo("stats_inversions");
                Utility.PrintMatrix((double[,])stats_inversion,LogInfo);
                LogInfo("r");
                Utility.PrintMatrix((double[,])r,LogInfo);
                LogInfo("DONE to Calculate Aeronet inversion Matrix");

                // 2 draw SSA
                LogInfo("Drawing SSA figures");
                // MWArray mwYear = new MWNumericArray(new int[] {2013});
                // MWArray mwOuputbase = new MWCharArray(new string[]{ outputbase});
                double mwYear = 2013;
                string mwOuputbase = outputbase;
                string mwRegion = STNS_FN;

                LogInfo("\tARGUMENTS:");
                LogInfo(string.Format("\t{0} : {1}", "YEAR", mwYear));
                LogInfo(string.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawSSA(stats_inversion,r, mwYear,mwRegion, mwOuputbase);
                drawing.WaitForFiguresToDie();
                LogInfo("DONE to draw SSA figures");

                // 3 draw SSA Statistic
                LogInfo("Drawing SSA Statistic figures");
                // MWArray mwRegion = new MWCharArray(new string[]{ STNS_FN});
                LogInfo("\tARGUMENTS:");
                LogInfo(string.Format("\t{0} : {1}", "YEAR", mwYear));
                LogInfo(string.Format("\t{0} : {1}", "REGION", mwRegion));
                LogInfo(string.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawSSAStatistisc(stats_inversion, r,mwYear, mwRegion, mwOuputbase);
                drawing.WaitForFiguresToDie();
                LogInfo("DONE to draw SSA Statistic figures");

                // 4 draw Aeronet Inversions
                LogInfo("Drawing Aeronet Inversions figures");
                LogInfo("\tARGUMENTS:");
                LogInfo(string.Format("\t{0} : {1}", "OUTPUT", mwOuputbase));
                drawing.DrawAeronetInversions(stats_inversion,r, mwOuputbase);
                drawing.WaitForFiguresToDie();
                LogInfo("DONE to drawing Aeronet Inversions figures");
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                success = false;
            }
            finally
            {
                LogInfo("***************************************************************");
                drawing.Dispose();
            }
            if (!success)
            {
                Exit();
                return;
            }

            LogInfo("All jobs are complete!");
            Exit();
        }
    }
}
