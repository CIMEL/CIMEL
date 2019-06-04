using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CIMEL.Core
{
    /// <summary>
    /// A line-data of a time point 
    /// </summary>
    public class ChartLine
    {
        public bool IsFake { get; private set; }
        public ChartLine(string lineData, List<double> axisXs)
        {
            this.IsFake = false;
            this.Init(lineData, axisXs);
        }

        public ChartLine(string timepoint, List<double> axisXs, bool isfake)
        {
            this.TimePoint = timepoint;

            this.Points = axisXs.Select(x => new ChartPoint(x)).ToArray();
            this.IsFake = isfake;
        }

        protected virtual void Init(string lineData, List<double> axisXs)
        {
            string[] data = lineData.Split(new[] { ',' }, StringSplitOptions.None);
            // the first 3 fields are year, month and day
            // then the following 3 fields are hour, minute and second
            this.TimePoint = string.Format("{0}:{1}:{2}", data[3].Trim(), data[4].Trim(), data[5].Trim());
            // then the following fields are the values of Axis Y
            int length = data.Length - 6;
            this.Points = new ChartPoint[length];
            for (int i = 0; i < this.Points.Length; i++)
            {
                // the fields in the header are the Axis X
                var point = new ChartPoint(axisXs[i], data[i + 6].Trim());
                this.Points[i] = point;
            }
        }

        public string TimePoint { get; protected set; }
        public ChartPoint[] Points { get; protected set; }
    }

    public class ChartLineCompare : IComparer<ChartLine>
    {
        public int Compare(ChartLine x, ChartLine y)
        {
            TimeSpan parse(string timepoint)
            {
                string[] parts = timepoint.Split(new char[] { ':' });
                int hours = 0, mins = 0, seconds = 0;
                if (parts.Length > 0)
                    if (!int.TryParse(parts[0], out hours)) hours = 0;
                if (parts.Length > 1)
                    if (!int.TryParse(parts[1], out mins)) mins = 0;
                if (parts.Length > 2)
                    if (!int.TryParse(parts[2], out seconds)) seconds = 0;

                return new TimeSpan(hours, mins, seconds);
            }

            TimeSpan left = parse(x.TimePoint);
            TimeSpan right = parse(y.TimePoint);

            int diff = (int)(left - right).TotalSeconds;
            return diff;
        }
    }

    public class ChartPoint
    {
        public ChartPoint(double x)
        {
            this.X = x;
            this.Y = 0f;
        }

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

        public ChartPoint(string y) : this()
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

    public class EXTTChartLine : ChartLine
    {
        public EXTTChartLine(string lineData, List<double> axisXs) : base(lineData, axisXs)
        {
        }

        protected override void Init(string lineData, List<double> axisXs)
        {
            string[] data = lineData.Split(new[] { ',' }, StringSplitOptions.None);
            // the first 3 fields are year, month and day
            // then the following 3 fields are hour, minute and second
            this.TimePoint = string.Format("{0}:{1}:{2}", data[3].Trim(), data[4].Trim(), data[5].Trim());
            // then the following fields are the values of Axis Y
            int length = data.Length - 6;
            this.Points = new ChartPoint[length];
            for (int i = 0; i < this.Points.Length; i++)
            {
                // the fields in the header are the Axis X
                // move aod550 to behind extt440
                string y;
                if (i == 0 && data.Length > 7)
                    y = data[i + 6 + 1].Trim();
                else if (i == 1 && data.Length > 6)
                    y = data[i + 6 - 1].Trim();
                else
                    y = data[i + 6].Trim();

                var point = new ChartPoint(axisXs[i], y);
                this.Points[i] = point;
            }
        }
    }

    public class AAODChartLine : ChartLine
    {
        public AAODChartLine(string lineData, List<double> axisXs) : base(lineData, axisXs)
        {
        }

        protected override void Init(string lineData, List<double> axisXs)
        {
            string[] data = lineData.Split(new[] { ',' }, StringSplitOptions.None);
            // the first 3 fields are year, month and day
            // then the following 3 fields are hour, minute and second
            this.TimePoint = string.Format("{0}:{1}:{2}", data[3].Trim(), data[4].Trim(), data[5].Trim());
            // then the following fields are the values of Axis Y
            int length = data.Length - 6;
            this.Points = new ChartPoint[length];
            for (int i = 0; i < this.Points.Length; i++)
            {
                // the fields in the header are the Axis X
                // move aaod550 to behind aaod440
                string y;
                if (i == 1)
                    y = data[data.Length - 1].Trim();
                else if (i > 1)
                    y = data[i + 6 - 1].Trim();
                else
                    y = data[i + 6].Trim();

                var point = new ChartPoint(axisXs[i], y);
                this.Points[i] = point;
            }
        }
    }

    public class ChartLineFactory
    {
        private string _chart;
        protected ChartLineFactory(string chart)
        {
            this._chart = chart;
        }

        public static ChartLineFactory GetFactory(string chart)
        {
            return new ChartLineFactory(chart);
        }

        public ChartLine Create(string lineData, List<double> axisXs)
        {
            switch (_chart)
            {
                case "EXT-T":
                    return new EXTTChartLine(lineData, axisXs);
                case "AAOD":
                    return new AAODChartLine(lineData, axisXs);
                default: return new ChartLine(lineData, axisXs);
            }
        }
    }
}
