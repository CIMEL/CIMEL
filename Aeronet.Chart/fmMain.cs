using Aeronet.Chart.Options;
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
            this.Load += fmMain_Load;
            // set position
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            // check if the options has been configurated
            if (!ConfigOptions.Singleton.IsInitialized)
            {
                fmOptions fmOptions = new fmOptions();
                fmOptions.StartPosition = FormStartPosition.CenterParent;
                fmOptions.ShowDialog(this);
            }
        }

        private void aeronetDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmAeronetData fmAeronetData = new fmAeronetData();
            fmAeronetData.StartPosition = FormStartPosition.CenterParent;
            fmAeronetData.ShowDialog(this);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmOptions fmOptions = new fmOptions();
            fmOptions.StartPosition = FormStartPosition.CenterParent;
            fmOptions.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(new CancelEventArgs(false));
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmAboutBox fmAboutBox = new fmAboutBox();
            fmAboutBox.StartPosition = FormStartPosition.CenterParent;
            fmAboutBox.ShowDialog(this);
        }
    }
}