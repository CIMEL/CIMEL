using Aeronet.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aeronet.Splitter
{
    public class ChartMappings
    {
        private static ChartMappings _default = new ChartMappings();

        // fields X chartmapping
        private Dictionary<string, ChartMapping> _antFieldChartMappings;

        // field index X chartmapping
        private Dictionary<int, ChartMapping> _antIndexChartMappings;

        /// <summary>
        /// all of the real chart mappings
        /// </summary>
        private Dictionary<string, ChartMapping> _dicChartMappings;

        /// <summary>
        /// all of the chart mapping groups
        /// </summary>
        private Dictionary<string, ChartMapping> _dicChartMappingGroups; 

        public AeronetFile AeronetFile { get; private set; }

        public static ChartMappings Signleton
        {
            get
            {
                return _default;
            }
        }

        protected ChartMappings()
        {
            this._antFieldChartMappings = new Dictionary<string, ChartMapping>();
            this._antIndexChartMappings = new Dictionary<int, ChartMapping>();
            this._dicChartMappings = new Dictionary<string, ChartMapping>();
            this._dicChartMappingGroups = new Dictionary<string, ChartMapping>();
            this.AeronetFile = new AeronetFile();
            this.Init();
        }

        private void Init()
        {
            string chartMappingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chartmappings.json");
            if (!File.Exists(chartMappingsFile))
                throw new FileNotFoundException("Not found chart mappings file: " + chartMappingsFile);
            string strChartMappings = File.ReadAllText(chartMappingsFile);
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
                        this._antFieldChartMappings.Add(fieldName, objChartMapping);
                }
                // pushes to the collection of the real charts
                if (!this._dicChartMappings.ContainsKey(objChartMapping.Name))
                    this._dicChartMappings.Add(objChartMapping.Name, objChartMapping);
                else
                    this._dicChartMappings[objChartMapping.Name] = objChartMapping;
            }
        }

        public ChartMapping this[int index, IndexType type = IndexType.Column]
        {
            get
            {
                if (type == IndexType.Column)
                {
                    if (this._antIndexChartMappings.ContainsKey(index))
                        return this._antIndexChartMappings[index];
                    else
                        return null;
                }
                return null;
            }
        }

        public ChartMapping this[string name, IndexType type]
        {
            get
            {
                switch (type)
                {
                    case IndexType.ColumnName:
                    {
                        if (this._antFieldChartMappings.ContainsKey(name))
                            return this._antFieldChartMappings[name];
                        else
                            return null;
                    }
                    case IndexType.ChartMappingName:
                    {
                        if (this._dicChartMappings.ContainsKey(name))
                            return this._dicChartMappings[name];
                        else
                            return null;
                    }
                    case IndexType.Column:
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// create the mapping between the column index and an instance of ChartMapping object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="chartMapping"></param>
        public void CreateIndexMapping(int index, ChartMapping chartMapping)
        {
            if (!this._antIndexChartMappings.ContainsKey(index))
                this._antIndexChartMappings.Add(index, chartMapping);
        }

        public Dictionary<string,ChartMapping> GetGroups()
        {
            return this._dicChartMappingGroups;
        }
    }

    public enum IndexType
    {
        Column = 0,
        ColumnName = 1,
        ChartMappingName = 2
    }
}