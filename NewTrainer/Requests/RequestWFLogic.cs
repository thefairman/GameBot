using NReco.PhantomJS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using xNet;

namespace NewTrainer
{
    public class RequestsWFLogic
    {
        ProxyLogic proxyLogic = new ProxyLogic(PrxType.HTTP);
        HttpRequest requestToWF = new HttpRequest();
        ProxyInfo proxyInfo;
        ReqInfo reqsInfo = new ReqInfo();
        static Uri urlWFSite = new Uri(@"https://wf.mail.ru");
        static Uri urlWFApi = new Uri(@"http://api.warface.ru");
        public RequestsWFLogic()
        {
            Rucaptcha.Key = "d9f70a1f3993f384391ef71c809194c3";
            proxyLogic.SomeError += Logman.LogEx;
            proxyInfo = proxyLogic.GetProxy();

            InitReq(requestToWF);

            phantomJS.OutputReceived += (sender, e) => {
                if (n_js_t == "")
                {
                    if (e.Data != null)
                        n_js_t = e.Data;
                }
                else
                {
                    if (e.Data != null)
                        n_js_d = e.Data;
                }
            };
        }

        void InitReq(HttpRequest req)
        {
            req.AllowAutoRedirect = true;
            req.UserAgent = Http.ChromeUserAgent();
            req.Cookies = new CookieDictionary();
            req.KeepAlive = true;
            req.ConnectTimeout = 10000;
            req.ReadWriteTimeout = 10000;
        }

        public void ResetBadProxy()
        {
            proxyLogic.ReseetBadProy();
        }

        public List<ProxyInfo> GetProxyList()
        {
            return proxyLogic.GetProxyList();
        }

        public void AddProxy(ProxyInfo proxy)
        {
            proxyLogic.AddProxy(proxy);
        }

        int GetBalance()
        {
            Global.UIdata.MessagesUI.AddRecord(new UIMessages("Rukaptcha # Getting Balance"));
            string bbalance = "?";

            try
            {
                bbalance = Rucaptcha.Balance();
            }
            catch (Exception ex) { Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message)); return 3; }

            Global.UIdata.MessagesUI.AddRecord(new UIMessages("Rukaptcha # Balance: " + bbalance));

