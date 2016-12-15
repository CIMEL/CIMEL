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
        public List<string> DataConfigFiles { get; private set; }

        // private string _dataFolder;
        // private DataConfigFile _dataConfigFile;

        public ChartPanel()
        {
            this.DataConfigFiles=new List<string>();

            InitializeComponent();
            // initial the chart control
            this.Init(true);
        }

        private void Init(bool state)
        {
            this.tsCmbDay.SelectedIndexChanged += tsCmbDay_SelectedIndexChanged;
            this.tsCmbMonth.SelectedIndexChanged += tsCmbMonth_SelectedIndexChanged;
            // remove all tab pages
            // todo: register events to tabcontrol later
            this.tabControl1.TabPages.Clear();
        }

        private void tsCmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the combo box of day
            this.tsCmbDay.Enabled = false;
            // initial the combox of day
            this.tsCmbDay.Items.Clear();
            // todo: call disable from chart control
            // this.DisableChart();

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
                this.tabControl1.Enabled = false;

                // the property DataConfigFile must be initialzied
                if (this.DataConfigFiles.Count==0 || this.DataConfigFiles.All(string.IsNullOrEmpty))
                    throw new NotImplementedException("属性 DataConfigFiles 没有初始化");

                DataConfigFile objFirstDataConfig;

                for (int i=0;i<this.DataConfigFiles.Count;i++)
                {
                    string strDataConfigFile = this.DataConfigFiles[i];
                    if (!File.Exists(strDataConfigFile))
                        throw new FileNotFoundException("没有找到图像集文件(.dataconfig)", strDataConfigFile);

                    // initial instance of DataConfigFile
                    DataConfigFile objDataConfigFile = new DataConfigFile(strDataConfigFile);

                    if (i == 0)
                        objFirstDataConfig = objDataConfigFile;


                }


                // initial Folder
                // this._dataFolder = Path.GetDirectoryName(this.DataConfigFile);

                this._dataConfigFile = new DataConfigFile(this.DataConfigFiles);

                // the label of year
                this.lblYear.Text = objFirstDataConfig.Year.ToString();


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

        //private ChartLine[] LoadChartLines()
        //{
        //    int year = this._dataConfigFile.Year;
        //    int month = (int)this.tsCmbMonth.SelectedItem;
        //    int day = (int)this.tsCmbDay.SelectedItem;
        //    string dataFolder = this._dataFolder;
        //    string dataName = this._dataConfigFile.Name;
        //    ChartReader chartReader = new ChartReader(dataFolder, dataName, year, month, day);
        //    ChartLine[] chartLines = chartReader.Read(this._dataConfigFile.AxisXs);
        //    return chartLines;
        //}

        //private void DrawChart(ChartLine[] chartLines)
        //{
        //    int year = this._dataConfigFile.Year;
        //    int month = (int)this.tsCmbMonth.SelectedItem;
        //    int day = (int)this.tsCmbDay.SelectedItem;

        //    this.chart1.Titles.Clear();
        //    this.chart1.Titles.Add(new Title(string.Format("{0} / {1} / {2}", month, day, year), Docking.Top)
        //    {
        //        DockedToChartArea = "Default",
        //        IsDockedInsideChartArea = false
        //    });

        //    // add series
        //    this.chart1.Series.Clear();
        //    foreach (ChartLine chartLine in chartLines)
        //    {
        //        Series timeLine = new Series(chartLine.TimePoint);
        //        timeLine.Legend = "Default";
        //        timeLine.ChartType = SeriesChartType.Line;
        //        timeLine.ChartArea = "Default";
        //        foreach (var point in chartLine.Points)
        //        {
        //            timeLine.Points.Add(new DataPoint(point.X, point.Y) { MarkerSize = 5, MarkerStyle = MarkerStyle.Square });
        //        }
        //        this.chart1.Series.Add(timeLine);
        //    }
        //}

        private void tsCmbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the chart panel
            // todo: call disable from chart control
            //this.DisableChart();

            //skip when selected empty item
            if (string.IsNullOrEmpty(tsCmbDay.Text) || tsCmbDay.Text == ComboBoxItem.EmptyItem.Text)
                return; // do nothing

            //// loads chart data
            //ChartLine[] chartLines = this.LoadChartLines();

            //// draw chart
            //this.DrawChart(chartLines);

            // todo: call disable from chart control
            //this.EnableChart();
        }

        //private void DisableChart()
        //{
        //    //this.chart1.Titles.Clear();
        //    //this.chart1.Series.Clear();
        //    //this.chart1.Enabled = false;
        //}

        //private void EnableChart()
        //{
        //    //this.chart1.Enabled = true;
        //}

        public void Disable()
        {
            this.lblYear.Text = ComboBoxItem.EmptyItem.Text;
            this.tsCmbMonth.Items.Clear();
            this.tsCmbMonth.Text = ComboBoxItem.EmptyItem.Text;
            this.tsCmbDay.Items.Clear();
            this.tsCmbDay.Text = ComboBoxItem.EmptyItem.Text;
            // todo: call disable from chart control
            // this.DisableChart();
            this.Enabled = false;
        }

        public void Enable()
        {
            this.Enabled = true;
        }
    }
}