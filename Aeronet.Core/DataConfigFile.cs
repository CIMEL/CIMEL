using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeronet.Core
{
    public class DataConfigFile
    {
        public string Year { get; set; }

        public Dictionary<int, List<int>> MonthAndDays { get; private set; }

        public DataConfigFile()
        {
            this.MonthAndDays = new Dictionary<int, List<int>>();
        }

        public void AddMonth(string month)
        {
            int intMonth;
            if (!int.TryParse(month, out intMonth))
                intMonth = 0;
            if (!this.MonthAndDays.ContainsKey(intMonth))
                this.MonthAndDays.Add(intMonth, new List<int>());
        }

        public void AddDay(string month, string day)
        {
            int intMonth;
            if (!int.TryParse(month, out intMonth))
                intMonth = 0;

            int intDay;
            if (!int.TryParse(day, out intDay))
                intDay = 0;

            if (!this.MonthAndDays.ContainsKey(intMonth))
                this.MonthAndDays.Add(intMonth, new List<int>());
            if (!this.MonthAndDays[intMonth].Contains(intDay))
                this.MonthAndDays[intMonth].Add(intDay);
        }

        public void Save(string chartSetPath, string chartName)
        {
            string extension = "dataconfig";
            string file = Path.Combine(chartSetPath, string.Format("{0}.{1}", chartName, extension));
            string[] arrMonthAndDays = new string[12];
            for (int i = 0; i < arrMonthAndDays.Length; i++)
            {
                int month = i + 1;
                if (this.MonthAndDays.ContainsKey(month))
                    arrMonthAndDays[i] = string.Join(",", this.MonthAndDays[month]);
                else
                    arrMonthAndDays[i] = string.Empty;
            }

            // apply defaults
            dynamic dataconfig = new
            {
                year = this.Year,
                monthAndDays = arrMonthAndDays
            };
            using (StreamWriter sw = new StreamWriter(file, false))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), dataconfig);
            }
        }
    }
}