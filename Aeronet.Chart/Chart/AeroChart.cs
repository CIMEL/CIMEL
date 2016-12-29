using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Aeronet.Core;

namespace Aeronet.Chart
{
    public partial class AeroChart : UserControl
    {
        public string DataFolder { get { return this._dataFolder; } set { this._dataFolder = value; } }
        public DataConfigFile DataConfigFile { get { return _objDataConfigFile; } set { _objDataConfigFile = value; } }

        private string _dataFolder;
        private DataConfigFile _objDataConfigFile;

        public AeroChart()
        {
            InitializeComponent();
            this.Init(true);
        }

        private void Init(bool state)
        {
            ChartArea caDefault = new ChartArea("Default");
            this.chart1.ChartAreas.Clear();
            this.chart1.ChartAreas.Add(caDefault);
            // X axis and Y axis Title
            caDefault.AxisX.Title = string.Empty;//"X Axis";
            caDefault.AxisY.Title = string.Empty;// "Y Axis";
            Legend lgDefault = new Legend("Default");
            this.chart1.Legends.Clear();
            this.chart1.Legends.Add(lgDefault);
            this.chart1.Series.Clear();
            this.chart1.Titles.Clear();
        }

        public void Draw(int year,int month,int day)
        {
            // loads chart data
            ChartLine[] chartLines = this.LoadChartLines(year,month,day);

            // draw chart
            this.DrawChart(chartLines, year, month, day);
        }

        public void Init()
        {
            //disiable
            this.Disable();

            if (this.DataConfigFile == null)
                throw new NotImplementedException("属性 DataConfigFile 没有初始化");

            if (this.DataFolder == null)
                throw new NotImplementedException("属性 DataFolder 没有初始化");


            // the label of year
            ChartArea caDefault = this.chart1.ChartAreas["Default"];

            // initial Y title
            var notesY = this.DataConfigFile.NotesY.ToList();
            // Just looks NotesY for Axis Y title
            //notesY.Insert(0,string.Format("{0} {1}", this.DataConfigFile.Name,
            //    this.DataConfigFile.Description));
            caDefault.AxisY.Title = string.Join(Environment.NewLine, notesY);

            // initial X title
            var notesX = this.DataConfigFile.NotesX;
            if (notesX.Count > 0)
                caDefault.AxisX.Title = string.Join(Environment.NewLine, notesX);

            // initial Axis X range
            double min;
            double max;

            double first = this._objDataConfigFile.AxisXs[0];
            double last = this._objDataConfigFile.AxisXs[this._objDataConfigFile.AxisXs.Count - 1];

            double avgDiff = Math.Round((last - first)/(double) this._objDataConfigFile.AxisXs.Count, 1,
                MidpointRounding.ToEven);

            if (first - avgDiff <= 0f)
            {
                max = last + first;
                min = 0f;
            }
            else
            {
                min = first - avgDiff;
                max = last + avgDiff;
            }

            caDefault.AxisX.Minimum = Math.Round(min, 1, MidpointRounding.ToEven);
            caDefault.AxisX.Maximum = Math.Round(max, 1, MidpointRounding.ToEven);

            caDefault.AxisX.IsLabelAutoFit = true;
            caDefault.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            // clean up the custom labels
            caDefault.AxisX.CustomLabels.Clear();
            // set to default
            caDefault.AxisX.LabelStyle.Angle = 0;
            caDefault.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont;

            // add custom labels on Axis X for non-SD chart
            if (this._objDataConfigFile.Name == "SD")
            {
                caDefault.AxisX.Interval = 2f;

            }

            int baseIndex = 0;
            int labelCount = this._objDataConfigFile.AxisXLabels.Count;
            int step = this._objDataConfigFile.Name == "SD" ? 2 : 1;

            for (int i = 0; i < labelCount; i++)
            {

                // the axis labels collection has the same length as the axis values
                string label = this._objDataConfigFile.AxisXLabels[i].ToUpper();
                int curIndex = baseIndex + i*step;
                int neighborIndex = i == labelCount - 1 ? baseIndex + (i - 1)*step : baseIndex + (i + 1)*step;

                var axisX = this._objDataConfigFile.AxisXs[curIndex];
                var axisXn = this._objDataConfigFile.AxisXs[neighborIndex];
                var offset = Math.Abs(axisXn - axisX)/2f;
                double fromPosition = axisX - offset <= caDefault.AxisX.Minimum
                    ? caDefault.AxisX.Minimum
                    : axisX - offset;
                double toPosition = axisX + offset >= caDefault.AxisX.Maximum ? caDefault.AxisX.Maximum : axisX + offset;
                caDefault.AxisX.CustomLabels.Add(new CustomLabel(fromPosition, toPosition, label, 0, LabelMarkStyle.None,
                    GridTickTypes.All));
            }
        }

