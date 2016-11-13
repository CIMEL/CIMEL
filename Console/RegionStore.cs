using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Peach.AeronetInversion
{
    public class RegionStore
    {
        private static RegionStore _default=new RegionStore();

        protected RegionStore()
        {
            string regionStore = ConfigOptions.REGIONS_STORE;
            string dataPath = Regex.IsMatch(regionStore, "^regions.json$", RegexOptions.IgnoreCase)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, regionStore)
                : regionStore;

            string content = File.ReadAllText(dataPath);
            var j = new JsonTextReader(new StringReader(content));
            var serializer = new JsonSerializer();
            regions = serializer.Deserialize<Regions>(j);
        }

        public static RegionStore Singleton { get { return _default;} }

        private Regions regions;

        public Region FindRegion(string regionName)
        {
            return
                this.regions.RegionList.FirstOrDefault(
                    r => Regex.IsMatch(r.Name, "^" + regionName + "$", RegexOptions.IgnoreCase));
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
