using Aeronet.Chart.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Aeronet.Chart
{
    public class ConfigOptions
    {
        //public static string STNS_FN = GetValue("ARG_STNS_FN", @default: "hangzhou");
        public static string FIPT = Utility.GetAppSettingValue("ARG_FIPT", @default: "ins_para");

        public static string FOUT = Utility.GetAppSettingValue("ARG_FOUT", @default: "input");
        public static string FBRDF = Utility.GetAppSettingValue("ARG_FBRDF", @default: "modis_brdf");
        public static string FDAT = Utility.GetAppSettingValue("ARG_FDAT", @default: "data");
        public static string PROGRAM_OUTPUTOR = Utility.GetAppSettingValue("Outputor", @default: "main.exe");
        public static string PROGRAM_CREATOR = Utility.GetAppSettingValue("Creator", @default: "create_input_carsnet.exe");
        public static string REGIONS_STORE = Utility.GetAppSettingValue("Regions_store", @default: "regions.json");

        private static ConfigOptions _default = new ConfigOptions();

        protected string OptionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Options", "options.json");

        protected ConfigOptions()
        {
            string dataPath =
            if (File.Exists(dataPath))
            {
                this.Load(dataPath);
            }
            else
            {
            }
        }

        public static ConfigOptions Singleton
        {
            get
            {
                return _default;
            }
        }

        public bool IsInitialized { get; protected set; }
        public string DATA_Dir { get; protected set; }
        public string MODIS_BRDF_Dir { get; protected set; }
        public string INS_PARA_Dir { get; protected set; }
        public string METADATA_Dir { get; protected set; }
        public string OUTPUT_Dir { get; protected set; }

        private void Load(string optionFile)
        {
            string content = File.ReadAllText(optionFile);
            var options = (dynamic)JObject.Parse(content);
            this.DATA_Dir = options.input.data;
            this.METADATA_Dir = options.input.modis_brdf;
            this.INS_PARA_Dir = options.input.ins_para;
            this.METADATA_Dir = options.input.metadata;
            this.OUTPUT_Dir = options.output;
        }

        private void Initial()
        {
        }

        private void Save()
        {
        }
    }
}