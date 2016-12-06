using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Peach.Log;

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
            _regions = serializer.Deserialize<Regions>(j);
        }

        public static RegionStore Singleton { get { return _default; } }

        private Regions _regions;

        public Region FindRegion(string regionName)
        {
            return
                this._regions.RegionList.FirstOrDefault(
                    r => Regex.IsMatch(r.Name, "^" + regionName + "$", RegexOptions.IgnoreCase));
        }

        public List<Region> GetRegions()
        {
            return this._regions.RegionList.OrderBy(r=>r.Name).ToList();
        }

        public bool Save(List<Region> lstRegions)
        {
            try
            {
                string optionFile = this.OptionsPath;
                this._regions.RegionList = lstRegions;

                using (StreamWriter sw = new StreamWriter(optionFile, false))
                {
                    JsonSerializer.Create().Serialize(new JsonTextWriter(sw), _regions);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Default.Error("Failed to save regions.json <- " + ex.Message);
                return false;
            }
        }
    }

    [DataContract]
    public class Region
    {
        [Category("地区"),
        DisplayName(@"名称"),
        Description("地区名称")]
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [Category("地区"),
        DisplayName(@"纬度"),
        Description("地区所在纬度")]
        [DataMember(Name = "lat")]
        public double Lat { get; set; }
        [Category("地区"),
        DisplayName(@"经度"),
        Description("地区所在经度")]
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