using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionLobby : SomeActionBase
    {
        public enum LobbyState { NotPVPMode, PVPModeNotStandart, PVPModeStandart, CreateNewRoom, InRoom, CreatePlayer, FrozenContracts}
        LobbyState lobbyState;
        LobbyState prevLobbyState = LobbyState.CreatePlayer;
        private bool selectedRounds = false;
        DateTime awaitingInRoom;
        DateTime canStartedGame;
        DateTime lastFriendInviteTime = DateTime.MinValue;

        override protected void FirstInit()
        {
            awaitingInRoom = DateTime.UtcNow;
            selectedRounds = false;
            prevLobbyState = LobbyState.CreatePlayer;
            canStartedGame = DateTime.MinValue;
        }

        bool HostCreateNewRoom()
        {
            if (prevLobbyState != LobbyState.CreateNewRoom)
            {
                selectedRounds = false;
            }

            if (!LobbyExecution.CheckAndClickOnBlitz())
                return true; // если не кликнуто выходим из метода and крутим главный цикл

            if (!LobbyExecution.CheckAndClickOnVilla())
                return true; // если не кликнуто выходим из метода and крутим главный цикл

            if (!LobbyExecution.CheckAndClickOnFreeConnection())
                return true; // если не кликнуто выходим из метода and крутим главный цикл

            if (!LobbyExecution.CheckAndClickOnKickWithHightLatency())
                return true;

            if (!LobbyExecution.CheckAndClickOnAllowConnect())
                return true;

            if (!LobbyExecution.CheckAndClickOnAutoBalance())
                return true;

            if (!LobbyExecution.CheckAndClickOnOvertime())
                return true;

            if (!selectedRounds)
            {
                LobbyExecution.ClickOnRounds();
                selectedRounds = true;
            }

            LobbyExecution.ClickAcceptAndCreateRoom();
            return false;
        }

        bool HostAction()
        {
            switch (lobbyState)
            {
                case LobbyState.PVPModeStandart:
                    LobbyExecution.ClickOnCreateRoom();
                    break;
                case LobbyState.CreateNewRoom:
                    if (HostCreateNewRoom()) return true;
                    break;
                case LobbyState.InRoom:
                    #region InRoom
                    if (prevLobbyState != LobbyState.InRoom)
                        awaitingInRoom = DateTime.UtcNow;

                    if (awaitingInRoom.AddMinutes(3) < DateTime.UtcNow)
                    {
                        // кликнем на настройки комнаты, чтобы изменить состояние лобби, чтобы не выкинуло, заодно проверит не зависла ли игра
                        LobbyExecution.ClickOnRoomSettings(); 
                        break;
                    }

                    int readyPlayers = LobbyChecker.GetReadyPlayers();

                    int playersWhoCanBeInvited = LobbyExecution.CountAndInvitingPlayers();

                    AddToMsg($"readyPlayers: {readyPlayers}\r\nplayersWhoCanBeInvited: {playersWhoCanBeInvited}");

                    if (readyPlayers > 2) // если готовых игроков более 2-ух
                    {
                        LobbyExecution.ClickOnStartGame();
                        //if (canStartedGame == DateTime.MinValue)
                        //    canStartedGame = DateTime.UtcNow;
                        //// если кол-во игроков равняется количеству готовых, или уже ждем более минуты, то начинаем игру
                        //if (playersWhoCanBeInvited == readyPlayers || canStartedGame.AddMinutes(1) < DateTime.UtcNow)
                        //    LobbyExecution.ClickOnStartGame();
                    }
                    else
                        canStartedGame = DateTime.MinValue;

                    LobbyExecution.RandomMouseMove();
                    #endregion
                    break;
                default:
                    break;
            }

            return true;
        }

        bool NotHostAction()
        {
            if (lobbyState == LobbyState.CreateNewRoom)
            {
                LobbyExecution.ClickCancelCreateRoom();
            }
            else
            {
                if (prevLobbyState == LobbyState.CreateNewRoom) // если мы до этого были в создании комнаты, то нужно выйти
                {
                    LobbyExecution.PressExitFromLobby();
                    return false;
                }

                // SomeLobbyAction
                // проверить есть ли в друзьях игрок (хост) онлайн
                if (LobbyExecution.CountAndInvitingPlayers(false) == 0 || !Global.userLogic.GetUserInfo().SendedInviteToHost)
                {
                    if (lastFriendInviteTime == DateTime.MinValue ||  lastFriendInviteTime.AddSeconds(30) < DateTime.UtcNow)
                    {
                        lastFriendInviteTime = DateTime.UtcNow;
                        LobbyExecution.PressAddFriend();
                        return false;
                    }
                    else
                        return true;
                }                

                if (lobbyState == LobbyState.InRoom) // Если в комнате, жмем готов
                {
                    // проверям какая кнопка в комнате, натсройки или не готов, если настройки, то выходим из комнаты
                    if (!LobbyChecker.IsRoomHaveSettingsButton())
                    {
                        LobbyExecution.PressExitFromLobby();
                        return false;
                    }

                    // wbLogic

                    if (LobbyChecker.CanPressReadyOrConnect())
                        LobbyExecution.ClickOnReadyOrConnect();

                    LobbyExecution.RandomMouseMove();
                    return true;
                }

                // кликаем на место кнопки создания комнаты
                if (inThisModeTime.AddMinutes(2) < DateTime.UtcNow)
                    LobbyExecution.ClickOnCreateRoom();
            }
            return true;
        }

        protected override bool DoAction()
        {
            if (lobbyState == LobbyState.FrozenContracts)
            {
                LobbyExecution.MouseMoveNearContracts();
            }
            else if (lobbyState == LobbyState.CreatePlayer)
            {
                LobbyExecution.CreatePlayer();
            }
            else if (lobbyState == LobbyState.NotPVPMode)
            {
                LobbyExecution.ClickOnPVPMode();
            }
            else if (lobbyState == LobbyState.PVPModeNotStandart)
            {
                LobbyExecution.ClickOnPVPModeStandart();
            }
            else
            {
                if (lobbyState != LobbyState.CreateNewRoom)
                {
                    // обновить валюты если не обновленны
                    if (!Global.userLogic.GetUserInfo().CurrenciesChecked)
                        Global.userLogic.UpdateUserCurrenciesByGameScreen();

                    // активировать випки если надо и если есть, так же если задано в настройках
                    if (Global.settingsMain.activateVips && !Global.userLogic.GetUserInfo().VipsChecked)
                        Global.userLogic.CheckAndGetVips();

                    // сменить пользователя и выйти из игры если достигли нужного ранга
                    if (Global.userLogic.GetUserInfo().InGameInfo.rank_id >= Global.settingsMain.neededRank)
                    {
                        Global.userLogic.ChangeUserInfoAndCheckAuthorize();
                        StopGame();
                    }
                }

                if (Global.settingsMain.isHost) // Если лидер комнаты
                {
                    if (!HostAction())
                        return false;
                }
                else // Если НЕ лидер
                {
                    if (!NotHostAction())
                        return false;
                }
            }
            return true;
        }

        override protected bool ExitGameIfIDLE()
        {
            if (base.ExitGameIfIDLE())
                return true;
            if (prevLobbyState != lobbyState)
            {
                inThisModeTime = DateTime.UtcNow;
            }
            return StopGameInLongIDLE();
        }

        protected override bool IsThisAction()
        {
            prevLobbyState = lobbyState;
            //if (LobbyChecker.CheckLobbyCreatePlayer(ref lobbyState))
            //    return true;
            if (LobbyChecker.CheckLobbyNotPVPModeOrCreateNewRoom(ref lobbyState))
                return true;
            if (LobbyChecker.CheckLobbyPVPModeNotStandart(ref lobbyState))
                return true;
            if (LobbyChecker.CheckLobbyPVPModeStandart(ref lobbyState))
                return true;
            if (LobbyChecker.CheckLobbyInRoom(ref lobbyState/*, ref curAutoRoom*/))
                return true;
            if (LobbyChecker.CheckLobbyInRoomNoAuto(ref lobbyState/*, ref curAutoRoom*/))
                return true;
            if (LobbyChecker.CheckLobbyFrozenContracts(ref lobbyState/*, ref curAutoRoom*/))
                return true;

            return false;
        }
    }
}
