using Aeronet.Chart.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    public partial class ConfigOptions
    {
        private static ConfigOptions _default = new ConfigOptions();

        protected string OptionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Options", "options.json");

        protected ConfigOptions()
        {
            // check if existing
            if (File.Exists(this.OptionsPath))
            {
                this.Load(this.OptionsPath);
            }
            else
            {
                // initial it
                this.Initial(this.OptionsPath);
            }

            // initial all of the working folders
            this.InitialEnv();
        }

        private void InitialEnv()
        {
            string[] pathes=new string[]
            {
                this.DATA_Dir,this.CHARTSET_Dir,this.INS_PARA_Dir,this.METADATA_Dir,this.MODIS_BRDF_Dir,this.OUTPUT_Dir
            };

            // initial path
            Parallel.ForEach(pathes, p =>
            {
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);
            });
        }

        [Browsable(false)]
        public static ConfigOptions Singleton
        {
            get
            {
                return _default;
            }
        }

        [Browsable(false)]
        public bool IsInitialized { get; set; }

        [Category(CATELOG_INPUT),
        DisplayName(@"主数据文件"),
        Description("AERONET反演算法输入文件的目录"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string DATA_Dir { get; set; }

        [Category(CATELOG_INPUT),
        DisplayName(@"BRDF文件"),
        Description("MODIS观测数据的BRDF参数目录"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string MODIS_BRDF_Dir { get; set; }

        [Category(CATELOG_INPUT),
        DisplayName(@"参数文件"),
        Description("参数文件以及数据文件主目录"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string INS_PARA_Dir { get; set; }

        [Category(CATELOG_INPUT),
        DisplayName(@"其他数据文件"),
        Description("其他必要数据文件目录"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string METADATA_Dir { get; set; }

        [Category(CATELOG_OUTPUT),
        DisplayName(@"Output"),
        Description("The output path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string OUTPUT_Dir { get; set; }

        [Category(CATELOG_OUTPUT),
        DisplayName(@"Chart Set"),
        Description("The chart set path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string CHARTSET_Dir { get; set; }

        [Category(CATELOG_PROGRAM),
        DisplayName(@"Outputor"),
        Description("Outputor"),
        ReadOnly(true)]
        public string PROGRAM_OUTPUTOR { get; set; }

        [Category(CATELOG_PROGRAM),
        DisplayName(@"Creator"),
        Description("Initial"),
        ReadOnly(true)]
        public string PROGRAM_CREATOR { get; set; }

        [Category(CATELOG_PROGRAM),
        DisplayName(@"Drawer"),
        ReadOnly(true)]

        public string PROGRAM_DRAWER { get; set; }
        [Category(CATELOG_PROGRAM),
        DisplayName(@"Splitter"),
        ReadOnly(true)]
        public string PROGRAM_SPLITTER { get; set; }

        /// <summary>
        /// Loads the options to memory
        /// </summary>
        /// <param name="optionFile"></param>
        private void Load(string optionFile)
        {
            string content = File.ReadAllText(optionFile);
            // initial if it's empty file
            if (string.IsNullOrEmpty(content))
            {
                this.Initial(optionFile);
            }
            else
            {
                try
                {
                    var options = (dynamic)JObject.Parse(content);
                    this.DATA_Dir = (string)options.input.data;
                    this.MODIS_BRDF_Dir = (string)options.input.modis_brdf;
                    this.INS_PARA_Dir = (string)options.input.ins_para;
                    this.METADATA_Dir = (string)options.input.metadata;
                    this.OUTPUT_Dir = (string)options.output.output;
                    this.CHARTSET_Dir = (string) options.output.chartset;
                    this.PROGRAM_CREATOR = (string)options.processor.creator;
                    this.PROGRAM_OUTPUTOR = (string)options.processor.outputor;
                    this.PROGRAM_DRAWER=(string)options.processor.drawer;
                    this.PROGRAM_SPLITTER = (string) options.processor.splitter;
                    bool isNotCompleted = string.IsNullOrEmpty(this.DATA_Dir)
                                          || string.IsNullOrEmpty(this.MODIS_BRDF_Dir)
                                          || string.IsNullOrEmpty(this.INS_PARA_Dir)
                                          || string.IsNullOrEmpty(this.METADATA_Dir)
                                          || string.IsNullOrEmpty(this.OUTPUT_Dir)
                                          || string.IsNullOrEmpty(this.CHARTSET_Dir)
                                          || string.IsNullOrEmpty(this.PROGRAM_CREATOR)
                                          || string.IsNullOrEmpty(this.PROGRAM_OUTPUTOR)
                                          || string.IsNullOrEmpty(this.PROGRAM_DRAWER)
                                          || string.IsNullOrEmpty(this.PROGRAM_SPLITTER);
                    this.IsInitialized = !isNotCompleted;
                }
                catch (Exception)
                {
                    // rebuild the config options if it's broken
                    this.Initial(optionFile);
                }

                if (!this.IsInitialized)
                {
                    this.Initial(optionFile);
                }
            }
        }

        /// <summary>
        /// Initial an empty option file
        /// </summary>
        /// <param name="optionFile"></param>
        private void Initial(string optionFile)
        {
            dynamic options = new
            {
                input = new
                {
                    data =
                        string.IsNullOrEmpty(this.DATA_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data")
                            : this.DATA_Dir,
                    modis_brdf =
                        string.IsNullOrEmpty(this.MODIS_BRDF_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modis_brdf")
                            : this.MODIS_BRDF_Dir,
                    ins_para =
                        string.IsNullOrEmpty(this.INS_PARA_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ins_para")
                            : this.INS_PARA_Dir,
                    metadata =
                        string.IsNullOrEmpty(this.METADATA_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadata")
                            : METADATA_Dir,
                },
                processor = new
                {
                    creator =
                        string.IsNullOrEmpty(this.PROGRAM_CREATOR)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData",
                                "create_input_carsnet.exe")
                            : this.PROGRAM_CREATOR,
                    outputor =
                        string.IsNullOrEmpty(this.PROGRAM_OUTPUTOR)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "main.exe")
                            : this.PROGRAM_OUTPUTOR,
                    drawer =
                        string.IsNullOrEmpty(this.PROGRAM_DRAWER)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "draw.exe")
                            : this.PROGRAM_DRAWER,
                    splitter =
                        string.IsNullOrEmpty(this.PROGRAM_SPLITTER)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "splitter.exe")
                            : this.PROGRAM_SPLITTER,
                },
                output = new
                {
                    output =
                        string.IsNullOrEmpty(this.OUTPUT_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output")
                            : OUTPUT_Dir,
                    chartset =
                        string.IsNullOrEmpty(this.CHARTSET_Dir)
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chartset")
                            : CHARTSET_Dir
                },
                isInit = true
            };

            using (StreamWriter sw = new StreamWriter(optionFile, false))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), options);
            }

            this.DATA_Dir = (string)options.input.data;
            this.MODIS_BRDF_Dir = (string)options.input.modis_brdf;
            this.INS_PARA_Dir = (string)options.input.ins_para;
            this.METADATA_Dir = (string)options.input.metadata;
            this.OUTPUT_Dir = (string)options.output.output;
            this.CHARTSET_Dir = (string) options.output.chartset;
            this.PROGRAM_CREATOR = (string) options.processor.creator;
            this.PROGRAM_OUTPUTOR = (string) options.processor.outputor;
            this.PROGRAM_DRAWER = (string) options.processor.drawer;
            this.PROGRAM_SPLITTER = (string) options.processor.splitter;
            this.IsInitialized = (bool)options.isInit;
        }

        /// <summary>
        /// Save the options from the user entered
        /// </summary>
        public void Save()
        {
            string optionFile = this.OptionsPath;
            dynamic option = new
            {
                input = new
                {
                    data = this.DATA_Dir,
                    modis_brdf = this.MODIS_BRDF_Dir,
                    ins_para = this.INS_PARA_Dir,
                    metadata = this.METADATA_Dir,
                },
                processor = new
                {
                    outputor = this.PROGRAM_OUTPUTOR,
                    creator = this.PROGRAM_CREATOR,
                    drawer=this.PROGRAM_DRAWER,
                    splitter=this.PROGRAM_SPLITTER
                },
                output = new
                {
                    output = this.OUTPUT_Dir,
                    chartset=this.CHARTSET_Dir
                }, 
                isInit = true
            };

            using (StreamWriter sw = new StreamWriter(optionFile, false))
            {
                JsonSerializer.Create().Serialize(new JsonTextWriter(sw), option);
            }
            // set the options to be initialized
            this.IsInitialized = true;
        }

        /// <summary>
        /// Retrieves all working folders within an array of FolderDescription
        /// </summary>
        /// <returns></returns>
        public FolderDescription[] GetFolders()
        {
            return new FolderDescription[]{
                new FolderDescription("Data",this.DATA_Dir,"The data folder of one year period of the region data"),
                new FolderDescription("Modis_BRDF",this.MODIS_BRDF_Dir,"The data folder of the Modis_BRDF data"),
                new FolderDescription("Ins_Para",this.INS_PARA_Dir,"The data folder of the Ins_Para data"),
                new FolderDescription("MetaData",this.METADATA_Dir,"The data folder containing other Aeronet computing meta data"),
                new FolderDescription("Output",this.OUTPUT_Dir,"The output folder to put the generated Aeronet data"),
                new FolderDescription("Chartset",this.CHARTSET_Dir,"The output folder to put the generated chart set data")
            };
        }
    }

    public class FolderDescription
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }

        public FolderDescription(string name, string path, string description)
        {
            this.Name = name;
            this.Path = path;
            this.Description = description;
        }
    }
}