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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            //ChartArea caDefault = new ChartArea("Default");
            //this.chart1.ChartAreas.Clear();
            //this.chart1.ChartAreas.Add(caDefault);
            //// X axis value labels
            //caDefault.AxisX.Minimum = 350;
            //caDefault.AxisX.Interval = 150;
            //caDefault.AxisX.Maximum = 900;
            // X axis and Y axis Title
            //caDefault.AxisX.Title = "X Axis";
            //caDefault.AxisY.Title = "Y Axis";
            Legend lgDefault = new Legend("Default");
            this.chart1.Legends.Clear();
            this.chart1.Legends.Add(lgDefault);
            this.chart1.Series.Clear();
            this.chart1.Titles.Clear();

            this.DrawChart();

            this.Text = "Aeronet Chart Sample";
        }

        private void DrawChart()
        {
            var data=new Data();

            this.chart1.ChartAreas.Clear();
            ChartArea ca1 = new ChartArea("ca1");
            this.chart1.ChartAreas.Add(ca1);

            //ChartArea ca2 = new ChartArea("ca2");
            //this.chart1.ChartAreas.Add(ca2);
            //// ca2.BackColor=Color.Transparent;

            //ChartArea ca3 = new ChartArea("ca3");
            //this.chart1.ChartAreas.Add(ca3);
            // ca3.BackColor = Color.Transparent;
            //ca3.CursorX.AutoScroll = true;
            //ca3.CursorX.IsUserEnabled = true;
            //ca3.CursorX.IsUserSelectionEnabled = true;

            // add series
            this.chart1.Series.Clear();
            ca1.AxisX.Minimum = 0f;
            //ca1.AxisX.Maximum = 3.857f;
            //ca1.AxisX2.Minimum = 5.051f;
            ca1.AxisX.Maximum = 15.50f;
            //ca1.CursorX.IsUserEnabled = true;
            //ca1.CursorX.IsUserSelectionEnabled = true;
            //ca1.CursorX.AxisType=AxisType.Secondary;
            //ca1.CursorX.AutoScroll = true;
            //ca1.AxisX.ScrollBar.Enabled = true;

            for(int i=0;i<data.Lines.Count;i++)
            {
                Series timeLine = new Series(i.ToString());
                timeLine.XAxisType=AxisType.Primary;
                timeLine.Legend = "Default";
                timeLine.ChartType = SeriesChartType.Line;
                var axisYs = data.Lines[i];
                var axisXs = data.AxisXs;
                var axisXLabels = data.AxisXLabels;
                for(int j=0;j<axisYs.Length;j++)
                {
                        timeLine.ChartArea = "ca1";

                        timeLine.Points.Add(new DataPoint(axisXs[j], axisYs[j]) { MarkerSize = 5, MarkerStyle = MarkerStyle.Square });
                }
                this.chart1.Series.Add(timeLine);
            }
            //ca2.Position = ca1.Position;
            //ca3.Position = ca1.Position;
        }
    }

    public class Data
    {
        private string _strAxisXLabels =
            "0.050,0.066,0.086,0.113,0.148,0.194,0.255,0.335,0.439,0.576,0.756,0.992,1.302,1.708,2.241,2.940,3.857,5.051,6.641,8.713,11.43,15.00";

        private string _strAxisXs =
            "0.050,0.066,0.086,0.113,0.148,0.194,0.255,0.335,0.439,0.576,0.756,0.992,1.302,1.708,2.241,2.940,3.857,5.051,6.641,8.713,11.43,15.00";

        private string[] _arrLines = new string[]
        {
            "0.0015,0.0052,0.0134,0.0245,0.0324,0.0324,0.0251,0.0162,0.0113,0.0097,0.0104,0.0129,0.0153,0.0158,0.0147,0.0126,0.0099,0.0074,0.0054,0.0038,0.0024,0.0015",
            "0.0012,0.0067,0.0231,0.0508,0.0684,0.0567,0.0348,0.0218,0.0161,0.0136,0.0125,0.0121,0.0129,0.0162,0.0256,0.0473,0.0868,0.1266,0.1184,0.0606,0.0155,0.0019"
        };

        public string[] AxisXLabels { get; private set; }

        public double[] AxisXs { get; private set; }

        public List<double[]> Lines { get; private set; }

        public Data()
        {
            this.AxisXLabels = this._strAxisXLabels.Split(',');
            this.AxisXs = this._strAxisXs.Split(',').Select(double.Parse).ToArray();
            this.Lines = this._arrLines.Select(strL => strL.Split(',').Select(double.Parse).ToArray()).ToList();
        }
    }
}
