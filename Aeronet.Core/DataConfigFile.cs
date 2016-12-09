using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aeronet.Core
{
    public class DataConfigFile
    {
        public string Name { get; set; }

        public int Year { get; set; }

        public Dictionary<int, List<int>> MonthAndDays { get; private set; }

        public string Description { get; set; }

        public List<string> AxisXLabels { get; private set; }

        public List<double> AxisXs { get; private set; } 

        public DataConfigFile()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Year = 0;
            this.MonthAndDays = new Dictionary<int, List<int>>();
            this.AxisXs=new List<double>();
            this.AxisXLabels=new List<string>();
        }

        public DataConfigFile(string dataConfigFile)
            : this()
        {
            // this.Name = Path.GetFileNameWithoutExtension(dataConfigFile);

            string strDataConfig = File.ReadAllText(dataConfigFile);

            // check if it's an empty file
            if (string.IsNullOrEmpty(strDataConfig))
                throw new FileLoadException("错误的图像集文件(.dataconfig)", dataConfigFile);
            // initial properties
            var objDataConfig = (dynamic)JObject.Parse(strDataConfig);
            this.Name = (string) objDataConfig.name;
            this.Description = (string) objDataConfig.description;
            this.Year = (int)objDataConfig.year;
            var arrMonthNDays = (JArray)objDataConfig.monthAndDays;
            for (int i = 0; i < arrMonthNDays.Count; i++)
            {
                //ignore the empty month
                string strDays = (string)arrMonthNDays[i];
                if (string.IsNullOrEmpty(strDays)) continue;

                List<int> days = strDays.Split(new char[] { ',' }, StringSplitOptions.None)
                        .Select(int.Parse)
                        .ToList();
                int month = i + 1;
                if (!this.MonthAndDays.ContainsKey(month))
                    this.MonthAndDays.Add(month, days);
                else
                    this.MonthAndDays[month] = days;
            }
            var strAxisXLabels = (string)objDataConfig.axisXLabels;
            if (!string.IsNullOrEmpty(strAxisXLabels))
                this.AxisXLabels = strAxisXLabels.Split(new char[] {','}, StringSplitOptions.None).ToList();
            var strAxisXs = (string)objDataConfig.axisXs;
            if (!string.IsNullOrEmpty(strAxisXs))
                this.AxisXs = strAxisXs.Split(new char[] {','}, StringSplitOptions.None).Select(a =>
                {
                    double v;
                    if (!double.TryParse(a, out v))
                        v = 0f;
                    return v;
                }).ToList();
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

        public string Save(string chartSetPath, string chartName)
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
                name = this.Name,
                description = this.Description,
                year = this.Year,
                monthAndDays = arrMonthAndDays,
                axisXLabels = string.Join(",",this.AxisXLabels),
                axisXs = string.Join(",",this.AxisXs)
            };
            using (StreamWriter sw = new StreamWriter(file, false))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), dataconfig);
                sw.Flush();
                sw.Close();
            }

            return file;
        }
    }
}