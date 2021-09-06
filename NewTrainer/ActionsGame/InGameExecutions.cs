using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewTrainer
{
    class InGameExecutions
    {
        public static void RePressTabKey()
        {
            Global.input.KeyDown(VirtualKeyCode.VK_TAB);
            System.Threading.Thread.Sleep(200);
            Global.input.KeyUp(VirtualKeyCode.VK_TAB);
            Global.input.KeyDown(VirtualKeyCode.VK_TAB);
        }

        public static void RePressEKey()
        {
            Global.input.KeyDown(VirtualKeyCode.VK_E);
            System.Threading.Thread.Sleep(300);
            Global.input.KeyUp(VirtualKeyCode.VK_E);
            Global.input.KeyDown(VirtualKeyCode.VK_E);
        }

        public static void ResetMouseMove()
        {
            //List<VirtualKeyCode> except = new List<VirtualKeyCode>();
            //except.Add(VirtualKeyCode.VK_TAB);
            //except.Add(VirtualKeyCode.VK_E);
            //Global.input.ReleaseButtons(except);
            Global.input.ResetMoveTowardWithRandomYInGame();
        }

        internal static void ReleaseAllButtons()
        {
            Global.input.ReleaseButtons();
            Global.input.ResetMoveTowardWithRandomYInGame();
        }

        public static void MoveMouseDownMax()
        {
            Random rnd = new Random();
            int Ytotal = 1080;
            while (Ytotal > 0)
            {
                int curY = rnd.Next(1, 25);
                if (curY > Ytotal)
                    curY = Ytotal;
                Ytotal -= curY;
                WinApi.mouse_event(WinApi.MouseFlags.Move, 0, curY, 0, UIntPtr.Zero);
                Thread.Sleep(rnd.Next(1, 15));
            }
        }

        public static void MoveMouseUp(int Ytotal)
        {
            Random rnd = new Random();
            while (Ytotal > 0)
            {
                int curY = rnd.Next(1, 20);
                if (curY > Ytotal)
                    curY = Ytotal;
                Ytotal -= curY;
                WinApi.mouse_event(WinApi.MouseFlags.Move, 0, -curY, 0, UIntPtr.Zero);
                Thread.Sleep(rnd.Next(1, 15));
            }
        }
    }
}
