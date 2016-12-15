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
        public string DataConfigFile { get; set; }

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

            // the property DataConfigFile must be initialzied
            if (string.IsNullOrEmpty(this.DataConfigFile))
                throw new NotImplementedException("属性 DataConfigFile 没有初始化");

            if (!File.Exists(this.DataConfigFile))
                throw new FileNotFoundException("没有找到图像集文件(.dataconfig)", this.DataConfigFile);

            // initial Folder
            this._dataFolder = Path.GetDirectoryName(this.DataConfigFile);

            // initial instance of DataConfigFile
            this._objDataConfigFile = new DataConfigFile(this.DataConfigFile);

            // the label of year
            ChartArea caDefault = this.chart1.ChartAreas["Default"];
            caDefault.AxisY.Title = string.Format("{0} {1}", this._objDataConfigFile.Name,
                this._objDataConfigFile.Description);

            // clean up the custom labels
            caDefault.AxisX.CustomLabels.Clear();
            // set to default
            caDefault.AxisX.LabelStyle.Angle = 0;
            caDefault.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont;

            double min;
            double max;
            if (this._objDataConfigFile.AxisXs.Count == 0)
            {
                min = 0f;
                max = 1200f;
            }
            else if (this._objDataConfigFile.Name == "SD")
            {
                min = 0f;
                max = 15.05f;
            }
            else
            {
                double first = this._objDataConfigFile.AxisXs[0];
                double last = this._objDataConfigFile.AxisXs[this._objDataConfigFile.AxisXs.Count - 1];

                double avgDiff = Math.Round((last - first) / this._objDataConfigFile.AxisXs.Count, 3);
                if (first - avgDiff <= 0f)
                    min = 0f;
                else
                    min = first - avgDiff;
                max = last + avgDiff;
            }

            caDefault.AxisX.Minimum = min;
            caDefault.AxisX.Maximum = max;
            var offset = 50f;
            // add custom labels for non-SD chart
            if (this._objDataConfigFile.Name != "SD")
            {
                for (int i = 0; i < this._objDataConfigFile.AxisXs.Count; i++)
                {
                    // the axis labels collection has the same length as the axis values
                    string label = this._objDataConfigFile.AxisXLabels[i].ToUpper();
                    var sizeF = this.CreateGraphics().MeasureString(label, chart1.Font);
                    var diff = sizeF.Width / 2f;
                    var axisX = this._objDataConfigFile.AxisXs[i];
                    double fromPosition = axisX - diff - offset <= 0 ? 0f : axisX - diff - offset;
                    double toPosition = axisX + diff + offset;
                    caDefault.AxisX.CustomLabels.Add(new CustomLabel(fromPosition, toPosition, label, 0, LabelMarkStyle.None, GridTickTypes.All));
                }
            }
        }

        public void Disable()
        {
            this.chart1.Titles.Clear();
            this.chart1.Series.Clear();
            this.chart1.Enabled = false;
        }

        public void Enable()
        {
            this.chart1.Enabled = true;
        }

        private ChartLine[] LoadChartLines(int year,int month,int day)
        {
            //int year = this._dataConfigFile.Year;
            //int month = (int)this.tsCmbMonth.SelectedItem;
            //int day = (int)this.tsCmbDay.SelectedItem;
            string dataFolder = this._dataFolder;
            string dataName = this._objDataConfigFile.Name;
            ChartReader chartReader = new ChartReader(dataFolder, dataName, year, month, day);
            ChartLine[] chartLines = chartReader.Read(this._objDataConfigFile.AxisXs);
            return chartLines;
        }

        private void DrawChart(ChartLine[] chartLines,int year,int month,int day)
        {
            //int year = this._dataConfigFile.Year;
            //int month = (int)this.tsCmbMonth.SelectedItem;
            //int day = (int)this.tsCmbDay.SelectedItem;

            this.chart1.Titles.Clear();
            this.chart1.Titles.Add(new Title(string.Format("{0} / {1} / {2}", month, day, year), Docking.Top)
            {
                DockedToChartArea = "Default",
                IsDockedInsideChartArea = false
            });

            // add series
            this.chart1.Series.Clear();
            foreach (ChartLine chartLine in chartLines)
            {
                Series timeLine = new Series(chartLine.TimePoint);
                timeLine.Legend = "Default";
                timeLine.ChartType = SeriesChartType.Line;
                timeLine.ChartArea = "Default";
                foreach (var point in chartLine.Points)
                {
                    timeLine.Points.Add(new DataPoint(point.X, point.Y) { MarkerSize = 5, MarkerStyle = MarkerStyle.Square });
                }
                this.chart1.Series.Add(timeLine);
            }
        }
    }
}
