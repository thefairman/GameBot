using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ShopExecutions
    {
        static public void ClickOnWarBoxes()
        {
            Global.input.MouseMove(426, 91);
            Global.input.MouseClick();
        }

        static public bool ClickOnNextWarBox(Point selectedWarBox)
        {
            if (selectedWarBox.X < ShopChecker.selectedColX.Length - 1) // next warbox on this row
            {
                Point point = new Point(ShopChecker.selectedColX[selectedWarBox.X] + 25, selectedWarBox.Y);
                if (ShopChecker.CheckHaveSomeMore25(point))
                {
                    Global.input.MouseMove(point.X, point.Y - 5);
                    Global.input.MouseClick();
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (selectedWarBox.Y > 325)
                    return false;
                Global.input.MouseMove(ShopChecker.selectedColX[0], selectedWarBox.Y - 5);
                Global.input.MouseClick();
                return true;
            }
        }

        internal static void ClickOnFirstWarBox()
        {
            Global.input.MouseMove(38, 185);
            Global.input.MouseClick();
        }

        internal static void ClickToEnterOnWarBox(Point selectedWarBox, WarboxesBase warBoxLogic)
        {
            ActionWarBox.SetWarBoxLogic(warBoxLogic);
            Global.input.MouseMove(ShopChecker.selectedColX[selectedWarBox.X], selectedWarBox.Y - 5);
            Global.input.MouseClick();
        }
    }
}
