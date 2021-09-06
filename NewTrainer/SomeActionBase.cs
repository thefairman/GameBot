using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    abstract class SomeActionBase
    {
        protected string strMsg;
        protected abstract bool IsThisAction();
        protected abstract bool DoAction();
        public SomeActionBase()
        {
            strMsg = GetClassName();
        }

        protected void AddToMsg(object msg)
        {
            strMsg += $" {msg}";
        }

        protected void Action()
        {
            do
            {
                if (MainWork.PausedOrPausing())
                {
                    LastMethod();
                    break;
                }
                if (ExitGameIfIDLE())
                    break;
                if (Global.settingsMain.showActionMessage && !Global.settingsMain.showActionMessageFirsOnly)
                {
                    ActionMsg();
                    strMsg = GetClassName();
                }
                if (!DoAction())
                    break;

                int st = (int)(lastActionTeime.AddMilliseconds(sleepTime) - DateTime.UtcNow).TotalMilliseconds;
                if (st > 0)
                {
                    Thread.Sleep(st);
                }
                lastActionTeime = DateTime.UtcNow;

                firstIter = false;

                Launcher.CloseNotAllowedWindows();
                ActionsWithGameLogic.RefreshScreenOnGame();
            } while (IsThisAction());
            LastMethod();
        }

        virtual public void LastMethod()
        {
            return;
        }

        virtual protected bool ExitGameIfIDLE()
        {
            if (inThisModeTime.AddMinutes(maxMinInOneLogic) < DateTime.UtcNow)
            {
                StopGame();
                return true;
            }
            return false;
        }

        protected int maxMinInOneMode = 8;
        protected int maxMinInOneLogic = 30;
        protected DateTime inThisModeTime;
        protected bool firstIter;
        protected int sleepTime = 200;
        protected DateTime lastActionTeime;
        public bool TryDoAction()
        {
            if (MainWork.PausedOrPausing())
            {
                LastMethod();
                return false;
            }
            if (!IsThisAction())
                return false;
            strMsg = GetClassName();
            firstIter = true;
            lastActionTeime = DateTime.UtcNow;
            inThisModeTime = DateTime.UtcNow;
            if (Global.settingsMain.showActionMessage && Global.settingsMain.showActionMessageFirsOnly)
                ActionMsg();
            FirstInit();
            Action();
            return true;
        }

        virtual protected void FirstInit()
        {
            
        }

        virtual protected void ActionMsg()
        {
            Global.UIdata.MessagesUI.AddRecord(new UIMessages(strMsg));
        }

        public string GetClassName()
        {
            return this.GetType().Name;
        }

        public void StopGame()
        {
            ActionsWithGameLogic.StopGame();
            Thread.Sleep(1000);
        }

        protected bool StopGameInLongIDLE()
        {
            if (inThisModeTime.AddMinutes(maxMinInOneMode) < DateTime.UtcNow)
            {
                ActionsWithGameLogic.SaveImageIW("idle");
                StopGame();
                return true;
            }
            return false;
        }
    }
}
