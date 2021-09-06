using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionInGame : SomeActionBase
    {
        enum CurrentGameLogic { WarFace, BlackWood, BlackWoodDefuse, Unknown }
        CurrentGameLogic currentTeamLogic = CurrentGameLogic.Unknown;
        int myTeam = 0;
        public static Point PlayerPos { get; private set; } = Point.Empty;
        public static double AngleOnMap { get; private set; } = 0;
        public static double AngleOnMiniMap { get; private set; } = 0;
        public static double AngleOfPlayerView { get; private set; } = 0;
        public static DateTime OnRoundTime { get; private set; } = DateTime.MinValue;
        Dictionary<CurrentGameLogic, ListOfInGameActions> listsOfInGameActions = new Dictionary<CurrentGameLogic, ListOfInGameActions>();
        public ActionInGame()
        {
            listsOfInGameActions[CurrentGameLogic.WarFace] = new ListOfInGameActionsWF();
            listsOfInGameActions[CurrentGameLogic.BlackWood] = new ListOfInGameActionsBW();
            listsOfInGameActions[CurrentGameLogic.BlackWoodDefuse] = new ListOfInGameActionsBWDefuse();
        }

        protected override void FirstInit()
        {
            currentTeamLogic = CurrentGameLogic.Unknown;
        }

        override public void LastMethod()
        {
            InGameExecutions.ReleaseAllButtons();
        }

        protected override bool DoAction()
        {
            InGameExecutions.ResetMouseMove();

            if (!InGameChecker.TabPressed())
            {
                InGameExecutions.RePressTabKey();
                return true;
            }

            if (!InGameChecker.RoundGoOn())
            {
                ResetPositionsAndOther();
                return true;
            }

            if (!InGameChecker.IsAlive())
                return true;

            Point curPlayerPos = InGameChecker.FindCoordOfPlayer();
            if (!InGameChecker.IsNegativeOrEmptyPoint(curPlayerPos))
                PlayerPos = curPlayerPos;

            double curAngleOnMap = InGameChecker.GetAngleOfPlayerOnMap();
            if (curAngleOnMap >= 0)
                AngleOnMap = curAngleOnMap;

            double curAngleOnMiniMap = InGameChecker.GetAngleOfPlayerOnMiniMap();
            if (curAngleOnMiniMap >= 0)
                AngleOnMiniMap = curAngleOnMiniMap;

            AngleOfPlayerView = InGameChecker.GetAngleOfPlayerView();

            if (currentTeamLogic == CurrentGameLogic.Unknown)
            {
                if (PlayerPos.Y > 120)
                    currentTeamLogic = CurrentGameLogic.WarFace;
                else
                {
                    if (InGameChecker.HaveTenRounds())
                        currentTeamLogic = CurrentGameLogic.BlackWoodDefuse;
                    else
                        currentTeamLogic = CurrentGameLogic.BlackWood;
                }
            }

            listsOfInGameActions[currentTeamLogic].DoActions();

            if (Global.settingsMain.showActionMessage && !Global.settingsMain.showActionMessageFirsOnly)
                AddToMsg($"| curPlayerPos: {curPlayerPos}\r\ncurAngleOnMap: {curAngleOnMap}" +
                    $"\r\ncurAngleOnMiniMap: {curAngleOnMiniMap}\r\nAngleOfPlayerView: {AngleOfPlayerView}" +
                    $"\r\ncurrentTeamLogic {currentTeamLogic}\r\nIterator: {listsOfInGameActions[currentTeamLogic].Iterator}" +
                    $"\r\nAngleFromPlayerToDest: {InGameChecker.AngleFromPlayerToDest}" +
                    $"\r\nrotateAngle: {InGameActionsLogic.rotateAngle}" +
                    $"\r\nAngleOfViewAndDestiny: {InGameActionsLogic.AngleOfViewAndDestiny}" +
                    $"\r\ndifrenceWithAngles: {InGameActionsLogic.difrenceWithAngles}");
            return true; 
        }

        void ResetPositionsAndOther()
        {
            //ActionsWithGameLogic.SaveImageIW("noround");
            OnRoundTime = DateTime.UtcNow;
            currentTeamLogic = CurrentGameLogic.Unknown;
            foreach (var item in listsOfInGameActions)
            {
                item.Value.ResetData();
            }
        }

        protected override bool IsThisAction()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                int tolerance = 80 * 80;
                int addX = 329;
                int addX2 = 469;
                int fst = 0;
                int sec = 0;
                Color blackWoodColor = Color.FromArgb(205, 36, 44);
                Color warfaceColor = Color.FromArgb(71, 124, 206);
                for (int y = 20; y < 29; y++)
                {
                    Color pix;
                    if (fst == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX, y];
                        if (Helpers.IsSearchedColor(pix, blackWoodColor, tolerance))
                        {
                            fst = 2;
                        }
                        else if (Helpers.IsSearchedColor(pix, warfaceColor, tolerance))
                        {
                            fst = 1;
                        }
                    }
                    if (sec == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX2, y];
                        if (Helpers.IsSearchedColor(pix, blackWoodColor, tolerance))
                        {
                            sec = 2;
                        }
                        else if (Helpers.IsSearchedColor(pix, warfaceColor, tolerance))
                        {
                            sec = 1;
                        }
                    }

                    if (fst != 0 && sec != 0 && fst != sec)
                    {
                        myTeam = fst;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
