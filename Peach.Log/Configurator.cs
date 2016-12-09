using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Peach.Log
{
    public class Configurator
    {
        public static void Configurate(string logConfig)
        {
           log4net.Config.XmlConfigurator.Configure(new FileInfo(logConfig));
        }
        public static void Configurate(Stream logConfigStream)
        {
            log4net.Config.XmlConfigurator.Configure(logConfigStream);
        }
    }
}
