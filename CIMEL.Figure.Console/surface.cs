using CIMEL.Core;
using MathWorks.MATLAB.NET.Arrays;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CIMELDraw;

namespace CIMEL.Figure
{
    public class surface
    {
        public void Draw()
        {
            int year = 2016;
            int month = 12;
            int[] days = { 1, 2 };
            string dataName = "AAOD";
            string dataFolder = @"C:\Users\baikangwang\Projects\CIMEL\CIMELParas\chartset\Shijiazh\Shijiazh_1226_20190327_20190327012057660";
            DataConfigFile configFile = new DataConfigFile(Path.Combine(dataFolder, "AAOD.dataconfig"));
            Dictionary<int, List<ChartLine>> all = new Dictionary<int, List<ChartLine>>();
            List<string> allTimepoints = new List<string>();
            List<double> avgs = new List<double>();

            string figureName = string.Format("{0}{1}{2}_sf.png", year, month, string.Join(string.Empty, days.Select(d => d.ToString())));
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string file = Path.Combine(path, figureName);

            Info("{0}: {1}/{2} ({3})", "AAOD", month, year, String.Join(",", days));
            Info("Reading data...");

            for (int i = 0; i < days.Length; i++)
            {
                ChartReader reader = new ChartReader(dataFolder, dataName, 2016, 12, days[i]);
                ChartLine[] lines = reader.Read(configFile.AxisXs);
                all.Add(days[i], new List<ChartLine>(lines));
                foreach (ChartLine line in lines)
                {
                    if (!allTimepoints.Contains(line.TimePoint))
                        allTimepoints.Add(line.TimePoint);
                }
            }
            allTimepoints.Sort((left, right) => {
                TimeSpan tl = TimeSpan.Parse(left);
                TimeSpan tr = TimeSpan.Parse(right);

                if (tl > tr) return 1;
                else if (tl < tr) return -1;
                else return 0;
            });

            Info("Read data. OK");
            Show(days, month, year, all);
            Info("");
            Info("Timepoints: {0} points", allTimepoints.Count);
            foreach (string point in allTimepoints)
            {
                Info(point);
            }

            Info("Making fake data...");
            foreach (int day in all.Keys)
            {
                var lines = all[day];
                foreach (string timepoint in allTimepoints)
                {
                    if (lines.All(l => l.TimePoint != timepoint))
                        lines.Add(new ChartLine(timepoint, configFile.AxisXs, true));
                }
                all[day].Sort(new ChartLineCompare());
            }
            Show(days, month, year, all);

            Info("");
            Info("Make fake data. OK");

            Info("");
            Info("Inserting average values...");
            foreach (int d in all.Keys)
            {
                List<ChartLine> allLines = all[d];
                for (int i = 0; i < configFile.AxisXs.Count; i++)
                {
                    for (int t = 0; t < allLines.Count; t++)
                    {
                        if (allLines[t].IsFake && allLines[t].Points[i].Y == 0f)
                        {
                            double left = -1f, right = -1f;

                            // right
                            if (t == allLines.Count - 1) right = -1f;
                            else
                            {
                                if (allLines.Count == 1) right = -1f;
                                else
                                {
                                    var firstR = allLines.Skip<ChartLine>(t + 1).FirstOrDefault(l => l.Points[i].Y > 0f);
                                    right = firstR == null ? -1f : firstR.Points[i].Y;
                                }
                            }
                            //left
                            if (t == 0) left = -1f;
                            else left = allLines[t - 1].Points[i].Y;

                            if (left < 0f && right < 0f) { left = 0f; right = 0f; }
                            else if (left < 0f && right >= 0f) left = right;
                            else if (right < 0f && left > 0f) right = left;

                            allLines[t].Points[i].Y = (left + right) / 2f;
                        }
                    }
                }
            }

            Info("");
            Show(days, month, year, all);
            Info("Insert average values. OK");

            Info("Outputting...");

            string[] strOptions = configFile.AxisXLabels.ToArray();
            MWCharArray dataOptions = new MWCharArray(strOptions);

            MWCharArray strTitle = new MWCharArray(dataName);

            string[] strDates = days.Select(d => { return string.Format("{0}-{1}-{2}", year, month, d); }).ToArray();
            MWCharArray arrDates = new MWCharArray(strDates);

            // initial time matrix
            MWCharArray arrTimes = new MWCharArray(allTimepoints.ToArray());

            // initial data matrix
            MWCellArray arrDatas = new MWCellArray(strOptions.Length);
            for (int o = 0; o < strOptions.Length; o++)
            {
                double x = configFile.AxisXs[o];
                MWNumericArray optionData = new MWNumericArray(new int[] { days.Length, allTimepoints.Count });

                for (int i = 0; i < days.Length; i++)
                {
                    var lines = all[days[i]];
                    var oneDayData = lines.SelectMany(l => l.Points.Where(p => p.X == x).Select(p => p.Y)).ToArray();
                    for (int j = 0; j < oneDayData.Length; j++)
                    {
                        optionData[i + 1, j + 1] = oneDayData[j];
                    }
//                    optionData[i + 1] = oneDayData;
                }

                arrDatas[o + 1] = optionData;
            }

            Drawing drawing = new Drawing();

            MWArray result = drawing.DrawSurface(strTitle, arrDatas, dataOptions, arrDates, arrTimes, file, new MWLogicalArray(true));
            string output = result.ToString();

            System.Console.Read();
        }

        private void Info(string msg, params object[] args)
        {
            string message = string.Format(msg, args);
            Logger.Default.Info(message);
            System.Console.WriteLine(message);
        }

        private void Show(int[] days, int month, int year, Dictionary<int, List<ChartLine>> all)
        {
            foreach (int day in days)
            {

                Info("{0}/{1}/{2}", day, month, year);
                foreach (ChartLine line in all[day])
                {
                    Info("{0:c}: {1}", TimeSpan.Parse(line.TimePoint), string.Join(",", line.Points.Select(p => string.Format("{0} = {1:F4}", p.X, p.Y))));
                }
            }
        }
    }
}
