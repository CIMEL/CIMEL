using CIMEL.Core;
using CIMEL.RSA;
using Microsoft.Win32;
using Peach.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CIMEL.Chart
{
    public class Register
    {
        private RSAKeys _RSAKeys;
        protected const string REG_ACTIVE = "ACTIVE";
        protected const string REG_KEYS = "KEYS";
        protected const string REG = @"SOFTWARE\AYFY";
        private Logger _logger;

        private static Register _singleton = new Register();

        public static Register Singleton { get { return _singleton; } }

        protected Register()
        {
            this._logger = Logger.CreateNew("register");

            this._RSAKeys = new RSAKeys();
            using (MemoryStream ms = new MemoryStream(Properties.Resources.rsa))
            using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
            {
                this._RSAKeys.PrivateKey = sr.ReadToEnd();
            }
            using (MemoryStream ms = new MemoryStream(Properties.Resources.rsa_pub))
            using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
            {
                this._RSAKeys.PublicKey = sr.ReadToEnd();
            }
        }

        private LicenseInfo GetActivedLicense()
        {
            LicenseInfo actived = null;

            RegistryKey reg = Registry.LocalMachine.OpenSubKey(REG);
            if (reg != null)
            {
                string encrypted = Convert.ToString(reg.GetValue(REG_ACTIVE));
                string licenseKey = Encryptor.Singleton.DecryptText(encrypted, this._RSAKeys.PrivateKey);
                actived = new LicenseInfo(licenseKey);
            }
            else
                actived = new LicenseInfo(false, "未注册");

            return actived;
        }

        public LicenseInfo DoRegister(string encryptedLicense)
        {
            LicenseInfo actived;
            try
            {
                string plainLicense = Encryptor.Singleton.DecryptText(encryptedLicense, this._RSAKeys.PrivateKey);
                DateTime registered = DateTime.Now;
                actived = new LicenseInfo(plainLicense, registered);
            }
            catch {
                return new LicenseInfo(false, "注册码无效");
            }

            if (!actived.IsValid) return actived;
            List<string> lstKeys;
            try
            {
                lstKeys = this.GetKeys();
                if (lstKeys.Contains(actived.Id))
                    return new LicenseInfo(false, "已注册，不能重复注册。");

            }
            catch (Exception ex)
            {
                LogError(ex);
                return new LicenseInfo(false, "分析注册码失败");
            }

            try
            {
                // update active license
                this.SaveLicense(actived);

                //update keys
                this.SaveKeys(lstKeys, actived.Id);
            }
            catch(Exception ex)
            {
                LogError(ex);
                return new LicenseInfo(false, "注册失败");
            }

            return actived;

        }

        private List<string> GetKeys()
        {
            List<string> keys = null;

            RegistryKey reg = Registry.LocalMachine.OpenSubKey(REG);
            if (reg == null)
            {
                keys = new List<string>();
                return keys;
            }
            string encryptedKeys = Convert.ToString(reg.GetValue(REG_KEYS));
            if (string.IsNullOrEmpty(encryptedKeys))
            {
                keys = new List<string>();
                return keys;
            }
            // don't need to decrypt the list of Id as they are stored in plain
            // string strKeys = Encryptor.Singleton.DecryptText(encryptedKeys, this._RSAKeys.PrivateKey);
            string[] arryKeys = encryptedKeys.Split(new char[] { ',' });
            keys = new List<string>(arryKeys);

            return keys;
        }

        private void SaveLicense(LicenseInfo licenseInfo)
        {
            RegistryKey reg = Registry.LocalMachine.CreateSubKey(REG);
            string plainLiceseInfo = licenseInfo.ToString();
            string encryptedLicenseInfo = Encryptor.Singleton.EncryptText(plainLiceseInfo, this._RSAKeys.PublicKey);
            reg.SetValue(REG_ACTIVE, encryptedLicenseInfo, RegistryValueKind.String);
            reg.Close();
        }

        private void SaveKeys(List<string> lstKeys,string newKey)
        {
            // remember the latest 10 keys, the new key will append the list of keys and the oldest key will be pop up out
            int maxKeys = 10;
            if (lstKeys.Count >= maxKeys)
                lstKeys.RemoveAt(0);
            lstKeys.Add(newKey);
            string keys = string.Join(",", lstKeys);
            // don't encrypted the keys any more as the maximum data length limitation
            // string encryptedKeys= Encryptor.Singleton.EncryptText(keys, this._RSAKeys.PublicKey);
            RegistryKey reg = Registry.LocalMachine.CreateSubKey(REG);
            reg.SetValue(REG_KEYS, keys, RegistryValueKind.String);
            reg.Close();
        }

        public LicenseInfo CheckLicense(LicenseInfo licenseKey=null)
        {
            if (licenseKey == null)
                licenseKey = this.GetActivedLicense();

            if (!licenseKey.IsValid) return licenseKey;

            DateTime dtRegistered = licenseKey.RegisteredDate;
            DateTime current = DateTime.Now;
            if (current < dtRegistered) return new LicenseInfo(false, "无效注册信息");
            // int expires = licenseKey.Expires;
            // int used= Math.Abs((current.Month - dtRegistered.Month) + 12 * (current.Year - dtRegistered.Year));
            // if (used >= expires)
            if (current >= licenseKey.ExpiredDate)
                licenseKey.SetStatus(false, "已过期");

            return licenseKey;
        }

        public string GetExpiresLabel(int expires)
        {
            switch (expires)
            {
                case 1: return "一个月";
                case 3: return "三个月";
                case 6: return "半年";
                case 12: return "一年";
                default:
                    return string.Format("{0}个月", expires);
            }
        }

        public MethodResult IsAlive(bool autoExit = false, IWin32Window owner = null)
        {
            MethodResult isActived = new MethodResult();

            LicenseInfo actived = this.CheckLicense();
            try
            {
                if (!actived.IsValid)
                {
                    isActived = new MethodResult(false, actived.Message);
                    LogError(string.Format("Is license active? {0}", isActived));
                }
            }
            catch(Exception ex)
            {
                LogError(string.Format("Is license active? {0}", ex.Message));
                isActived = new MethodResult(false, string.Empty);
            }

#if DEBUG
            if (isActived)
                LogInfo(string.Format("Is license active? {0}", isActived));
#endif

            if (!isActived && autoExit)
            {
                string message = string.IsNullOrEmpty(isActived.Message)
                    ? @"运行禁止!"
                    : string.Format("运行禁止: {0}", isActived.Message);
                if (owner != null)
                    MessageBox.Show(owner, message, @"注册信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(message, @"注册信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (System.Windows.Forms.Application.MessageLoop)
                {
                    // Use this since we are a WinForms app
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    // Use this since we are a console app
                    System.Environment.Exit(1);
                }
            }

            return isActived;
        }

        private void LogDebug(string message)
        {
            this._logger.Debug(message);
        }

        private void LogError(string message)
        {
            this._logger.Error(message);
            // todo: show dog's session info
        }

        private void LogError(Exception ex)
        {
            this._logger.Error(ex);
            // todo: show dog's session info
        }

        private void LogInfo(string message)
        {
            this._logger.Info(message);
        }
    }

    public class LicenseInfo
    {
        public bool IsValid { get; private set; }
        /// <summary>
        /// the id of the license
        /// </summary>
        public string Id { get; private set; }
        /// <summary>
        /// The registred date
        /// </summary>
        public DateTime RegisteredDate { get; private set; }
        /// <summary>
        /// The expired date
        /// </summary>
        public DateTime ExpiredDate { get; private set; }
        /// <summary>
        /// The user name or email
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// The expired period
        /// </summary>
        public int Expires { get; private set; }
        /// <summary>
        /// The error message, if IsValid = true then empty
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The max number of the region entries being allowed to add
        /// </summary>
        public int MaxRegions { get; private set; }

        public LicenseInfo(string plainLicenseInfo)
        {
            if (string.IsNullOrEmpty(plainLicenseInfo))
            {
                this.IsValid = false;
                this.Message = "空注册信息";
                return;
            }

            string[] paras = plainLicenseInfo.Split(new char[] { '|' });
            if (paras.Length < 6)
            {
                this.IsValid = false;
                this.Message = "注册信息无效";
                return;
            }

            this.Id = paras[0];
            this.Key = paras[1];

            int intExpires;
            if (!int.TryParse(paras[2], out intExpires)) intExpires = -1;
            this.Expires = intExpires;

            DateTime dtRegisteredDate;
            if (!DateTime.TryParse(paras[3], out dtRegisteredDate)) dtRegisteredDate = DateTime.MinValue;
            this.RegisteredDate = dtRegisteredDate;

            int intRegions;
            if (!int.TryParse(paras[4], out intRegions)) intRegions = -1;
            this.MaxRegions = intRegions;

            DateTime dtExpiredDate;
            if (!DateTime.TryParse(paras[5], out dtExpiredDate)) dtExpiredDate = DateTime.MinValue;
            this.ExpiredDate = dtExpiredDate;

            Guid guid;
            if(string.IsNullOrEmpty(this.Id)||!Guid.TryParse(this.Id,out guid))
            {
                this.IsValid = false;
                this.Message = "注册信息无效";
                return;
            }
            if (string.IsNullOrEmpty(this.Key))
            {
                this.IsValid = false;
                this.Message = "注册信息无效：无用户名或邮箱";
                return;
            }

            if (this.Expires<0)
            {
                this.IsValid = false;
                this.Message = "注册信息无效：有效期无效";
                return;
            }

            if (this.RegisteredDate == DateTime.MinValue)
            {
                this.IsValid = false;
                this.Message = "注册信息无效：注册日期无效";
                return;
            }

            if (this.MaxRegions < 0)
            {
                this.IsValid = false;
                this.Message = "注册信息无效：最大站点数无效";
                return;
            }

            if (this.ExpiredDate == DateTime.MinValue)
            {
                this.IsValid = false;
                this.Message = "注册信息无效：过期日期无效";
                return;
            }

            this.IsValid = true;
            this.Message = string.Empty;
        }

        public LicenseInfo(string plainLicense,DateTime registered)
        {
            if (string.IsNullOrEmpty(plainLicense))
            {
                this.IsValid = false;
                this.Message = "空注册信息";
                return;
            }

            string[] arryLicenseInfo = plainLicense.Split(new char[] { '|' });
            if (arryLicenseInfo.Length < 4)
            {
                this.IsValid = false;
                this.Message = "注册码无效";
                return;
            }

            this.Id = arryLicenseInfo[0];
            this.Key = arryLicenseInfo[1];
            int expires;
            if (!int.TryParse(arryLicenseInfo[2], out expires))
                expires = -1;
            this.Expires = expires;
            this.RegisteredDate = registered;
            int intRegions;
            if (!int.TryParse(arryLicenseInfo[3], out intRegions))
                expires = -1;
            this.MaxRegions = intRegions;

            Guid guid;
            if (string.IsNullOrEmpty(this.Id) || !Guid.TryParse(this.Id, out guid))
            {
                this.IsValid = false;
                this.Message = "注册码无效";
                return;
            }
            if (string.IsNullOrEmpty(this.Key))
            {
                this.IsValid = false;
                this.Message = "注册码无效";
                return;
            }

            if (this.Expires < 0)
            {
                this.IsValid = false;
                this.Message = "有效期无效";
                return;
            }

            if (this.RegisteredDate == DateTime.MinValue)
            {
                this.IsValid = false;
                this.Message = "注册日期无效";
                return;
            }

            if (this.MaxRegions < 0)
            {
                this.IsValid = false;
                this.Message = "最大站点数无效";
                return;
            }

            this.ExpiredDate = this.RegisteredDate.AddMonths(this.Expires);

            this.IsValid = true;
            this.Message = string.Empty;
        }

        public LicenseInfo(bool isValid, string message)
        {
            this.IsValid = IsValid;
            this.Message = message;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3:yyyy-MM-dd HH:mm:ss.fff}|{4}|{5:yyyy-MM-dd HH:mm:ss.fff}", Id, Key, Expires, RegisteredDate,MaxRegions,ExpiredDate);
        }

        public void SetStatus(bool isValid, string message)
        {
            this.IsValid = isValid;
            this.Message = message;
        }
    }
}
