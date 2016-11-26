using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Aeronet.Splitter
{
    public class AeronetFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public List<string> DataConfigs { get; private set; }

        public AeronetFile()
        {
            this.DataConfigs=new List<string>();
        }

        public void Save(string root, string chartSetName)
        {
            string extension = "aeronet";
            string file = System.IO.Path.Combine(root, string.Format("{0}.{1}", chartSetName, extension));
            string[] arrDatas = this.DataConfigs.ToArray();

            // apply defaults
            dynamic aeronet = new
            {
                name = this.Name,
                datapath = this.Path,
                datas=arrDatas
            };
            using (StreamWriter sw = new StreamWriter(file, false))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), aeronet);
            }
        }
    }
}
