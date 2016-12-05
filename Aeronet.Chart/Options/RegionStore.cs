using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Aeronet.Chart.Options
{
    public class RegionStore
    {
        private static RegionStore _default = new RegionStore();

        protected string OptionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Options", "regions.json");

        protected RegionStore()
        {
            string dataPath = OptionsPath;

            string content = File.ReadAllText(dataPath);
            var j = new JsonTextReader(new StringReader(content));
            var serializer = new JsonSerializer();
            regions = serializer.Deserialize<Regions>(j);
        }

        public static RegionStore Singleton { get { return _default; } }

        private Regions regions;

        public Region FindRegion(string regionName)
        {
            return
                this.regions.RegionList.FirstOrDefault(
                    r => Regex.IsMatch(r.Name, "^" + regionName + "$", RegexOptions.IgnoreCase));
        }

        public List<Region> GetRegions()
        {
            return this.regions.RegionList;
        }
    }

    [DataContract]
    public class Region
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "lat")]
        public double Lat { get; set; }

        [DataMember(Name = "lon")]
        public double Lon { get; set; }
    }

    [DataContract]
    public class Regions
    {
        [DataMember(Name = "regions")]
        public List<Region> RegionList { get; set; }
    }
}