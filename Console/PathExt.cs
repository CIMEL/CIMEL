#define NET35
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#if NET35
namespace Peach.AeronetInversion
{
    public class Path
    {
        public static string Combine(string path, params string[] paths)
        {
            if (paths == null || paths.Length == 0) return path;
            else
            {
                string result = path;
                foreach (string p in paths)
                {
                    result = System.IO.Path.Combine(result, p);
                }
                return result;
            }
        }
    }
}
#endif
