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
            var strFields = (string) objChartMapping.fields;
            this.FieldNames = strFields.Split(new char[] {','}, StringSplitOptions.None).Select(f=>f.Trim().ToLower()).ToArray();
            this.Fields=new Dictionary<string, Index>();
            foreach (var strField in this.FieldNames)
            {
                if (!this.Fields.ContainsKey(strField))
                    this.Fields.Add(strField, new Index());
            }
            this.DataConfigFile=new DataConfigFile();
            this.DataFiles=new DataFiles();
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Dictionary<string,Index> Fields { get; private set; }

        public string[] FieldNames { get; private set; }

        public DataConfigFile DataConfigFile { get; private set; }

        public DataFiles DataFiles { get; private set; }

        public string ToHeader()
        {
            string @fixed = "year,mm,dd,hh,mm,ss,";
            // sorting the field names by the global index
            string[] fields = this.Fields.OrderBy(p => p.Value.Global).Select(p => p.Key).ToArray();
            string strFields = string.Join(",", fields);
            string header = @fixed + strFields;
            return header;
        }
    }

    public class Index
    {
        public int Global { get; set; }

        public int Local { get; set; }

        public Index()
        {
            this.Global = -1;
            this.Local = -1;
        }
    }
}