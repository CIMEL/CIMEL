using System.Collections.Generic;
using System.IO;

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
}