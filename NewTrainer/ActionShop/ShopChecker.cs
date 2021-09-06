using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class ShopChecker
    {
        public static int[] selectedColX = { 181, 344, 507 };

        public static Point FindSelectedWarBox()
        {
            for (int col = 0; col < 3; col++)
            {
                int selectedColX1 = selectedColX[col];
                int whitePixelsRow = 0;
                for (int y = 138; y < 320; y++)
                {
                    Color pix = ActionsWithGameLogic.ScreenOfGame[selectedColX1, y];
                    if (Helpers.Between(pix, Color.White, 5) || Helpers.Between(pix, 255, 68, 0, 15))
                        whitePixelsRow++;
                    else
                    {
                        if (whitePixelsRow >= 20)
                        {
                            pix = ActionsWithGameLogic.ScreenOfGame[selectedColX1, y + 1];
                            if (Helpers.Between(pix, 255, 68, 0, 15))
                            {
                                return new Point(col, y + 1);
                            }
                        }
                        whitePixelsRow = 0;
                    }
                }
            }
            return Point.Empty;
        }

        public static bool CheckHaveSomeMore25(Point point)
        {
            for (int y = point.X - 5; y < point.X + 2; y++)
            {
                if (Helpers.MoreAny(ActionsWithGameLogic.ScreenOfGame[point], 25))
                    return true;
            }
            return false;
        }
    }
}
