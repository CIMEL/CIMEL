using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aeronet.Chart.Options;

namespace Aeronet.Chart
{
    public partial class fmMain : Form
    {
        public fmMain()
        {
            InitializeComponent();
            this.Load += fmMain_Load;
        }

        void fmMain_Load(object sender, EventArgs e)
        {
            // check if the options has been configurated
            if (!ConfigOptions.Singleton.IsInitialized)
            {
                fmOptions fmOptions=new fmOptions();
                fmOptions.ShowDialog(this);
            }

        }

        private void aeronetDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmAeronetData fmAeronetData = new fmAeronetData();
            fmAeronetData.ShowDialog(this);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmOptions fmOptions = new fmOptions();
            fmOptions.ShowDialog(this);
        }
    }
}