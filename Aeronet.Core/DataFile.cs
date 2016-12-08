using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeronet.Core
{
    public class DataFiles
    {
        private Dictionary<string, DataFile> _dataFiles;

        public DataFiles()
        {
            this._dataFiles = new Dictionary<string, DataFile>();
        }

        public void AddData(string year, string month, string day, string hour, string min, string second,
            int fieldIndex, string value)
        {
            string key = string.Format("{0},{1},{2}", year, month, day);
            if (!this._dataFiles.ContainsKey(key))
                this._dataFiles.Add(key, new DataFile());

            this._dataFiles[key].AddData(hour, min, second, fieldIndex, value);
        }

        public string Save(string chartSetPath, string chartName, string strHeader)
        {
            string extension = "data";
            foreach (string day in this._dataFiles.Keys)
            {
                string oneDay = day.Replace(',', '.');
                string dataFile = Path.Combine(chartSetPath, string.Format("{0}.{1}.{2}", chartName, oneDay, extension));
                this._dataFiles[day].Save(dataFile, day, strHeader);
            }

            return this._dataFiles.Count.ToString();
        }

        public int Count
        {
            get { return this._dataFiles.Count; }
        }
    }

    public class DataFile
    {
        public DataFile()
        {
            this.DataLines = new Dictionary<string, DataLine>();
        }

        public Dictionary<string, DataLine> DataLines { get; private set; }

        public void AddData(string hour, string min, string second,
            int fieldIndex, string value)
        {
            string key = string.Format("{0},{1},{2}", hour, min, second);
            if (!this.DataLines.ContainsKey(key))
                this.DataLines.Add(key, new DataLine());

            this.DataLines[key].AddData(fieldIndex, value);
        }

        public string Save(string fileName, string dayKeys, string strHeader)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(strHeader);
            foreach (string timeKeys in this.DataLines.Keys)
            {
                string dataLine = string.Format("{0},{1},{2}", dayKeys, timeKeys, this.DataLines[timeKeys].ToString());
                sb.AppendLine(dataLine);
            }
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                sw.Write(sb.ToString());
                sw.Flush();
                sw.Close();
            }
            return fileName;
        }

        public bool IsEmpty()
        {
            return this.DataLines.Count == 0;
        }
    }

    public class DataLine
    {
        public DataLine()
        {
            this.Values = new Dictionary<int, string>();
        }

        // index, value
        public Dictionary<int, string> Values { get; private set; }

        public void AddData(int fieldIndex, string value)
        {
            // don't add duplicated value
            if (!this.Values.ContainsKey(fieldIndex))
                this.Values.Add(fieldIndex, value);
        }

        public override string ToString()
        {
            string[] arrValues = this.Values.OrderBy(p => p.Key).Select(p => p.Value).ToArray();
            string strValues = string.Join(",", arrValues);
            return strValues;
        }
    }
}