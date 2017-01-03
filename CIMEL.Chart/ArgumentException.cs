using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    public class ArgumentException:Exception
    {
        public ArgumentException(): base()
        {
        }

        public ArgumentException(string message)
            : base(message)
        {
            
        }
    }
}
