using CIMEL.Core;
using CIMELDraw;
using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CIMEL.Figure
{
    class multiplelines
    {
        public void Draw() {
            // draw aeronent inversion
            Drawing drawing = new Drawing();

            try
            {
                int year = 2016;
                int month = 12;
                int[] days = { 1, 2 };
                string dataName = "AAOD";
                string dataFolder = @"C:\Users\baikangwang\Projects\CIMEL\CIMELParas\chartset\Shijiazh\Shijiazh_1226_20190327_20190327012057660";
                DataConfigFile configFile = new DataConfigFile(Path.Combine(dataFolder, "AAOD.dataconfig"));
                Dictionary<int, List<ChartLine>> all = new Dictionary<int, List<ChartLine>>();
                //Info("{0}: {1}/{2} ({3})", "AAOD", month, year, String.Join(",", days));
                //Info("Reading data...");
                string figureName = string.Format("{0}{1}{2}_ml.png", year, month, string.Join(string.Empty, days.Select(d => d.ToString())));
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string file = Path.Combine(path, figureName);

                for (int i = 0; i < days.Length; i++)
                {
                    ChartReader reader = new ChartReader(dataFolder, dataName, 2016, 12, days[i]);
                    ChartLine[] lines = reader.Read(configFile.AxisXs);
                    all.Add(days[i], new List<ChartLine>(lines));
                }
                //Info("Read data. OK");

                string[] strOptions = configFile.AxisXLabels.ToArray();
                MWCellArray dataOptions = new MWCellArray(new MWCharArray(strOptions));

                MWCharArray strTitle = new MWCharArray(dataName);

                string[] strDates = days.Select(d => { return string.Format("{0}-{1}-{2}", year, month, d); }).ToArray();
                MWCellArray arrDates = new MWCellArray(new MWCharArray(strDates));

                // initial time matrix
                MWCellArray arrTimes = new MWCellArray(days.Length);
                for (int i = 0; i < days.Length; i++)
                {
                    int day = days[i];
                    var lines = all[day];
                    var arrTimepoints = lines.Select(l => l.TimePoint).ToList();
                    // based one
                    arrTimes[i + 1] = new MWCellArray(new MWCharArray(arrTimepoints.ToArray()));
                }

                // initial data matrix
                MWCellArray arrDatas = new MWCellArray(strOptions.Length);
                for (int o = 0; o < strOptions.Length; o++)
                {
                    double x = configFile.AxisXs[o];
                    MWCellArray optionData = new MWCellArray(days.Length);

                    for (int i = 0; i < days.Length; i++)
                    {
                        var lines = all[days[i]];
                        var oneDayData = lines.SelectMany(l => l.Points.Where(p => p.X == x).Select(p => p.Y)).ToArray();
                        optionData[i + 1] = new MWNumericArray(oneDayData);
                    }

                    arrDatas[o + 1] = optionData;
                }


                MWArray result = drawing.DrawMultiplelines(strTitle, arrDatas, dataOptions, arrDates, arrTimes, file, new MWLogicalArray(true));
                string output = result.ToString();
                if (File.Exists(output))
                    Process.Start(output);
                else
                    OnFailed("[ERROR]: Multiple lines figure FAILED.");

            }
            catch (Exception ex)
            {
                OnFailed(ex.Message);
            }
            finally
            {
                drawing.Dispose();
            }
        }

        /// <summary>
        /// put the error message to error stream
        /// </summary>
        /// <param name="error"></param>
        private void OnFailed(string error)
        {
            System.Console.Error.WriteLine(error);
        }

        /// <summary>
        /// put the info message to std output stream
        /// </summary>
        /// <param name="info"></param>
        private void OnInformed(string info)
        {
            System.Console.Out.WriteLine(info);
        }
    }
}
