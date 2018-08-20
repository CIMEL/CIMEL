using CIMEL.RSA;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    public class Register
    {
        private RSAKeys _RSAKeys;
        protected const string REG_ACTIVE = "ACTIVE";
        protected const string REG_KEYS = "KEYS";
        protected const string REG = @"SOFTWARE/AYFY";

        private static Register _singleton = new Register();

        public static Register Singleton { get { return _singleton; } }

        protected Register()
        {
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

        public LicenseInfo GetActivedLicense()
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

            List<string> lstKeys = this.GetKeys();
            if (lstKeys.Contains(actived.Id))
                return new LicenseInfo(false, "已注册，不能重复注册。");

            RegistryKey reg = Registry.LocalMachine.CreateSubKey(REG);
            // update active license
            string plainLiceseInfo = actived.ToString();
            string encryptedLicenseInfo = Encryptor.Singleton.EncryptText(plainLiceseInfo, this._RSAKeys.PublicKey);
            reg.SetValue(REG_ACTIVE, encryptedLicenseInfo, RegistryValueKind.String);

            //update keys
            lstKeys.Add(actived.Id);
            string keys = string.Join(",", lstKeys);
            string encryptedKeys= Encryptor.Singleton.EncryptText(keys, this._RSAKeys.PublicKey);
            reg.SetValue(REG_KEYS, encryptedKeys, RegistryValueKind.String);
            reg.Close();

            return actived;

        }

        public List<string> GetKeys()
        {
            List<string> keys = null;

            RegistryKey reg = Registry.LocalMachine.OpenSubKey(REG);
            string encryptedKeys = Convert.ToString(reg.GetValue(REG_KEYS));
            if (string.IsNullOrEmpty(encryptedKeys))
            {
                keys = new List<string>();
                return keys;
            }

            string strKeys = Encryptor.Singleton.DecryptText(encryptedKeys, this._RSAKeys.PrivateKey);
            string[] arryKeys = strKeys.Split(new char[] { ',' });
            keys = new List<string>(arryKeys);

            return keys;
        }

        public LicenseInfo CheckLicense(LicenseInfo licenseKey=null)
        {
            if (licenseKey == null)
                licenseKey = this.GetActivedLicense();

            if (!licenseKey.IsValid) return licenseKey;

            DateTime dtRegistered = licenseKey.RegisteredDate;
            DateTime current = DateTime.Now;
            if (current < dtRegistered) return new LicenseInfo(false, "无效注册信息");
            int expires = licenseKey.Expires;
            int used= Math.Abs((current.Month - dtRegistered.Month) + 12 * (current.Year - dtRegistered.Year));
            if (used > expires)
                licenseKey.SetStatus(false, "已过期");

            return licenseKey;
        }

    }

    public class LicenseInfo
    {
        public bool IsValid { get; private set; }
        public string Id { get; private set; }
        public DateTime RegisteredDate { get; private set; }
        public string Key { get; private set; }
        public int Expires { get; private set; }

        public string Message { get; private set; }

        public LicenseInfo(string plainLicenseInfo)
        {
            if (string.IsNullOrEmpty(plainLicenseInfo))
            {
                this.IsValid = false;
                this.Message = "空注册信息";
                return;
            }

            string[] paras = plainLicenseInfo.Split(new char[] { '|' });
            if (paras.Length < 4)
            {
                this.IsValid = false;
                this.Message = "注册信息无效";
                return;
            }

            this.Id = paras[0];
            this.Key = paras[1];
            int intExpires;
            if (!int.TryParse(paras[2], out intExpires)) intExpires = -1;
            this.Expires = -1;
            DateTime dtRegisteredDate;
            if (!DateTime.TryParse(paras[3], out dtRegisteredDate)) dtRegisteredDate = DateTime.MinValue;
            this.RegisteredDate = dtRegisteredDate;

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
                this.Message = "注册信息无效";
                return;
            }

            if (this.Expires<0)
            {
                this.IsValid = false;
                this.Message = "注册信息无效";
                return;
            }

            if (this.RegisteredDate == DateTime.MinValue)
            {
                this.IsValid = false;
                this.Message = "注册信息无效";
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
            if (arryLicenseInfo.Length < 3)
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
            return string.Format("{0}|{1}|{2}|{3:yyyy-MM-dd HH:mm:ss.fff}", Id, Key, Expires, RegisteredDate);
        }

        public void SetStatus(bool isValid, string message)
        {
            this.IsValid = IsValid;
            this.Message = message;
        }
    }
}
