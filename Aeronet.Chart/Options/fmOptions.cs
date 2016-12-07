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
                MessageBox.Show(this, @"开始数据处理与绘制图像前请先完成参数配置",
                    DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void fmOptions_Load(object sender, EventArgs e)
        {
            this.Init();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string valFormat = @"抱歉, 请设置[{0}]";
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.DATA_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.DATA_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.MODIS_BRDF_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.MODIS_BRDF_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.INS_PARA_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.INS_PARA_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.METADATA_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.METADATA_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.OUTPUT_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.OUTPUT_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(ConfigOptions.Singleton.CHARTSET_Dir))
            {
                MessageBox.Show(string.Format(valFormat, ConfigOptions.CHARTSET_NAME), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConfigOptions.Singleton.Save();
            MessageBox.Show(@"保存成功!",DLG_TITLE);
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