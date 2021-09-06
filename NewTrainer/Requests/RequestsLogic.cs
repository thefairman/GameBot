using NReco.PhantomJS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using xNet;

namespace NewTrainer
{
    public class RequestsLogic
    {
        public DateTime LastTimeServerUpdate { get; private set; } = DateTime.MinValue;

        static Uri urlMySite = new Uri(@"http://wfka4.hol.es");
        public static RequestsWFLogic wfReqLogic = new RequestsWFLogic();
        System.Timers.Timer timerSendAsync = new System.Timers.Timer(5000);
        public RequestsLogic()
        {
            timerSendAsync.Elapsed += timerSendAsync_Elapsed;
        }

        private void timerSendAsync_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSendAsync.Stop();
            try
            {
                var key = requestQueue.Keys.First();
                if (key != null)
                {
                    SendData sdata;
                    if (requestQueue.TryGetValue(key, out sdata))
                    {
                        bool isOk;
                        SendToServer(sdata, out isOk);
                        if (isOk)
                            requestQueue.TryRemove(key, out sdata);
                    }
                }
            }
            catch (Exception ex)
            {
                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
            }
            if (requestQueue.Count > 0)
                timerSendAsync.Start();
        }

        ConcurrentDictionary<string, SendData> requestQueue = new ConcurrentDictionary<string, SendData>();

        void TryAddToQueue(SendData sData)
        {
            SendData prevSData;
            if (requestQueue.TryRemove(sData.GetKeyString(), out prevSData))
            {
                foreach (var item in prevSData.Parametrs)
                {
                    if (!sData.Parametrs.ContainsKey(item.Key))
                        sData.Parametrs[item.Key] = item.Value;
                }
            }
            while (!requestQueue.TryAdd(sData.GetKeyString(), sData)) Thread.Sleep(50);
            timerSendAsync.Start();
        }

