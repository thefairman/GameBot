using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class InGameChecker
    {
        public static Point PointOfBombOnMap { get; private set; } = new Point(132, 168);
        public static Point PointOfMiddleOnMiniMap { get; private set; } = new Point(93, 110);
        static public bool RoundGoOn()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                for (int i = 376; i < 400; i++)
                {
                    Color pix = ActionsWithGameLogic.ScreenOfGame[i, 31];
                    if (Helpers.MoreAny(pix, 160))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static public bool IsAlive()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                int addX = 93;//96;
                int addY = 110/* + lishneeY*/;
                bool white = false;
                bool gray = false;

                for (int x = -5; x < 6; x++)
                {
                    for (int y = -5; y < 6; y++)
                    {
                        Color pix = ActionsWithGameLogic.ScreenOfGame[x + addX, y + addY];
                        if (Helpers.More(pix, 253))
                        {
                            white = true;
                        }
                        else
                            gray = true;
                        if (white && gray) return true;
                    }
                }
                return white && gray;

            }
            return false;
        }

        static public bool TabPressed()
        {
            if (ActionsWithGameLogic.ScreenOfGame != null)
            {
                for (int i = 0; i < 150; i++)
                {
                    Color pix = ActionsWithGameLogic.ScreenOfGame[330 + i, 196];
                    if (Helpers.More(pix, 60))
                        return false;
                }
                return true;
            }
            return false;
        }

        static bool MoreThen0Experience(int y, out bool haveExperience)
        {
            haveExperience = false;
            bool secondNum = false;
            for (int x = 260; x > 240; x--)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, y];
                if (Helpers.MoreAny(pix, 155))
                {
                    haveExperience = true;
                    if (x <= 251)
                    {
                        secondNum = true;
                        break;
                    }
                }
                if (x < 245)
                {
                    if (!secondNum)
                        return false;
                }
            }
            return secondNum;
        }

        internal static bool HaveTenRounds()
        {
            Color pix = ActionsWithGameLogic.ScreenOfGame[346, 37];
            if (Helpers.More(pix, 155))
            {
                pix = ActionsWithGameLogic.ScreenOfGame[358, 30];
                if (Helpers.More(pix, 155))
                    return true;
            }
            return false;
        }

        static bool MoreThen0ExperienceRight(int y, out bool haveExperience)
        {
            haveExperience = false;
            bool secondNum = false;
            for (int x = 784; x > 764; x--)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, y];
                if (Helpers.MoreAny(pix, 155))
                {
                    haveExperience = true;
                    if (x < 775)
                    {
                        secondNum = true;
                        break;
                    }
                }
                if (x < 775)
                {
                    if (!haveExperience)
                        return false;
                }
            }
            return secondNum;
        }

        static public bool ShouldITakeAExperience()
        {
            Color searchedColor = Color.FromArgb(254, 68, 0);

            int x1 = 130;
            int startY = 188;

            bool haveExperience;
            for (int y = startY; y < 426; y++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x1, y];
                if (Helpers.Between(pix, searchedColor, 10))
                {
                    if (!MoreThen0Experience(y + 3, out haveExperience))
                        return true;
                    break;
                }
            }

            int plYPlus = 32;
            startY = 200;
            for (int pl = 0; pl < 8; pl++)
            {
                if (!MoreThen0Experience(startY + plYPlus * pl, out haveExperience))
                {
                    if (haveExperience)
                        return false;
                    else
                        break;
                }
            }

            return true;
        }

        static public bool EveryOneHaveExperience()
        {
            bool haveExperience;
            int plYPlus = 32;
            int startY = 200;
            for (int pl = 0; pl < 8; pl++)
            {
                if (!MoreThen0Experience(startY + plYPlus * pl, out haveExperience))
                    if (haveExperience) return false;
                if (!MoreThen0ExperienceRight(startY + plYPlus * pl, out haveExperience))
                    if (haveExperience) return false;
            }
            return true;
        }

        public static Point FindCoordOfPlayer()
        {
            int startX = 302;
            int startY = 200;

            int toleranceSquared = 6400; // 80 * 80
            //int searchedR = 232, searchedG = 178, searchedB = 4;
            Color searchedColor = Color.FromArgb(232, 178, 4);

            for (int ya = 0; ya < 240; ya++)
            {
                for (int xa = 0; xa < 195; xa++)
                {
                    int x = startX + xa;
                    int y = startY + ya;

                    Color pixel = ActionsWithGameLogic.ScreenOfGame[x, y];
                    if (Helpers.IsSearchedColor(pixel, searchedColor, toleranceSquared))
                        return new Point(xa, ya);

                    //int diffR = pixel.R - searchedR;
                    //int diffG = pixel.G - searchedG;
                    //int diffB = pixel.B - searchedB;

                    //int distance = diffR * diffR + diffG * diffG + diffB * diffB;
                    //if (distance <= toleranceSquared) return new Point(xa, ya);
                }
            }

            return new Point(-1, -1);
        }

        //public static double GetAngleBetweenPoints(Point point1, Point point2)
        //{
        //    double x1 = point1.X, y1 = point1.Y;
        //    double x2 = point2.X, y2 = point2.Y;
        //    double A = Math.Atan2(y1 - y2, x1 - x2) / Math.PI * 180;
        //    return (A < 0) ? A + 360 : A;   //Без этого диапазон от 0...180 и -1...-180
        //}

        static Point FindBombExactly(int startX, int startY, int plusX, bool top, bool left)
        {
            int bombHalfWidth = 4;
            int bombHalfHeight = 2;
            if (!top)
            {
                if (left)
                {
                    return new Point(startX + plusX - bombHalfWidth, startY + bombHalfHeight);
                }
                else
                {
                    return new Point(startX + bombHalfWidth, startY + bombHalfHeight);
                }
            }

            if (left)
            {
                for (int tmpY = 0; tmpY < 10; tmpY++)
                {
                    for (int tmpX = 0; tmpX < 10; tmpX++)
                    {
                        if (!Helpers.Between(ActionsWithGameLogic.ScreenOfGame[startX + plusX - tmpX, startY + tmpY + 1], searchedBombCol, toleranceBomp))
                        {
                            if (tmpX < 3)
                            {
                                return new Point(startX + plusX - bombHalfWidth, startY + tmpY - bombHalfHeight);
                            }
                        }
                    }
                }
            }
            else
            {
                for (int tmpY = 0; tmpY < 10; tmpY++)
                {
                    for (int tmpX = 0; tmpX < 10; tmpX++)
                    {
                        if (!Helpers.Between(ActionsWithGameLogic.ScreenOfGame[startX + tmpX, startY + tmpY + 1], searchedBombCol, toleranceBomp))
                        {
                            if (tmpX < 3)
                            {
                                return new Point(startX + bombHalfWidth, startY + tmpY - bombHalfHeight);
                            }
                        }
                    }
                }
            }
            return new Point(-1, -1);
        }

        static Color searchedBombCol = Color.FromArgb(255, 38, 38);
        static Color searchedBombCol2 = Color.FromArgb(145, 139, 196);
        static Color searchedBombCol3 = Color.FromArgb(255, 8, 8);
        static int toleranceBomp = 15;
        static public Point FindCoordOfBombOnMinimap()
        {
            int startX = 26;
            int startY = 43;

            int pixelsX = 136;
            int pixelsY = 136;

            for (int ya = 0; ya < pixelsY; ya++)
            {
                for (int xa = 0; xa < pixelsX; xa++)
                {
                    int x = startX + xa;
                    int y = startY + ya;

                    Color pixel = ActionsWithGameLogic.ScreenOfGame[x, y];
                    if (Helpers.Between(pixel, searchedBombCol, toleranceBomp) || 
                        Helpers.Between(pixel, searchedBombCol2, toleranceBomp) ||
                        Helpers.Between(pixel, searchedBombCol3, toleranceBomp))
                    {
                        int plusX = 0;
                        for (; plusX < 10; plusX++)
                        {
                            if (!Helpers.Between(ActionsWithGameLogic.ScreenOfGame[x + plusX + 1, y], searchedBombCol, toleranceBomp) &&
                                !Helpers.Between(ActionsWithGameLogic.ScreenOfGame[x + plusX + 1, y], searchedBombCol2, toleranceBomp) &&
                                !Helpers.Between(ActionsWithGameLogic.ScreenOfGame[x + plusX + 1, y], searchedBombCol3, toleranceBomp))
                                break;
                        }
                        if (plusX >= 3)
                        {
                            return FindBombExactly(x, y, plusX, y < pixelsY / 2, x < pixelsX / 2);
                        }
                    }

                }
            }
            return new Point(-1, -1);
        }

        public static bool IsNegativeOrEmptyPoint(Point point)
        {
            return point.IsEmpty || point.X < 0 || point.Y < 0;
        }

        public static int GetDistanceBetweenPoints(Point point1, Point point2)
        {
            double x1 = point1.X, y1 = point1.Y;
            double x2 = point2.X, y2 = point2.Y;
            return (int)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static double GetAngleOfPlayerOnMap()
        {
            if (IsNegativeOrEmptyPoint(ActionInGame.PlayerPos))
                return -1;
            if (GetDistanceBetweenPoints(ActionInGame.PlayerPos, PointOfBombOnMap) < 5)
                return -1;
            double angle = Helpers.GetAngleBetweenPoints(ActionInGame.PlayerPos, PointOfBombOnMap);
            //if (angle > 180)
            //    return angle - 180;
            return angle;
        }

        public static double GetAngleOfPlayerOnMiniMap()
        {
            if (IsNegativeOrEmptyPoint(ActionInGame.PlayerPos))
                return -1;
            if (GetDistanceBetweenPoints(PointOfMiddleOnMiniMap, FindCoordOfBombOnMinimap()) < 4)
                return -1;
            Point bombCoordOnMiniMap = FindCoordOfBombOnMinimap();
            return Helpers.GetAngleBetweenPoints(PointOfMiddleOnMiniMap, bombCoordOnMiniMap);
        }

        public static double GetAngleOfPlayerView()
        {
            double AngleOfMiniMapOrient = Helpers.MiniusDegrees(ActionInGame.AngleOnMiniMap, 90);
            if (AngleOfMiniMapOrient == 0)
                return ActionInGame.AngleOnMap;
            if (ActionInGame.AngleOnMap == 0)
                return AngleOfMiniMapOrient;

            return Helpers.GetDifrenceBetweenAngels(AngleOfMiniMapOrient, ActionInGame.AngleOnMap);
            //double difrence;
            //if (AngleOfMiniMapOrient > ActionInGame.AngleOnMap)
            //    difrence = AngleOfMiniMapOrient - ActionInGame.AngleOnMap;
            //else
            //    difrence = ActionInGame.AngleOnMap - AngleOfMiniMapOrient;

            //if (AngleOfMiniMapOrient > ActionInGame.AngleOnMap)
            //    return 360 - difrence;
            //else
            //    return difrence;

            //double angleOfPlayerView = Helpers.MinusOrPlusDegreesBy180(ActionInGame.AngleOnMap, AngleOfMiniMapOrient);
            //return angleOfPlayerView;
        }

        //tmp
        static public double AngleFromPlayerToDest;
        public static double GetAngleOfPlayerAndDestiny(Point destinyPoint, out double difrenceAngleWithViewAndDest)
        {
            double angleFromPlayerToDest = Helpers.GetAngleBetweenPoints(ActionInGame.PlayerPos, destinyPoint);
            AngleFromPlayerToDest = angleFromPlayerToDest;

            difrenceAngleWithViewAndDest = Helpers.GetDifrenceBetweenAngels(ActionInGame.AngleOfPlayerView, angleFromPlayerToDest);
            //if (ActionInGame.AngleOfPlayerView > angleFromPlayerToDest)
            //    difrenceAngleWithViewAndDest = 360 - (ActionInGame.AngleOfPlayerView - angleFromPlayerToDest);
            //else
            //    difrenceAngleWithViewAndDest = angleFromPlayerToDest - ActionInGame.AngleOfPlayerView;
            return angleFromPlayerToDest;
        }

        /// <summary>
        /// возвращает разниуц в углах относительно angle
        /// </summary>
        /// <param name="pointStart">стартовая точка</param>
        /// <param name="pointEnd">конечная точка</param>
        /// <param name="angle">уголо относительно которого сравниваем</param>
        /// <param name="reverse">обратный ли угол</param>
        /// <returns>Угол</returns>
        public static double GetDifrenceBetweenPointsAndAngleItsForTowards(Point pointStart, Point pointEnd, double angle, out bool reverse)
        {
            double angleBetweenPoints = Helpers.GetAngleBetweenPoints(pointStart, pointEnd);

            if (angleBetweenPoints > angle)
            {
                reverse = false;
                return angleBetweenPoints - angle;
            }
            else
            {
                reverse = true;
                return angle - angleBetweenPoints;
            }
        }

        public static bool IsPressedE()
        {
            int x = 344;
            for (int y = 362; y < 369; y++)
            {
                int avg1 = Helpers.GetAvgColor(ActionsWithGameLogic.ScreenOfGame[x, y]);
                int avg2 = Helpers.GetAvgColor(ActionsWithGameLogic.ScreenOfGame[x + 1, y]);
                if (avg1 + 7 > avg2) return false;
            }

            return true;
        }
    }
}
