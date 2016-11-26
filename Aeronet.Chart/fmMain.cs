using Aeronet.Chart.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aeronet.Chart.AeronetData;

namespace Aeronet.Chart
{
    public partial class fmMain : Form
    {
        private AeronetDataSet _currentDataSet;

        public fmMain()
        {
            InitializeComponent();
            this.Load += fmMain_Load;
            // set position
            this.StartPosition = FormStartPosition.CenterScreen;
            // clean up items of the combox
            this.cmbDataSets.Items.Clear();
            this.cmbCharts.Items.Clear();
            // initial item structure
            this.cmbDataSets.DisplayMember = "Text";
            this.cmbDataSets.ValueMember = "Value";

            this.cmbCharts.DisplayMember = "Text";
            this.cmbCharts.ValueMember = "Value";
            // register selectedChanged
            this.cmbDataSets.SelectedIndexChanged += cmbDataSets_SelectedIndexChanged;
            this.cmbCharts.SelectedIndexChanged += cmbCharts_SelectedIndexChanged;
        }

        void cmbCharts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbCharts.Text))
                return; // nothiing to do
            string chartConfigDataFile = Path.Combine(this._currentDataSet.Path,((dynamic)cmbCharts.SelectedItem).Value);
            this.chartPanel1.DataConfigFile = chartConfigDataFile;
            // !!! don't forget to initial the chart panel
            this.chartPanel1.Init();
        }

        void cmbDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // clean up the items of combox charts
            this.cmbCharts.Items.Clear();

            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbDataSets.Text))
                return; // nothiing to do

            string dataSetFile = ((dynamic)cmbDataSets.SelectedItem).Value;
            this._currentDataSet = new AeronetDataSet(dataSetFile);


            foreach (var chart in this._currentDataSet.Datas)
            {
                // without the extension
                int splitor = chart.IndexOf(".", StringComparison.Ordinal);
                this.cmbCharts.Items.Add(new {Text=chart.Substring(0,splitor),Value=chart});
            }
            this.cmbCharts.SelectedIndex = 0;
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

            // the aeronet data sets
            this.Scan();
            
        }
        /// <summary>
        /// Scans the processed Aeronet data sets within output folder
        /// </summary>
        private void Scan()
        {
            // clean up the combox of data sets
            this.cmbDataSets.Items.Clear();
            string outputFolder = ConfigOptions.Singleton.OUTPUT_Dir;
            string[] dataSets = Directory.GetFiles(outputFolder, "*.aeronet", SearchOption.TopDirectoryOnly);
            if (dataSets.Length == 0)
            {
                MessageBox.Show(this,
                    @"Not found any processed aeronet data sets, please process the data within the Aeronet Data dialog firstly.",
                    @"Aeronet Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (string dataSet in dataSets)
            {
                // without the extension
                string fileName = Path.GetFileNameWithoutExtension(dataSet);
                this.cmbDataSets.Items.Add(new {Text=fileName,Value=dataSet});
            }

            this.cmbDataSets.SelectedIndex = 0;
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
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fmAboutBox fmAboutBox = new fmAboutBox();
            fmAboutBox.StartPosition = FormStartPosition.CenterParent;
            fmAboutBox.ShowDialog(this);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            this.Scan();
        }
    }
}