using CIMEL.Core;
using CIMEL.Dog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIMEL.Chart
{
    public class ActiveChecker
    {
        private static ActiveChecker _singleton = new ActiveChecker();
        public static ActiveChecker Singleton { get { return _singleton; } }

        protected ActiveChecker() { }

        public MethodResult IsActive(bool autoExit=false)
        {
            // checks if the super dog is still working
            MethodResult result = CIMELDog.Default.IsAlive(autoExit);
            if (!result) return result;
            // checks if the license is still active
            result = Register.Singleton.IsAlive(autoExit);
            return result;
        }
    }
}
