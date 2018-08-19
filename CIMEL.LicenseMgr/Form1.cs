using CIMEL.RSA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CIMEL.LicenseMgr
{
    public partial class Form1 : Form
    {
        private string _publicKey;
        public Form1()
        {
            InitializeComponent();
            using (MemoryStream ms = new MemoryStream(Properties.Resources.rsa))
            using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
            {
                this._publicKey = sr.ReadToEnd();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtKey.Text = "default@foo.com";
            cmbExpires.SelectedIndex = 3;
            txtLicense.Text = string.Empty;
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKey.Text))
            {
                MessageBox.Show(this, "用户名/邮箱不能为空。", "注册码生成器", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string strExpires = Convert.ToString(cmbExpires.Items[cmbExpires.SelectedIndex]);
            if (string.IsNullOrEmpty(strExpires))
            {
                MessageBox.Show(this, "请选择有效期", "注册码生成器", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string key = txtKey.Text;
            if (key.Contains("|"))
            {
                MessageBox.Show(this, "用户名/邮箱存在无效字符：“|”", "注册码生成器", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int expires = this.GetExpires(strExpires);
            string plainKey = string.Format("{0}|{1}", key, expires);
            string encrypted = Encryptor.Singleton.EncryptText(plainKey, this._publicKey);
            txtLicense.Text = encrypted;
            MessageBox.Show(this, string.Format("生成注册码！有效期为{0}", strExpires), "注册码生成器", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int GetExpires(string strExpires)
        {
            switch (strExpires)
            {
                case "一个月":
                    return 1;
                case "三个月":
                    return 3;
                case "半年":
                    return 6;
                case "一年":
                    return 12;
                // for trailer
                default:
                    return 1;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnGen_Click(this.btnGen, new EventArgs());
        }

        private void txtKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.btnGen_Click(this.btnGen, new EventArgs());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog dialog=new SaveFileDialog())
            {
                dialog.FileName = string.Format("{0}.txt", txtKey.Text);
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string file = dialog.FileName;
                    File.WriteAllText(file, txtLicense.Text, Encoding.UTF8);
                    MessageBox.Show(this, string.Format("注册码已保存至{0}", file), "注册码生成器", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Process.Start(file);
                }
            }
        }
    }
}
