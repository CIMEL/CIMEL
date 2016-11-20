using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Aeronet.Chart
{
    public class Utility
    {
        public static void PrintMatrix(double[,] arrary,Action<string,bool> log )
        {
            int d1 = arrary.GetLength(0);
            int d2 = arrary.GetLength(1);
            for (int i = 0; i < d1; i++)
            {
                string line = string.Empty;
                for (int j = 0; j < d2; j++)
                {
                    line += arrary[i, j] + " ";

                }
                log.Invoke(line,true);
            }
        }

        public static string GetAppSettingValue(string key,string @default)
        {
            try
            {
                string o = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(o)) return o;
                return @default;
            }
            catch (Exception)
            {
                return @default;
            }
        }
    }
}
