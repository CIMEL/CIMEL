using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Core.Event
{
    public class MessageArgs:EventArgs
    {
        public string Message { get; protected set; }
        public MessageArgs(string message) { this.Message = message; }
    }
}
