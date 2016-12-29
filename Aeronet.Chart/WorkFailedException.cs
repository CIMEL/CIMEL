using System;

namespace Aeronet.Chart
{
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