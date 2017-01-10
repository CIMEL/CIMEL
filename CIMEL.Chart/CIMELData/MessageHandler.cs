using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Chart.CIMELData
{
    public class EventMessage
    {
        public EventMessage() { }

        public EventMessage(string message, bool external,bool showDlg=false)
        {
            this.IsExternal = external;
            this.Message = message;
            this.ShowDlg = showDlg;
        }

        public bool IsExternal { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// if true should display the message on the alert dialog
        /// </summary>
        public bool ShowDlg { get; set; }
    }


    public delegate void MessageHandler(object sender, EventMessage message);
}
