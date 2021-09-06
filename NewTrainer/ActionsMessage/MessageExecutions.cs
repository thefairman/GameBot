using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NewTrainer.ActionMessage;

namespace NewTrainer
{
    class MessageExecutions
    {
        Dictionary<MessegeTypes, List<KeyValuePair<MessageState, Func<bool>>>> rowsExecutions =
            new Dictionary<MessegeTypes, List<KeyValuePair<MessageState, Func<bool>>>>();
        public MessageExecutions()
        {
            List<KeyValuePair<MessageState, Func<bool>>> orderExecutions = new List<KeyValuePair<MessageState, Func<bool>>>();
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.Yes, DefaultAction));
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.Close, DefaultAction));
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.Ok, DefaultAction));
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.TraininigLater, DefaultAction));
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.Next, NextAction));
            orderExecutions.Add(new KeyValuePair<MessageState, Func<bool>>(MessageState.Close2, DefaultAction));

            rowsExecutions[MessegeTypes.Default] = orderExecutions;
        }

        MessageState lastProcessedMessage;
        public bool CheckCanPressAndDoSomeAction(Dictionary<MessageState, Point> messages, out Point point)
        {
            foreach (var item in rowsExecutions[ActionMessage.MsgType])
            {
                if (messages.ContainsKey(item.Key))
                {
                    point = messages[item.Key];
                    lastProcessedMessage = item.Key;
                    return item.Value();
                }
            }
            point = Point.Empty;
            return false;
        }

        bool DefaultAction()
        {
            return true;
        }

        bool NextAction()
        {
            if (lastProcessedMessage != MessageState.Next)
            {
                Global.userLogic.ExperienceShouldBeChanged = true;
            }
            if (Global.settingsMain.saveScoreScreen && Global.settingsMain.isHost)
                ActionsWithGameLogic.SaveImageIW("score");
            return true;
        }
    }
}
