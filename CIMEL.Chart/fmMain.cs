using CIMEL.Chart.CIMELData;
using CIMEL.Chart.Options;
using CIMEL.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CIMEL.Dog;

namespace CIMEL.Chart
{
    public partial class fmMain : Form
    {
        private CIMELFile _currentFile;

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
            this.cmbRegions.SelectedIndexChanged += cmbRegions_SelectedIndexChanged;
            this.cmbModes.SelectedIndexChanged += CmbModes_SelectedIndexChanged;
        }

        private void fmMain_Load(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!CIMELDog.Default.IsAlive(true)) return;

            // check if register the license
            if (!Register.Singleton.IsAlive())
            {
                using (fmRegister fmRegister = new fmRegister())
                {
                    fmRegister.StartPosition = FormStartPosition.CenterParent;
                    if (DialogResult.Abort == fmRegister.ShowDialog(this))
                    {
                        Application.Exit();
                    }
                }
            }

            // check if the options has been configurated
            if (!ConfigOptions.Singleton.IsInitialized)
            {
                string validationMsg = ConfigOptions.Singleton.ValidateDirs();
                if (!string.IsNullOrEmpty(validationMsg))
                {
                    this.ShowAlert(validationMsg, fmOptions.DLG_TITLE_ERROR);
                    using (fmOptions fmOptions = new fmOptions())
                    {
                        fmOptions.AllowForceExit = true;
                        fmOptions.StartPosition = FormStartPosition.CenterParent;
                        if (DialogResult.Abort == fmOptions.ShowDialog(this))
                        {
                            Application.Exit();
                        }
                    }
                }
            }

