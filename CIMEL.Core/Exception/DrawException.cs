using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CIMEL.Core.Exception
{
    public class DrawException : ApplicationException
    {
        public DrawException()
        {
        }

        public DrawException(string message) : base(message)
        {
        }

        public DrawException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected DrawException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
