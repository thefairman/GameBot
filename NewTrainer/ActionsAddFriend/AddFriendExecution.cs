using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class AddFriendExecution
    {
        static public void ClickToFieldNick()
        {
            Global.input.MouseMove(404, 245);
            Global.input.MouseClick();
        }

        static public void PasteNickToField(string nick)
        {
            Global.input.PasteText(nick);
        }

        static public void ClickToSendFriendReq()
        {
            Global.input.MouseMove(344, 334);
            Global.input.MouseClick();
        }

        static public void ClickClose()
        {
            Global.input.MouseMove(455, 334);
            Global.input.MouseClick();
        }
    }
}
