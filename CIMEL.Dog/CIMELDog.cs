using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using CIMEL.Core;
using Peach.Log;
using SuperDog;

namespace CIMEL.Dog
{
    public class CIMELDog
    {
        private static CIMELDog _default=new CIMELDog();

        private Dictionary<int,string> _dogMsgs=new Dictionary<int, string>(); 

        // private SuperDog.Dog _dog;

        private Logger _logger;

        protected CIMELDog()
        {
            // next could be considered ugly.
            // build up a string collection holding
            // the status codes in a human readable manner.
            _dogMsgs.Add(0, "请求成功");//0|"Request successfully completed."
            _dogMsgs.Add(1, "请求超出数据文件的范围");//1|"Request exceeds data file range."
            _dogMsgs.Add(3, "系统内存不足");//3|"System is out of memory."
            _dogMsgs.Add(4, "打开的登录会话数目过多");//0|"Too many open login sessions."
            _dogMsgs.Add(5, "禁止访问");//5|"Access denied."
            _dogMsgs.Add(7, "未找到超级狗");//7|"Required SuperDog not found."
            _dogMsgs.Add(8, "加密/解密的数据长度太短");//8|"Encryption/decryption data length is too short."
            _dogMsgs.Add(9, "无效的文件句柄");//9|"Invalid input handle."
            _dogMsgs.Add(10, "无法识别的文件");//10|"Specified File ID not recognized by API."
            _dogMsgs.Add(15, "无效的XML格式");//15|"Invalid XML format."
            _dogMsgs.Add(16, "不支持所请求的功能");
            _dogMsgs.Add(17, "不正确的升级文件");
            _dogMsgs.Add(18, "未找到指定的超级狗");//18|"SuperDog to be updated not found."
            _dogMsgs.Add(19, "未找到所需的XML标记");//19|"Required XML tags not found; Contents in binary data are missing or invalid."
            _dogMsgs.Add(20, "超级狗不支持的升级请求");//20|"Update not supported by SuperDog."
            _dogMsgs.Add(21, "升级计数器设置不正确");//21|"Update counter is set incorrectly."
            _dogMsgs.Add(22, "开发商代码不正确");//22|"Invalid Vendor Code passed."
            _dogMsgs.Add(24, "输入的时间值超出支持的范围");//24|"Passed time value is outside supported value range."
            _dogMsgs.Add(26, "升级要求回执数据，但输入的接收参数为空");//26|"Acknowledge data requested by the update, however the ack_data input parameter is NULL."
            _dogMsgs.Add(27, "检测到程序在终端服务器上运行");//27|"Program running on a terminal server."
            _dogMsgs.Add(29, "不支持的算法");//29|"Unknown algorithm used in V2C file."
            _dogMsgs.Add(30, "签名验证失败");//30|"Signature verification failed."
            _dogMsgs.Add(31, "未找到特征");//31|"Requested Feature not available."
            _dogMsgs.Add(33, "和超级狗运行环境通讯错误");//33|"Communication error between API and local SuperDog License Manager."
            _dogMsgs.Add(34, "无法识别的开发商代码");//34|"Vendor Code not recognized by API."
            _dogMsgs.Add(35, "无效的XML格式");//35|"Invalid XML specification."
            _dogMsgs.Add(36, "无效的XML范围");//36|"Invalid XML scope."
            _dogMsgs.Add(37, "当前连接的超级狗数目过多");//37|"Too many SuperDog currently connected."
            _dogMsgs.Add(39, "会话被中断");//39|"Session was interrupted."
            _dogMsgs.Add(41, "特征已失效");//41|"Feature has expired."
            _dogMsgs.Add(42, "超级狗的运行环境版本太低");//42|"SuperDog License Manager version too old."
            _dogMsgs.Add(43, "与超级狗通讯过程中出现USB错误");//43|"USB error occurred when communicating with a SuperDog."
            _dogMsgs.Add(45, "检测到系统时钟被篡改");//45|"System time has been tampered."
            _dogMsgs.Add(46, "安全通道中发生了错误");//46|"Communication error occurred in secure channel."
            _dogMsgs.Add(47, "超级狗软许可存储区损坏");
            _dogMsgs.Add(50, "未找到与范围匹配的特征");//50|"Unable to locate a Feature matching the scope."
            _dogMsgs.Add(52, "超级狗软许可与此硬件不兼容");
            _dogMsgs.Add(54, "升级文件太旧，此升级文件已经安装过");
            //54|"Trying to install a V2C file with an update counter that is out" + 
            //54|"of sequence with the update counter in the SuperDog." + 
            //54|"The values of the update counter in the file are lower than" + 
            //54|"those in the SuperDog.", 
            _dogMsgs.Add(55, "升级文件太新，必须先安装其它的升级文件");
            //55|"Trying to install a V2C file with an update counter that is out" + 
            //55|"of sequence with the update counter in the SuperDog." + 
            //55|"The first value of the update counter in the file is greater than" + 
            //55|"the value in the SuperDog."
            _dogMsgs.Add(64, "检测到克隆的超级狗软许可");
            _dogMsgs.Add(65, "此升级文件已经应用过，不能再次应用");
            _dogMsgs.Add(78, "安全存储ID不匹配");
            _dogMsgs.Add(400, "没有找到供应商提供的API类库");//402|A required API dynamic library was not found.
            _dogMsgs.Add(401, "供应商提供的API类库不能通过校验");//401|The found and assigned API dynamic library could not be verified.
            _dogMsgs.Add(500, "对象没有正确初始化");//500|Object incorrectly initialized.
            _dogMsgs.Add(501, "一个无效的参数");//501|A parameter is invalid.
            _dogMsgs.Add(502, "重复登入");//502|Already logged in.
            _dogMsgs.Add(503, "重复登出");//502|Already logged in.
            _dogMsgs.Add(525, "不支持该系统版本");//525|Incorrect use of system or platform.
            _dogMsgs.Add(698, "不支持所请求的功能");//698|"Capability is not available."
            _dogMsgs.Add(699, "API发生内部错误");//699|"Internal API error."
            // windows
            _dogMsgs.Add(1001, "可执行文件不正确");
            _dogMsgs.Add(1002, "系统内存不足");
            _dogMsgs.Add(1003, "发生内部错误");
            _dogMsgs.Add(1004, "解密失败");
            _dogMsgs.Add(1019, "不支持的操作系统");
            _dogMsgs.Add(1030, "检测到调试器");
            _dogMsgs.Add(1031, "未找到超级狗");
            _dogMsgs.Add(1032, "发生未知错误");

            this._logger=Logger.CreateNew("dog");
        }

