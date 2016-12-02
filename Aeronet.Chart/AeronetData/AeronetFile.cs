using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeronet.Chart.AeronetData
{
    public class AeronetFile
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public List<string> DataConfigs { get; private set; }

        public AeronetFile(string dataSetFile)
        {
            this.Read(dataSetFile);
        }

        private void Read(string dataSetFile)
        {
            if (!File.Exists(dataSetFile))
                throw new FileNotFoundException("Not found the data set file", dataSetFile);

            string strDataSet = File.ReadAllText(dataSetFile);
            var objDataSet = (dynamic)JObject.Parse(strDataSet);

            this.Name = (string)objDataSet.name;
            this.Path = (string)objDataSet.datapath;
            this.DataConfigs = ((JArray)objDataSet.datas).Select(d => (string)d).ToList();
        }
    }
}