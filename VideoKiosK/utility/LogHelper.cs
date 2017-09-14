using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace VideoKiosK.utility
{
    class LogHelper
    {
        public static readonly ILog logger = LogManager.GetLogger("root");

        public static void configure(string file)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileStream(file, System.IO.FileMode.Open));
            logger.Info("Log Configured");
        }
    }
}
