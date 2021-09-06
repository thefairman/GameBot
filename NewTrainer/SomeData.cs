using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{

    public class WindowData
    {
        public string WindowTitle { get; set; }
        public string WindowClassName { get; set; }
        public WindowData()
        {

        }
        public WindowData(string title, string className)
        {
            WindowTitle = title;
            WindowClassName = className;
        }
    }

    public static class GetWindows
    {
        // Methods
        public static Dictionary<IntPtr, WindowData> WindowsToList(bool notMinimized = true)
        {
            Dictionary<IntPtr, WindowData> list = new Dictionary<IntPtr, WindowData>();
            //List<KeyValuePair<string, IntPtr>> el = new List<KeyValuePair<string, IntPtr>>();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                if (IsWindowVisible(hWnd) && (!notMinimized || !IsIconic(hWnd)) && GetWindowTextLength(hWnd) != 0)
                {
                    StringBuilder sb = new StringBuilder(100);
                    GetClassName(hWnd, sb, sb.Capacity);

                    list[hWnd] = new WindowData(GetWindowText(hWnd), sb.ToString());
                    //el.Add(new KeyValuePair<string, IntPtr>($"{GetWindowText(hWnd)} ({sb})", hWnd));
                }
                return true;
            }, IntPtr.Zero);
            return list;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            StringBuilder sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        // Nested Types
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }

    #region MessagesBlock
    abstract public class UIDataRecordBase
    {
        public DateTime TimeRecord { get; protected set; }
        public UIDataRecordBase()
        {
            TimeRecord = DateTime.Now;
        }
        abstract public List<object> GetListForDGV();
        protected string TimeToFormatString()
        {
            return TimeRecord.ToString("dd.MM.yyyy HH:mm:ss.fff");
        }
    }

    public class UIDataUnitBase
    {
        BlockingCollection<UIDataRecordBase> messages = new BlockingCollection<UIDataRecordBase>();
        public void AddRecord(UIDataRecordBase record)
        {
            messages.Add(record);
        }
        public List<UIDataRecordBase> GetListOfMessages()
        {
            List<UIDataRecordBase> lst = new List<UIDataRecordBase>();
            UIDataRecordBase tmpMsg;
            while (messages.TryTake(out tmpMsg, 10))
            {
                lst.Add(tmpMsg);
            }
            return lst;
        }
    }

    public class UIMessages : UIDataRecordBase
    {
        string message;
        public UIMessages(string message)
        {
            this.message = message;
        }
        public override List<object> GetListForDGV()
        {
            List<object> list = new List<object>();
            list.Add(message);
            list.Add(TimeToFormatString());
            return list;
        }
    }

    public class UIData
    {
        public List<UIDataRecordBase> MessagesUI { get; set; }
        public List<UIDataRecordBase> ErrororsUI { get; set; }
        public double ruCaptchaBalance;
    }

    public class UIDataMain
    {
        public UIDataUnitBase MessagesUI { get; protected set; } = new UIDataUnitBase();
        public UIDataUnitBase ErrororsUI { get; protected set; } = new UIDataUnitBase();
        public double ruCaptchaBalance = -2;

        public UIData GetUIData()
        {
            return new UIData
            {
                MessagesUI = MessagesUI.GetListOfMessages(),
                ErrororsUI = ErrororsUI.GetListOfMessages(),
                ruCaptchaBalance = ruCaptchaBalance
            };
        }
    }
    #endregion

    class BotPausedException : Exception
    {
        public BotPausedException(string message = "Bot is paused")
            : base(message)
        { }
    }
}