            RefreshRegions();

        }

        /// <summary>
        /// Reset the controls to initial state including 
        /// - the combo boxe of Chart Sets
        /// - the combo box CIMEL Data 
        /// - the chart panel 
        /// </summary>
        private void Reset()
        {
            // disable actions
            this.EnableDataSets(false);
            this.cmbDataSets.Text = ComboBoxItem.EmptyItem.Text;
            this.EnableCharts(false);
            this.cmbCharts.Items.Clear();
            this.cmbCharts.Text = ComboBoxItem.EmptyItem.Text;
            this.chartPanel1.Disable();
        }

        #region Menu
        private void cimelDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            using (fmCIMELData fmCIMELData = new fmCIMELData())
            {
                fmCIMELData.StartPosition = FormStartPosition.CenterParent;
                fmCIMELData.ShowDialog(this);
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            using (fmOptions fmOptions = new fmOptions())
            {
                fmOptions.StartPosition = FormStartPosition.CenterParent;
                fmOptions.ShowDialog(this);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (fmAboutBox fmAboutBox = new fmAboutBox())
            {
                fmAboutBox.StartPosition = FormStartPosition.CenterParent;
                fmAboutBox.ShowDialog(this);
            }
        }

        private void regionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            using (fmRegions fmRegions = new fmRegions())
            {
                fmRegions.StartPosition = FormStartPosition.CenterParent;
                fmRegions.ShowDialog(this);
            }
        }
        #endregion

        #region Regions

        private void cmbRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            this.Reset();
            if (cmbRegions.SelectedText != ComboBoxItem.EmptyItem.Text)
            {
                // the CIMEL data sets
                this.Scan();
            }
        }

        private void btnNextRegion_Click(object sender, EventArgs e)
        {
            int index = this.cmbRegions.SelectedIndex;
            if (index == this.cmbRegions.Items.Count - 1)
            {
                this.cmbRegions.SelectedIndex = this.cmbRegions.Items.Count > 1 ? 1 : 0;
            }
            else
            {
                this.cmbRegions.SelectedIndex++;
            }
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshRegions();
        }

        private void RefreshRegions()
        {
            this.cmbRegions.Items.Clear();
            this.cmbRegions.Refresh();
            this.cmbRegions.DisplayMember = ComboBoxItem.DisplayName;
            this.cmbRegions.ValueMember = ComboBoxItem.ValueName;
            this.cmbRegions.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());
            try
            {
                var regions = RegionStore.Singleton.GetRegions();
                foreach (var region in regions)
                {
                    this.cmbRegions.Items.Add(
                        new
                        {
                            Text = string.Format("{0} ({1} , {2})", region.Name, region.Lat, region.Lon),
                            Value = region.Name
                        });
                }
            }
            catch (Exception ex)
            {
                this.ShowAlert(ex.Message, fmRegions.DLG_TITLE_ERROR);
            }
            this.cmbRegions.SelectedIndex = 0;
            this.cmbRegions.Refresh();
        }

        #endregion

        #region DataSets

        private void btnScan_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            this.Reset();
            this.Scan();
        }

        private void cmbDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            // disable the combo box of charts
            this.EnableCharts(false);
            // clean up the items of combox charts
            this.cmbCharts.Items.Clear();
            this.chartPanel1.Disable();

            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbDataSets.Text) || cmbDataSets.Text == ComboBoxItem.EmptyItem.Text)
                return; // nothiing to do

            string dataSetFile = ((dynamic)cmbDataSets.SelectedItem).Value;
            this._currentFile = new CIMELFile(dataSetFile);

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
            this.EnableCharts(true);
            // initial to select the empty item
            this.cmbCharts.SelectedIndex = 0;
        }

        private void btnNextDataSet_Click(object sender, EventArgs e)
        {
            int index = this.cmbDataSets.SelectedIndex;
            if (index == this.cmbDataSets.Items.Count - 1)
            {
                this.cmbDataSets.SelectedIndex = this.cmbDataSets.Items.Count > 1 ? 1 : 0;
            }
            else
            {
                this.cmbDataSets.SelectedIndex++;
            }
        }

        /// <summary>
        /// Scans the processed CIMEL data sets within output folder
        /// </summary>
        private void Scan()
        {
            // region
            if (this.cmbRegions.Text == ComboBoxItem.EmptyItem.Text)
            {
                return;
            }
            string region = ((dynamic)this.cmbRegions.SelectedItem).Value;

            // clean up the combox of data sets
            this.cmbDataSets.Items.Clear();
            // insert the empty item
            this.cmbDataSets.Items.Insert(0, ComboBoxItem.EmptyItem.ToItem());

            string outputFolder = Path.Combine(ConfigOptions.Singleton.CHARTSET_Dir, region);
            if (!Directory.Exists(outputFolder))
            {
                this.ShowInfo(@"没有找到已生成的图像数据集，请先进入[工具]->[数据处理...]处理CIMEL光度计数据",
                    CIMELConst.GLOBAL_DLG_TITLE);

                return;
            }
            string[] dataSets = Directory.GetFiles(outputFolder, "*.cimel", SearchOption.TopDirectoryOnly);
            if (dataSets.Length == 0)
            {
                this.ShowInfo(@"没有找到已生成的图像数据集，请先进入[工具]->[数据处理...]处理CIMEL光度计数据",
                    CIMELConst.GLOBAL_DLG_TITLE);
                return;
            }
            foreach (string dataSet in dataSets)
            {
                // without the extension
                string fileName = Path.GetFileNameWithoutExtension(dataSet);
                this.cmbDataSets.Items.Add(new { Text = fileName, Value = dataSet });
            }

            // enable the combo box of dataset when completely loads items
            this.EnableDataSets(true);
            // targets to the empty item
            this.cmbDataSets.SelectedIndex = 0;
        }

        private void EnableDataSets(bool enable)
        {
            this.cmbDataSets.Enabled = enable;
            this.btnNextChartSet.Enabled = enable;
            this.btnScan.Enabled = enable;
        }

        #endregion

        #region Charts

        private void cmbCharts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            // disable the chart panel
            this.chartPanel1.Disable();

            // check if the selected data set is empty
            if (string.IsNullOrEmpty(cmbCharts.Text) || cmbCharts.Text == ComboBoxItem.EmptyItem.Text)
                return; // nothiing to do

            string strChartNames = ((dynamic)cmbCharts.SelectedItem).Value;
            string[] arrChartNames = strChartNames.Split('@');


            if (this.chartPanel1.DataConfigFiles.Count > 0)
                this.chartPanel1.DataConfigFiles.Clear();
            this.chartPanel1.DataConfigFiles.AddRange(arrChartNames
                .Select(
                    cn =>
                    {
                        string strCIMELFile = ((dynamic)cmbDataSets.SelectedItem).Value;
                        string strRoot = Path.GetDirectoryName(strCIMELFile);
                        return Path.Combine(strRoot, this._currentFile.Path, string.Format("{0}.{1}", cn, "dataconfig"));
                    })
                .ToList());

            FigureType figureType = (FigureType)cmbModes.SelectedIndex;

            // !!! don't forget to initial the chart panel
            this.chartPanel1.Init(figureType);
            // enable the panel
            this.chartPanel1.Enable();
        }

        private void CmbModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            if (!cmbModes.Enabled) return;
            // disable the chart panel
            this.chartPanel1.Disable();

            FigureType figureType = (FigureType)cmbModes.SelectedIndex;

            // !!! don't forget to initial the chart panel
            this.chartPanel1.Init(figureType);
            // enable the panel
            this.chartPanel1.Enable();
        }

        private void EnableCharts(bool enable)
        {
            this.cmbCharts.Enabled = enable;
            this.btnNextChart.Enabled = enable;
            this.cmbModes.Enabled = enable;
            if (!enable)
                this.cmbModes.SelectedIndex = 0;
        }

        private void btnNextChart_Click(object sender, EventArgs e)
        {
            int index = this.cmbCharts.SelectedIndex;
            if (index == this.cmbCharts.Items.Count - 1)
            {
                this.cmbCharts.SelectedIndex = this.cmbCharts.Items.Count > 1 ? 1 : 0;
            }
            else
            {
                this.cmbCharts.SelectedIndex++;
            }
        }
        #endregion

        private void registerStripMenuItem_Click(object sender, EventArgs e)
        {
            using (fmRegister fmRegister = new fmRegister())
            {
                fmRegister.StartPosition = FormStartPosition.CenterParent;
                fmRegister.ShowDialog(this);
            }
        }
    }
}