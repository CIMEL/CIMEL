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
using System.Windows.Forms;

namespace Aeronet.Chart
{
    public class ConfigOptions
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

        [Category("INPUT"),
        DisplayName(@"Data"),
        Description("The data path of the aeronet data of a region"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string DATA_Dir { get; set; }

        [Category("INPUT"),
        DisplayName(@"Modis_BRDF"),
        Description("The modis_brdf data path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string MODIS_BRDF_Dir { get; set; }

        [Category("INPUT"),
        DisplayName(@"INS_Para"),
        Description("The ins_para data path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string INS_PARA_Dir { get; set; }

        [Category("INPUT"),
        DisplayName(@"Metadata"),
        Description("The metadata path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string METADATA_Dir { get; set; }

        [Category("OUTPUT"),
        DisplayName(@"Output"),
        Description("The output path"),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string OUTPUT_Dir { get; set; }

        [Category("PROCESSOR"),
        DisplayName(@"Outputor"),
        Description("Outputor"),
        ReadOnly(true),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string PROGRAM_OUTPUTOR { get; set; }

        [Category("PROCESSOR"),
        DisplayName(@"Creator"),
        Description("Initial"),
        ReadOnly(true),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
        public string PROGRAM_CREATOR { get; set; }

        [Category("PROCESSOR"),
        DisplayName(@"Drawer"),
        ReadOnly(true),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]

        public string PROGRAM_DRAWER { get; set; }
        [Category("PROCESSOR"),
        DisplayName(@"Splitter"),
        ReadOnly(true),
        Editor(typeof(FolderBrowserEditor), typeof(UITypeEditor))]
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
                    this.OUTPUT_Dir = (string)options.output;
                    this.PROGRAM_CREATOR = (string)options.processor.creator;
                    this.PROGRAM_OUTPUTOR = (string)options.processor.outputor;
                    bool isNotCompleted = string.IsNullOrEmpty(this.DATA_Dir)
                                          || string.IsNullOrEmpty(this.MODIS_BRDF_Dir)
                                          || string.IsNullOrEmpty(this.INS_PARA_Dir)
                                          || string.IsNullOrEmpty(this.METADATA_Dir)
                                          || string.IsNullOrEmpty(this.OUTPUT_Dir)
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
            // apply defaults
            dynamic options = new
            {
                input = new
                {
                    data = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"),
                    modis_brdf = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modis_brdf"),
                    ins_para = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ins_para"),
                    metadata = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadata"),
                },
                processor = new
                {
                    creator = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"AeronetData", "create_input_carsnet.exe"),
                    outputor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "main.exe"),
                    drawer = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "draw.exe"),
                    splitter = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AeronetData", "splitter.exe"),
                },
                output = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output"),
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
            this.OUTPUT_Dir = (string)options.output;
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
                    creator = this.PROGRAM_CREATOR
                },
                output = this.OUTPUT_Dir,
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