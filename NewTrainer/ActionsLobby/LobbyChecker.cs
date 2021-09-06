using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    static class LobbyChecker
    {
        static Color pix;
        public static bool CheckLobbyNotPVPModeOrCreateNewRoom(ref ActionLobby.LobbyState lobbyState)
        {
            if (true || Global.userLogic.GetUserInfo().InGameInfo.rank_id >= 25)
            {
                bool spec = false;
                for (int y = 71; y < 84; y++)
                {
                    //pix = ActionsWithGameLogic.ScreenOfGame[261, y]; // pve
                    pix = ActionsWithGameLogic.ScreenOfGame[221, y]; // pve syndicate
                    if (Helpers.Between(pix, 220, 60, 5, 5))
                    {
                        lobbyState = ActionLobby.LobbyState.NotPVPMode;
                        return true;
                    }
                    pix = ActionsWithGameLogic.ScreenOfGame[338, y]; // spec
                    if (Helpers.Between(pix, 220, 60, 5, 5))
                    {
                        lobbyState = ActionLobby.LobbyState.NotPVPMode;
                        spec = true;

                    }
                    pix = ActionsWithGameLogic.ScreenOfGame[493, y]; // mm
                    if (Helpers.Between(pix, 220, 60, 5, 5))
                    {
                        lobbyState = ActionLobby.LobbyState.NotPVPMode;
                        return true;
                    }
                }
                if (spec)
                {
                    for (int y = 71; y < 84; y++)
                    {
                        Color pix = ActionsWithGameLogic.ScreenOfGame[345, y]; // new room
                        if (Helpers.Between(pix, 220, 60, 5, 5))
                        {
                            lobbyState = ActionLobby.LobbyState.CreateNewRoom;
                            break;
                        }
                    }
                    return true;
                }
            }
            else
            {
                bool pve = false;
                for (int y = 71; y < 84; y++)
                {
                    Color pix = ActionsWithGameLogic.ScreenOfGame[326, y]; // pve
                    if (Helpers.Between(pix, 220, 60, 5, 5))
                    {
                        lobbyState = ActionLobby.LobbyState.NotPVPMode;
                        pve = true;
                    }
                    pix = ActionsWithGameLogic.ScreenOfGame[402, y]; // spec
                    if (Helpers.Between(pix, 220, 60, 5, 5))
                    {
                        lobbyState = ActionLobby.LobbyState.NotPVPMode;
                        return true;
                    }
                }
                if (pve)
                {
                    for (int y = 71; y < 84; y++)
                    {
                        Color pix = ActionsWithGameLogic.ScreenOfGame[345, y]; // new room
                        if (Helpers.Between(pix, 220, 60, 5, 5))
                        {
                            lobbyState = ActionLobby.LobbyState.CreateNewRoom;
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static bool CheckLobbyPVPModeNotStandart(ref ActionLobby.LobbyState lobbyState)
        {
            for (int y = 71; y < 84; y++)
            {
                //pix = ActionsWithGameLogic.ScreenOfGame[411, y]; // pvp
                pix = ActionsWithGameLogic.ScreenOfGame[371, y]; // pvp syndicate
                if (Helpers.Between(pix, 220, 60, 5, 5))
                {
                    for (int y2 = 103; y2 < 115; y2++)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[368, y2];
                        if (Helpers.Between(pix, 222, 63, 5, 5))
                        {
                            lobbyState = ActionLobby.LobbyState.PVPModeNotStandart;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool CheckLobbyPVPModeStandart(ref ActionLobby.LobbyState lobbyState)
        {
            for (int y = 71; y < 84; y++)
            {
                //pix = ActionsWithGameLogic.ScreenOfGame[411, y]; // pvp
                pix = ActionsWithGameLogic.ScreenOfGame[371, y]; // pvp syndicate
                if (Helpers.Between(pix, 220, 60, 5, 5))
                {
                    for (int y2 = 103; y2 < 115; y2++)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[437, y2];
                        if (Helpers.Between(pix, 222, 63, 5, 5))
                        {
                            lobbyState = ActionLobby.LobbyState.PVPModeStandart;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool CheckLobbyInRoom(ref ActionLobby.LobbyState lobbyState/*, ref bool curAutoRoom*/)
        {
            if (IsInRoom())
            {
                lobbyState = ActionLobby.LobbyState.InRoom;
                //curAutoRoom = true;
                return true;
            }
            return false;
        }

        public static bool CheckLobbyInRoomNoAuto(ref ActionLobby.LobbyState lobbyState/*, ref bool curAutoRoom*/)
        {
            if (IsInRoomNoAuto())
            {
                //curAutoRoom = false;
                lobbyState = ActionLobby.LobbyState.InRoom;
                return true;
            }
            return false;
        }

        public static bool CheckLobbyCreatePlayer(ref ActionLobby.LobbyState lobbyState)
        {
            for (int iy = 110; iy < 330; iy++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[350, iy];
                if (Helpers.Between(pix, 246, 62, 5, 10))
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[430, iy];
                    if (Helpers.Between(pix, 246, 62, 5, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[350, iy + 82];
                        if (Helpers.Between(pix, 246, 62, 5, 10))
                        {
                            pix = ActionsWithGameLogic.ScreenOfGame[430, iy + 82];
                            if (Helpers.Between(pix, 246, 62, 5, 10))
                            {
                                lobbyState = ActionLobby.LobbyState.CreatePlayer;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static bool IsInRoom()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                int addX = 41;
                int addX2 = 513;
                int addY = 81;
                int fst = 0;
                int sec = 0;
                for (int y = addY; y < addY + 26; y++)
                {
                    if (fst == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX, y];
                        if (Helpers.Between(pix, 255, 5, 5, 25, 5, 5))
                        {
                            fst = 2;
                        }
                        else if (Helpers.Between(pix, 74, 142, 255, 15, 15, 15))
                        {
                            fst = 1;
                        }
                    }
                    if (sec == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX2, y];
                        if (Helpers.Between(pix, 255, 4, 4, 25, 5, 5))
                        {
                            sec = 2;
                        }
                        else if (Helpers.Between(pix, 74, 142, 255, 15, 15, 15))
                        {
                            sec = 1;
                        }
                    }
                    if (fst != 0 && sec != 0 && fst != sec)
                        return true;
                }
            }

            return false;
        }

        static bool IsInRoomNoAuto()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                int addX = 241;
                int addX2 = 329;
                int addY = 81;
                int fst = 0;
                int sec = 0;
                for (int y = addY; y < addY + 26; y++)
                {
                    if (fst == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX, y];
                        if (Helpers.Between(pix, 255, 5, 5, 25, 5, 5))
                        {
                            fst = 2;
                        }
                        else if (Helpers.Between(pix, 74, 142, 255, 15, 15, 15))
                        {
                            fst = 1;
                        }
                    }
                    if (sec == 0)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[addX2, y];
                        if (Helpers.Between(pix, 255, 4, 4, 25, 5, 5))
                        {
                            sec = 2;
                        }
                        else if (Helpers.Between(pix, 74, 142, 255, 15, 15, 15))
                        {
                            sec = 1;
                        }
                    }
                    if (fst != 0 && sec != 0 && fst != sec)
                        return true;
                }
            }

            return false;
        }

        internal static bool CheckLobbyFrozenContracts(ref ActionLobby.LobbyState lobbyState)
        {
            int numOfButtons = 0;
            int searchedColCount = 0;
            for (int y = 175; y < 415; y++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[80, y];
                if (Helpers.Between(pix, 255, 68, 0, 10))
                    searchedColCount++;
                else
                {
                    if (searchedColCount >= 19)
                        numOfButtons++;
                    searchedColCount = 0;
                }
            }
            return numOfButtons == 3;
        }

        static public int GetReadyPlayers()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                int readyPlayers = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int x1 = 0; x1 < 25; x1++)
                    {
                        bool was = false;
                        for (int y1 = 0; y1 < 6; y1++)
                        {
                            pix = ActionsWithGameLogic.ScreenOfGame[230 + x1, 130 + i * 26 + y1];
                            if (Helpers.Between(pix, 50, 200, 50, 15))
                            {
                                was = true;
                                readyPlayers++;
                                break;
                            }
                        }
                        if (was) break;
                    }

                    for (int x1 = 0; x1 < 25; x1++)
                    {
                        bool was = false;
                        for (int y1 = 0; y1 < 6; y1++)
                        {
                            pix = ActionsWithGameLogic.ScreenOfGame[474 + x1, 130 + i * 26 + y1];
                            if (Helpers.Between(pix, 50, 200, 50, 15))
                            {
                                was = true;
                                readyPlayers++;
                                break;
                            }
                        }
                        if (was) break;
                    }
                }

                return readyPlayers;
            }

            return 0;
        }

        public static bool IsRoomHaveSettingsButton()
        {
            for (int x = 565; x < 576; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, 396];
                if (!Helpers.Between(pix, 245, 68, 0, 15) && Helpers.MoreAny(pix, 100))
                    return false;
            }
            return true;
        }

        public static bool CanPressReadyOrConnect()
        {
            bool haveSomeWhite = false;
            for (int x = 670; x < 680; x++)
            {
                pix = ActionsWithGameLogic.ScreenOfGame[x, 393];
                if (!Helpers.MoreAny(pix, 200))
                    return true;
                if (Helpers.More(pix, 120))
                    haveSomeWhite = true;
            }
            return haveSomeWhite;
        }
    }
}
