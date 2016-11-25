using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aeronet.Chart.Options
{
    public partial class fmOptions : Form
    {
        public fmOptions()
        {
            InitializeComponent();
            this.Load += fmOptions_Load;
            this.Closing += fmOptions_Closing;
        }

        private void fmOptions_Closing(object sender, CancelEventArgs e)
        {
            // prevent it from closing if the options are not initialized
            if (!ConfigOptions.Singleton.IsInitialized)
            {
                MessageBox.Show(this, "Must configurate the required options before anything stuff please!",
                    "Warning of Option configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void fmOptions_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.DATA_Dir))
            {
                MessageBox.Show(@"The Data path cannot be empty please!", @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.MODIS_BRDF_Dir))
            {
                MessageBox.Show(@"the Modis_BRDF path cannot be empty please!", @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.INS_PARA_Dir))
            {
                MessageBox.Show(@"The Ins_Para path cannot be empty please!", @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.METADATA_Dir))
            {
                MessageBox.Show(@"The Metadata path cannot be empty please!", @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.OUTPUT_Dir))
            {
                MessageBox.Show(@"The Output path cannot be empty please!", @"Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConfigOptions.Singleton.Save();
            MessageBox.Show(@"Save Successfully!");
            this.DialogResult = DialogResult.OK;
        }

        private void Init()
        {
            // loads the configOption instance to the property grid
            this.propertyGrid1.SelectedObject = ConfigOptions.Singleton;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}