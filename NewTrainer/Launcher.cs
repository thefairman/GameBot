using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace NewTrainer
{
    class Launcher
    {
        public static string[] servers = { "Альфа", "Браво", "Чарли" };
        int server;

        DateTime timeToTryLaunch;

        List<WindowData> needToClose = new List<WindowData>();

        public Launcher()
        {
            needToClose.Add(new WindowData("Ошибка авторизации", "GameCenter.TOkForm"));
            needToClose.Add(new WindowData("Игровой центр - Ошибка", "GameCenter.TOkForm"));
            needToClose.Add(new WindowData("Выполнение операции невозможно", "GameCenter.TOkForm"));
            needToClose.Add(new WindowData("Уведомление безопасности", "GameCenter.TGameWebForm"));
        }

        void ClosePopupWindows()
        {
            foreach (var item in needToClose)
            {
                IntPtr wndws = WinApi.FindWindow(item.WindowClassName, item.WindowTitle);
                if (wndws != IntPtr.Zero)
                    WinApi.CloseWindow(wndws);
            }
        }

        public static void CloseNotAllowedWindows()
        {
            var windows = GetWindows.WindowsToList();
            foreach (var item in windows)
            {
                if (!Global.allowedWindows.Contains(item.Value.WindowTitle, item.Value.WindowClassName))
                    WinApi.CloseWindow(item.Key);
            }
            windows = GetWindows.WindowsToList();
            if (windows.Count <= 0)
                return;
            Thread.Sleep(500);
            windows = GetWindows.WindowsToList();
            foreach (var item in windows)
            {
                uint procId;
                if (!Global.allowedWindows.Contains(item.Value.WindowTitle, item.Value.WindowClassName))
                {
                    WinApi.GetWindowThreadProcessId(item.Key, out procId);
                    if (procId != 0)
                    {
                        Process Id = Process.GetProcessById((int)procId);
                        Id.Kill();
                    }
                }
            }
        }

        Point GetPointActionBtnMain(AutomationElement window)
        {
            var mainBtn = window.FindFirstElementFromAutomationID("ActionBtnMain");
            if (mainBtn != null)
            {
                if (mainBtn.Current.Name == "Скачать")
                {
                    mainBtn.GetPattern<InvokePattern>().Invoke();
                }
                else if (mainBtn.Current.Name == "Играть")
                {
                    int x;
                    if (lprect.Left < 0)
                        x = (int)mainBtn.Current.BoundingRectangle.X + -lprect.Left;
                    else
                        x = (int)mainBtn.Current.BoundingRectangle.X - lprect.Left;
                    int y;
                    if (lprect.Top < 0)
                        y = (int)mainBtn.Current.BoundingRectangle.Y + -lprect.Top;
                    else
                        y = (int)mainBtn.Current.BoundingRectangle.Y - lprect.Top;
                    Thread.Sleep(200);
                    return new Point(x, y);
                }
            }
            Thread.Sleep(200);
            return new Point(-1, -1);
        }


        /// <summary>
        ///  Метод запуска игры через игровой центр
        ///  возврыщаемые значения: 0 - ок; 1 - не удалось авторизоваться; 2 - слишком долго не удалось запуститься; 3 - пользователь нажал на паузу
        /// </summary>
        public int LaunchWF(string login, string password, int server)
        {
            SetServer(server);

            CloseNotAllowedWindows();

            MainWork.LastActionName = "";
            timeToTryLaunch = DateTime.UtcNow;
            bool LoginisOk = false;
            bool ServerIsOk = false;
            int lastMinClosedWindows = 0;
            Point pointActionBtnMain = new Point(-1, -1);
            DateTime sleepTime = DateTime.UtcNow;
            while (true)
            {
                try
                {
                    if (ActionsWithGameLogic.IsLaunchedWF())
                        return 0;

                    if (MainWork.PausedOrPausing())
                        return 3;

                    if (sleepTime.AddMilliseconds(50) > DateTime.UtcNow)
                        Thread.Sleep(50);
                    sleepTime = DateTime.UtcNow;

                    TimeSpan ts = DateTime.UtcNow - timeToTryLaunch;
                    if (ts.TotalMinutes % 2 == 0 && ts.TotalMinutes != lastMinClosedWindows)
                    {
                        CloseNotAllowedWindows();
                        lastMinClosedWindows = (int)ts.TotalMinutes;
                    }

                    IntPtr gcHandle;
                    StartAndSetGCForeGround(out gcHandle);

                    AutomationElement window = null;
                    //if (!LoginisOk || !ServerIsOk)
                    window = AutomationElement.FromHandle(gcHandle);

                    pointActionBtnMain = GetPointActionBtnMain(window);
                    if (pointActionBtnMain.X == -1)
                        continue;

                    if (timeToTryLaunch.AddMinutes(5) <= DateTime.UtcNow)
                        return 2; // слишком долго не удалось запуститься

                    if (!LoginisOk)
                    {
                        int setLoginRes = CheckAndSetLogin(window, login, password);
                        if (setLoginRes == 1)
                            return 1;
                        else if (setLoginRes == 0)
                            LoginisOk = true;
                        else
                            continue;
                    }

                    if (!ServerIsOk)
                    {
                        ServerIsOk = CheckAndSetServer(window);
                        if (!ServerIsOk)
                            continue;
                    }

                    LaunchGame(pointActionBtnMain);
                }
                catch (Exception ex)
                {
                    Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
                }
            }
        }

        void LaunchGame(Point pointActionBtnMain)
        {
            Cursor.Position = new Point(lprect.Left + pointActionBtnMain.X + 2, lprect.Top + pointActionBtnMain.Y + 2);
            Global.input.MouseClickWithOutChecks();
            Thread.Sleep(500);
        }

        IntPtr GetGameCenterHandle()
        {
            return WinApi.FindWindow("GameCenter.TMainForm", "Игровой центр");
        }

        void LauchGameCenter()
        {
            Process.Start("mailrugames://play/0.1177");
        }

        void SetServer(int server)
        {
            if (server < 0)
                this.server = 0;
            if (server > 2)
                this.server = 2;
            this.server = server;
        }

        bool CheckAndSetServer(AutomationElement window)
        {
            DateTime timeToTryingSetServer = DateTime.UtcNow;
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return false;
                if (timeToTryingSetServer.AddSeconds(4) < DateTime.UtcNow)
                    return false;
                var gameShardBtn = window.FindFirstElementFromAutomationID("GameShard");
                if (gameShardBtn.Current.Name == servers[server])
                    return true;

                gameShardBtn.GetPattern<InvokePattern>().Invoke();
                Thread.Sleep(100);

                var shardpopup = window.FindFirstElementFromAutomationID("ShardPopupMenu");
                var serverChoseBtn = shardpopup.FirstDescendantByTypeAndName(ControlType.MenuItem, servers[server]);
                serverChoseBtn.GetPattern<InvokePattern>().Invoke();
                Thread.Sleep(100);

                var confirmChangeServerWindow = window.FirstDescendantByTypeAndName(ControlType.Window, "Игровой центр - Смена сервера");
                var confirmChangeServerBtn = confirmChangeServerWindow.FindFirstElementFromAutomationID("YesButton");
                confirmChangeServerBtn.GetPattern<InvokePattern>().Invoke();
                Thread.Sleep(100);
            }
        }

        private void InsertTextUsingUIAutomation(IntPtr handle, AutomationElement element, string value)
        {
            Cursor.Position = new Point((int)element.Current.BoundingRectangle.X, (int)element.Current.BoundingRectangle.Y);
            //input.MouseMoveEasy((int)userNameBtn.Current.BoundingRectangle.X, (int)userNameBtn.Current.BoundingRectangle.Y);
            Global.input.MouseClickWithOutChecks();
            Thread.Sleep(50);

            // Delete existing content in the control and insert new content.
            SendKeys.SendWait("^{HOME}");   // Move to start of control
            SendKeys.SendWait("^+{END}");   // Select everything
            SendKeys.SendWait("{DEL}");     // Delete selection
            Global.input.PasteText(handle, value);
            //SendKeys.SendWait(/*value*/".");
        }

        int CheckAndSetLogin(AutomationElement window, string login, string password)
        {
            int tryingAutoriz = 0;
            while (true)
            {
                if (MainWork.PausedOrPausing())
                    return 2;
                IntPtr autorizHandle = WinApi.FindWindow("GameCenter.TLoginForm", "Авторизация");
                if (autorizHandle != IntPtr.Zero)
                {
                    var authorizWindow = AutomationElement.FromHandle(autorizHandle);
                    if (authorizWindow != null)
                    {
                        if (tryingAutoriz >= 2)
                            return 1;
                        var loginText = authorizWindow.FindFirstElementFromAutomationID("Login");
                        InsertTextUsingUIAutomation(autorizHandle, loginText, login);

                        var passText = authorizWindow.FindFirstElementFromAutomationID("Password");
                        InsertTextUsingUIAutomation(autorizHandle, passText, password);

                        var loginBtn = authorizWindow.FirstDescendantByTypeAndName(ControlType.Button, "Войти");
                        loginBtn.GetPattern<InvokePattern>().Invoke();
                        Thread.Sleep(100);
                        tryingAutoriz++;
                        continue;
                    }
                }

                var userNameBtn = window.FindFirstElementFromAutomationID("UserNameBtn");
                if (userNameBtn.Current.Name == "Войти")
                {
                    IntPtr tmp;
                    if (!StartAndSetGCForeGround(out tmp))
                        return 2;
                    userNameBtn = window.FindFirstElementFromAutomationID("UserNameBtn");
                    Cursor.Position = new Point((int)userNameBtn.Current.BoundingRectangle.X, (int)userNameBtn.Current.BoundingRectangle.Y);
                    //input.MouseMoveEasy((int)userNameBtn.Current.BoundingRectangle.X, (int)userNameBtn.Current.BoundingRectangle.Y);
                    Global.input.MouseClickWithOutChecks();

                    continue;
                }
                userNameBtn.GetPattern<InvokePattern>().Invoke();
                Thread.Sleep(100);


                var TMultiLoginForm = window.FindFirstElementFromAutomationID("TMultiLoginForm");
                var loginEmail = TMultiLoginForm.FindFirstElementFromAutomationID("UserEmail");
                if (loginEmail.Current.Name == login)
                {
                    TMultiLoginForm.GetPattern<WindowPattern>().Close();
                    Thread.Sleep(100);
                    return 0;
                }

                var logOutBtn = TMultiLoginForm.FirstDescendantByTypeAndName(ControlType.Button, "Выйти из всех аккаунтов");
                logOutBtn.GetPattern<InvokePattern>().Invoke();
                Thread.Sleep(100);
            }
        }

        WinApi.RECT lprect;
        bool StartAndSetGCForeGround(out IntPtr gcHandle)
        {
            gcHandle = GetGameCenterHandle();
            DateTime tryLaunchGCTiem = DateTime.UtcNow.AddMinutes(-1);
            while (gcHandle == IntPtr.Zero)
            {
                if (MainWork.PausedOrPausing())
                    return false;
                if (tryLaunchGCTiem.AddSeconds(2) < DateTime.UtcNow)
                {
                    LauchGameCenter();
                    tryLaunchGCTiem = DateTime.UtcNow;
                }
                Thread.Sleep(50);
                gcHandle = GetGameCenterHandle();
            }
            DateTime startSetForeGroundTime = DateTime.UtcNow;
            tryLaunchGCTiem = DateTime.UtcNow;
            ClosePopupWindows();
            WinApi.SwitchToThisWindow(gcHandle, true);
            while (gcHandle != WinApi.GetForegroundWindow() || !WinApi.IsWindowVisible(gcHandle))
            {
                if (MainWork.PausedOrPausing())
                    return false;
                if (!WinApi.IsWindowVisible(gcHandle) && tryLaunchGCTiem.AddSeconds(2) < DateTime.UtcNow)
                {
                    ClosePopupWindows();
                    LauchGameCenter();
                    tryLaunchGCTiem = DateTime.UtcNow;
                }
                if (startSetForeGroundTime.AddSeconds(5) < DateTime.UtcNow)
                    return false;
                WinApi.SwitchToThisWindow(gcHandle, true);
                WinApi.SetForegroundWindow(gcHandle);
                Thread.Sleep(100);
            }

            lprect = new WinApi.RECT();
            WinApi.GetWindowRect(gcHandle, out lprect);

            if (lprect.Right - 140 >= Screen.PrimaryScreen.Bounds.Width || lprect.Top + 597 >= Screen.PrimaryScreen.Bounds.Height ||
                lprect.Right < -935 || lprect.Top < -145 || lprect.Right - lprect.Left > 1065)
            {
                int left = Screen.PrimaryScreen.Bounds.Width >= 1065/*(lprect.Right - lprect.Left)*/ - 130 ? 1 : Screen.PrimaryScreen.Bounds.Width - 1065/*(lprect.Right - lprect.Left)*/ + 130;
                int top = Screen.PrimaryScreen.Bounds.Height > 599 ? 1 : -145;
                WinApi.MoveWindow(gcHandle, left, top, 1020, 625, true);
                WinApi.GetWindowRect(gcHandle, out lprect);
            }
            return true;
        }
    }

    static class AutomationHelpers
    {
        static public T GetPattern<T>(this AutomationElement element)
            where T : BasePattern
        {
            var pattern = (AutomationPattern)typeof(T).GetField("Pattern").GetValue(null);
            return (T)element.GetCurrentPattern(pattern);
        }

        static public AutomationElement FirstChildByType(
            this AutomationElement element, ControlType ct)
        {
            return element.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ct));
        }

        static public AutomationElement FirstDescendantByTypeAndName(
            this AutomationElement element, ControlType ct, string name)
        {
            return element.FindFirst(
                TreeScope.Descendants,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ct),
                    new PropertyCondition(AutomationElement.NameProperty, name)));
        }

        static public AutomationElementCollection FindElementsFromAutomationID(this AutomationElement targetApp,
    string automationID)
        {
            return targetApp.FindAll(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationID));
        }

        static public AutomationElement FindFirstElementFromAutomationID(this AutomationElement targetApp,
    string automationID)
        {
            return targetApp.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, automationID));
        }

        static public AutomationElement FindWindowFrom(AutomationElement control)
        {
            var walker = TreeWalker.ControlViewWalker;
            while (control.Current.ControlType != ControlType.Window)
                control = walker.GetParent(control);
            return control;
        }
    }
}
