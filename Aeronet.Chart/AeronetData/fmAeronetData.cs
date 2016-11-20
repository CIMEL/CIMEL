using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aeronet.Chart.AeronetData;

namespace Aeronet.Chart
{
    public partial class fmAeronetData : Form
    {
        public fmAeronetData()
        {
            InitializeComponent();
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            bool isStart = button.Text.CompareTo("Start") == 0;
            if (isStart)
            {
                // start the worker
                // 1 show stop function
                button.Text = @"Stop";
                // 2 initial worker and run it
                var worker = new DataWorker();
                worker.Work();
            }
            else
            {
                // stop the worker
                // 1 stop the running worker
                // 2 show start function
                button.Text = @"Start";
            }
        }
    }
}
