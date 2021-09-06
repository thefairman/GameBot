using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public class UserInfo
    {
        public string Login { get; set; }
        public string Password { get; set; } = "";
        public int Server { get; set; }
        public UserInGameInfo InGameInfo { get; set; } = null;
        public bool AuthorizChecked { get; set; } = false;
        public bool CurrenciesChecked { get; set; } = false;
        public bool VipsChecked { get; set; } = false;
        public bool SendedInviteToHost { get; set; } = false;
        public int Crowns { get; set; } = 0;
        public int WB { get; set; } = 0;

        public UserInfo()
        {
            if (Password == "")
                Password = Base64Decode("***");
           
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class UserInGameInfo
    {
        public string nickname { get; set; } = "";
        public int experience { get; set; } = 0;
        public int rank_id { get; set; } = 0;
        public Dictionary<int, List<RequestsWFLogic.JsonItem>> items = new Dictionary<int, List<RequestsWFLogic.JsonItem>>();
    }

    public class UserLogic
    {
        UserInfo UserData = null;
        public bool ExperienceShouldBeChanged { get; set; } = false;
        System.Timers.Timer updateExperienceTimer = new System.Timers.Timer(10000);
        public UserLogic()
        {
            updateExperienceTimer.Elapsed += UpdateExperienceTimer_Elapsed;
        }
        public UserInfo GetNoSetUserInfo()
        {
            return UserData;
        }

        private void UpdateExperienceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateExperienceTimer.Stop();
            if (ExperienceShouldBeChanged)
            {
                int prevExp = UserData.InGameInfo.experience;
                UpdateUserInGameInfoByAPI();
                if (prevExp != UserData.InGameInfo.experience)
                    ExperienceShouldBeChanged = false;
            }
            updateExperienceTimer.Start();
        }

        public UserInfo GetUserInfo()
        {
            if (UserData != null)
                return UserData;
            return UserData = Global.requestsLogic.GetAndCheckUser();
        }

        public void AsyncUpdateUserOnServer()
        {
            Global.requestsLogic.AsyncUpdateUserOnServer(UserData);
        }

        public UserInfo ChangeUserInfoAndCheckAuthorize()
        {
            UserData = null;
            return GetUserInfo();
        }

        int GetCartidOfBoost15PercentLessOrEq7Day()
        {
            foreach (var itemsID in UserData.InGameInfo.items)
            {
                foreach (var item in itemsID.Value)
                {
                    if (item.iteminfo.itemid == "booster_03" && item.iteminfo.duration <= 7 && item.iteminfo.duration_type == "day")
                    {
                        return item.cartid;
                    }
                }
            }
            return -1;
        }

        int GetCartidOfBoost100Percent1Day()
        {
            foreach (var itemsID in UserData.InGameInfo.items)
            {
                foreach (var item in itemsID.Value)
                {
                    if (item.iteminfo.itemid == "booster_02" && item.iteminfo.duration == 1 && item.iteminfo.duration_type == "day")
                    {
                        return item.cartid;
                    }
                }
            }
            return -1;
        }

        int UpdateCartAndGetCartidOfBoost100Percent1Day()
        {
            var inGameInfo = Global.requestsLogic.GetUserInGameInfo(UserData);
            UserData.InGameInfo.items = inGameInfo.items;
            return GetCartidOfBoost100Percent1Day();
        }

        public void CheckAndGetVips()
        {
            List<int> list = new List<int>();
            int currentVipPercent = Global.wfDigits.GetVipValue();
            if (currentVipPercent < 115)
            {
                if (currentVipPercent < 15 || currentVipPercent == 100) // check for 15% booster
                {
                    int cartid = GetCartidOfBoost15PercentLessOrEq7Day();
                    if (cartid >= 0)
                        list.Add(cartid);
                }
                if (currentVipPercent < 100) // check for 100% booster
                {
                    int cartid = GetCartidOfBoost100Percent1Day();
                    if (cartid >= 0)
                        list.Add(cartid);
                    else
                    {
                        if (Global.requestsLogic.GetVip1())
                        {
                            cartid = UpdateCartAndGetCartidOfBoost100Percent1Day();
                            if (cartid >= 0)
                                list.Add(cartid);
                        }
                        else
                        {
                            if (Global.requestsLogic.GetVip2())
                            {
                                cartid = UpdateCartAndGetCartidOfBoost100Percent1Day();
                                if (cartid >= 0)
                                    list.Add(cartid);
                            }
                        }
                    }
                }
            }
            if (list.Count > 0)
                Global.requestsLogic.CartProcess(list, UserData);
            UserData.VipsChecked = true;
        }

        public void UpdateUserCurrenciesByGameScreen(/*WFCurrency currency, int quantity, bool updateOnServerAsync*/)
        {
            if (UserData == null)
                return;
            //switch (currency)
            //{
            //    case WFCurrency.WB:
            //        UserData.WB = quantity;
            //        break;
            //    case WFCurrency.Crowns:
            //        UserData.Crowns = quantity;
            //        break;
            //    case WFCurrency.Credits:
            //        break;
            //    default:
            //        break;
            //}
            //if (updateOnServerAsync)
            int curCrowns = Global.wfDigits.GetCrowns();
            int curWB = Global.wfDigits.GetWB();
            bool needUpdt = false;
            if (UserData.Crowns != curCrowns)
            {
                UserData.Crowns = curCrowns;
                needUpdt = true;
            }
            if (UserData.WB != curWB)
            {
                UserData.WB = curWB;
                needUpdt = true;
            }
            if (needUpdt)
                Global.requestsLogic.AsyncUpdateUserInfo(UserData);
            UserData.CurrenciesChecked = true;
        }

        public void UpdateUserInGameInfoByAPI()
        {
            int prevExp = UserData.InGameInfo.experience;

            UserInGameInfo inGameInfo = Global.requestsLogic.AsyncUpdateUserInGameInfoByAPI(UserData);
            UserData.InGameInfo.experience = inGameInfo.experience;
            UserData.InGameInfo.rank_id = inGameInfo.rank_id;

            if (prevExp != UserData.InGameInfo.experience)
                ExperienceShouldBeChanged = false;
        }

        internal string GetNick()
        {
            if (!String.IsNullOrEmpty(UserData.InGameInfo.nickname))
                return UserData.InGameInfo.nickname;

            return TranslitLoginToNick();
        }

        int minLengthNick = 4;
        int maxLenghtNick = 16;

        string TranslitLoginToNick()
        {
            int smbls = UserData.Login.IndexOf("@") + 1;
            if (smbls <= 0)
                smbls = UserData.Login.Length;
            string nick = UserData.Login.Substring(0, smbls);
            nick = Helpers.ToTranslit(nick);
            if (nick.Length > maxLenghtNick - 3)
                nick = nick.Substring(0, maxLenghtNick - 3);
            for (int i = nick.Length; i < minLengthNick; i++)
            {
                nick += ".";
            }
            return nick;
        }
    }
}
