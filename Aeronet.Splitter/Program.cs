using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace Aeronet.Splitter
{
    internal class Program
    {
        private static readonly Encoding EncodingCode = Encoding.UTF8;

        private static void Main(string[] args)
        {
            try
            {
                // check argument
                if (args == null || args.Length == 0)
                    throw new ArgumentException("Missing Argument: aeronet data file (.dat)");

                string datFile = args[0];
                if (args.Length < 2)
                    throw new ArgumentException("Missing Argument: chart set root directory");

                if (string.IsNullOrEmpty(Path.GetDirectoryName(datFile)))
                    datFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, datFile);
                // check if the file exists
                if (!File.Exists(datFile))
                    throw new FileNotFoundException("Not found file: " + datFile);

                // loads file
                OnInformed(string.Format("Loading dat file <- {0}", datFile));
                int intDataLines = 0;
                using (FileStream fs = new FileStream(datFile, FileMode.Open))
                {
                    if (fs.Length == 0)
                        throw new FileLoadException("Empty data file: " + datFile);

                    try
                    {
                        // loop data lines
                        using (StreamReader sr = new StreamReader(fs,EncodingCode))
                        {
                            try
                            {
                                // reads header
                                if (!sr.EndOfStream)
                                {
                                    string strHeader = sr.ReadLine();
                                    // skips the empty line
                                    if (string.IsNullOrEmpty(strHeader))
                                        throw new FileLoadException("Not found header line");
                                    OnInformed("Reading header line");
                                    string[] arrFields = strHeader.Split(new Char[] {','}, StringSplitOptions.None);
                                    if (arrFields.Length < 6) throw new FileLoadException("Missing aeronet data");
                                    // skip the first 6 fields which are the date and time fields
                                    for (int i = 6; i < arrFields.Length; i++)
                                    {
                                        string strField = arrFields[i].Trim().ToLower();
                                        // initial global index
                                        ChartMapping chartMapping = ChartMappings.Signleton[strField,IndexType.ColumnName];
                                        if (chartMapping == null) continue;
                                        // create the mapping of column index - chart mapping
                                        ChartMappings.Signleton.CreateIndexMapping(i, chartMapping);
                                        if (chartMapping.Fields.ContainsKey(strField))
                                            // obtains the column index in the header line
                                            chartMapping.Fields[strField] = i;
                                    }

                                    // reads line data
                                    OnInformed("Reading data lines");
                                    while (!sr.EndOfStream)
                                    {
                                        string strLineData = sr.ReadLine();
                                        // skips the empty line
                                        if (string.IsNullOrEmpty(strLineData)) continue;
                                        string[] arrLineDatas = strLineData.Split(new Char[] {','},
                                            StringSplitOptions.None);
                                        // skips the broken line
                                        if (arrLineDatas.Length < 6) continue;
                                        // the first 6 fields are the date and time fields
                                        string strYear = arrLineDatas[0].Trim();
                                        string month = arrLineDatas[1].Trim();
                                        string day = arrLineDatas[2].Trim();
                                        string hour = arrLineDatas[3].Trim();
                                        string min = arrLineDatas[4].Trim();
                                        string second = arrLineDatas[5].Trim();
                                        for (int i = 6; i < arrLineDatas.Length; i++)
                                        {
                                            string strValue = arrLineDatas[i].Trim();
                                            // validates the values, drop it if equals 'NaN'
                                            if (string.IsNullOrEmpty(strValue) ||
                                                string.Compare(strValue, "NaN",
                                                    StringComparison.CurrentCultureIgnoreCase) == 0)
                                                continue;
                                            // lookup ChartMapping
                                            var chartMapping = ChartMappings.Signleton[i];
                                            if (chartMapping == null) continue;
                                            // initial year, month and day to .dataconfig
                                            var dataConfig = chartMapping.DataConfigFile;
                                            int year;
                                            if (!int.TryParse(strYear, out year))
                                                year = DateTime.Now.Year;
                                            dataConfig.Year = year;
                                            dataConfig.AddDay(month, day);
                                            // initial current value to .data file
                                            var datas = chartMapping.DataFiles;
                                            datas.AddData(year.ToString(), month, day, hour, min, second, i, strValue);
                                        }

                                        intDataLines++;
                                    }
                                }
                                else
                                {
                                    throw new FileLoadException("Empty file!");
                                }
                            }
                            finally
                            {
                                sr.Close();
                            }
                        }
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
                OnInformed(string.Format("Read {0} data lines", intDataLines));
                OnInformed("Initial chart set data");
                // initial Aeronet file attributes
                string chartSetName = Path.GetFileNameWithoutExtension(datFile);
                chartSetName = string.Format("{0}_{1:yyyMMddhhmmssfff}", chartSetName, DateTime.Now);
                string root = args[1];
                string chartSetPath = Path.Combine(root, chartSetName);

                // initial the chartSet folder
                if (!Directory.Exists(chartSetPath))
                    Directory.CreateDirectory(chartSetPath);

                var aeronetFile = ChartMappings.Signleton.AeronetFile;
                aeronetFile.Name = chartSetName;
                aeronetFile.Path = chartSetPath;
                OnInformed(string.Format("Chart Set Name -> {0}",chartSetName));
                OnInformed(string.Format("Chart Set Root-> {0}", chartSetPath));

                // save data config files and data files
                foreach (var pair in ChartMappings.Signleton.GetGroups())
                {
                    var chartMappingGroup = pair.Value;

                    string strGroupNames=string.Empty;

                    string[] arrGroups = chartMappingGroup.SubCharts;

                    foreach (var strChartName in arrGroups)
                    {
                        var objChartMapping = ChartMappings.Signleton[strChartName, IndexType.ChartMappingName];

                        // skip the chart mapping if no data file extracted from the aeronet inversion file(.dat)
                        if (!objChartMapping.HasDataFiles) continue;

                        OnInformed(string.Format("Processing data: {0} - {1}", objChartMapping.Name, objChartMapping.Description));
                        string chartName = objChartMapping.Name;
                        strGroupNames += string.Format("{0}@", chartName);
                        // generate data config file (.dataconfig)
                        string dataConfigFile = objChartMapping.DataConfigFile.Save(chartSetPath, chartName);
                        OnInformed(string.Format("{0}: Saved -> {1}", objChartMapping.Name, dataConfigFile));
                        // splits data into one-day data file (.data)
                        string strHeader = objChartMapping.ToHeader();
                        string strSaved = objChartMapping.DataFiles.Save(chartSetPath, chartName, strHeader);
                        OnInformed(string.Format("{0}: Saved -> {1} data files", objChartMapping.Name, strSaved));
                    }

                    string chartDesc = chartMappingGroup.Description;
                    if (strGroupNames.EndsWith("@"))
                        strGroupNames = strGroupNames.Substring(0, strGroupNames.Length - 1);
                    // ChartName | Description
                    aeronetFile.DataConfigs.Add(string.Format("{0}|{1}", strGroupNames, chartDesc));
                }
                // generate aeronet file (.aeronet)
                string aeronetfile = aeronetFile.Save(root, chartSetName);
                OnInformed(string.Format("Saved aeronet file -> {0}", aeronetfile));
            }
            catch (Exception ex)
            {
                // alerts the error msg and exit program
                OnFailed(ex.Message);
            }
        }

        /// <summary>
        /// put the error message to error stream
        /// </summary>
        /// <param name="error"></param>
        private static void OnFailed(string error)
        {
            Console.Error.WriteLine(error);
        }

        /// <summary>
        /// put the info message to std output stream
        /// </summary>
        /// <param name="info"></param>
        private static void OnInformed(string info)
        {
            Console.Out.WriteLine(info);
        }
    }
}