using Aeronet.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aeronet.Splitter
{
    public class ChartMapping
    {
        public ChartMapping(dynamic objChartMapping)
        {
            this.Name = (string)objChartMapping.name;
            this.Description = (string)objChartMapping.description;
            var strFields = (string)objChartMapping.fields;
            var strAxisXs = (string)objChartMapping.axisXs;
            double[] arrAxisXs =
                strAxisXs.Split(new char[] { ',' }, StringSplitOptions.None).Select(a =>
                {
                    double v;
                    if (!double.TryParse(a, out v))
                        v = 0f;
                    return v;
                }).ToArray();
            this.FieldNames = strFields.Split(new char[] { ',' }, StringSplitOptions.None).Select(f => f.Trim().ToLower()).ToArray();
            this.Fields = new Dictionary<string, int>();

            foreach (var strField in this.FieldNames)
            {
                if (!this.Fields.ContainsKey(strField))
                    // give an enough large number so that the field can be sorted to down in an asc order
                    this.Fields.Add(strField, 9999);
            }
            this.DataConfigFile = new DataConfigFile();
            this.DataConfigFile.Name = this.Name;
            this.DataConfigFile.Description = this.Description;
            this.DataConfigFile.AxisXLabels.AddRange(this.FieldNames);
            this.DataConfigFile.AxisXs.AddRange(arrAxisXs);
            this.DataFiles = new DataFiles();
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Dictionary<string, int> Fields { get; private set; }

        public string[] FieldNames { get; private set; }

        public DataConfigFile DataConfigFile { get; private set; }

        public DataFiles DataFiles { get; private set; }

        public bool HasDataFiles
        {
            get { return this.DataFiles.Count>0; }
        }

        public string ToHeader()
        {
            string @fixed = "year,mm,dd,hh,mm,ss,";
            // sorting the field names by the global index
            string[] fields = this.Fields.OrderBy(p => p.Value).Select(p => p.Key).ToArray();
            string strFields = string.Join(",", fields);
            string header = @fixed + strFields;
            return header;
        }
    }
}