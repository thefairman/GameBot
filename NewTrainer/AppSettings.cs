using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public class AppSettings<T> where T : new()
    {
        public void Save(string filePath)
        {
            File.WriteAllText(filePath, Newtonsoft.Json.JsonConvert.SerializeObject(this, new CustomDateTimeConverter()));
        }
        public static T Load(string filePath)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath), new CustomDateTimeConverter());
        }
    }

    public class AppSettingsManager
    {
        public static void LoadSettings()
        {
            if (File.Exists(GetSettingsPath()))
            {
                Global.settingsMain = MySettings.Load(GetSettingsPath());
                if (Global.settingsMain == null)
                    Global.settingsMain = new MySettings();
            }
            else
                Global.settingsMain = new MySettings();
        }

        public static string GetSettingsPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "settings.json");
        }
    }

    public class MySettings : AppSettings<MySettings>
    {
        public int maxErrorsUIMessages = 20;
        public int maxLogUIMessages = 20;
        public string wfWindowClass = "CryENGINE";
        public string wfWindowName = "Warface";
        public bool isHost = true;
        public int server = 0;
        public int reqMax = 10;
        public int maxNumOfBadPoxy = 3;
        public bool usingProxyWF = false;
        public bool switchToProxyIfBanned = true;
        public bool wfOnTop = false;
        public bool wfForeGround = true;
        public bool showActionMessage = true;
        public bool showActionMessageFirsOnly = false;
        public bool testing = true;
        public bool activateVips = true;
        public bool saveScoreScreen = true;
        public int neededRankExperience = 23800;
        public int neededRank = 11;
    }
}
