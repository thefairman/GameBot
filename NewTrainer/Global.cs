using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public class AllowedWindows
    {
        public static List<WindowData> allowedWindows = new List<WindowData>();
        public bool Contains(string title, string className)
        {
            foreach (var item in allowedWindows)
            {
                if (item.WindowTitle == title && item.WindowClassName == className)
                    return true;
            }
            return false;
        }
        public void Add(string title, string className)
        {
            allowedWindows.Add(new WindowData(title, className));
        }
        public void Clear()
        {
            allowedWindows.Clear();
        }
    }

    public static class Global
    {
        public static RequestsLogic requestsLogic = new RequestsLogic();
        public static UIDataMain UIdata { get; set; } = new UIDataMain();
        public static MySettings settingsMain;
        public static Input input = new Input();
        public static AllowedWindows allowedWindows = new AllowedWindows();
        public static UserLogic userLogic = new UserLogic();
        public static WarboxesForLevelUp warboxesForLevelUp = new WarboxesForLevelUp();
        public static WarBoxesForDonate warBoxesForDonate = new WarBoxesForDonate();
        public static WFDigits wfDigits = new WFDigits();
    }
}
