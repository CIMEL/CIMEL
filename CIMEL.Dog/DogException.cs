using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Dog
{
    public class DogException:Exception
    {
        private string _message;
        public DogException() : base() { }

        public DogException(string message) : base(message)
        {
            this._message = message;
        }

        public override string Message
        {
            get { return string.IsNullOrEmpty(this._message) ? "安全锁异常!" : this._message; }
        }
    }
}