        public void Disable()
        {
            this.chart1.Titles.Clear();
            this.chart1.Series.Clear();
            this.chart1.Enabled = false;
            this.chart1.Refresh();
        }

        public void Enable()
        {
            this.chart1.Enabled = true;
            this.chart1.Refresh();
        }

        private ChartLine[] LoadChartLines(int year,int month,int day)
        {
            if (this.DataFolder == null)
                throw new NotImplementedException("属性 DataFolder 没有初始化");

            string dataFolder = this._dataFolder;
            string dataName = this._objDataConfigFile.Name;
            ChartReader chartReader = new ChartReader(dataFolder, dataName, year, month, day);
            ChartLine[] chartLines = chartReader.Read(this._objDataConfigFile.AxisXs);
            return chartLines;
        }

        private void DrawChart(ChartLine[] chartLines,int year,int month,int day)
        {
            // initial title
            this.chart1.Titles.Clear();
            var notes = this.DataConfigFile.Notes.ToList();
            notes.Insert(0, string.Format("{0} / {1} / {2}", month, day, year));
            this.chart1.Titles.Add(new Title(string.Join(Environment.NewLine, notes), Docking.Top));

            double first=-1f;
            double last=-1f;
            // add series
            this.chart1.Series.Clear();
            foreach (ChartLine chartLine in chartLines)
            {
                Series timeLine = new Series(chartLine.TimePoint);
                timeLine.Legend = "Default";
                timeLine.ChartArea = "Default";
                foreach (var point in chartLine.Points)
                {
                    // initial min and max guards of Axis Y
                    if (first < 0)
                        first = point.Y;
                    if (last < 0)
                        last = point.Y;

                    // check if it is great than the max guard of Axis Y
                    if (point.Y > last)
                        last = point.Y;
                    // check if it is less than the min guard of Axis Y
                    if (point.Y < first)
                        first = point.Y;

                    timeLine.Points.Add(new DataPoint(point.X, point.Y));
                }

                if (this._objDataConfigFile.Name == "AE_AAE")
                {
                    /*
                     * AE和AAE使用不同形状的点表示，之间不用连线，只显示点就可以
                     */
                    // Shows AE points as square
                    if (timeLine.Points.Count > 0)
                        timeLine.Points[0].MarkerStyle = MarkerStyle.Diamond;
                    // Shows AAE points as cross
                    if (timeLine.Points.Count > 1)
                        timeLine.Points[1].MarkerStyle = MarkerStyle.Cross;

                    timeLine.ChartType = SeriesChartType.Point;
                }
                else
                {
                    timeLine.MarkerStyle = MarkerStyle.Diamond;
                    timeLine.ChartType = SeriesChartType.Line;
                }

                // initial marker size
                timeLine.MarkerSize = 10;
                // bold line
                timeLine.BorderWidth = 4;
                this.chart1.Series.Add(timeLine);
            }

            // optimize Axis Y range
            var caDefault = this.chart1.ChartAreas["Default"];
            caDefault.AxisY.IntervalAutoMode=IntervalAutoMode.VariableCount;
            double avgDiff = (last - first)/chartLines.Length;
            double max, min;
            // check if min is down below Zero
            if (first - avgDiff <= 0)
            {
                max = last + first;
                min = 0f;
            }
            else
            {
                max = last + avgDiff;
                min = first - avgDiff;
            }

            caDefault.AxisY.Minimum = Math.Round(min, 3, MidpointRounding.ToEven);
            caDefault.AxisY.Maximum = Math.Round(max, 3, MidpointRounding.ToEven);
        }
    }
}
