using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewTrainer
{
    class ActionAddFriend : SomeActionBase
    {
        string hostNick { get; set; } = "";
        bool sended = false;
        DateTime lastGetHostNickTime = DateTime.MinValue;
        protected override bool DoAction()
        {
            if (!sended)
            {
                if (lastGetHostNickTime.AddSeconds(30) < DateTime.UtcNow)
                {
                    hostNick = Global.requestsLogic.GetHostNick();
                    lastGetHostNickTime = DateTime.UtcNow;
                }
                if (String.IsNullOrEmpty(hostNick))
                    return true;
                AddFriendExecution.ClickToFieldNick();
                SendKeys.SendWait("^{HOME}");   // Move to start of control
                Thread.Sleep(100);
                SendKeys.SendWait("^+{END}");   // Select everything
                Thread.Sleep(100);
                SendKeys.SendWait("{DEL}");     // Delete selection
                Thread.Sleep(100);
                AddFriendExecution.PasteNickToField(hostNick);
                AddFriendExecution.ClickToSendFriendReq();
                sended = true;
                Global.userLogic.GetUserInfo().SendedInviteToHost = true;
            }
            else
            {
                AddFriendExecution.ClickClose();
                Thread.Sleep(500);
                return false;
            }
            return true;
        }

        protected override void FirstInit()
        {
            if (MainWork.LastActionName != GetClassName())
                sended = false;
        }

        protected override bool IsThisAction()
        {
            for (int x = 350; x < 460; x++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, 235];
                if (!Helpers.Between(pix, Color.White, 2))
                    return false;
                pix = ActionsWithGameLogic.ScreenOfGame[x, 256];
                if (!Helpers.Between(pix, Color.White, 2))
                    return false;
                pix = ActionsWithGameLogic.ScreenOfGame[x, 260];
                if (!Helpers.Between(pix, Color.Black, 2))
                    return false;
            }
            return true;
        }
    }
}
