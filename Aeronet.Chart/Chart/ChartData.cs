using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    /// <summary>
    /// A line-data of a time point 
    /// </summary>
    public class ChartLine
    {
        public ChartLine(string lineData,string[] headerData)
        {
            this.Init(lineData,headerData);
        }

        private void Init(string lineData,string[] headerData)
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
                var point=new ChartPoint(headerData[i+6].Trim(), data[i+6].Trim());
                this.Points[i] = point;
            }
        }

        public string TimePoint { get; private set; }
        public ChartPoint[] Points { get; private set; }
    }

    public class ChartPoint
    {
        public ChartPoint(string x, string y)
        {
            // initial Axis label of X
            this.AxisLabelX = x;

            // initial X which would be either integer or null
            var m=Regex.Match(x, "\\d+", RegexOptions.Compiled);
            if (m.Success)
            {
                int intX;
                if (!int.TryParse(m.Value, out intX))
                    intX = 0;
                this.X = intX;
            }

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
            this.AxisLabelX = string.Empty;
            this.Y = 0f;
        }

        public int X { get; set; }
        public double Y { get; set; }
        public string AxisLabelX { get; set; }
    }
}
