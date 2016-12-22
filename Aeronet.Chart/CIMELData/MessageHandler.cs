using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aeronet.Chart.AeronetData
{
    public class EventMessage
    {
        public EventMessage() { }

        public EventMessage(string message, bool external)
        {
            this.IsExternal = external;
            this.Message = message;
        }

        public bool IsExternal { get; set; }
        public string Message { get; set; }
    }


    public delegate void MessageHandler(object sender, EventMessage message);
}
