using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aeronet.Chart
{
    public class Utility
    {
        public static bool IsEmpty(string dir)
        {
            return string.IsNullOrEmpty(dir);
        }

        public static bool IsExist(string dir)
        {
            return Directory.Exists(dir);
        }

        public static bool IsInit(string dir)
        {
            return !IsEmpty(dir) && IsExist(dir);
        }
    }
}
