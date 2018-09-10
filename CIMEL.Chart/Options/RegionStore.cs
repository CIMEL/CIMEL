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
using System.Windows.Forms;
using Peach.Log;

namespace CIMEL.Chart.Options
{
    public class RegionStore
    {
        private static readonly Encoding EncodingCode = Encoding.GetEncoding("GB2312");

        private static RegionStore _default = new RegionStore();

        protected string OptionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Options", "regions.json");

        protected RegionStore()
        {
            try
            {
                string dataPath = OptionsPath;

                string content = File.ReadAllText(dataPath, EncodingCode);
                var j = new JsonTextReader(new StringReader(content));
                var serializer = new JsonSerializer();
                _regions = serializer.Deserialize<Regions>(j);
            }
            catch (Exception ex)
            {
                _regions = new Regions();
                Logger.Default.Error("Failed to load states from regions.json", ex);
                Form fmMain = Application.OpenForms.Cast<Form>().FirstOrDefault();
                Utility.ShowAlertDlg(fmMain,@"缺少站点配置", fmRegions.DLG_TITLE_ERROR);
            }
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

                using (StreamWriter sw = new StreamWriter(optionFile, false,EncodingCode))
                {
                    JsonSerializer.Create().Serialize(new JsonTextWriter(sw), _regions);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Default.Error("保存 regions.json 失败 <- " + ex.Message);
                return false;
            }
        }
    }

    [DataContract]
    [DefaultProperty("Name")]
    public partial class Region
    {
        [Category(CATELOG_REGION),
        DisplayName(@"名称"),
        Description("站点名称"),
        DataMember(Name = "name"),
        PropertyOrder(0)]
        public string Name { get; set; }
        [Category(CATELOG_REGION),
        DisplayName(@"纬度"),
        Description("站点所在纬度"),
        DataMember(Name = "lat"),
        PropertyOrder(2)]
        public double Lat { get; set; }
        [Category(CATELOG_REGION),
        DisplayName(@"经度"),
        Description("站点所在经度"),
        DataMember(Name = "lon"),
        PropertyOrder(1)]
        public double Lon { get; set; }
    }

    [DataContract]
    public class Regions
    {
        [DataMember(Name = "regions")]
        public List<Region> RegionList { get; set; }

        public Regions()
        {
            RegionList=new List<Region>();
        }
    }
}