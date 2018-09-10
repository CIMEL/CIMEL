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
using CIMEL.Core;
using CIMEL.Dog;
using Peach.Log;

namespace CIMEL.Chart
{
    public partial class ChartPanel : UserControl
    {
        public List<string> DataConfigFiles { get; private set; }

        // private string _dataFolder;
        // the selected the chartdata
        private DataConfigFile _dataConfigFile;

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
            this.tabControl1.TabPages.Clear();
        }

        private void tsCmbMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ActiveChecker.Singleton.IsActive(true)) return;

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
            // initial to select the empty item, defaults to select the first day having data
            this.tsCmbDay.SelectedIndex = this.tsCmbDay.Items.Count > 1 ? 1 : 0;
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

                // initial tab pages
                this.tabControl1.TabPages.Clear();
                for (int i=0;i<this.DataConfigFiles.Count;i++)
                {
                    string strDataConfigFile = this.DataConfigFiles[i];
                    if (!File.Exists(strDataConfigFile))
                        throw new FileNotFoundException("没有找到图像集文件(.dataconfig)", strDataConfigFile);

                    // initial instance of DataConfigFile
                    DataConfigFile objDataConfigFile = new DataConfigFile(strDataConfigFile);

                    if (i == 0)
                        this._dataConfigFile = objDataConfigFile;
                    // initial rootfolder
                    string rootFolder = Path.GetDirectoryName(strDataConfigFile);

                    AeroChartPage chartPage = new AeroChartPage
                    {
                        DataConfigFile = objDataConfigFile,
                        DataFolder = rootFolder
                    };
                    // initial font
                    //var oFont = this.Font;
                    //chartPage.Font = new Font(oFont.FontFamily, 12);
                    chartPage.Init();

                    this.tabControl1.TabPages.Add(chartPage);
                }
                // defaults to select the first page
                if(this.tabControl1.TabPages.Count>0)
                    this.tabControl1.SelectTab(0);

                if (this._dataConfigFile == null)
                    throw new NotImplementedException("初始化图像集文件失败");
                // the label of year
                this.lblYear.Text = this._dataConfigFile.Year.ToString();


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
                // initial to select the empty item, defaults to select Jan.
                this.tsCmbMonth.SelectedIndex = this.tsCmbMonth.Items.Count > 1 ? 1 : 0;
            }
            catch (Exception ex)
            {
                this.ShowAlert(ex.Message, @"图像加载错误");

                // disable all of funtions
                this.Enabled = false;
            }
        }

        private void tsCmbDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            // disable the chart panel
            this.DisableChart();

            //skip when selected empty item
            if (string.IsNullOrEmpty(tsCmbDay.Text) || tsCmbDay.Text == ComboBoxItem.EmptyItem.Text)
                return; // do nothing

            this.DrawChart();

            this.EnableChart();
        }

        private void DrawChart()
        {
            // check if there are any tabs
            if (this.tabControl1.TabPages.Count == 0) return;
            
            int year = this._dataConfigFile.Year;
            int month = (int)this.tsCmbMonth.SelectedItem;
            int day = (int)this.tsCmbDay.SelectedItem;

            foreach (var tabPage in this.tabControl1.TabPages)
            {
                var aeroChartPage = tabPage as AeroChartPage;
                if(aeroChartPage==null) continue;
                try
                {
                    aeroChartPage.Draw(year, month, day);
                    aeroChartPage.Refresh();
                }
                catch (Exception ex)
                {
                    Logger.Default.Error(ex);
                    // do nothing
                }
            }
            this.tabControl1.Refresh();
        }

        private void DisableChart()
        {
            foreach (var tabPage in this.tabControl1.TabPages)
            {
                var aeroChartPage = tabPage as AeroChartPage;
                if (aeroChartPage == null) continue;
                aeroChartPage.Disable();
                aeroChartPage.Refresh();
            }
            this.tabControl1.Enabled = false;
            this.tabControl1.Refresh();
        }

        private void EnableChart()
        {
            foreach (var tabPage in this.tabControl1.TabPages)
            {
                var aeroChartPage = tabPage as AeroChartPage;
                if (aeroChartPage == null) continue;
                aeroChartPage.Enable();
                aeroChartPage.Refresh();
            }
            this.tabControl1.Enabled = true;
            this.tabControl1.Refresh();
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

        private void btnNext_Click(object sender, EventArgs e)
        {
            int curDayIndex = this.tsCmbDay.SelectedIndex;
            if (curDayIndex == this.tsCmbDay.Items.Count - 1)
            {
                // go to next month
                int curMonthIndex = this.tsCmbMonth.SelectedIndex;
                if (curMonthIndex == this.tsCmbMonth.Items.Count - 1)
                {
                    // go to Jan.
                    this.tsCmbMonth.SelectedIndex = this.tsCmbMonth.Items.Count > 1 ? 1 : 0;
                }
                else
                    this.tsCmbMonth.SelectedIndex++;
            }
            else
            {
                // go to next day
                this.tsCmbDay.SelectedIndex++;
            }
        }
    }
}