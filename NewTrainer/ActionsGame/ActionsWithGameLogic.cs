using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ActionsWithGameLogic
    {
        static Bitmap screen;
        static public ImageWrapper ScreenOfGame { get; private set; }
        static Launcher launcher = new Launcher();

        static IntPtr GetGameCenterHandle()
        {
            return WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
        }

        static IntPtr FindWindowLoop()
        {
            bool runninigWithoutWindow = false;
            DateTime runninigWithoutWindowTime = DateTime.UtcNow;
            IntPtr wfHandle = GetGameCenterHandle();
            DateTime awaitForLaunchGame = DateTime.UtcNow;

            while (wfHandle == IntPtr.Zero)
            {
                if (!IsLaunchedWF())
                {
                    int resLaunch = 0;
                    do
                    {
                        var userInfo = Global.userLogic.GetUserInfo();
                        resLaunch = launcher.LaunchWF(userInfo.Login, userInfo.Password, userInfo.Server - 1);
                    } while (resLaunch == 1);

                    while (!IsLaunchedWF())
                    {
                        if (MainWork.PausedOrPausing())
                            throw new BotPausedException();
                        if (awaitForLaunchGame.AddMinutes(2) <= DateTime.UtcNow)
                            throw new Exception("Game no started after try launch them");
                        System.Threading.Thread.Sleep(5000);
                    }
                    wfHandle = GetGameCenterHandle();
                }
                else
                {
                    if (!runninigWithoutWindow)
                    {
                        runninigWithoutWindow = true;
                        runninigWithoutWindowTime = DateTime.UtcNow;
                    }
                    else
                    {
                        if (runninigWithoutWindowTime.AddMinutes(2) < DateTime.UtcNow)
                            throw new Exception("Can't find warface windows, but game is running");
                    }
                    wfHandle = GetGameCenterHandle();
                }
                //throw new Exception("Can't find warface windows, but game is running");
                if (MainWork.PausedOrPausing())
                    throw new BotPausedException();
                System.Threading.Thread.Sleep(1000);
            }

            return wfHandle;
        }

        static bool prevWfOnTop = false;
        private static readonly object LockerScreen = new object();
        static public void RefreshScreenOnGame()
        {
            lock (LockerScreen)
            {
                IntPtr wfHandle = FindWindowLoop();

                // Make Calculator the foreground application and send it 
                // a set of calculations.
                WinApi.ShowWindow(wfHandle, 9); //9 - restore
                WinApi.BringWindowToTop(wfHandle);
                if (Global.settingsMain.wfForeGround)
                    WinApi.SetForegroundWindow(wfHandle);

                while (wfHandle != WinApi.GetForegroundWindow()) // Видимо тут меняется ебаный хэнд окна...
                {
                    if (MainWork.PausedOrPausing())
                        throw new BotPausedException();
                    wfHandle = FindWindowLoop();

                    if (Global.settingsMain.wfForeGround)
                    {
                        WinApi.ShowWindow(wfHandle, 9); //9 - restore
                        WinApi.BringWindowToTop(wfHandle);
                        WinApi.SetForegroundWindow(wfHandle);
                    }
                    System.Threading.Thread.Sleep(400);
                }

                WinApi.RECT lprect = new WinApi.RECT();
                WinApi.GetWindowRect(wfHandle, out lprect);

                if (prevWfOnTop != Global.settingsMain.wfOnTop)
                {
                    if (Global.settingsMain.wfOnTop)
                    {
                        WinApi.SetWindowPos(wfHandle, (IntPtr)WinApi.SpecialWindowHandles.HWND_TOPMOST, lprect.Left, lprect.Top,
                                 lprect.Right - lprect.Left,
                                 lprect.Bottom - lprect.Top,
                     WinApi.SetWindowPosFlags.SWP_SHOWWINDOW);

                    }
                    else
                    {
                        WinApi.SetWindowPos(wfHandle, (IntPtr)WinApi.SpecialWindowHandles.HWND_NOTOPMOST, lprect.Left, lprect.Top,
                                 lprect.Right - lprect.Left,
                                 lprect.Bottom - lprect.Top,
                     WinApi.SetWindowPosFlags.SWP_SHOWWINDOW);
                    }
                    prevWfOnTop = Global.settingsMain.wfOnTop;
                }

                //if (useTab)
                //{
                //    if (WinApi.GetAsyncKeyState(Keys.Tab) == false || lastTabPressed.AddSeconds(2) <= DateTime.UtcNow)
                //    {
                //        lastTabPressed = DateTime.UtcNow;
                //        //richTextBox1.AppendText("tab - fasle\r\n");
                //        input.KeyDown(Input.VirtualKeyCode.VK_TAB);
                //        //InputSimulator.SimulateKeyDown(VirtualKeyCode.TAB);
                //        System.Threading.Thread.Sleep(200);
                //        input.KeyUp(Input.VirtualKeyCode.VK_TAB);
                //        //InputSimulator.SimulateKeyUp(VirtualKeyCode.TAB);
                //        input.KeyDown(Input.VirtualKeyCode.VK_TAB);
                //        //InputSimulator.SimulateKeyDown(VirtualKeyCode.TAB);
                //        System.Threading.Thread.Sleep(200);
                //    }
                //}

                int totalWidth = lprect.Right - lprect.Left;
                int totalHeight = lprect.Bottom - lprect.Top;

                int lishX = (totalWidth - 800) / 2;
                startDisplayOfGame.X = lishX + lprect.Left;

                int lishY = totalHeight - 600 - lishX;
                startDisplayOfGame.Y = lishY + lprect.Top;

                //if (w > 0 && h > 0)
                //{
                //    totalWidth = w;
                //    totalHeight = h;
                //}
                var gameScreenRect = new System.Drawing.Rectangle(startDisplayOfGame.X, startDisplayOfGame.Y, (totalWidth < 800 ? totalWidth : 800), (totalHeight < 600 ? totalHeight : 600));
                if (ScreenOfGame != null) ScreenOfGame.Dispose();
                if (screen != null) screen.Dispose();
                screen = GetScreenImage(gameScreenRect);
                ScreenOfGame = new ImageWrapper(screen);
            }
        }

        static public void TestingSetImage(Bitmap img)
        {
            ScreenOfGame = new ImageWrapper(img);
        }

        static public void SaveImageIW(string folder = null)
        {
            if (ScreenOfGame == null)
                return;
            if (String.IsNullOrEmpty(folder))
                folder = "tmp";
            string fullPath = Path.Combine(folder, DateTime.Now.ToString("dd.MM.yyyy"));
            Directory.CreateDirectory(fullPath);
            string plLogin = Global.userLogic.GetNoSetUserInfo() == null ? "unknown" : Global.userLogic.GetUserInfo().Login.Replace("@", "(at)");
            string fileName = $"{plLogin} {DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss.fff")}";
            fullPath = Path.Combine(fullPath, fileName);
            ScreenOfGame.SaveToFile(fullPath, false);
        }

        static Point startDisplayOfGame = new Point();
        static Bitmap GetScreenImage(Rectangle rect)
        {
            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            return bmp;
        }

        static public bool IsLaunchedWF()
        {
            bool launchedHss = false;
            Process[] pr2 = Process.GetProcesses();
            for (int i = 0; i < pr2.Length; i++)
            {
                if (pr2[i].ProcessName == "Game")
                {
                    launchedHss = true;
                    break;
                }
            }
            return launchedHss;
        }

        public static void StopGame()
        {
            while (IsLaunchedWF())
            {
                Process[] procs = Process.GetProcessesByName("Game");
                foreach (Process process in procs)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                    }
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
