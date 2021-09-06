using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public class Logman
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LogAndDisplay(Exception ex, string msg = null)
        {
            string dispmsg = msg;
            if (dispmsg == null)
                dispmsg = ex.Message;
            logger.Error(ex, msg);
            Global.UIdata.ErrororsUI.AddRecord(new UIMessages(dispmsg));
        }
        public static void LogAndDisplay(string msg)
        {
            logger.Error(msg);
            Global.UIdata.ErrororsUI.AddRecord(new UIMessages(msg));
        }
        public static void Log(Exception ex, string msg = null)
        {
            logger.Error(ex, msg);
        }
        public static void LogEx(Exception ex)
        {
            logger.Error(ex);
        }
        public static void Log(string msg)
        {
            logger.Error(msg);
        }
    }
}
