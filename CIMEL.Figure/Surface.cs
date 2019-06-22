using CIMEL.Core;
using CIMEL.Core.Event;
using CIMEL.Core.Exception;
using CIMELDraw;
using MathWorks.MATLAB.NET.Arrays;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CIMEL.Figure
{
    public class Surface
    {
        public event EventHandler<MessageArgs> Info;

        public static string FigureName(string dataName, int year, int month, int[] days) {
            string figureName = string.Format("{3}_{0}{1}{2}_sf.png", year, month, string.Join(string.Empty, days.Select(d => d.ToString())),dataName);
            return figureName;
        }

        public string Draw(int year,int month,int[] days, DataConfigFile configFile, string dataFolder)
        {
            if (configFile == null)
                throw new ArgumentException("参数 DataConfigFile 没有初始化");

            if (dataFolder == null || string.IsNullOrEmpty(dataFolder))
                throw new ArgumentNullException("dataFolder");

            if (!Directory.Exists(dataFolder))
                throw new ArgumentException("数据不存在: " + dataFolder);

            string dataName = configFile.Name;
            Dictionary<int, List<ChartLine>> all = new Dictionary<int, List<ChartLine>>();
            List<string> allTimepoints = new List<string>();

            this.OnInfo("{0}: {1}/{2} ({3})", dataName, month, year, String.Join(",", days));
            OnInfo("读取数据中...");
            string figureName = FigureName(dataName, year, month, days);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string file = Path.Combine(path, figureName);

            // draw aeronent inversion
            using (Drawing drawing = new Drawing())
            {
                try
                {
                    OnInfo("读取观测日期数据...");
                    // 初始化天的数组（X轴）
                    string[] strDates = days.Select(d => { return string.Format("{0}-{1}-{2}", year, month, d); }).ToArray();
                    MWCharArray arrDates = new MWCharArray(strDates);
                    OnInfo("读取观测日期数据完毕");

                    OnInfo("读取观测时间数据...");
                    // 初始化时序数组（Y轴）
                    for (int i = 0; i < days.Length; i++)
                    {
                        ChartReader reader = new ChartReader(dataFolder, dataName, year, month, days[i]);
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

                    MWCharArray arrTimes = new MWCharArray(allTimepoints.ToArray());
                    OnInfo("读取观测时间数据完毕");

                    OnInfo("读取观测项数据...");
                    // 过滤掉没有观测数据的观测项
                    List<double> arrOptions = configFile.AxisXs; // 假设所有观测项都有测试数据
                    List<string> arrStrOptions = new List<string>(configFile.AxisXLabels);
                    List<double> arrFiltered = new List<double>(arrOptions);
                    List<string> arrStrFiltered = new List<string>(arrStrOptions);
                    for (int i=0; i<arrStrOptions.Count;i++) {
                        string strOpt = arrStrOptions[i];
                        foreach (int d in days) {
                            var lines = all[d];
                            if (lines.Any(l => l.Points.Where(p => {
                                if (string.IsNullOrEmpty(p.AxisLabelX))
                                {
                                    // X轴标签==null时，比较X轴值
                                    double dOpt = arrOptions[i];
                                    return p.X == dOpt;
                                }
                                else
                                    return p.AxisLabelX.Equals(strOpt);
                            }).Count() == 0))
                            {
                                // 只要某观测项不存在观测数据就剔除掉
                                arrStrFiltered.Remove(strOpt);
                                arrFiltered.Remove(arrOptions[i]);
                                break;
                            }
                        }
                    }

                    // 重置观测项数组
                    string[] strOptions = arrStrFiltered.ToArray();
                    double[] dOptions = arrFiltered.ToArray();
                    // 初始化图例数组
                    MWCharArray dataOptions = new MWCharArray(strOptions);
                    OnInfo("读取观测项数据完毕");

                    OnInfo("读取数据...");
                    OnInfo("计算趋势数据...");
                    foreach (int day in all.Keys)
                    {
                        var lines = all[day];
                        foreach (string timepoint in allTimepoints)
                        {
                            if (lines.All(l => l.TimePoint != timepoint))
                                lines.Add(new ChartLine(timepoint, arrOptions, true));
                        }
                        all[day].Sort(new ChartLineCompare());
                    }
                    foreach (int d in all.Keys)
                    {
                        List<ChartLine> allLines = all[d];
                        for (int i = 0; i < strOptions.Length; i++)
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
                    OnInfo("计算趋势数据完成");
                    // 初始化观测数据矩阵（Z轴），先按观测项分组，每组又按天分组，每天的数据组包含观测时序的数据
                    MWCellArray arrDatas = new MWCellArray(strOptions.Length);
                    for (int o = 0; o < strOptions.Length; o++)
                    {
                        double x = dOptions[o];
                        MWNumericArray optionData = new MWNumericArray(new int[] { days.Length, allTimepoints.Count });

                        for (int i = 0; i < days.Length; i++)
                        {
                            var lines = all[days[i]];
                            var oneDayData = lines.SelectMany(l => l.Points.Where(p => p.X == x).Select(p => p.Y)).ToArray();
                            for (int j = 0; j < oneDayData.Length; j++)
                            {
                                optionData[i + 1, j + 1] = oneDayData[j];
                            }
                        }

                        arrDatas[o + 1] = optionData;
                    }

                    OnInfo("读取数据完毕");

                    OnInfo("读取标题...");
                    // 初始化线图的标题
                    if (dataName == @"AE_AAE")
                        dataName = @"AE/AAE";

                    MWCharArray strTitle = new MWCharArray(dataName);
                    OnInfo("读取标题完毕");

                    OnInfo("绘制中...");
                    MWArray result = drawing.DrawSurface(strTitle, arrDatas, dataOptions, arrDates, arrTimes, file, new MWLogicalArray(false));
                    OnInfo("绘制完毕");

                    string output = result.ToString();

                    if (File.Exists(output))
                        return output;
                    else
                        throw new DrawException("3D面图绘制失败");

                }
                catch (Exception ex) when (!(ex is DrawException))
                {
                    Logger.Default.Error(ex.Message, ex);
                    throw new DrawException("3D面图绘制失败: " + ex.Message);
                }
            }
        }


        protected void OnInfo(string message) {

            if (Info != null) Info(this, new MessageArgs(message));
        }

        protected void OnInfo(string format,params object[] args)
        {
            this.OnInfo(string.Format(format, args));
        }
    }
}