        public void AsyncUpdateUserInGameInfo(UserInfo userInfo)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["login"] = userInfo.Login;
            parametrs["experience"] = userInfo.InGameInfo.experience.ToString();
            parametrs["nick"] = userInfo.InGameInfo.nickname;
            parametrs["rank"] = userInfo.InGameInfo.rank_id.ToString();
            SendData sData = new SendData() { Parametrs = parametrs, Path = "update_acc.php" };
            TryAddToQueue(sData);
        }

        public void AsyncUpdateUserOnServer(UserInfo userInfo)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["login"] = userInfo.Login;
            SendData sData = new SendData() { Parametrs = parametrs, Path = "update_acc.php" };
            TryAddToQueue(sData);
        }

        public UserInGameInfo AsyncUpdateUserInGameInfoByAPI(UserInfo userInfo)
        {
            UserInGameInfo inGameInfo = wfReqLogic.GetUserInGameInfoWithApi(userInfo);
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["login"] = userInfo.Login;
            parametrs["experience"] = inGameInfo.experience.ToString();
            parametrs["rank"] = inGameInfo.rank_id.ToString();
            SendData sData = new SendData() { Parametrs = parametrs, Path = "update_acc.php" };
            TryAddToQueue(sData);
            return inGameInfo;
        }

        public void AsyncUpdateUserInfo(UserInfo userInfo)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["login"] = userInfo.Login;
            parametrs["crowns"] = userInfo.Crowns.ToString();
            parametrs["wb"] = userInfo.WB.ToString();
            SendData sData = new SendData() { Parametrs = parametrs, Path = "update_acc.php" };
            TryAddToQueue(sData);
        }

        bool CompairInGameInfo(UserInGameInfo a, UserInGameInfo b, bool checkExperience = false)
        {
            if (a == null || b == null)
                return false;
            return a.nickname == b.nickname && a.rank_id == b.rank_id && (!checkExperience || a.experience == b.experience);
        }

        void CheckAndSetInGameInfo(UserInfo userInfo, UserInGameInfo uInGameInfo)
        {
            bool someChanged = !CompairInGameInfo(userInfo.InGameInfo, uInGameInfo);
            userInfo.InGameInfo = uInGameInfo;
            if (someChanged) AsyncUpdateUserInGameInfo(userInfo);
        }

        public string GetHostNick()
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["server"] = (Global.settingsMain.server + 1).ToString();
            SendData sData = new SendData() { Parametrs = parametrs, Path = "get_host_nick.php" };
            string resp = CycleToSend(sData);
            UserInfo uinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(resp, new CustomDateTimeConverter());
            return uinfo.InGameInfo.nickname;
        }

        public UserInfo GetAndCheckUser()
        {
            UserInfo userInfo = GetAccFromServerAndSetAuthorize();
            CheckAndSetInGameInfo(userInfo, wfReqLogic.GetUserInGameInfo());
            return userInfo;
        }

        public UserInGameInfo GetUserInGameInfo(UserInfo userInfo)
        {
            UserInGameInfo userInGameInfo = wfReqLogic.GetUserInGameInfo();
            CheckAndSetInGameInfo(userInfo, wfReqLogic.GetUserInGameInfo());
            return userInGameInfo;
        }

        public bool GetVip1()
        {
            return wfReqLogic.GetVip1();
        }
        public bool GetVip2()
        {
            return wfReqLogic.GetVip2();
        }
        public void CartProcess(List<int> list, UserInfo userInfo)
        {
            wfReqLogic.CartProcess(list, userInfo);
        }
        //public UserInfo Check

        UserInfo GetAccFromServerAndSetAuthorize()
        {
            UserInfo userInfo;
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return null;
                userInfo = GetFromServerAcc();
                if (!wfReqLogic.MakeAAuthorize(userInfo))
                    SetAccNoAuthorize(userInfo);
                else
                    break;
            }
            userInfo.AuthorizChecked = true;
            return userInfo;
        }

        string CycleToSend(SendData sData)
        {
            int errorsCount = 0;
            bool isOk;
            string content;
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return "";
                content = SendToServer(sData, out isOk);
                if (isOk)
                    return content;
                else
                {
                    if (errorsCount >= 3)
                        Thread.Sleep(5000);
                    else
                        Thread.Sleep(500);
                    errorsCount++;
                }
            }
        }

        void SetAccNoAuthorize(UserInfo userInfo)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["login"] = userInfo.Login;
            parametrs["no_authorize"] = "1";
            SendData sData = new SendData() { Parametrs = parametrs, Path = "update_acc.php" };
            CycleToSend(sData);
        }

        UserInfo GetFromServerAcc()
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs["server"] = (Global.settingsMain.server + 1).ToString();
            parametrs["host"] = (Global.settingsMain.isHost ? "1" : "0");
            SendData sData = new SendData() { Parametrs = parametrs, Path = "get_acc.php" };
            string resp = CycleToSend(sData);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(resp, new CustomDateTimeConverter());
        }

        string SendToServer(SendData sData, out bool isOk)
        {
            bool ok = false;
            string strResp = "";

            var reqParams = new RequestParams();

            foreach (var item in sData.Parametrs)
            {
                reqParams[item.Key] = item.Value;
            }

            try
            {
                using (HttpRequest requestToServer = new HttpRequest())
                {
                    requestToServer.AllowAutoRedirect = true;
                    requestToServer.UserAgent = Http.ChromeUserAgent();
                    requestToServer.KeepAlive = true;
                    requestToServer.CharacterSet = Encoding.GetEncoding(65001);

                    Uri uri = new Uri(urlMySite, sData.Path);
                    HttpResponse resp = requestToServer.Post(uri, reqParams);
                    if (resp != null)
                    {
                        strResp = resp.ToString();
                        if (strResp == "ok" || strResp.Contains("\"success\":1"))
                        {
                            Global.UIdata.MessagesUI.AddRecord(new UIMessages($"{sData.Path} | params: {sData.ParamsToString()} | response: {strResp}"));
                            ok = true;
                        }
                        else
                            Global.UIdata.ErrororsUI.AddRecord(new UIMessages($"{sData.Path} | params: {sData.ParamsToString()} | response: {strResp}"));
                    }
                }
            }
            catch (Exception ex)
            {
                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
            }

            isOk = ok;
            if (ok)
                LastTimeServerUpdate = DateTime.UtcNow;
            return strResp;
        }
    }
}