        public static CIMELDog Default { get { return _default;} }

        public string GetStatus(DogStatus status)
        {
            try
            {
                int key = (int) status;

                if (this._dogMsgs.ContainsKey(key))
                    return string.Format("{0} :: {1}", this._dogMsgs[(int) status], status);
                else
                    return status.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private const string VENDOR_CODE =
            "gecPzp8tW+/uGVzMaqUSR4On6+ubUxKDxOobq5vaySU8hyiUTKjxry" +
            "BrppGzrtXWeZX1L8H7U42sIXYOFfcyjNsNq2Ay5wzotO5H60Q6rqxH" +
            "Hsj5tHuDdQq9ISfKDUjNqzS2IpE043a9ItUVCioUDggvgzgrrwfSiJ" +
            "3JPX2wUSZSctb5vKU2MDOrmFiT9wB0uOIMh8n+bZaTM2h+HMYBCV+t" +
            "XxfN3uKBtWCl274C5GcAnck+ES5Qanp+m+24zf5DWs18oSCCMfAYLG" +
            "gWnAQ3G+1DIaUHIT1Ba0LT85nigdmMZP3ro42Znx8IASQ+aS06bVjP" +
            "BSYxfAOzHYRXm3vJ5YgtTFZlmoyQp/Z6Bo71XcLCPQA9r7QX7gzrD9" +
            "WKeugDAEO7j+t0ACb41HJAiFpFIPa04H0LITcMW4ihsgwKQWG/kyzz" +
            "EKJTDp0E6OXhMVouzavdffrcLBQSD01uoldVP6l5f9vLDQti0OSj7y" +
            "/0i/YM7Dyq2rcAxGl8ICadb3Y0Hg/htgE4Yfb0F5bscscNQK1JBuU0" +
            "7LjqbtkrdgtGQw/uCq0mU2HNwPlCvPDybEcSgWjZpbLik9MG+4oIMf" +
            "Ok4bdJR4a+rPce9jSb8wR5Gn9KJJlfUeIutZGMK6nyK1EIWZ4opdRy" +
            "EFU/ykfDo4gQspf7u+SVwJna6vcvTMCUFzsVvSJPDb4pVsDInGwHXj" +
            "2MVf+5Db024LiBi3xn6jYtmBrIgsWnODeGZ9r90J13IUyyqt0Ls3cW" +
            "V8oCLzpy7PkpaCZq/KCdLOVl2iQujze8dXWnI4VBKEJOrnhLSkB5cJ" +
            "cFwlWbhHPok5yCZgL+OKiwbUF9iamchxQciGXRqxIlF/mKavtqmQrz" +
            "Wspxpa9tI+bWJY9NfjhlCJRrKX+TFkJxLQwNsUrVCr+4gm/wHF0qlk" +
            "q4R27XOevHuLbalRISFR4SMGf91XPnWKESN8pHPYs4krs5cfvNl04M" +
            "gdCBCPzs1w==";

        private static DogFeature _global = DogFeature.FromFeature(1);

        private const string DEFAULT_SCOPE = "<dogscope />";

        public static string VendorCode
        {
            get
            {
                return VENDOR_CODE;
            }
        }

        public static string DefaultScope { get { return DEFAULT_SCOPE; } }

        /// <summary>
        /// checks if the dog is active, if not wakes it up; in dog term means checks if the dog instance is still valid.
        /// </summary>
        /// <returns></returns>
        public MethodResult IsAlive(bool autoExit=false,IWin32Window owner=null)
        {
            return new MethodResult();

            MethodResult isActived = new MethodResult();

            using (SuperDog.Dog dog=new SuperDog.Dog(_global))
            {
                try
                {
                    string scope = DefaultScope;
                    DogStatus status = dog.Login(VendorCode, scope);
                    if (status != DogStatus.StatusOk)
                    {
                        isActived = new MethodResult(false, this.GetStatus(status));
                        LogError(string.Format("Is dog active? {0}", isActived));
                    }
                }
                catch (Exception ex)
                {
                    LogError(string.Format("Is dog active? {0}", ex.Message));
                    isActived = new MethodResult(false, string.Empty);
                }
            }

#if DEBUG
            // var isActived = new MethodResult();
            if (isActived)
                LogInfo(string.Format("Is dog active? {0}", isActived));
#endif

            if (!isActived && autoExit)
            {
                string message = string.IsNullOrEmpty(isActived.Message)
                    ? @"运行禁止!"
                    : string.Format("运行禁止: {0}", isActived.Message);
                if (owner != null)
                    MessageBox.Show(owner, message, @"安全锁", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    MessageBox.Show(message, @"安全锁", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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


            /* unused
            if (!this._dog.IsLoggedIn())
            {
                LogInfo("The dog is still in sleep, wakes it up");
                var result = this.WakeUp();
                LogInfo(string.Format("Is dog active? {0}", result));
                return result;
            }

            try
            {
                var isActived = new MethodResult(this._dog.IsValid(), string.Empty);
                LogInfo(string.Format("Is dog active? {0}", isActived));
                return isActived;
            }
            catch (Exception ex)
            {
                var result = new MethodResult(false, ex.Message);
                LogError(string.Format("Is dog active? {0}", result));
                return new MethodResult(false, string.Empty);
            }
            */
        }

        private void LogDebug(string message)
        {
            this._logger.Debug(message);
        }

        public void LogError(string message)
        {
            this._logger.Error(message);
            // todo: show dog's session info
        }

        public void LogError(Exception ex)
        {
            this._logger.Error(ex);
            // todo: show dog's session info
        }

        public void LogInfo(string message)
        {
            this._logger.Info(message);
        }
    }
}
