using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CIMEL.Core
{
    public class DataConfigFile
    {
        private static readonly Encoding EncodingCode = Encoding.GetEncoding("GB2312");

        public string Name { get; set; }

        public int Year { get; set; }

        public Dictionary<int, List<int>> MonthAndDays { get; private set; }

        public string Description { get; set; }

        public List<string> AxisXLabels { get; private set; }

        public List<double> AxisXs { get; private set; }

        public List<string> Notes { get; private set; }

        public List<string> NotesX { get; private set; }

        public List<string> NotesY { get; private set; } 

        public DataConfigFile()
        {
            this.Name = string.Empty;
            this.Description = string.Empty;
            this.Year = 0;
            this.MonthAndDays = new Dictionary<int, List<int>>();
            this.AxisXs=new List<double>();
            this.AxisXLabels=new List<string>();
            this.Notes=new List<string>();
            this.NotesX=new List<string>();
            this.NotesY=new List<string>();
        }

        public DataConfigFile(string dataConfigFile)
            : this()
        {
            // this.Name = Path.GetFileNameWithoutExtension(dataConfigFile);

            string strDataConfig = File.ReadAllText(dataConfigFile,EncodingCode);

            // check if it's an empty file
            if (string.IsNullOrEmpty(strDataConfig))
                throw new FileLoadException("错误的图像集文件(.dataconfig)", dataConfigFile);
            // deserialize from json
            JObject joDataConfig = JObject.Parse(strDataConfig);

            // extracts raw values
            JToken value;
            string strName = !joDataConfig.TryGetValue("name", out value)?string.Empty:value.Value<string>();
            string strDesc = !joDataConfig.TryGetValue("description", out value) ? string.Empty : value.Value<string>();
            int intYear = !joDataConfig.TryGetValue("year", out value) ? 1900 : value.Value<int>();
            JArray jarrMonthNDays = !joDataConfig.TryGetValue("monthAndDays", out value) ? new JArray() : value.Value<JArray>();
            string strAxisXLabels = !joDataConfig.TryGetValue("axisXLabels", out value) ? string.Empty : value.Value<string>();
            string strAxisXs = !joDataConfig.TryGetValue("axisXs", out value) ? string.Empty : value.Value<string>();
            string strNotes = !joDataConfig.TryGetValue("notes", out value) ? string.Empty : value.Value<string>();
            string strNotesX = !joDataConfig.TryGetValue("notesX", out value) ? string.Empty : value.Value<string>();
            string strNotesY = !joDataConfig.TryGetValue("notesY", out value) ? string.Empty : value.Value<string>();

            // initial properties
            this.Name = strName;
            this.Description = strDesc;
            this.Year = intYear;
            var arrMonthNDays = jarrMonthNDays;
            for (int i = 0; i < arrMonthNDays.Count; i++)
            {
                //ignore the empty month
                string strDays = (string)arrMonthNDays[i];
                if (string.IsNullOrEmpty(strDays)) continue;

                List<int> days = strDays.Split(',').Select(int.Parse).ToList();
                int month = i + 1;
                if (!this.MonthAndDays.ContainsKey(month))
                    this.MonthAndDays.Add(month, days);
                else
                    this.MonthAndDays[month] = days;
            }

            // defaults to be the same as AxisX
            if (string.IsNullOrEmpty(strAxisXLabels))
                strAxisXLabels = strAxisXs;
            if (!string.IsNullOrEmpty(strAxisXLabels))
                this.AxisXLabels = strAxisXLabels.Split(',').ToList();

            if (!string.IsNullOrEmpty(strAxisXs))
                this.AxisXs = strAxisXs.Split(',').Select(a =>
                {
                    double v;
                    if (!double.TryParse(a, out v))
                        v = 0f;
                    return v;
                }).ToList();
            this.Notes = string.IsNullOrEmpty(strNotes) ? new List<string>() : strNotes.Split('|').ToList();
            this.NotesX = string.IsNullOrEmpty(strNotesX) ? new List<string>() : strNotesX.Split('|').ToList();
            this.NotesY = string.IsNullOrEmpty(strNotesY) ? new List<string>() : strNotesY.Split('|').ToList();
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
                axisXs = string.Join(",",this.AxisXs),
                notes=string.Join(",",this.Notes),
                notesX=string.Join(",",this.NotesX),
                notesY=string.Join(",",this.NotesY)
            };
            using (StreamWriter sw = new StreamWriter(file, false,EncodingCode))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), dataconfig);
                sw.Flush();
                sw.Close();
            }

            return file;
        }
    }
}