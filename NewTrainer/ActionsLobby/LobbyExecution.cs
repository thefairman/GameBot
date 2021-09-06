using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class LobbyExecution
    {
        static Color pix;
        public static void CreatePlayer()
        {
            Global.input.MouseMove(525, 440);
            Global.input.MouseClick();
            Global.input.PasteText(Global.userLogic.GetNick());
            Global.input.MouseMove(530, 492);
            Global.input.MouseClick();
        }

        public static void ClickOnPVPMode()
        {
            //Global.input.MouseMove(411, 61);
            Global.input.MouseMove(371, 61); // cyndicate
            Global.input.MouseClick();
        }

        public static void ClickOnPVPModeStandart()
        {
            Global.input.MouseMove(438, 93);
            Global.input.MouseClick();
        }

        public static void ClickOnCreateRoom()
        {
            Global.input.MouseMove(556, 397);
            Global.input.MouseClick();
        }

        public static void ClickOnRoomSettings()
        {
            Global.input.MouseMove(609, 427);
            Global.input.MouseClick();
        }

        public static bool CheckAndClickOnBlitz()
        {
            for (int x = 188; x < 199; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, 337];
                if (Helpers.Between(pix, 251, 67, 1, 10))
                {
                    return true;
                }
            }
            Global.input.MouseMove(168, 337);
            Global.input.MouseClick();
            return false;
        }

        public static bool CheckAndClickOnVilla()
        {
            for (int x = 418; x < 424; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, 130];
                if (Helpers.Between(pix, 254, 80, 17, 15))
                {
                    return true;
                }
            }
            Global.input.MouseMove(310, 130);
            Global.input.MouseClick();
            return false;
        }

        static bool CheckAndClickOnCBInCreatingRoom(int yCoord, bool firstRow, bool ShouldBeSelected)
        {
            bool selected = false;
            int xStart = firstRow ? 453 : 603;
            for (int x = xStart; x < xStart + 4; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, yCoord];
                if (!Helpers.Between(pix, 2, 2, 2, 15))
                {
                    selected = true;
                    break;
                }
            }
            if (selected && !ShouldBeSelected)
            {
                Global.input.MouseMove(xStart, yCoord);
                Global.input.MouseClick();
                return false;
            }
            return true;
        }

        internal static bool CheckAndClickOnFreeConnection()
        {
            return CheckAndClickOnCBInCreatingRoom(263, true, true);
        }

        internal static bool CheckAndClickOnKickWithHightLatency()
        {
            return CheckAndClickOnCBInCreatingRoom(307, true, false);
        }

        internal static bool CheckAndClickOnAllowConnect()
        {
            return CheckAndClickOnCBInCreatingRoom(241, false, false);
        }

        internal static bool CheckAndClickOnAutoBalance()
        {
            return CheckAndClickOnCBInCreatingRoom(263, false, true);
        }

        internal static bool CheckAndClickOnOvertime()
        {
            return CheckAndClickOnCBInCreatingRoom(307, false, false);
        }

        public static void ClickOnRounds()
        {
            Global.input.MouseMove(497, 353);
            Global.input.MouseClick();
            Global.input.MouseMove(497, 399);
            System.Threading.Thread.Sleep(150);
            Global.input.MouseClick();
        }

        public static void ClickAcceptAndCreateRoom()
        {
            Global.input.MouseMove(655, 396);
            Global.input.MouseClick();
        }

        static bool CanBeInvited(int yCoord, bool justOnline = false)
        {
            bool isEmtpy = true;
            for (int x = 535; x < 542; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, yCoord];
                if (Helpers.MoreAny(pix, 40))
                {
                    isEmtpy = false;
                    break;
                }
            }
            if (isEmtpy)
                return false;
            for (int x = 538; x < 542; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, yCoord];
                // если игрок играет, то у него будет красно-оранжевая кнопка, значит он не приглашаемый
                if (!justOnline && Helpers.Between(pix, 224, 61, 2, 31))
                    return false;
                if (!Helpers.Between(pix, 78, 78, 78, 10))
                {
                    return true;
                }
            }
            return false;
        }

        static DateTime lastInviteTime = DateTime.UtcNow;
        public static int CountAndInvitingPlayers(bool invite = true)
        {
            int playersCouldBeInvited = 0;
            bool lastInviteTimeOk = lastInviteTime.AddSeconds(10) <= DateTime.UtcNow;
            for (int i = 0; i < 7; i++)
            {
                int yCoord = 462 + i * 15;
                if (CanBeInvited(yCoord, !invite))
                {
                    playersCouldBeInvited++;
                    if (invite && lastInviteTimeOk)
                    {
                        Global.input.MouseMove(679, yCoord);
                        System.Threading.Thread.Sleep(250);
                        Global.input.MouseClick(200);
                        System.Threading.Thread.Sleep(150);
                        lastInviteTime = DateTime.UtcNow;
                    }
                }
            }
            return playersCouldBeInvited;
        }

        public static void ClickOnStartGame()
        {
            Global.input.MouseMove(754, 398);
            Global.input.MouseClick();
        }

        static DateTime lastReadyClickTime = DateTime.MinValue;
        public static void ClickOnReadyOrConnect()
        {
            if (lastReadyClickTime.AddSeconds(3) < DateTime.UtcNow)
            {
                ClickOnStartGame();
                lastReadyClickTime = DateTime.UtcNow;
            }
        }

        static DateTime lastMoveInRoom = DateTime.UtcNow;

        internal static void MouseMoveNearContracts()
        {
            Random rnd = new Random();
            Global.input.MouseMove(rnd.Next(120, 140), rnd.Next(85, 110));
        }

        public static void RandomMouseMove()
        {
            if (lastMoveInRoom.AddSeconds(30) < DateTime.UtcNow)
            {
                Random rnd = new Random();
                Global.input.MouseMove(rnd.Next(350, 450), rnd.Next(250, 350));
                lastMoveInRoom = DateTime.UtcNow;
            }
        }

        public static void ClickCancelCreateRoom()
        {
            Global.input.MouseMove(762, 398);
            Global.input.MouseClick();
        }

        public static void PressExitFromLobby()
        {
            Global.input.MouseMove(182, 14);
            Global.input.MouseClick();
        }

        public static void PressAddFriend()
        {
            Global.input.MouseMove(765, 440);
            Global.input.MouseClick();
        }
    }
}
