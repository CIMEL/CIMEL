using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart.Chart
{
    public partial class ChartPanel : UserControl
    {
        public string DataFolder { get; set; }

        public DataConfig DataConfig { get; set; }

        public ChartPanel()
        {
            InitializeComponent();
        }
    }
}
