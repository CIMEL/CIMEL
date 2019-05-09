using CIMEL.Chart;
using CIMEL.Core;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMEL.Test
{
    public class Agv
    {
        public void Run()
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

            List<ChartLine> allLines = all.Values.SelectMany(v => v).ToList();
            for (int i = 0; i < configFile.AxisXs.Count; i++)
            {
                double avg = allLines.Select(l => l.Points[i]).Average(point => point.Y);
                foreach (int day in all.Keys)
                {
                    foreach (ChartLine l in all[day])
                    {
                        if (l.IsFake && l.Points[i].Y == 0f)
                            l.Points[i].Y = avg;
                    }
                }
                avgs.Add(avg);
            }
            Info("Make fake data. OK");

            Info("Avgs:");
            for (int i = 0; i < configFile.AxisXs.Count; i++)
            {
                Info("{0} = {1}", configFile.AxisXs[i], avgs[i]);
            }
            Info("");
            Show(days, month, year, all);

            Info("Outputting...");
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} = [{1}]", "legends", String.Join(",", configFile.AxisXs));
            sb.AppendLine();
            for (int i = 0; i < configFile.AxisXs.Count; i++)
            {
                sb.AppendLine("legend = " + configFile.AxisXs[i]);
                sb.AppendLine(String.Format("x = [{0}]", String.Join(",", days)));
                bool showY = false;
                foreach (int day in days)
                {
                    if (!showY)
                    {
                        //sb.AppendFormat("{0}-{1}-{2}", year, month, day);
                        //sb.AppendLine();
                        sb.AppendFormat("times = [{0}]", String.Join(",", all[day].Select(l => "'" + l.TimePoint + "'")));
                        sb.AppendLine();
                        sb.AppendFormat("y = [{0}]", String.Join(",", all[day].Select((l, index) => index + 1)));
                        sb.AppendLine();
                        showY = true;
                    }
                    sb.AppendLine(string.Format("z = [{0}]", String.Join(",", all[day].Select(l => l.Points[i].Y))));
                }
                sb.AppendLine();
            }
            string file = Path.Combine(@"C:\Users\baikangwang\Projects\CIMEL\CIMELParas\chartset\Shijiazh\", dataName + ".matlab");
            File.WriteAllText(file, sb.ToString(), Encoding.UTF8);
            Info("Output. OK");
            Info("file = {0}", file);
            Console.Read();
        }

        private void Info(string msg, params object[] args)
        {
            string message = string.Format(msg, args);
            Logger.Default.Info(message);
            Console.WriteLine(message);
        }

        private void Show(int[] days, int month, int year, Dictionary<int, List<ChartLine>> all)
        {
            foreach (int day in days)
            {

                Info("{0}/{1}/{2}", day, month, year);
                foreach (ChartLine line in all[day])
                {
                    Info("{0}: {1}", line.TimePoint, string.Join(",", line.Points.Select(p => string.Format("{0} = {1}", p.X, p.Y))));
                }
            }
        }
    }
}
