using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aeronet.Chart
{
    public class WorkCancelException:Exception
    {
        private string _message;
        public WorkCancelException():base() { }

        public WorkCancelException(string message) : base(message)
        {
            this._message = message;
        }

        public override string Message
        {
            get { return string.IsNullOrEmpty(this._message) ? "用户取消操作!" : this._message; }
        }
    }
}
