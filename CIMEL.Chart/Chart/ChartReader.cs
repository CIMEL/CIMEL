using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    /// <summary>
    /// Reads the chart data from the one-day data file
    /// </summary>
    public class ChartReader
    {
        private string _dataFile;

        private string _dataName;

        public ChartReader(string dataFolder,string dataName, int year, int month, int day)
        {
            // initial data file full name
            string fileName = string.Format("{0}.{1}.{2}.{3}.data",dataName,year,month,day);
            _dataName = dataName;
            this._dataFile = Path.Combine(dataFolder, fileName);
        }

        public ChartLine[] Read(List<double> axisXs )
        {
            ChartLine[] result;
            using (FileStream fs=new FileStream(this._dataFile,FileMode.Open))
            {
                using (StreamReader sr=new StreamReader(fs))
                {
                    // reads header
                    if (!sr.EndOfStream)
                    {
                        // extract header data
                        sr.ReadLine();
                        //if (string.IsNullOrEmpty(headerLine)) return new ChartLine[] {};
                        //string[] headerData = headerLine.Split(new char[] { ',' }, StringSplitOptions.None);
                        IList<ChartLine> chartlines = new List<ChartLine>();
                        while (!sr.EndOfStream)
                        {
                            // initial line data
                            string lineData = sr.ReadLine();
                            var chartLine = ChartLineFactory.GetFactory(this._dataName).Create(lineData, axisXs);
                            chartlines.Add(chartLine);
                        }
                        result= chartlines.ToArray();
                    }
                    else
                    {
                        result= new ChartLine[] {};
                    }
                    sr.Close();
                }
            }
            return result;
        }
    }
}
