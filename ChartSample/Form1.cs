using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AeronetChartSample
{
    public partial class Form1 : Form
    {
        private IDictionary<string, IDictionary<string, dynamic[]>> _data;

        public Form1()
        {
            InitializeComponent();

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

            InitData();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.Text = "Aeronet Chart Sample";
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            var combox = sender as ComboBox;
            var day = combox.Text;
            //validate
            if (!this._data.ContainsKey(day))
            {
                MessageBox.Show("No Available data");
                return;
            }

            // load data
            var data = this._data[day];

            // add title
            var year = comboBox1.Text;
            var month = comboBox2.Text;
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add(new Title(string.Format("{0} / {1} / {2}", month, day, year), Docking.Top)
            {
                DockedToChartArea = "Default",
                IsDockedInsideChartArea = false
            });

            // add series
            this.chart1.Series.Clear();
            foreach (string time in data.Keys)
            {
                Series timeLine = new Series(time);
                timeLine.Legend = "Default";
                timeLine.ChartType = SeriesChartType.Line;
                timeLine.ChartArea = "Default";
                var points = data[time];
                foreach (var point in points)
                {
                    timeLine.Points.Add(new DataPoint(point.X, point.Y) { MarkerSize = 5, MarkerStyle = MarkerStyle.Square });
                }
                this.chart1.Series.Add(timeLine);
            }
        }

        private void InitData()
        {
            this._data = new Dictionary<string, IDictionary<string, dynamic[]>>();
            this._data.Add(
                "01", new Dictionary<string, dynamic[]>()
                {
                    {"08:12:33",new dynamic[]{new{X=400,Y=60.88},new {X=550,Y=58.33},new {X=600,Y=43.56},new {X=720,Y=40.12},new {X=880,Y=30.22}}},
                    {"13:55:04",new dynamic[]{new{X=400,Y=70.78},new {X=550,Y=68.33},new {X=600,Y=53.56},new {X=720,Y=50.12},new {X=880,Y=48.22}}},
                    {"21:01:01",new dynamic[]{new{X=400,Y=50.11},new {X=550,Y=45.33},new {X=600,Y=32.56},new {X=720,Y=31.12},new {X=880,Y=20.22}}}
                });
            this._data.Add(
                "02", new Dictionary<string, dynamic[]>()
                {
                    {"04:22:16",new dynamic[]{new{X=400,Y=90.88},new {X=550,Y=70.33},new {X=600,Y=66.56},new {X=720,Y=65.12},new {X=880,Y=40.22}}},
                    {"17:35:02",new dynamic[]{new{X=400,Y=88.78},new {X=550,Y=71.33},new {X=600,Y=61.56},new {X=720,Y=65.12},new {X=880,Y=54.22}}},
                });
            this._data.Add(
                "03", new Dictionary<string, dynamic[]>()
                {
                    {"02:38:16",new dynamic[]{new{X=400,Y=90.88},new {X=550,Y=89.33},new {X=600,Y=72.56},new {X=720,Y=65.12},new {X=880,Y=31.22}}},
                    {"08:02:55",new dynamic[]{new{X=400,Y=63.78},new {X=550,Y=71.33},new {X=600,Y=37.56},new {X=720,Y=56.12},new {X=880,Y=44.22}}},
                    {"12:33:11",new dynamic[]{new{X=400,Y=78.88},new {X=550,Y=70.33},new {X=600,Y=65.56},new {X=720,Y=60.12},new {X=880,Y=40.22}}},
                    {"20:04:24",new dynamic[]{new{X=400,Y=80.78},new {X=550,Y=75.33},new {X=600,Y=68.56},new {X=720,Y=70.12},new {X=880,Y=54.22}}},
                });
        }
    }
}