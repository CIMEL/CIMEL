namespace Peach.AeronetInversion
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
    }
}