using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aeronet.Chart.Chart
{
    public class DataConfig
    {
        public string Name { get; set; }

        public int Year { get; set; }

        public IDictionary<int, int[]> MonthAndDays { get; set; }

        public DataConfig()
        {
            this.Name = string.Empty;
            this.Year = 0;
            this.MonthAndDays=new Dictionary<int, int[]>();
        }

        public DataConfig(string dataConfigFile):this()
        {
            this.Name = Path.GetFileNameWithoutExtension(dataConfigFile);

            string strDataConfig = File.ReadAllText(dataConfigFile);

            // check if it's an empty file
            if (string.IsNullOrEmpty(strDataConfig))
                throw new FileLoadException("Invalid the data config file", dataConfigFile);
            // initial properties
            var objDataConfig = (dynamic)JObject.Parse(strDataConfig);
            this.Year = (int)objDataConfig.year;
            var arrMonthNDays = (JArray)objDataConfig.monthAndDays;
            for (int i = 0; i < arrMonthNDays.Count; i++)
            {
                //ignore the empty month
                string strDays = (string)arrMonthNDays[i];
                if(string.IsNullOrEmpty(strDays)) continue;

                int[] days = strDays.Split(new char[] { ',' }, StringSplitOptions.None)
                        .Select(int.Parse)
                        .ToArray();
                int month = i + 1;
                if (!this.MonthAndDays.ContainsKey(month))
                    this.MonthAndDays.Add(month, days);
                else
                    this.MonthAndDays[month] = days;
            }
        }
    }
}
