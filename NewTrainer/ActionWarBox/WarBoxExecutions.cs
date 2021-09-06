using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class WarBoxExecutions
    {
        public static void ClickExitWarBox()
        {
            Global.input.MouseMove(43, 10);
            Global.input.MouseClick();
        }
    }
}
