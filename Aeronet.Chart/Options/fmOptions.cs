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
            this.Init();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder valErrors=new StringBuilder();
            foreach (var folderDesc in ConfigOptions.Singleton.GetFolders())
            {
                string dir = folderDesc.Path;
                if (Utility.IsEmpty(dir))
                    valErrors.AppendLine(string.Format(@"抱歉, 请设置[{0}]", folderDesc.Name));
                if (!Utility.IsExist(dir))
                    valErrors.AppendLine(string.Format(@"抱歉, 目录不存在, 请重新设置[{0}]", folderDesc.Name));
            }
            if (valErrors.Length > 0)
            {
                MessageBox.Show(valErrors.ToString(), DLG_TITLE_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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