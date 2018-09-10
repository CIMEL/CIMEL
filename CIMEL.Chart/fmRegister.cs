using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CIMEL.Chart
{
    public partial class fmRegister : Form
    {
        public fmRegister()
        {
            InitializeComponent();
        }

        private void fmRegister_Load(object sender, EventArgs e)
        {
            LicenseInfo actived = Register.Singleton.CheckLicense();

            if (string.IsNullOrEmpty(actived.Key)) this.txtKey.Text = "未注册用户";
            else
                this.txtKey.Text = actived.Key;

            if (actived.IsValid)
            {
                this.txtExpires.Text = Register.Singleton.GetExpiresLabel(actived.Expires);
                this.txtExpiredDate.Text = actived.ExpiredDate.ToString("yyyy年MM月dd日");
                this.txtMaxRegions.Text = actived.MaxRegions.ToString();
            }
            else
            {
                this.txtExpires.Text = actived.Message;
                this.txtExpiredDate.Text = "未知";
                this.txtMaxRegions.Text = "未知";
            }

            txtLicense.Text = string.Empty;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fmRegister_FormClosing(object sender, FormClosingEventArgs e)
        {
            LicenseInfo actived = Register.Singleton.CheckLicense();
            if (!actived.IsValid)
            {
                if (DialogResult.No
                    == MessageBox.Show(this, "使用前请输入注册码，是否要注册？\r\n[是]注册\r\n[否]关闭程序", "注册", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    this.DialogResult = DialogResult.Abort;
                    return;
                }
                else
                    e.Cancel = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(txtLicense.Text))
                {
                    if (DialogResult.No
                        == MessageBox.Show(this, "所填注册码还未注册，是否要继续注册？\r\n[是]继续注册\r\n[否]取消", "注册", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        this.DialogResult = DialogResult.Cancel;
                        return;
                    }
                    else
                        e.Cancel = true;
                }

                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string encryptedLicese = txtLicense.Text;
            if (string.IsNullOrEmpty(encryptedLicese))
            {
                MessageBox.Show(this, "请输入注册码", "注册", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LicenseInfo info = Register.Singleton.DoRegister(encryptedLicese);
            if (info.IsValid)
            {
                txtLicense.Text = string.Empty;
                txtKey.Text = info.Key;
                txtExpires.Text = Register.Singleton.GetExpiresLabel(info.Expires);
                txtExpiredDate.Text = info.ExpiredDate.ToString("yyyy年MM月dd日");
                txtMaxRegions.Text = info.MaxRegions.ToString();
                MessageBox.Show(this, string.Format("注册成功！有效期{0}", txtExpires.Text), "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, info.Message, "关于", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                dialog.CheckFileExists = true;
                if (DialogResult.OK == dialog.ShowDialog(this))
                {
                    try
                    {
                        using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                        {
                            string encryptedLicense = sr.ReadToEnd();
                            txtLicense.Text = encryptedLicense;
                        }
                        MessageBox.Show(this, "读取完毕", "注册", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch
                    {
                        txtLicense.Text = string.Empty;
                        MessageBox.Show(this, "无法读取注册码", "注册", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
