using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class Testing
    {
        static public void RotateTest()
        {
            ActionsWithGameLogic.RefreshScreenOnGame();
            Thread.Sleep(500);
            // 2402 - 360
            for (int i = 0; i < 24; i++)
            {
                IntPtr wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
                bool fgrnd = (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());

                WinApi.mouse_event(WinApi.MouseFlags.Move, 100, 0, 0, UIntPtr.Zero);
                Thread.Sleep(10);
            }
            WinApi.mouse_event(WinApi.MouseFlags.Move, 2, 0, 0, UIntPtr.Zero);
        }

        static public void MoveMouseYTest()
        {
            ActionsWithGameLogic.RefreshScreenOnGame();
            Thread.Sleep(500);
            // 2402 - 360
            for (int i = 0; i < 1080; i++)
            {
                IntPtr wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
                bool fgrnd = (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());

                if (fgrnd)
                {
                    WinApi.mouse_event(WinApi.MouseFlags.Move, 0, 1, 0, UIntPtr.Zero);
                    Thread.Sleep(1);
                }
            }
            // up
            for (int i = 0; i < 450; i++)
            {
                IntPtr wfHandle = WinApi.FindWindow(Global.settingsMain.wfWindowClass, Global.settingsMain.wfWindowName);
                bool fgrnd = (wfHandle != IntPtr.Zero && wfHandle == WinApi.GetForegroundWindow());

                if (fgrnd)
                {
                    WinApi.mouse_event(WinApi.MouseFlags.Move, 0, -1, 0, UIntPtr.Zero);
                    Thread.Sleep(1);
                }
            }
        }
    }
}
