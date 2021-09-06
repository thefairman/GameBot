using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class MessageChecker
    {
        public static bool CheckWasInGame()
        {
            int tolerance = 80 * 80;
            int addX = 329;
            int addX2 = 469;
            int fst = 0;
            int sec = 0;
            Color blackWoodColor = Color.FromArgb(205, 36, 44);
            Color warfaceColor = Color.FromArgb(71, 124, 206);
            for (int y = 25; y < 40; y++)
            {
                Color pix;
                if (fst == 0)
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[addX, y];
                    if (Helpers.IsSearchedColor(pix, blackWoodColor, tolerance))
                    {
                        fst = 2;
                    }
                    else if (Helpers.IsSearchedColor(pix, warfaceColor, tolerance))
                    {
                        fst = 1;
                    }
                }
                if (sec == 0)
                {
                    pix = ActionsWithGameLogic.ScreenOfGame[addX2, y];
                    if (Helpers.IsSearchedColor(pix, blackWoodColor, tolerance))
                    {
                        sec = 2;
                    }
                    else if (Helpers.IsSearchedColor(pix, warfaceColor, tolerance))
                    {
                        sec = 1;
                    }
                }

                if (fst != 0 && sec != 0 && fst != sec)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
