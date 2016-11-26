using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Aeronet.Chart.AeronetData
{
    public class AeronetDataSet
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public string[] Datas { get; private set; }

        public AeronetDataSet(string dataSetFile)
        {
            this.Init(dataSetFile);
        }

        private void Init(string dataSetFile)
        {
            if (!File.Exists(dataSetFile))
                throw new FileNotFoundException("Not found the data set file", dataSetFile);

            string strDataSet = File.ReadAllText(dataSetFile);
            var objDataSet=(dynamic)JObject.Parse(strDataSet);

            this.Name = (string) objDataSet.name;
            this.Path = (string) objDataSet.datapath;
            this.Datas = ((JArray) objDataSet.datas).Select(d=>(string)d).ToArray();
        }

    }
}
