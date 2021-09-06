using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionMessage : SomeActionBase
    {
        public enum MessegeTypes { Default, WarBox }
        public static MessegeTypes MsgType { get; protected set; } = MessegeTypes.Default;
        public enum MessageState
        {
            Ok,
            Close,
            /// <summary>
            /// кнопка закрыть при выборе следующего открываемого предмета
            /// </summary>
            Close2,
            Yes,
            TraininigLater,
            Next
        }
        Dictionary<MessageState, Point> currentMessages;

        MessageExecutions msgExecutions = new MessageExecutions();

        protected override bool DoAction()
        {
            Point messagePoint;
            if (msgExecutions.CheckCanPressAndDoSomeAction(currentMessages, out messagePoint))
            {
                if (messagePoint != Point.Empty)
                {
                    Global.input.MouseMove(messagePoint.X, messagePoint.Y);
                    Global.input.MouseClick();
                }
            }
            return true;
        }

        public static void SetMessageType(MessegeTypes type)
        {
            MsgType = type;
        }

        string GetCurrentMessages()
        {
            string msgs = "";
            foreach (var item in currentMessages)
            {
                msgs += $"{item.Key}; ";
            }
            return msgs;
        }

        override protected void ActionMsg()
        {
            Global.UIdata.MessagesUI.AddRecord(new UIMessages($"{GetClassName()} | {GetCurrentMessages()}"));
        }

        bool CheckHorizontalLines(int y1, int y2, int xMsg, Color col)
        {
            int row1 = 0;
            int row2 = 0;
            for (int x = xMsg - 15; x < xMsg + 16; x++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, y1];
                if (Helpers.Between(pix, col, 10))
                    row1++;
                else
                    row1 = 0;

                pix = ActionsWithGameLogic.ScreenOfGame[x, y2];
                if (Helpers.Between(pix, col, 10))
                    row2++;
                else
                    row2 = 0;

                if (row1 > 15 && row2 > 15)
                    return true;
            }
            return false;
        }

        Color msgColor1 = Color.FromArgb(246, 60, 5);
        Color msgColor2 = Color.FromArgb(246, 80, 25);
        Color msgColor3 = Color.FromArgb(246, 80, 20);
        Color msgColor4 = Color.FromArgb(246, 63, 0);
        int xMYes = 282;
        int xMYes2 = 293;
        int xMClose = 503;
        int xMClose2 = 570;
        int xMOk = 380;
        int xMTrainingLater = 682;
        int xNext = 784;
        protected override bool IsThisAction()
        {
            currentMessages = new Dictionary<MessageState, Point>();
            for (int y = 310; y < 575; y++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[xMYes, y];
                if (Helpers.Between(pix, msgColor1, 10))
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMYes, y + 19];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        if (CheckHorizontalLines(y, y + 19, xMYes, msgColor1))
                        {
                            currentMessages[MessageState.Yes] = new Point(xMYes, y + 10);
                        }
                    }
                }

                pix = ActionsWithGameLogic.ScreenOfGame[xMYes2, y];
                if (Helpers.Between(pix, msgColor1, 10) || Helpers.Between(pix, Color.White, 10))
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMYes2, y + 19];
                    if (Helpers.Between(pix, msgColor1, 10) || Helpers.Between(pix, Color.White, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xMYes2 + 110, y + 19];
                        if (Helpers.Between(pix, msgColor1, 10) || Helpers.Between(pix, Color.White, 10))
                        {
                            if (CheckHorizontalLines(y, y + 19, xMYes2, msgColor1) || CheckHorizontalLines(y, y + 19, xMYes2, Color.White))
                            {
                                if (!currentMessages.ContainsKey(MessageState.Yes))
                                    currentMessages[MessageState.Yes] = new Point(xMYes2, y + 10);
                            }
                        }
                    }
                }

                if (y < 520)
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMClose, y];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xMClose, y + 19];
                        if (Helpers.Between(pix, msgColor1, 10))
                        {
                            // проверка на кнопки слайдера в главном меню
                            bool isSlider = true;

                            for (int sx = 395; sx < 406; sx++)
                            {
                                pix = ActionsWithGameLogic.ScreenOfGame[sx, y + 2];
                                if (!Helpers.Between(pix, msgColor1, 10) && !Helpers.Between(pix, msgColor2, 10))
                                {
                                    isSlider = false;
                                    break;
                                }
                            }

                            if (isSlider)
                            {
                                for (int sx = 490; sx < 501; sx++)
                                {
                                    pix = ActionsWithGameLogic.ScreenOfGame[sx, y + 2];
                                    if (!Helpers.Between(pix, msgColor1, 10) && !Helpers.Between(pix, msgColor2, 10))
                                    {
                                        isSlider = false;
                                        break;
                                    }
                                }
                            }
                            if (!isSlider)
                            {
                                if (CheckHorizontalLines(y, y + 19, xMClose, msgColor1) || CheckHorizontalLines(y, y + 19, xMClose, msgColor2))
                                {
                                    if (!currentMessages.ContainsKey(MessageState.Close))
                                        currentMessages[MessageState.Close] = new Point(xMClose, y + 10);
                                }
                            }
                        }
                    }
                }

                pix = ActionsWithGameLogic.ScreenOfGame[xMOk, y];
                if (Helpers.Between(pix, msgColor1, 10))
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMOk, y + 19];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        if (CheckHorizontalLines(y, y + 19, xMOk, msgColor1))
                        {
                            if (!currentMessages.ContainsKey(MessageState.Ok))
                                currentMessages[MessageState.Ok] = new Point(xMOk, y + 10);
                        }
                    }
                }
                pix = ActionsWithGameLogic.ScreenOfGame[xMOk, y];
                if (Helpers.Between(pix, msgColor3, 10) || Helpers.Between(pix, msgColor4, 10))
                {
                    for (int i = 1; i < 15; i++)
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xMOk - 10, y + i];
                        if (!Helpers.Between(pix, msgColor3, 10) && !Helpers.Between(pix, msgColor4, 10))
                            break;
                        if (i == 14)
                        {
                            if (CheckHorizontalLines(y, y + i, xMOk, msgColor3) || CheckHorizontalLines(y, y + i, xMOk, msgColor4))
                            {
                                if (!currentMessages.ContainsKey(MessageState.Ok))
                                    currentMessages[MessageState.Ok] = new Point(xMOk, y + 10);
                            }
                        }
                    }
                }

                if (y >= 540)
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMTrainingLater, y];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xMTrainingLater, y + 19];
                        if (Helpers.Between(pix, msgColor1, 10))
                        {
                            if (CheckHorizontalLines(y, y + 19, xMTrainingLater, msgColor1))
                            {
                                if (!currentMessages.ContainsKey(MessageState.TraininigLater))
                                    currentMessages[MessageState.TraininigLater] = new Point(xMTrainingLater, y + 10);
                            }
                        }
                    }

                    pix = ActionsWithGameLogic.ScreenOfGame[xNext, y];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xNext, y + 19];
                        if (Helpers.Between(pix, msgColor1, 10))
                        {
                            if (CheckHorizontalLines(y, y + 19, xNext, msgColor1))
                            {
                                if (!currentMessages.ContainsKey(MessageState.Next))
                                    currentMessages[MessageState.Next] = new Point(xNext, y + 10);
                            }
                        }
                    }
                }

                if (y >= 455 && y <= 490)
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[xMClose2, y];
                    if (Helpers.Between(pix, msgColor1, 10))
                    {
                        pix = ActionsWithGameLogic.ScreenOfGame[xMClose2, y + 19];
                        if (Helpers.Between(pix, msgColor1, 10))
                        {
                            if (CheckHorizontalLines(y, y + 19, xMClose2, msgColor1))
                            {
                                if (!currentMessages.ContainsKey(MessageState.Close2))
                                    currentMessages[MessageState.Close2] = new Point(xMClose2, y + 10);
                            }
                        }
                    }
                }
            }
            return currentMessages.Count > 0;
        }
    }
}