            try
            {
                double blnc = Convert.ToDouble(bbalance.Replace('.', ','));
                Global.UIdata.ruCaptchaBalance = blnc;
                if (blnc < 0.1)
                {
                    return 1;
                }
            }
            catch (Exception ex) { Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message)); return 3; }

            return 0;
        }

        void SetProxyToReq(HttpRequest requestChecker, ProxyInfo proxyInfo)
        {
            string prxstr = $"{proxyInfo.address}:{proxyInfo.port}";
            if (proxyInfo.login != "" && proxyInfo.pass != "")
                prxstr += ":" + proxyInfo.login + ":" + proxyInfo.pass;

            switch (proxyLogic.proxyType)
            {
                case PrxType.HTTP:
                    requestChecker.Proxy = ProxyClient.Parse(ProxyType.Http, prxstr);
                    break;
                case PrxType.Socks4:
                    requestChecker.Proxy = ProxyClient.Parse(ProxyType.Socks4, prxstr);
                    break;
                case PrxType.Socks5:
                    requestChecker.Proxy = ProxyClient.Parse(ProxyType.Socks5, prxstr);
                    break;
                default:
                    break;
            }
            requestChecker.Proxy.ConnectTimeout = 10000;
            requestChecker.Proxy.ReadWriteTimeout = 10000;
        }

        bool CheckIsBlockedString(string content)
        {
            return content.Contains("captcha_input") || content.Contains("redirect");
        }

        string n_js_t, n_js_d;
        PhantomJS phantomJS = new PhantomJS();
        static readonly object lockJSCoockies = new object();
        KeyValuePair<string, string> GetJSCoockies(HttpRequest request)
        {
            lock (lockJSCoockies)
            {
                request.Referer = "https://wf.mail.ru/";

                HttpResponse resp = null;

                request.AddHeader("Accept", "*/*");
                resp = request.Get(@"https://wf.mail.ru/n.js");

                string content = resp.ToString();

                if (CheckIsBlockedString(content))
                {
                    return new KeyValuePair<string, string>(null, null);
                }

                string jsstr = "";

                string rplsStr = ");console.log(";
                content = content.Replace(");d.cookie='n_js_t=", rplsStr);

                int dotcom = content.IndexOf(';', content.IndexOf(rplsStr) + 5);
                jsstr = content.Substring(0, dotcom);
                string rplsStr2 = ";d.cookie='n_js_d='+h+';path=/;secure'";
                int endstrdotcom = content.IndexOf(rplsStr2);
                jsstr += ")" + content.Substring(endstrdotcom);
                jsstr = jsstr.Replace(rplsStr2, ";console.log(h); phantom.exit();");
                //jsstr += " phantom.exit();";

                n_js_t = n_js_d = "";

                phantomJS.RunScript(jsstr, null);
                //phantomJS.RunScript();

                while (n_js_t == "" || n_js_d == "")
                {
                    Thread.Sleep(200);
                }

                return new KeyValuePair<string, string>(n_js_t, n_js_d);
            }
        }

        bool UnBlockedIp(ProxyInfo proxyInfo)
        {
            Global.UIdata.MessagesUI.AddRecord(new UIMessages($"UnBlockedIp IP: {proxyInfo.address} # Start to unblocking"));
            ///string ip = await GetCurIp();
            using (HttpRequest req = new HttpRequest())
            {
                req.AllowAutoRedirect = true;
                req.UserAgent = Http.ChromeUserAgent();
                req.Cookies = new CookieDictionary();
                req.CharacterSet = Encoding.GetEncoding(65001);
                req.KeepAlive = true;
                req.ConnectTimeout = 2000;

                SetProxyToReq(req, proxyInfo);

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        HttpResponse resp = null;

                        resp = req.Get(@"https://c.mail.ru/0");

                        if (resp == null)
                        {
                            //MessageBox.Show("Ошибка получения запроса");
                            throw new Exception("error getting image");
                        }
                        byte[] imageByte = resp.ToBytes();
                        using (MemoryStream ms = new MemoryStream(imageByte, 0, imageByte.Length))
                        {
                            ms.Write(imageByte, 0, imageByte.Length);
                            Bitmap bmp = new Bitmap(Image.FromStream(ms, true));

                            string imgDirPath = Environment.CurrentDirectory + "\\img\\";
                            if (!Directory.Exists(imgDirPath))
                                Directory.CreateDirectory(imgDirPath);

                            Random rnd = new Random();
                            string imgPath = imgDirPath + "v" + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
                                + "_" + rnd.Next(0, 10000) + ".jpg";
                            bmp.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            bmp.Dispose();
                            string cptText = "";

                            if (GetBalance() == 1)
                                throw (new Exception("Low balance!"));

                            Global.UIdata.MessagesUI.AddRecord(new UIMessages($"UnBlockedIp IP: {proxyInfo.address} # waiting for blocked Captcha answer..."));

                            try
                            {
                                cptText = Rucaptcha.Recognize(imgPath);
                            }
                            catch (Exception ex)
                            {
                                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                            }

                            if (!cptText.ToLower().Contains("error") && cptText != "")
                            {
                                Global.UIdata.MessagesUI.AddRecord(new UIMessages($"UnBlockedIp IP: {proxyInfo.address} # entering blocked captcha..."));

                                req.Get(@"https://wf.mail.ru/validate/process.php?captcha_input=" + cptText);

                                resp = req.Get(@"https://wf.mail.ru/dynamic/auth/?profile_reload=0");

                                string respstr = resp.ToString();

                                if (!respstr.ToLower().Contains("validate"))
                                {
                                    Global.UIdata.MessagesUI.AddRecord(new UIMessages($"UnBlockedIp IP: {proxyInfo.address} # unblocked success!"));
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                    }
                }
            }

            return false;
        }

        void ChangeProxy(ref ProxyInfo proxy, ref ReqInfo reqInfo)
        {
            ProxyInfo proxyInfo;
            if (reqInfo.blocked)
            {
                proxyInfo = proxyLogic.ChangeBannedProxy(proxy.address, reqInfo.reqsts, false);
                ProxyInfo prxUnBlocked = (ProxyInfo)proxy.Clone();
                Task.Factory.StartNew(() => { ProxyInfo prxUnBlocked1 = prxUnBlocked; if (UnBlockedIp(prxUnBlocked1)) proxyLogic.UnbannedProxy(prxUnBlocked1.address); });
            }
            else if (reqInfo.numOfBadPoxy >= Global.settingsMain.maxNumOfBadPoxy)
            {
                proxyInfo = proxyLogic.ChangeBadProxy(proxy.address);
            }
            else
            {
                proxyInfo = proxyLogic.ChangeProxy(proxy.address, reqInfo.reqsts);
            }

            reqInfo.reqsts = 0;
            reqInfo.numOfBadPoxy = 0;
            reqInfo.blocked = false;
            proxy = proxyInfo;
        }

        string WFMakeReq(SendData sData)
        {
            int errorsCount = 0;
            do
            {
                try
                {
                    if (Global.settingsMain.usingProxyWF)
                    {
                        if ((reqsInfo.reqsts > Global.settingsMain.reqMax) || reqsInfo.blocked || reqsInfo.numOfBadPoxy >= Global.settingsMain.maxNumOfBadPoxy)
                        {
                            ChangeProxy(ref proxyInfo, ref reqsInfo);
                        }

                        SetProxyToReq(requestToWF, proxyInfo);
                    }
                    else
                    {
                        if (Global.settingsMain.switchToProxyIfBanned && reqsInfo.blocked)
                        {
                            reqsInfo = new ReqInfo();
                            Global.settingsMain.usingProxyWF = true;
                            continue;
                        }
                        requestToWF.Proxy = null;
                    }

                    requestToWF.Referer = "https://wf.mail.ru/";
                    requestToWF.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                    requestToWF.AddHeader("Accept-Language", "ru-RU");
                    requestToWF.AddHeader("X-Requested-With", "XMLHttpRequest");

                    if (sData.NeedACookiesJS)
                    {
                        KeyValuePair<string, string> jsCoockies = GetJSCoockies(requestToWF);

                        if (String.IsNullOrEmpty(jsCoockies.Key) || String.IsNullOrEmpty(jsCoockies.Value))
                        {
                            string ip = Global.settingsMain.usingProxyWF ? proxyInfo.address : "main IP";
                            Global.UIdata.ErrororsUI.AddRecord(new UIMessages($"blocked IP: {ip}"));
                            reqsInfo.blocked = true;
                            continue;
                        }

                        requestToWF.Cookies.Remove("n_js_t");
                        requestToWF.Cookies.Remove("n_js_d");
                        requestToWF.Cookies.Remove("has_js");

                        requestToWF.Cookies.Add("n_js_t", jsCoockies.Key);
                        requestToWF.Cookies.Add("n_js_d", jsCoockies.Value);

                        requestToWF.Cookies.Add("has_js", "1");
                    }

                    HttpResponse resp = null;
                    Uri uri = sData.Api ? new Uri(urlWFApi, sData.Path) : new Uri(urlWFSite, sData.Path);
                    if (sData.Post)
                    {
                        var reqParams = new RequestParams();

                        foreach (var item in sData.Parametrs)
                        {
                            reqParams[item.Key] = item.Value;
                        }
                        resp = requestToWF.Post(uri, reqParams);
                    }
                    else
                    {
                        uri = new Uri(uri, sData.ParamsToString());
                        resp = requestToWF.Get(uri);
                    }
                    reqsInfo.reqsts++;

                    string content = resp.ToString();
                    if (CheckIsBlockedString(content))
                    {
                        string ip = Global.settingsMain.usingProxyWF ? proxyInfo.address : "main IP";
                        Global.UIdata.ErrororsUI.AddRecord(new UIMessages($"blocked IP: {ip}"));
                        reqsInfo.blocked = true;
                        continue;
                    }

                    Global.UIdata.MessagesUI.AddRecord(new UIMessages($"{sData.Path} | params: {sData.ParamsToString()}"));
                    return content;
                }
                catch (ProxyException ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                    reqsInfo.numOfBadPoxy++;
                }
                catch (HttpException ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                    if (ex.Status == HttpExceptionStatus.ProtocolError)
                    {
                        if ((int)ex.HttpStatusCode != 401)
                            reqsInfo.numOfBadPoxy++;
                    }
                    else
                        reqsInfo.numOfBadPoxy++;
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }

                if (MainWork.PausedOrPausing())
                    return "";

                if (errorsCount >= 3)
                    Thread.Sleep(5000);
                else
                    Thread.Sleep(1000);
                errorsCount++;
            } while (true);
        }

        public UserInGameInfo GetUserInGameInfoWithApi(UserInfo userInfo)
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs.Add("name", userInfo.InGameInfo.nickname);
            parametrs.Add("server", userInfo.Server.ToString());
            SendData sData = new SendData()
            {
                Path = $"user/stat/",
                NeedACookiesJS = false,
                Post = false,
                Api = true,
                Parametrs = parametrs
            };

            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return null;
                try
                {
                    string content = WFMakeReq(sData);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<UserInGameInfo>(content);
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }
            }
        }

        public bool GetVip1()
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs.Add("74", "217");
            parametrs.Add("70", "209");
            parametrs.Add("63", "184");
            parametrs.Add("69", "207");
            parametrs.Add("73", "215");
            parametrs.Add("67", "199");
            parametrs.Add("75", "220");
            parametrs.Add("68", "203");
            parametrs.Add("64", "187");
            parametrs.Add("65", "190");
            parametrs.Add("61", "177");
            parametrs.Add("72", "213");
            parametrs.Add("62", "180");
            parametrs.Add("71", "211");
            parametrs.Add("66", "194");
            SendData sData = new SendData()
            {
                Path = @"dynamic/tests/?a=tests",
                NeedACookiesJS = true,
                Post = true,
                Api = false,
                Parametrs = parametrs
            };

            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return false;
                try
                {
                    string content = WFMakeReq(sData);
                    return content.ToLower().Contains("поздравляем");
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }
            }
        }

        public bool GetVip2()
        {
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs.Add("8", "1");
            parametrs.Add("1", "2");
            parametrs.Add("4", "1");
            parametrs.Add("3", "2");
            parametrs.Add("10", "2");
            parametrs.Add("9", "2");
            parametrs.Add("5", "2");
            parametrs.Add("2", "3");
            parametrs.Add("6", "3");
            parametrs.Add("7", "1");
            SendData sData = new SendData()
            {
                Path = @"dynamic/tests/?a=phonetests",
                NeedACookiesJS = true,
                Post = true,
                Api = false,
                Parametrs = parametrs
            };

            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return false;
                try
                {
                    string content = WFMakeReq(sData);
                    return content.ToLower().Contains("поздравляем");
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }
            }
        }

        public void CartProcess(List<int> list, UserInfo userInfo)
        {
            if (list.Count == 0)
                return;
            Dictionary<string, string> parametrs = new Dictionary<string, string>();
            parametrs.Add("shard_id", userInfo.Server.ToString());
            foreach (var item in list)
            {
                parametrs.Add($"items[{item}]", "on");
            }
            SendData sData = new SendData()
            {
                Path = @"dynamic/cart/?a=item_process",
                NeedACookiesJS = true,
                Post = true,
                Api = false,
                Parametrs = parametrs
            };

            WFMakeReq(sData);
        }

        public UserInGameInfo GetUserInGameInfo()
        {
            SendData sData = new SendData()
            {
                Path = @"dynamic/cart/",
                NeedACookiesJS = true,
                Post = false
            };
            MainJsonItems stuff;
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return null;
                try
                {
                    string content = WFMakeReq(sData);

                    int _cartJSind = content.IndexOf("parseJSON('");
                    if (_cartJSind == -1)
                    {
                        Global.UIdata.ErrororsUI.AddRecord(new UIMessages("no parseJSON"));
                        continue;
                    }

                    int strtCartJSind = content.IndexOf("{", _cartJSind);
                    int endCartJSind = content.IndexOf("}');", strtCartJSind + 1);

                    string cart = content.Substring(strtCartJSind, endCartJSind - strtCartJSind + 1);

                    cart = Regex.Unescape(cart).Replace("\"user\":[]", "\"user\":{\"4\":{\"username\":\"\u043f\u0435\u0448 - \u04401\u0430\u0443\u0435\u0433.\u043078\",\"level\":\"3\",\"shard\":\"\u0410\u043b\u044c\u0444\u0430\"}}");

                    stuff = Newtonsoft.Json.JsonConvert.DeserializeObject<MainJsonItems>(cart);
                    break;
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }
            }

            UserInGameInfo inGameUserInfo = new UserInGameInfo();
            if (stuff.user.ContainsKey(uInfo.Server))
            {
                inGameUserInfo.nickname = stuff.user[uInfo.Server].username;
                inGameUserInfo.rank_id = stuff.user[uInfo.Server].level;
            }
            inGameUserInfo.items = stuff.items;

            return inGameUserInfo;
        }

        UserInfo uInfo;
        public bool MakeAAuthorize(UserInfo uInfo)
        {
            if (requestToWF != null)
                requestToWF.Dispose();
            HttpRequest req = new HttpRequest();
            InitReq(req);
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return false;
                HttpResponse autorizResp = req.Get(@"https://aj-https.mail.ru/cgi-bin/auth?ajax_call=1&language=ru_RU&reqmode=fg&country=RU&Password=" + Uri.EscapeDataString(uInfo.Password) + "&simple=1&device_name=iPad5&mp=iOS&mmp=mail&udid=d32b8e29ca4247ef04fb01fe0080934e&os=iOS&device_vendor=Apple&mob_json=1&ver=4.2.1.10996&Login=" + uInfo.Login + "&timezone=GMT%2B3&model=iPad%20Air%20%28WiFi%29&os_version=8.1&device_type=Tablet&DeviceInfo=%7B%22OS%22%3A%22iOS%208.1%22%2C%22AppVersion%22%3A%224.2.1.10996%22%2C%22DeviceName%22%3A%22iPad5%22%2C%22Timezone%22%3A%22GMT%2B3%22%2C%22Device%22%3A%22Apple%20iPad%20Air%20%28WiFi%29%22%7D&DeviceID=7B44E6AF-70FE-4AC0-A7C7-7301ED07233F&");
                if (!autorizResp.Cookies.ContainsKey("t"))
                {
                    if (autorizResp.ToString().ToLower().Contains("ajaxresponse"))
                    {
                        Global.UIdata.ErrororsUI.AddRecord(new UIMessages($"Ошибка авторизации# {uInfo.Login}"));
                        req.Dispose();
                        return false;
                    }
                    Thread.Sleep(1000);
                }
                else
                    break;
            }
            requestToWF = req;
            this.uInfo = uInfo;
            return true;
        }

        class ReqInfo
        {
            public bool blocked { get; set; } = false;
            public int reqsts { get; set; } = 0;
            public int numOfBadPoxy { get; set; } = 0;
        }

        public class JsonItemInfo
        {
            public string title { get; set; }
            public int? duration { get; set; }
            public int? regular { get; set; }
            public int? permanent { get; set; }
            public string duration_type { get; set; }
            public string tag { get; set; }
            public string itemid { get; set; }
        }

        public class JsonItem
        {
            public int itemid { get; set; } = 0;
            public int cartid { get; set; } = 0;
            public JsonItemInfo iteminfo { get; set; }
        }

        class JsonUser
        {
            public string username { get; set; }
            public int level { get; set; }
            public string shard { get; set; }
        }

        class MainJsonItems
        {
            public Dictionary<int, List<JsonItem>> items = new Dictionary<int, List<JsonItem>>();
            public Dictionary<int, JsonUser> user = new Dictionary<int, JsonUser>();
        }
    }

    public class SendData
    {
        public string Path { get; set; }
        public Dictionary<string, string> Parametrs { get; set; } = new Dictionary<string, string>();
        public bool NeedACookiesJS { get; set; } = false;
        public bool Post { get; set; } = true;
        public bool Api { get; set; } = false;
        public string ParamsToString()
        {
            if (Parametrs.Count <= 0)
                return "";
            string str = "?";
            int i = 0;
            foreach (var item in Parametrs)
            {
                str += $"{item.Key}={item.Value}";
                if (i++ != Parametrs.Count) str += "&";
            }
            return str;
        }

        public string GetKeyString()
        {
            return $"{Parametrs["login"]}:{Path}";

            string str = Path;
            var pars = Parametrs.Keys.ToList();
            pars.Sort();
            foreach (var item in pars)
            {
                str += item;
            }
            return str;
        }
    }
}
