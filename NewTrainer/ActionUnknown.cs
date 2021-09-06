using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionUnknown
    {
        DateTime unknownTime = DateTime.UtcNow;
        DateTime unknownTimeAction = DateTime.UtcNow;
        bool first = true;

        public void ResetTime()
        {
            unknownTime = DateTime.UtcNow;
            first = true;
        }

        public void DoAction()
        {
            Global.UIdata.MessagesUI.AddRecord(new UIMessages("Unknown"));
            if (first)
            {
                first = false;
                unknownTimeAction = DateTime.UtcNow;
            }

            if (unknownTimeAction.AddMinutes(8) < DateTime.UtcNow)
            {
                ActionsWithGameLogic.SaveImageIW("idle");
                ActionsWithGameLogic.StopGame();
                Thread.Sleep(1000);
                return;
            }
            if (String.IsNullOrEmpty(MainWork.LastActionName))
            {
                Global.input.MouseMove(10, 10);
                Global.input.MouseClick();
                return;
            }
            if (unknownTime.AddMinutes(3) <= DateTime.UtcNow)
            {
                if (unknownTime.AddMinutes(4) >= DateTime.UtcNow)
                {
                    LobbyExecution.PressExitFromLobby();
                    Thread.Sleep(500);
                }
                else
                {
                    Global.input.KeyPress(VirtualKeyCode.VK_ESCAPE);
                    unknownTime = DateTime.UtcNow.AddSeconds(-160);
                }
            }
        }
    }
}
