using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aeronet.Chart
{
    public class WorkCancelException:Exception
    {

        public WorkCancelException():base() { }

        public WorkCancelException(string message) : base(message) { }

        public override string Message
        {
            get { return string.IsNullOrEmpty(base.Message) ? "User Cancelled!" : base.Message; }
        }
    }

    public class WorkFailedException : Exception
    {
        public WorkFailedException(): base()
        {
        }

        public WorkFailedException(string message) : base(message)
        {
            
        }
    }
}
