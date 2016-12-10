using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Aeronet.Core;

namespace Aeronet.Chart
{
    public partial class ChartPanel : UserControl
    {
        public string DataConfigFile { get; set; }

        private string _dataFolder;
        private DataConfigFile _dataConfigFile;

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
            // X axis and Y axis Title
            caDefault.AxisX.Title = string.Empty;//"X Axis";
            caDefault.AxisY.Title = string.Empty;// "Y Axis";
            Legend lgDefault = new Legend("Default");
            this.chart1.Legends.Clear();
            this.chart1.Legends.Add(lgDefault);
            this.chart1.Series.Clear();
            this.chart1.Titles.Clear();
        }

        private void tsCmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the combo box of day
            this.tsCmbDay.Enabled = false;
            // initial the combox of day
            this.tsCmbDay.Items.Clear();
            this.DisableChart();

            if (string.IsNullOrEmpty(this.tsCmbMonth.Text) || this.tsCmbMonth.Text == ComboBoxItem.EmptyItem.Text)
                return; // do nothing

            foreach (int day in this._dataConfigFile.MonthAndDays[(int)this.tsCmbMonth.SelectedItem])
            {
                this.tsCmbDay.Items.Add(day);
            }
            // insert empty item
            this.tsCmbDay.Items.Insert(0, ComboBoxItem.EmptyItem.Text);
            // enable the combo box of day
            this.tsCmbDay.Enabled = true;
            // initial to select the empty item
            this.tsCmbDay.SelectedIndex = 0;
        }

        public void Init()
        {
            try
            {
                //disiable the comboxes of month and day
                this.tsCmbMonth.Enabled = false;
                this.tsCmbDay.Enabled = false;
                this.chart1.Enabled = false;

                // the property DataConfigFile must be initialzied
                if (string.IsNullOrEmpty(this.DataConfigFile))
                    throw new NotImplementedException("属性 DataConfigFile 没有初始化");

                if (!File.Exists(this.DataConfigFile))
                    throw new FileNotFoundException("没有找到图像集文件(.dataconfig)", this.DataConfigFile);

                // initial Folder
                this._dataFolder = Path.GetDirectoryName(this.DataConfigFile);

                // initial instance of DataConfigFile
                this._dataConfigFile = new DataConfigFile(this.DataConfigFile);

                // the label of year
                this.lblYear.Text = this._dataConfigFile.Year.ToString();
                ChartArea caDefault = this.chart1.ChartAreas["Default"];
                caDefault.AxisY.Title = string.Format("{0} {1}", this._dataConfigFile.Name,
                    this._dataConfigFile.Description);

                // clean up the custom labels
                caDefault.AxisX.CustomLabels.Clear();
                // set to default
                caDefault.AxisX.LabelStyle.Angle = 0;
                caDefault.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont;

                double min;
                double max;
                if (this._dataConfigFile.AxisXs.Count == 0)
                {
                    min = 0f;
                    max = 1200f;
                }
                else if (this._dataConfigFile.Name == "SD")
                {
                    min = 0f;
                    max = 15.05f;
                }
                else
                {
                    double first = this._dataConfigFile.AxisXs[0];
                    double last = this._dataConfigFile.AxisXs[this._dataConfigFile.AxisXs.Count-1];

                    double avgDiff = Math.Round((last - first)/this._dataConfigFile.AxisXs.Count,3);
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
                if (this._dataConfigFile.Name != "SD")
                {
                    for (int i = 0; i < this._dataConfigFile.AxisXs.Count; i++)
                    {
                        // the axis labels collection has the same length as the axis values
                        string label = this._dataConfigFile.AxisXLabels[i].ToUpper();
                        var sizeF = this.CreateGraphics().MeasureString(label, chart1.Font);
                        var diff = sizeF.Width / 2f;
                        var axisX = this._dataConfigFile.AxisXs[i];
                        double fromPosition = axisX - diff - offset <= 0 ? 0f : axisX - diff - offset;
                        double toPosition = axisX + diff + offset;
                        caDefault.AxisX.CustomLabels.Add(new CustomLabel(fromPosition, toPosition, label, 0, LabelMarkStyle.None, GridTickTypes.All));
                    }
                }

                // initial the combox of month
                this.tsCmbMonth.Items.Clear();
                foreach (int month in this._dataConfigFile.MonthAndDays.Keys.OrderBy(k => k).ToArray())
                {
                    this.tsCmbMonth.Items.Add(month);
                }

                // insert empty item
                this.tsCmbMonth.Items.Insert(0, ComboBoxItem.EmptyItem.Text);
                // enable month combo box
                this.tsCmbMonth.Enabled = true;
                // initial to select the empty item
                this.tsCmbMonth.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, @"图像加载错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // disable all of funtions
                this.Enabled = false;
            }
        }

        private ChartLine[] LoadChartLines()
        {
            int year = this._dataConfigFile.Year;
            int month = (int)this.tsCmbMonth.SelectedItem;
            int day = (int)this.tsCmbDay.SelectedItem;
            string dataFolder = this._dataFolder;
            string dataName = this._dataConfigFile.Name;
            ChartReader chartReader = new ChartReader(dataFolder, dataName, year, month, day);
            ChartLine[] chartLines = chartReader.Read(this._dataConfigFile.AxisXs);
            return chartLines;
        }

        private void DrawChart(ChartLine[] chartLines)
        {
            int year = this._dataConfigFile.Year;
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

        private void tsCmbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the chart panel
            this.DisableChart();

            //skip when selected empty item
            if (string.IsNullOrEmpty(tsCmbDay.Text) || tsCmbDay.Text == ComboBoxItem.EmptyItem.Text)
                return; // do nothing

            // loads chart data
            ChartLine[] chartLines = this.LoadChartLines();

            // draw chart
            this.DrawChart(chartLines);

            this.EnableChart();
        }

        private void DisableChart()
        {
            this.chart1.Titles.Clear();
            this.chart1.Series.Clear();
            this.chart1.Enabled = false;
        }

        private void EnableChart()
        {
            this.chart1.Enabled = true;
        }

        public void Disable()
        {
            this.lblYear.Text = ComboBoxItem.EmptyItem.Text;
            this.tsCmbMonth.Items.Clear();
            this.tsCmbMonth.Text = ComboBoxItem.EmptyItem.Text;
            this.tsCmbDay.Items.Clear();
            this.tsCmbDay.Text = ComboBoxItem.EmptyItem.Text;
            this.DisableChart();
            this.Enabled = false;
        }

        public void Enable()
        {
            this.Enabled = true;
        }
    }
}