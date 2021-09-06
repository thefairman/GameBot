using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionShop : SomeActionBase
    {
        bool OnWarBoxes = false;
        WarboxesBase warBoxLogic = null;
        Point lastCheckedWarBox = Point.Empty;
        int numOfMatchBox = 0;
        protected override bool DoAction()
        {
            if (OnWarBoxes)
            {
                Warbox warbox = warBoxLogic.OptimalWarBox;
                if (warbox == null)
                {
                    LobbyExecution.PressExitFromLobby();
                    return false;
                }
                Point selectedWarBox = ShopChecker.FindSelectedWarBox();
                if (selectedWarBox == Point.Empty)
                {
                    ShopExecutions.ClickOnFirstWarBox();
                    Thread.Sleep(500);
                    return true;
                }
                if (selectedWarBox == lastCheckedWarBox)
                {
                    if (!ShopExecutions.ClickOnNextWarBox(selectedWarBox))
                    {
                        warBoxLogic.NotFoundNeedBox();
                        LobbyExecution.PressExitFromLobby();
                        return false;
                    }
                    Thread.Sleep(500);
                    return true;
                }
                else
                {
                    lastCheckedWarBox = selectedWarBox;
                    WFCurrency currency;
                    int cost = Global.wfDigits.GetCostOfWarBox(out currency);
                    if (currency == warbox.currency && cost == warbox.firstBoxPrice)
                    {
                        numOfMatchBox++;
                        if (numOfMatchBox == warbox.numInShop)
                        {

                            ShopExecutions.ClickToEnterOnWarBox(selectedWarBox, warBoxLogic);
                            Thread.Sleep(500);
                            return false;
                        }
                    }
                }
            }
            else
            {
                ShopExecutions.ClickOnWarBoxes();
            }
            return true;
        }

        protected override void FirstInit()
        {
            numOfMatchBox = 0;
            lastCheckedWarBox = Point.Empty;

            if (Global.userLogic.GetUserInfo().InGameInfo.rank_id < Global.settingsMain.neededRank)
                warBoxLogic = Global.warboxesForLevelUp;
            else
                warBoxLogic = Global.warBoxesForDonate;
        }

        protected override bool IsThisAction()
        {
            bool thisAct = false;
            for (int i = 25; i < 36; i++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[320, i];
                if (Helpers.Between(pix, 246, 62, 5, 10))
                {
                    thisAct = true;
                    break;
                }
            }
            if (!thisAct)
                return false;

            OnWarBoxes = false;
            for (int x = 395; x < 400; x++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, 92];
                if (Helpers.Between(pix, 251, 61, 5, 10))
                {
                    OnWarBoxes = true;
                    break;
                }
            }

            return true;
        }
    }
}
