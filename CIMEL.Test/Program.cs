using CIMEL.Chart;
using CIMEL.Core;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIMEL.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Agv agv = new Agv();
            agv.Run();
            //MultipleLine lines = new MultipleLine();
            //lines.Run();
            //Agv2 agv2 = new Agv2();
            //agv2.Run();
        }
    }
}
