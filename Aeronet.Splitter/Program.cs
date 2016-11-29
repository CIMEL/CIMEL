using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace Aeronet.Splitter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // check argument
                if (args == null || args.Length == 0)
                    throw new ArgumentException("Missing Argument: aeronet data file (.dat)");

                string datFile = args[0];
                // check if the file exists
                if (!File.Exists(datFile))
                    throw new FileNotFoundException("Not found file: " + datFile);

                // loads file
                using (FileStream fs = new FileStream(datFile, FileMode.Open))
                {
                    if (fs.Length == 0)
                        throw new FileLoadException("Empty data file: " + datFile);

                    try
                    {
                        // loop data lines
                        using (StreamReader sr = new StreamReader(fs))
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
                                    string[] arrFields = strHeader.Split(new Char[] { ',' }, StringSplitOptions.None);
                                    if (arrFields.Length < 6) throw new FileLoadException("Missing aeronet data");
                                    // skip the first 6 fields which are the date and time fields
                                    for (int i = 6; i < arrFields.Length; i++)
                                    {
                                        string strField = arrFields[i].Trim().ToLower();
                                        // initial global index
                                        ChartMapping chartMapping = ChartMappings.Signleton[strField];
                                        if (chartMapping == null) continue;
                                        // create the mapping of column index - chart mapping
                                        ChartMappings.Signleton.CreateIndexMapping(i, chartMapping);
                                        if (chartMapping.Fields.ContainsKey(strField))
                                            chartMapping.Fields[strField].Global = i;
                                    }

                                    // reads line data
                                    while (!sr.EndOfStream)
                                    {
                                        string strLineData = sr.ReadLine();
                                        // skips the empty line
                                        if (string.IsNullOrEmpty(strLineData)) continue;
                                        string[] arrLineDatas = strLineData.Split(new Char[] { ',' },
                                            StringSplitOptions.None);
                                        // skips the broken line
                                        if (arrLineDatas.Length < 6) continue;
                                        // the first 6 fields are the date and time fields
                                        string year = arrLineDatas[0].Trim();
                                        string month = arrLineDatas[1].Trim();
                                        string day = arrLineDatas[2].Trim();
                                        string hour = arrLineDatas[3].Trim();
                                        string min = arrLineDatas[4].Trim();
                                        string second = arrLineDatas[5].Trim();
                                        for (int i = 6; i < arrLineDatas.Length; i++)
                                        {
                                            string strValue = arrLineDatas[i].Trim();
                                            // lookup ChartMapping
                                            var chartMapping = ChartMappings.Signleton[i];
                                            if (chartMapping == null) continue;
                                            // initial year, month and day to .dataconfig
                                            var dataConfig = chartMapping.DataConfigFile;
                                            dataConfig.Year = year;
                                            dataConfig.AddDay(month, day);
                                            // initial current value to .data file
                                            var datas = chartMapping.DataFiles;
                                            datas.AddData(year, month, day, hour, min, second, i, strValue);
                                        }
                                    }
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

                // initial Aeronet file attributes
                string chartSetName = Path.GetFileNameWithoutExtension(datFile);
                chartSetName = string.Format("{0}_{1:yyyMMddhhmmssfff}", chartSetName, DateTime.Now);
                string root = Path.GetDirectoryName(datFile);
                string chartSetPath = Path.Combine(root, chartSetName);

                // initial the chartSet folder
                if (!Directory.Exists(chartSetPath))
                    Directory.CreateDirectory(chartSetPath);

                var aeronetFile = ChartMappings.Signleton.AeronetFile;
                aeronetFile.Name = chartSetName;
                aeronetFile.Path = chartSetPath;

                // save data config files and data files
                foreach (ChartMapping objChartMapping in ChartMappings.Signleton.GetAll())
                {
                    string chartName = objChartMapping.Name;
                    string chartDesc = objChartMapping.Description;
                    // generate data config file (.dataconfig)
                    objChartMapping.DataConfigFile.Save(chartSetPath, chartName);
                    // ChartName | Description
                    aeronetFile.DataConfigs.Add(string.Format("{0}|{1}", chartName, chartDesc));
                    // splits data into one-day data file (.data)
                    string strHeader = objChartMapping.ToHeader();
                    objChartMapping.DataFiles.Save(chartSetPath, chartName, strHeader);
                }
                // generate aeronet file (.aeronet)
                aeronetFile.Save(root, chartSetName);
            }
            catch (Exception ex)
            {
                // alerts the error msg and exit program
                Console.WriteLine(ex.Message);
            }
        }
    }
}