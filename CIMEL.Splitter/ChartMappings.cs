using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CIMEL.Core;

namespace CIMEL.Splitter
{
    public class ChartMappings
    {
        private static ChartMappings _default = new ChartMappings();

        // fields X chartmapping
        private Dictionary<string, List<ChartMapping>> _antFieldChartMappings;

        // field index X chartmapping
        private Dictionary<int, List<ChartMapping>> _antIndexChartMappings;

        /// <summary>
        /// all of the real chart mappings
        /// </summary>
        private Dictionary<string, ChartMapping> _dicChartMappings;

        /// <summary>
        /// all of the chart mapping groups
        /// </summary>
        private Dictionary<string, ChartMapping> _dicChartMappingGroups; 

        public CIMELFile CIMELFile { get; private set; }

        public static ChartMappings Signleton
        {
            get
            {
                return _default;
            }
        }

        protected ChartMappings()
        {
            this._antFieldChartMappings = new Dictionary<string, List<ChartMapping>>();
            this._antIndexChartMappings = new Dictionary<int, List<ChartMapping>>();
            this._dicChartMappings = new Dictionary<string, ChartMapping>();
            this._dicChartMappingGroups = new Dictionary<string, ChartMapping>();
            this.CIMELFile = new CIMELFile();
            this.Init();
        }

        private void Init()
        {
            string chartMappingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chartmappings.json");
            if (!File.Exists(chartMappingsFile))
                throw new FileNotFoundException("Not found chart mappings file: " + chartMappingsFile);
            string strChartMappings = File.ReadAllText(chartMappingsFile,Encoding.GetEncoding("GB2312"));
            var jCharts = (JArray)((dynamic)JObject.Parse(strChartMappings)).charts;

            foreach (JToken jChart in jCharts)
            {
                JObject joChart = jChart as JObject;
                if (joChart == null) continue;

                // initial an instance of ChartMapping
                var objChartMapping = new ChartMapping(joChart);

                // push the chart group and self-group to the group dictionary
                if (objChartMapping.Type == ChartMappingType.SelfGroup ||
                    objChartMapping.Type == ChartMappingType.ChartGroup)
                {
                    if (!this._dicChartMappingGroups.ContainsKey(objChartMapping.Name))
                        this._dicChartMappingGroups.Add(objChartMapping.Name, objChartMapping);
                    else
                        this._dicChartMappingGroups[objChartMapping.Name] = objChartMapping;
                }

                // skip the chart group when building fields mapping 
                if (objChartMapping.Type == ChartMappingType.ChartGroup) continue;

                foreach (string fieldName in objChartMapping.FieldNames)
                {
                    if (!this._antFieldChartMappings.ContainsKey(fieldName))
                        this._antFieldChartMappings.Add(fieldName, new List<ChartMapping>());
                    this._antFieldChartMappings[fieldName].Add(objChartMapping);
                }
                // pushes to the collection of the real charts
                if (!this._dicChartMappings.ContainsKey(objChartMapping.Name))
                    this._dicChartMappings.Add(objChartMapping.Name, objChartMapping);
                else
                    this._dicChartMappings[objChartMapping.Name] = objChartMapping;
            }
        }

        /// <summary>
        /// Gets the list of column mappings by column index
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public List<ChartMapping> this[int columnIndex]
        {
            get
            {
                if (this._antIndexChartMappings.ContainsKey(columnIndex))
                    return this._antIndexChartMappings[columnIndex];
                else
                    return new List<ChartMapping>();
            }
        }

        /// <summary>
        /// Gets the list of column mappings by column name
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public List<ChartMapping> this[string columnName]
        {
            get
            {
                if (this._antFieldChartMappings.ContainsKey(columnName))
                    return this._antFieldChartMappings[columnName];
                else
                    return new List<ChartMapping>();
            }
        }

        /// <summary>
        /// Gets a chart mapping instace by chart mapping name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ChartMapping Get(string name)
        {
            if (this._dicChartMappings.ContainsKey(name))
                return this._dicChartMappings[name];
            else
                return null;
        }

        /// <summary>
        /// create the mapping between the column index and an instance of ChartMapping object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="chartMapping"></param>
        public void CreateIndexMapping(int index, ChartMapping chartMapping)
        {
            if (!this._antIndexChartMappings.ContainsKey(index))
                this._antIndexChartMappings.Add(index, new List<ChartMapping>());

            this._antIndexChartMappings[index].Add(chartMapping);
        }

        public Dictionary<string,ChartMapping> GetGroups()
        {
            return this._dicChartMappingGroups.OrderBy(p=>p.Value.Index,Comparer<int>.Default).ToDictionary(p=>p.Key,p=>p.Value);
        }
    }

    public enum IndexType
    {
        Column = 0,
        ColumnName = 1,
        ChartMappingName = 2
    }
}