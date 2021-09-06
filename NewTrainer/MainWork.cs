using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class MainWork
    {
        static public bool Running { get; private set; }
        static bool Paused { get; set; } = false;
        List<SomeActionBase> listActions = new List<SomeActionBase>();
        static public string LastActionName { get; set; } = "";
        ActionUnknown unknownAction = new ActionUnknown();
        System.Timers.Timer someBGWork = new System.Timers.Timer(60000);
        public string LastActionNameNotMessage { get; set; }
        ActionMessage actionMessage = new ActionMessage();
        ActionInGame actionInGame = new ActionInGame();
        public MainWork()
        {
            FillActionsList();
            someBGWork.Elapsed += SomeBGWork_Elapsed;
            someBGWork.Start();
        }

        private void SomeBGWork_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (PausedOrPausing())
                return;
            someBGWork.Stop();
            try
            {
                if (Global.userLogic.GetUserInfo().InGameInfo.rank_id >= Global.settingsMain.neededRank)
                {
                    Global.userLogic.ChangeUserInfoAndCheckAuthorize();
                    ActionsWithGameLogic.StopGame();
                    Thread.Sleep(1000);
                }

                if (oneACtionTime.AddMinutes(30) < DateTime.UtcNow)
                {
                    ActionsWithGameLogic.SaveImageIW("idle");
                    ActionsWithGameLogic.StopGame();
                    Thread.Sleep(1000);
                }

                if (Global.requestsLogic.LastTimeServerUpdate.AddMinutes(7) < DateTime.UtcNow)
                {
                    if (Global.userLogic.GetNoSetUserInfo() != null)
                        Global.userLogic.UpdateUserInGameInfoByAPI();
                }
                    
            }
            catch (Exception ex)
            {
                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
            }
            someBGWork.Start();
        }

        public void TestingSetRunning(bool running)
        {
            Running = running;
        }

        void FillActionsList()
        {
            listActions.Add(actionInGame);
            listActions.Add(new ActionLobby());
            listActions.Add(new ActionAddFriend());
            listActions.Add(new ActionShop());
            listActions.Add(new ActionWarBox());
            listActions.Add(actionMessage);
        }

        public static bool PausedOrPausing()
        {
            return !Running || Paused;
        }

        protected int sleepTime = 200;
        protected DateTime lastActionTime;
        DateTime oneACtionTime = DateTime.UtcNow;
        void MainWorkCycle()
        {
            while (true)
            {
                if (Paused)
                {
                    Running = false;
                    Paused = false;
                }
                if (!Running)
                {
                    Thread.Sleep(500);
                    continue;
                }

                int st = (int)(lastActionTime.AddMilliseconds(sleepTime) - DateTime.UtcNow).TotalMilliseconds;
                if (st > 0)
                {
                    Thread.Sleep(st);
                }
                lastActionTime = DateTime.UtcNow;

                try
                {
                    Launcher.CloseNotAllowedWindows();
                    ActionsWithGameLogic.RefreshScreenOnGame();

                    string prevActionName = LastActionName;
                    string prevActionNameNotMessage = LastActionNameNotMessage;
                    bool haveSomeAction = false;
                    foreach (var item in listActions)
                    {
                        if (item.TryDoAction())
                        {
                            LastActionName = item.GetClassName();
                            if (LastActionName != actionMessage.GetClassName())
                                LastActionNameNotMessage = LastActionName;
                            haveSomeAction = true;
                            break;
                        }
                    }

                    if (prevActionName != LastActionName)
                        oneACtionTime = DateTime.UtcNow;

                    if (prevActionNameNotMessage != LastActionNameNotMessage)
                    {
                        ActionMessage.SetMessageType(ActionMessage.MessegeTypes.Default);
                    }

                    if (LastActionName == actionInGame.GetClassName())
                        Global.userLogic.UpdateUserInGameInfoByAPI();

                    if (!haveSomeAction)
                        unknownAction.DoAction();
                    else
                        unknownAction.ResetTime();
                }
                catch (BotPausedException bps)
                {
                    Global.UIdata.MessagesUI.AddRecord(new UIMessages(bps.Message));
                    ExecuteLastMethods();
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                    ExecuteLastMethods();
                }
            }
        }

        void ExecuteLastMethods()
        {
            foreach (var item in listActions)
            {
                item.LastMethod();
            }
        }

        void FillAllowedWindows()
        {
            Global.allowedWindows.Clear();
            Global.allowedWindows.Add(Global.settingsMain.wfWindowName, Global.settingsMain.wfWindowClass);

            var windows = GetWindows.WindowsToList(false);
            foreach (var item in windows)
            {
                Global.allowedWindows.Add(item.Value.WindowTitle, item.Value.WindowClassName);
            }
        }

        Task mwc = null;
        public void Start()
        {
            if (mwc == null || mwc.Status != TaskStatus.Running)
            {
                mwc = Task.Factory.StartNew(MainWorkCycle);
            }
            FillAllowedWindows();
            Running = true;
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }
    }
}
