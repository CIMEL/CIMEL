﻿using Aeronet.Chart.AeronetData;
using Aeronet.Chart.Options;
using Aeronet.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart
{
    public partial class fmMain : Form
    {
        private AeronetFile _currentFile;

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
            this.cmbDataSets.DisplayMember = ComboBoxItem.DisplayName;
            this.cmbDataSets.ValueMember = ComboBoxItem.ValueName;

            this.cmbCharts.DisplayMember = ComboBoxItem.DisplayName;
            this.cmbCharts.ValueMember = ComboBoxItem.ValueName;
            // register selectedChanged
            this.cmbDataSets.SelectedIndexChanged += cmbDataSets_SelectedIndexChanged;
            this.cmbCharts.SelectedIndexChanged += cmbCharts_SelectedIndexChanged;
        }

        private void cmbCharts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the chart panel
            this.chartPanel1.Disable();

            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbCharts.Text) || cmbCharts.Text == ComboBoxItem.EmptyItem.Text)
                return; // nothiing to do
            // appends the extension (.dataconfig)
            string chartConfigDataFile = Path.Combine(this._currentFile.Path,
                string.Format("{0}.{1}", ((dynamic)cmbCharts.SelectedItem).Value, "dataconfig"));
            this.chartPanel1.DataConfigFile = chartConfigDataFile;
            // !!! don't forget to initial the chart panel
            this.chartPanel1.Init();
            // enable the panel
            this.chartPanel1.Enable();
        }

        private void cmbDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // disable the combo box of charts
            this.cmbCharts.Enabled = false;
            // clean up the items of combox charts
            this.cmbCharts.Items.Clear();
            this.chartPanel1.Disable();

            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbDataSets.Text) || cmbDataSets.Text == ComboBoxItem.EmptyItem.Text)
                return; // nothiing to do

            string dataSetFile = ((dynamic)cmbDataSets.SelectedItem).Value;
            this._currentFile = new AeronetFile(dataSetFile);

            foreach (var chart in this._currentFile.DataConfigs)
            {
                // ChartName|ChartDescription
                string[] pair = chart.Split(new char[] { '|' }, StringSplitOptions.None);
                // 1: Description
                // 0: ChartName
                this.cmbCharts.Items.Add(new { Text = pair[1], Value = pair[0] });
            }

            // insert empty item
            this.cmbCharts.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());
            // enable the combox
            this.cmbCharts.Enabled = true;
            // initial to select the empty item
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
            // disable actions
            this.cmbDataSets.Enabled = false;
            this.cmbCharts.Enabled = false;
            this.cmbCharts.Items.Clear();
            this.chartPanel1.Disable();

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
                this.cmbDataSets.Items.Add(new { Text = fileName, Value = dataSet });
            }
            // insert the empty item
            this.cmbDataSets.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());

            // enable the combo box of dataset when completely loads items
            this.cmbDataSets.Enabled = true;
            // targets to the empty item
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