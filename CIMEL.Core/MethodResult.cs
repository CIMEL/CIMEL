using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Core
{
    public class MethodResult
    {
        public MethodResult(bool result, string message)
        {
            this.Result = result;
            this.Message = message;
        }

        public MethodResult()
        {
            this.Result = true;
            this.Message = string.Empty;
        }

        public MethodResult(string message) : this(true, message)
        {
            
        }

        public bool Result { get; private set; }

        public string Message { get; private set; }

        public static implicit operator bool(MethodResult result)
        {
            return result.Result;
        }

        public static implicit operator MethodResult(bool result)
        {
            return new MethodResult(result,string.Empty);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Message))
                return Result ? "True" : "False";
            return string.Format("{0}: {1}", Result ? "True" : "False", Message);
        }
    }
}
