using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    public class CIMELConst
    {
        public const string GLOBAL_DLG_TITLE = "CIMEL太阳光度计数据处理软件";
    }

    public partial class ConfigOptions
    {
        public const string CATELOG_INPUT = "输入";
        public const string CATELOG_OUTPUT = "输出";
        public const string CATELOG_PROGRAM = "程序";
        public const string DATA_NAME = "主数据输入目录";
        public const string MODIS_BRDF_NAME = "BRDF参数目录";
        public const string INS_PARA_NAME = "参数目录";
        public const string METADATA_NAME = "工作目录";
        public const string OUTPUT_NAME = "主数据输出目录";
        public const string CHARTSET_NAME = "图像集输入目录";
        public const string DATA_DESC = "CIMEL反演算法输入文件的目录";
        public const string MODIS_BRDF_DESC = "MODIS观测数据的BRDF参数目录";
        public const string INS_PARA_DESC = "参数文件以及数据文件主目录";
        public const string METADATA_DESC = "程序工作目录，包含所有参数和其他必要数据文件目录";
        public const string OUTPUT_DESC = "生成矩阵数据文件目录";
        public const string CHARTSET_DESC = "生成的数据图像集数据目录";
    }
}

namespace CIMEL.Chart.Options
{
    public partial class fmRegions
    {
        public const string DLG_TITLE_ERROR = "站点配置错误";
        public const string DLG_TITLE = "站点配置";
    }

    public partial class Region
    {
        public const string CATELOG_REGION = @"站点";
    }

    public partial class fmOptions
    {
        public const string DLG_TITLE_ERROR = "参数配置错误";
        public const string DLG_TITLE = "参数配置";
    }
}
