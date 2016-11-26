using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aeronet.Splitter
{
    public class ChartMappings
    {
        private static ChartMappings _default = new ChartMappings();
        // fields X chartmapping
        private Dictionary<string,ChartMapping> _antFieldChartMappings;

        // field index X chartmapping
        private Dictionary<int, ChartMapping> _antIndexChartMappings;

        private List<ChartMapping> _chartMappings;

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
            this._antIndexChartMappings=new Dictionary<int, ChartMapping>();
            this._chartMappings=new List<ChartMapping>();
            this.AeronetFile=new AeronetFile();
            this.Init();
        }

        private void Init()
        {
            string chartMappingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chartmappings.json");
            if (!File.Exists(chartMappingsFile))
                throw new FileNotFoundException("Not found chart mappings file: " + chartMappingsFile);
            string strChartMappings = File.ReadAllText(chartMappingsFile);
            var jCharts = (JArray)((dynamic) JObject.Parse(strChartMappings)).charts;

            foreach (JToken jChart in jCharts)
            {
                // initial an instance of ChartMapping
                var objChartMapping=new ChartMapping((dynamic)jChart);
                foreach (string fieldName in objChartMapping.FieldNames)
                {
                    if(!this._antFieldChartMappings.ContainsKey(fieldName))
                        this._antFieldChartMappings.Add(fieldName,objChartMapping);
                }
                this._chartMappings.Add(objChartMapping);
            }
        }

        public ChartMapping this[int index]
        {
            get
            {
            if (this._antIndexChartMappings.ContainsKey(index))
                return this._antIndexChartMappings[index];
            else
                return null;
            }
        }

        public ChartMapping this[string fieldName]
        {
            get
            {
                if (this._antFieldChartMappings.ContainsKey(fieldName))
                    return this._antFieldChartMappings[fieldName];
                else
                    return null;
            }
        }

        /// <summary>
        /// create the mapping between the column index and an instance of ChartMapping object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="chartMapping"></param>
        public void CreateIndexMapping(int index, ChartMapping chartMapping)
        {
            if(!this._antIndexChartMappings.ContainsKey(index))
                this._antIndexChartMappings.Add(index,chartMapping);
        }

        public List<ChartMapping> GetAll()
        {
            return this._chartMappings;
        } 
    }
}
