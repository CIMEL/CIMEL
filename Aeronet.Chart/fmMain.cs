using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
        }

        private void aeronetDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmAeronetData fmAeronetData = new fmAeronetData();
            fmAeronetData.ShowDialog(this);
        }
    }
}