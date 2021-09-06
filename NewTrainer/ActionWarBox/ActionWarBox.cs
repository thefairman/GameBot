using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionWarBox : SomeActionBase
    {
        static WarboxesBase warBoxLogic = null;
        Warbox prevWarBox = null;
        Warbox warbox = null;
        protected override bool DoAction()
        {
            if (warBoxLogic == null)
            {
                WarBoxExecutions.ClickExitWarBox();
                return false;
            }
            prevWarBox = warbox;
            warbox = warBoxLogic.OptimalWarBox;
            if (prevWarBox != null && prevWarBox.title != warbox.title)
            {
                WarBoxExecutions.ClickExitWarBox();
                return false;
            }
            return true;
        }

        public static void SetWarBoxLogic(WarboxesBase wbLogic)
        {
            warBoxLogic = wbLogic;
        }

        protected override void FirstInit()
        {
            prevWarBox = null;
            warbox = null;
            ActionMessage.SetMessageType(ActionMessage.MessegeTypes.WarBox);
        }

        public override void LastMethod()
        {
            warBoxLogic = null;
        }

        protected override bool IsThisAction()
        {
            int numOfBlackLines = 0;
            int numOfBlackPix = 0;
            int numOfHackiPix = 0;
            for (int x = 422; x < 471; x++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, 475];
                if (Helpers.Less(pix, 40))
                {
                    numOfBlackPix++;
                }
                else
                {
                    if (numOfBlackPix >= 2 && numOfBlackPix <= 4)
                    {
                        numOfBlackLines++;
                    }
                    numOfBlackPix = 0;
                    if (numOfBlackLines == 1)
                    {
                        if (Helpers.Between(pix, 104, 97, 77, 15))
                        {
                            numOfHackiPix++;
                        }
                    }
                }
            }
            return numOfBlackLines == 2 && numOfHackiPix > 3;
        }
    }
}
