using Aeronet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Aeronet.Splitter
{
    public class ChartMapping
    {
        public ChartMapping(JObject joChartMapping)
        {
            // extracts raw values
            JToken jValue;
            string strName = !joChartMapping.TryGetValue("name", out jValue) ? string.Empty : jValue.Value<string>();
            string strDesc = !joChartMapping.TryGetValue("description", out jValue) ? string.Empty : jValue.Value<string>();
            string strFields = !joChartMapping.TryGetValue("fields", out jValue)
                ? string.Empty
                : jValue.Value<string>();
            string strAxisXs = !joChartMapping.TryGetValue("axisXs", out jValue) ? string.Empty : jValue.Value<string>();
            string strAxisXLabels = !joChartMapping.TryGetValue("axisXLabels", out jValue) ? string.Empty : jValue.Value<string>();
            string strNotes = !joChartMapping.TryGetValue("notes", out jValue) ? string.Empty : jValue.Value<string>();
            string strSubCharts = !joChartMapping.TryGetValue("subcharts", out jValue) ? string.Empty : jValue.Value<string>();
            string strNotesX = !joChartMapping.TryGetValue("notesX", out jValue) ? string.Empty : jValue.Value<string>();
            string strNotesY = !joChartMapping.TryGetValue("notesY", out jValue) ? string.Empty : jValue.Value<string>();
            int type = !joChartMapping.TryGetValue("type", out jValue) ? -1 : jValue.Value<int>();
            int index = !joChartMapping.TryGetValue("index", out jValue) ? -1 : jValue.Value<int>();

            // initial this chart mapping instance 
            this.Name = strName;
            this.Description = strDesc;
            this.Index = index;
            // defaults to 3(Self-group)
            this.Type = type < 0 ? ChartMappingType.SelfGroup : (ChartMappingType) type;
            // for Self-Group the subcharts just contains itself
            this.SubCharts = this.Type == ChartMappingType.SelfGroup
                ? new string[] {this.Name}
                : (string.IsNullOrEmpty(strSubCharts) ? new string[] {this.Name} : strSubCharts.Split('|'));

            this.FieldNames = string.IsNullOrEmpty(strFields)
                ? new string[0]
                : strFields.Split(',').Select(f => f.Trim().ToLower()).ToArray();
            this.Fields = new Dictionary<string, int>();
            foreach (var strField in this.FieldNames)
            {
                if (!this.Fields.ContainsKey(strField))
                    // give an enough large number so that the field can be sorted to down in an asc order
                    this.Fields.Add(strField, 9999);
            }

            // initial the instance of data config file
            double[] arrAxisXs = string.IsNullOrEmpty(strAxisXs)
                ? new double[0]
                : strAxisXs.Split(',').Select(a =>
                {
                    double v;
                    if (!double.TryParse(a, out v))
                        v = 0f;
                    return v;
                }).ToArray();

            // defaults to be same as the AxisX
            if (string.IsNullOrEmpty(strAxisXLabels))
                strAxisXLabels = strAxisXs;

            string[] arrAxisXLabels = string.IsNullOrEmpty(strAxisXLabels) ? new string[0] : strAxisXLabels.Split(',');
            string[] arrNotes = string.IsNullOrEmpty(strNotes) ? new string[0] : strNotes.Split('|');
            string[] arrNotesX = string.IsNullOrEmpty(strNotesX) ? new string[0] : strNotesX.Split('|');
            string[] arrNotesY = string.IsNullOrEmpty(strNotesY) ? new string[0] : strNotesY.Split('|');

            this.DataConfigFile = new DataConfigFile
            {
                Name = this.Name,
                Description = this.Description
            };
            this.DataConfigFile.AxisXLabels.AddRange(arrAxisXLabels);
            this.DataConfigFile.AxisXs.AddRange(arrAxisXs);
            this.DataConfigFile.Notes.AddRange(arrNotes);
            this.DataConfigFile.NotesX.AddRange(arrNotesX);
            this.DataConfigFile.NotesY.AddRange(arrNotesY);

            this.DataFiles = new DataFiles();
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Dictionary<string, int> Fields { get; private set; }

        public string[] FieldNames { get; private set; }

        public DataConfigFile DataConfigFile { get; private set; }

        public DataFiles DataFiles { get; private set; }

        public ChartMappingType Type { get; private set; }

        public int Index { get; private set; }

        public string[] SubCharts { get; private set; }

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

    /// <summary>
    /// The type of the chartmapping
    /// </summary>
    /// <summary>
    /// the chart mapping type, available values are 1,2,3
    ///     1: Chart group (Virtual chart)
    ///     2: Child chart of a group (Real chart)
    ///     3: Self-group chart (which is a real chart and considered as a group as well, and the child chart is itself)
    /// defaults to 3 and the subcharts is itself
    /// </summary>
    public enum ChartMappingType
    {
        ChartGroup = 1,
        ChildChart = 2,
        SelfGroup = 3
    }
}