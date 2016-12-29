using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aeronet.Chart.Properties;
using Aeronet.Dog;

namespace Aeronet.Chart.Options
{
    public partial class fmOptions : Form
    {
        public bool AllowForceExit { get; set; }

        public fmOptions()
        {
            InitializeComponent();
            this.Load += fmOptions_Load;
            this.Closing += fmOptions_Closing;
            this.AllowForceExit = false;
        }

        private void fmOptions_Closing(object sender, CancelEventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            // prevent it from closing if the options are not initialized
            if (!ConfigOptions.Singleton.IsInitialized)
            {
                if (this.AllowForceExit)
                {
                    if (DialogResult.No ==
                        MessageBox.Show(this, "抱歉，参数配置没有完成程序无法启动，确定要退出吗?\r\n[Yes]关闭程序\r\n[No]继续设置参数",
                            Settings.Default.FM_OPTION_CONFIG_TEXT, MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question))
                        e.Cancel = true;
                    else
                    {
                        this.DialogResult=DialogResult.Abort;
                        e.Cancel = false;
                    }
                }
                else
                {
                    MessageBox.Show(this, @"开始操作前请先完成参数配置",
                        DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void fmOptions_Load(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            this.Init();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            string validationMsg = ConfigOptions.Singleton.ValidateDirs();
            if (!string.IsNullOrEmpty(validationMsg))
            {
                MessageBox.Show(validationMsg, DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // this.propertyGrid1.PropertySort=PropertySort.CategorizedAlphabetical;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            this.DialogResult = DialogResult.Cancel;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            ConfigOptions.Singleton.Refresh();
            this.Init();
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            if (e.ChangedItem.GridItemType != GridItemType.Property) return;

            if (e.ChangedItem.PropertyDescriptor.Name == "METADATA_Dir")
            {
                string root = Convert.ToString(e.ChangedItem.Value);
                if (string.IsNullOrEmpty(root)) return;

                ConfigOptions.Singleton.CHARTSET_Dir = Path.Combine(root, "chartset");
                ConfigOptions.Singleton.INS_PARA_Dir = Path.Combine(root, "ins_para");
                ConfigOptions.Singleton.MODIS_BRDF_Dir = Path.Combine(root, "modis_brdf");
                ConfigOptions.Singleton.OUTPUT_Dir = Path.Combine(root, "output");
                this.propertyGrid1.Refresh();
                string data=Path.Combine(root, "data");
                string question = string.Format("自动设置[{0}]吗？\n\r\n\r设置为: {1}", ConfigOptions.DATA_NAME, data);
                if (MessageBox.Show(this, question, DLG_TITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    ConfigOptions.Singleton.DATA_Dir = data;
                    this.propertyGrid1.Refresh();
                }
            }
        }
    }
}