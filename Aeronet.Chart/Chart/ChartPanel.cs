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
using Newtonsoft.Json.Linq;

namespace Aeronet.Chart.Chart
{
    public partial class ChartPanel : UserControl
    {
        public string DataConfigFile { get; set; }

        private string _dataFolder;
        private DataConfig _dataConfig;

        public ChartPanel()
        {
            InitializeComponent();
            // initial the chart control
            this.InitChart();
        }

        private void InitChart()
        {
            this.tsCmbDay.SelectedIndexChanged += tsCmbDay_SelectedIndexChanged;
            this.tsCmbMonth.SelectedIndexChanged += tsCmbMonth_SelectedIndexChanged;
            ChartArea caDefault = new ChartArea("Default");
            this.chart1.ChartAreas.Clear();
            this.chart1.ChartAreas.Add(caDefault);
            // X axis value labels
            caDefault.AxisX.Minimum = 350;
            caDefault.AxisX.Interval = 150;
            caDefault.AxisX.Maximum = 900;
            // X axis and Y axis Title
            caDefault.AxisX.Title = "X Axis";
            caDefault.AxisY.Title = "Y Axis";
            Legend lgDefault = new Legend("Default");
            this.chart1.Legends.Clear();
            this.chart1.Legends.Add(lgDefault);
            this.chart1.Series.Clear();
            this.chart1.Titles.Clear();
        }

        void tsCmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            // initial the combox of day
            this.tsCmbDay.Items.Clear();
            foreach (int day in this._dataConfig.MonthAndDays[(int)this.tsCmbMonth.SelectedItem])
            {
                this.tsCmbDay.Items.Add(day);
            }
            this.tsCmbDay.SelectedIndex = 0;
        }

        public void Init()
        {
            // enable the funtion
            this.Enabled = true;

            try
            {
                // the property DataConfigFile must be initialzied
                if (string.IsNullOrEmpty(this.DataConfigFile))
                    throw new NotImplementedException("The property DataConfigFile must be initialized.");

                if (!File.Exists(this.DataConfigFile))
                    throw new FileNotFoundException("Not found the data config file", this.DataConfigFile);

                // initial Folder
                this._dataFolder = Path.GetDirectoryName(this.DataConfigFile);

                // initial instance of DataConfig
                this._dataConfig = new DataConfig(this.DataConfigFile);

                // the label of year
                this.lblYear.Text = this._dataConfig.Year.ToString();

                // initial the combox of month
                this.tsCmbMonth.Items.Clear();
                foreach (int month in this._dataConfig.MonthAndDays.Keys.OrderBy(k => k).ToArray())
                {
                    this.tsCmbMonth.Items.Add(month);
                }
                this.tsCmbMonth.SelectedIndex = 0;
                
                // initial the combox of day
                this.tsCmbDay.Items.Clear();
                foreach (int day in this._dataConfig.MonthAndDays[(int)this.tsCmbMonth.SelectedItem])
                {
                    this.tsCmbDay.Items.Add(day);
                }
                this.tsCmbDay.SelectedIndex = 0;
                // register selectedChanged
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, @"Chart Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // disable all of funtions
                this.Enabled = false;
            }

        }

        private ChartLine[] LoadChartLines()
        {
            int year = this._dataConfig.Year;
            int month = (int)this.tsCmbMonth.SelectedItem;
            int day = (int)this.tsCmbDay.SelectedItem;
            string dataFolder = this._dataFolder;
            string dataName = this._dataConfig.Name;
            ChartReader chartReader = new ChartReader(dataFolder, dataName, year, month, day);
            ChartLine[] chartLines = chartReader.Read();
            return chartLines;
        }

        private void DrawChart(ChartLine[] chartLines)
        {
            int year = this._dataConfig.Year;
            int month = (int)this.tsCmbMonth.SelectedItem;
            int day = (int)this.tsCmbDay.SelectedItem;

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

        void tsCmbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            // loads chart data
            ChartLine[] chartLines = this.LoadChartLines();

            // draw chart
            this.DrawChart(chartLines);
        }
    }
}
