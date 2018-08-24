using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyCleaning.Log
{
    public class Log
    {
        private static ILog _logger = null;
        private static log4net.ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = LogManager.GetLogger(typeof(Log));
                    log4net.Config.XmlConfigurator.Configure();
                }
                return _logger;
            }
        }

        public static void LogError(string msg, Exception ex)
        {
            Logger.Error(msg, ex);
        }

        public static void LogInfo(string msg)
        {
            Logger.Info(msg);
        }
    }
}
