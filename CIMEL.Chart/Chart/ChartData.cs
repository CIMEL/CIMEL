﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CIMEL.Chart
{
    /// <summary>
    /// A line-data of a time point 
    /// </summary>
    public class ChartLine
    {
        public ChartLine(string lineData,List<double> axisXs)
        {
            this.Init(lineData, axisXs);
        }

        private void Init(string lineData, List<double> axisXs)
        {
            string[] data = lineData.Split(new[] {','}, StringSplitOptions.None);
            // the first 3 fields are year, month and day
            // then the following 3 fields are hour, minute and second
            this.TimePoint = string.Format("{0}:{1}:{2}", data[3].Trim(), data[4].Trim(), data[5].Trim());
            // then the following fields are the values of Axis Y
            int length = data.Length - 6;
            this.Points=new ChartPoint[length];
            for (int i = 0; i < this.Points.Length; i++)
            {
                // the fields in the header are the Axis X
                var point = new ChartPoint(axisXs[i], data[i + 6].Trim());
                this.Points[i] = point;
            }
        }

        public string TimePoint { get; private set; }
        public ChartPoint[] Points { get; private set; }
    }

    public class ChartPoint
    {
        public ChartPoint(double x, string y)
        {
            // initial X which would be either integer or null
            this.X = x;

            // initial Y
            double fY;
            if (!double.TryParse(y, out fY))
                fY = 0f;
            this.Y = fY;
        }

        public ChartPoint(string y):this()
        {
            // initial Y
            double fY;
            if (!double.TryParse(y, out fY))
                fY = 0f;
            this.Y = fY;
        }

        public ChartPoint()
        {
            this.X = 0;
            this.Y = 0f;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public string AxisLabelX { get; set; }
    }
}