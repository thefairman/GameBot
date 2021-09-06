using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace NewTrainer
{
    public enum VirtualKeyCode { VK_A, VK_S, VK_D, VK_W, VK_E, VK_G, VK_TAB, VK_RSHIFT, VK_RCTRL, VK_4, VK_ESCAPE, VK_BACK }

    public class Input
    {
        //public enum VirtualKeyCode { VK_A = 0x41, VK_S = 0x53, VK_D = 0x44, VK_W = 0x57, VK_TAB = 0x09, VK_RSHIFT = 0xA0, VK_RCTRL = 0xa2 }
        //          enum ScanKeyCode { VK_A = 0x1e, VK_S = 0x1f, VK_D = 0x20, VK_W = 0x11, VK_TAB = 0x0f, VK_RSHIFT = 0x2a, VK_RCTRL = 0x1d }

        public static Dictionary<VirtualKeyCode, KeyValuePair<byte, byte>> virtulaKeyCodes = new Dictionary<VirtualKeyCode, KeyValuePair<byte, byte>>();

        private const UInt32 KEYEVENTF_KEYDOWN = 0;
        private const UInt32 KEYEVENTF_EXTENDEDKEY = 1;
        private const UInt32 KEYEVENTF_KEYUP = 2;

        bool wfIsForeGround = true;
        Point startDisplayOfGame = new Point();
        System.Timers.Timer timerCheckWfForeGround = null;
        System.Timers.Timer timerCheckWfForeGroundAndDisplayPos = null;
        System.Timers.Timer timerMoveTowardsWithRandomYInGame = null;
        int checkTime;
        public Input(int checkTime = 251)
        {
            FillKeyCodes();
            this.checkTime = checkTime;
            timerCheckWfForeGround = new System.Timers.Timer(checkTime);
            timerCheckWfForeGround.Elapsed += TimerCheckWfForeGround_Elapsed;

            timerCheckWfForeGroundAndDisplayPos = new System.Timers.Timer(checkTime);
            timerCheckWfForeGroundAndDisplayPos.Elapsed += TimerCheckWfForeGroundAndDisplayPos_Elapsed;

            timerMoveTowardsWithRandomYInGame = new System.Timers.Timer(1);
            timerMoveTowardsWithRandomYInGame.Elapsed += timerMoveTowardsWithRandomYInGame_Elapsed;

            timerMoveTowardsWithRandomYInGame.Start();
        }

        int moveTowawrd;
        bool minusToward;
        Random rnd = new Random();
        DateTime mouseTowardsAwaitingTime = DateTime.UtcNow;
        private void timerMoveTowardsWithRandomYInGame_Elapsed(object sender, ElapsedEventArgs e)
        {
            //timerMoveTowardsWithRandomYInGame.Stop();
            if (moveTowawrd == 0)
            {
                return;
            }

            if (mouseTowardsAwaitingTime > DateTime.UtcNow)
                return;

            wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
            if (wfHandle == IntPtr.Zero || wfHandle != WinApi.GetForegroundWindow()) return;

            int tmpX = rnd.Next(1, 25);
            int dirx = tmpX > moveTowawrd ? moveTowawrd : tmpX;
            moveTowawrd -= dirx;

            int diry = 0;
            int tmpY = rnd.Next(1, 10);
            if (tmpY <= 4)
                diry = rnd.Next(-7, 10);

            if (dirx != 1)
            {
                if (dirx >= moveTowawrd)
                {
                    mouseTowardsAwaitingTime.AddMilliseconds(20);
                    dirx = 1;
                }
                //if (dirx * 1.1 >= moveTowawrd)
                //{
                //    dirx /= 3;
                //    if (dirx <= 10)
                //        mouseTowardsAwaitingTime.AddMilliseconds(50);
                //}
                //if (dirx == 0)
                //    dirx = 1;
            }
            WinApi.mouse_event(WinApi.MouseFlags.Move, minusToward ? -dirx : dirx, diry, 0, UIntPtr.Zero);
            //WinApi.mouse_event(WinApi.MouseFlags.Move, minusToward ? -moveTowawrd : moveTowawrd, diry, 0, UIntPtr.Zero);
        }

        public void ReleaseButtons(List<VirtualKeyCode> except = null)
        {
            foreach (var item in virtulaKeyCodes)
            {
                if (except!= null && !except.Contains(item.Key)) KeyUp(item.Key);
            }
        }

        public void MoveTowardWithRandomYInGame(int distance)
        {
            mouseTowardsAwaitingTime = DateTime.UtcNow;
            if (distance < 0)
                minusToward = true;
            else
                minusToward = false;
            moveTowawrd = Math.Abs(distance);
        }

        public void ResetMoveTowardWithRandomYInGame()
        {
            moveTowawrd = 0;
        }

        void FillKeyCodes()
        {
            virtulaKeyCodes[VirtualKeyCode.VK_A] = new KeyValuePair<byte, byte>(0x41, 0x1e);
            virtulaKeyCodes[VirtualKeyCode.VK_S] = new KeyValuePair<byte, byte>(0x53, 0x1f);
            virtulaKeyCodes[VirtualKeyCode.VK_D] = new KeyValuePair<byte, byte>(0x44, 0x20);
            virtulaKeyCodes[VirtualKeyCode.VK_W] = new KeyValuePair<byte, byte>(0x57, 0x11);
            virtulaKeyCodes[VirtualKeyCode.VK_E] = new KeyValuePair<byte, byte>(0x45, 0x12);
            virtulaKeyCodes[VirtualKeyCode.VK_G] = new KeyValuePair<byte, byte>(0x47, 0x22);
            virtulaKeyCodes[VirtualKeyCode.VK_TAB] = new KeyValuePair<byte, byte>(0x09, 0x0f);
            virtulaKeyCodes[VirtualKeyCode.VK_RSHIFT] = new KeyValuePair<byte, byte>(0xA0, 0x2a);
            virtulaKeyCodes[VirtualKeyCode.VK_RCTRL] = new KeyValuePair<byte, byte>(0xa2, 0x1d);
            virtulaKeyCodes[VirtualKeyCode.VK_4] = new KeyValuePair<byte, byte>(0x34, 0x05);
            virtulaKeyCodes[VirtualKeyCode.VK_ESCAPE] = new KeyValuePair<byte, byte>(0x1B, 0x01);
            virtulaKeyCodes[VirtualKeyCode.VK_BACK] = new KeyValuePair<byte, byte>(0x08, 0x0e);
        }

        private void TimerCheckWfForeGroundAndDisplayPos_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ForeFroundAndDisplayPos();
        }

        void ForeFroundAndDisplayPos()
        {
            wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
            bool fgrnd = (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());

            wfIsForeGround = fgrnd;

            if (!fgrnd)
            {
                timerCheckWfForeGround.Stop();
                return;
            }
            WinApi.RECT lprect = new WinApi.RECT();
            WinApi.GetWindowRect(wfHandle, out lprect);

            int totalWidth = lprect.Right - lprect.Left;
            int totalHeight = lprect.Bottom - lprect.Top;

            int lishX = (totalWidth - 800) / 2;
            startDisplayOfGame.X = lishX + lprect.Left;

            int lishY = totalHeight - 600 - lishX;
            startDisplayOfGame.Y = lishY + lprect.Top;
        }

        IntPtr wfHandle = IntPtr.Zero;
        bool FWIsForeGround()
        {
            wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
            return (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());
        }

        private void TimerCheckWfForeGround_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
            bool fgrnd = (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());

            wfIsForeGround = fgrnd;

            if (!fgrnd)
            {
                timerCheckWfForeGround.Stop();
                return;
            }
        }

        #region Mouse
        public void MouseMove(int X, int Y)
        {
            ForeFroundAndDisplayPos();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }

            timerCheckWfForeGroundAndDisplayPos.Start();
            DateTime startMoveTime = DateTime.UtcNow;
            Point dst = new Point(X, Y);
            while (Cursor.Position != dst)
            {
                if (startMoveTime.AddSeconds(30) <= DateTime.UtcNow)
                {
                    timerCheckWfForeGroundAndDisplayPos.Stop();
                    break;
                }
                dst.X = X + startDisplayOfGame.X;
                dst.Y = Y + startDisplayOfGame.Y;
                if (dst.X >= Screen.PrimaryScreen.Bounds.Width || dst.Y >= Screen.PrimaryScreen.Bounds.Height)
                {
                    timerCheckWfForeGroundAndDisplayPos.Stop();
                    throw new WfNotForeGroundException("Stopping Input simulate, coordinates of mouse bigges then primary screen");
                }
                Point np = Cursor.Position;
                int dx = dst.X - np.X;
                int dy = dst.Y - np.Y;

                int dirx = dx == 0 ? 0 : (dx > 0 ? (dx > 11 ? 10 : 1) : (dx < 11 ? -10 : -1));
                int diry = dy == 0 ? 0 : (dy > 0 ? (dy > 11 ? 10 : 1) : (dy < 11 ? -10 : -1));

                WinApi.mouse_event(WinApi.MouseFlags.Move, dirx, diry, 0, UIntPtr.Zero);

                System.Threading.Thread.Sleep(1);
                if (!wfIsForeGround)
                {
                    timerCheckWfForeGroundAndDisplayPos.Stop();
                    throw new WfNotForeGroundException("Stopping Input simulate");
                }
            }
            timerCheckWfForeGroundAndDisplayPos.Stop();
        }

        public void MouseMoveWithOutChecks(Point startDisplay, int X, int Y)
        {
            DateTime startMoveTime = DateTime.UtcNow;
            Point dst = new Point(X, Y);
            while (Cursor.Position != dst)
            {
                if (startMoveTime.AddSeconds(30) <= DateTime.UtcNow)
                    break;
                dst.X = X + startDisplay.X;
                dst.Y = Y + startDisplay.Y;

                Point np = Cursor.Position;
                int dx = dst.X - np.X;
                int dy = dst.Y - np.Y;

                int dirx = dx == 0 ? 0 : (dx > 0 ? (dx > 11 ? 10 : 1) : (dx < 11 ? -10 : -1));
                int diry = dy == 0 ? 0 : (dy > 0 ? (dy > 11 ? 10 : 1) : (dy < 11 ? -10 : -1));

                WinApi.mouse_event(WinApi.MouseFlags.Move, dirx, diry, 0, UIntPtr.Zero);

                System.Threading.Thread.Sleep(1);
            }
        }

        public void MouseClick(int millisec = 300)
        {
            Thread.Sleep(100);
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            DateTime timeOnClick = DateTime.UtcNow;
            WinApi.mouse_event(WinApi.MouseFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            if (millisec > checkTime)
            {
                timerCheckWfForeGround.Start();
                int resttime = millisec + (int)(timeOnClick - DateTime.UtcNow).TotalMilliseconds;
                while (resttime > 0)
                {
                    if (!wfIsForeGround)
                    {
                        timerCheckWfForeGround.Stop();
                        WinApi.mouse_event(WinApi.MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
                        throw new WfNotForeGroundException("Stopping Input simulate");
                    }
                    resttime = millisec + (int)(timeOnClick - DateTime.UtcNow).TotalMilliseconds;
                    if (resttime > 0)
                        Thread.Sleep(checkTime < resttime ? checkTime : resttime);
                }
                WinApi.mouse_event(WinApi.MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
                timerCheckWfForeGround.Stop();
            }
            else
            {
                Thread.Sleep(millisec);
                WinApi.mouse_event(WinApi.MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
            }
        }

        public void MouseClickWithOutChecks(int millisec = 150)
        {

            WinApi.mouse_event(WinApi.MouseFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(millisec);
            WinApi.mouse_event(WinApi.MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }

        public void MouseDown()
        {
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            WinApi.mouse_event(WinApi.MouseFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
        }

        public void MouseUp()
        {
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            WinApi.mouse_event(WinApi.MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }
        #endregion

        #region KeyBoard

        public void KeyPress(VirtualKeyCode key, int millisec = 150)
        {
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            DateTime timeOnClick = DateTime.UtcNow;
            WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            //InputSimulator.SimulateKeyDown(key);
            //mouse_event(MouseFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            if (millisec > checkTime)
            {
                timerCheckWfForeGround.Start();
                int resttime = millisec + (int)(timeOnClick - DateTime.UtcNow).TotalMilliseconds;
                while (resttime > 0)
                {
                    if (!wfIsForeGround)
                    {
                        timerCheckWfForeGround.Stop();
                        WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYUP, UIntPtr.Zero);
                        //InputSimulator.SimulateKeyUp(key);
                        throw new WfNotForeGroundException("Stopping Input simulate");
                    }
                    resttime = millisec + (int)(timeOnClick - DateTime.UtcNow).TotalMilliseconds;
                    if (resttime > 0)
                        Thread.Sleep(checkTime < resttime ? checkTime : resttime);
                }
                WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYUP, UIntPtr.Zero);
                //InputSimulator.SimulateKeyUp(key);
                //mouse_event(MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
                timerCheckWfForeGround.Stop();
            }
            else
            {
                Thread.Sleep(millisec);
                WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYUP, UIntPtr.Zero);
                //InputSimulator.SimulateKeyUp(key);
                //mouse_event(MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
            }
        }

        public void KeyDown(VirtualKeyCode key)
        {
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            //InputSimulator.SimulateKeyDown(key);
        }

        public void KeyUp(VirtualKeyCode key)
        {
            wfIsForeGround = FWIsForeGround();
            if (!wfIsForeGround)
            {
                throw new WfNotForeGroundException("Stopping Input simulate");
            }
            WinApi.keybd_event(virtulaKeyCodes[key].Key, virtulaKeyCodes[key].Value, KEYEVENTF_KEYUP, UIntPtr.Zero);
            //InputSimulator.SimulateKeyUp(key);
        }

        public void PasteText(string text)
        {
            if (!FWIsForeGround())
                throw new WfNotForeGroundException("FW Is Not ForeGround");
            try
            {
                //активизируем окно, которое имело фокус
                WinApi.SetForegroundWindow(wfHandle);
                int WM_CHAR = 0x0102;
                //передаем ему текст посимвольно
                foreach (char ch in text)
                {
                    WinApi.PostMessage(wfHandle, WM_CHAR, ch, 1);
                    Thread.Sleep(50);
                }
            }
            catch (Exception error)
            {
                throw new WfNotForeGroundException(error.Message);
                //WriteLog(error.Message + " | " + DateTime.Now.ToString(), true);
                //MessageBox.Show(error.Message);
            }
        }

        public void PasteText(IntPtr hControl, string text)
        {
            try
            {
                //активизируем окно, которое имело фокус
                WinApi.SetForegroundWindow(hControl);
                int WM_CHAR = 0x0102;
                //передаем ему текст посимвольно
                foreach (char ch in text)
                {
                    WinApi.PostMessage(hControl, WM_CHAR, ch, 1);
                    Thread.Sleep(1);
                }
            }
            catch (Exception error)
            {
                throw new WfNotForeGroundException(error.Message);
                //WriteLog(error.Message + " | " + DateTime.Now.ToString(), true);
                //MessageBox.Show(error.Message);
            }
        }

        #endregion
    }

    [Serializable()]
    public class WfNotForeGroundException : System.Exception
    {
        public WfNotForeGroundException() : base() { }
        public WfNotForeGroundException(string message) : base(message) { }
        public WfNotForeGroundException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected WfNotForeGroundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
