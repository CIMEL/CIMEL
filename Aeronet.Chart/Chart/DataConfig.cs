using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aeronet.Chart.Chart
{
    public class DataConfig
    {
        public string Name { get; set; }

        public string Year { get; set; }

        public IDictionary<string, string[]> MonthAndDays { get; set; } 
    }
}
