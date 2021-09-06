using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class InGameActionsLogic
    {
        public const int PixelsIn360d = 2402;
        static void AdditionalMovementTowards(ListOfInGameActions listInGameActions, double angleFromPlayerToDest)
        {
            double dopDegrees = 7;
            Point prevPoint;
            if (listInGameActions.Iterator == 0)
            {
                if (listInGameActions.SpawnPoit != Point.Empty)
                    prevPoint = listInGameActions.SpawnPoit;
                else
                    prevPoint = listInGameActions.ListActions[0].StartPoint;
            }
            else
                prevPoint = listInGameActions.ListActions[listInGameActions.Iterator - 1].StartPoint;

            Point nextPoint;
            GetNextPoint(listInGameActions, out nextPoint);

            bool revers;
            double difAng = InGameChecker.GetDifrenceBetweenPointsAndAngleItsForTowards(prevPoint, nextPoint, angleFromPlayerToDest, out revers);
            if (difAng >= dopDegrees)
            {
                if (revers)
                    listInGameActions.AddButton(VirtualKeyCode.VK_D);
                else
                    listInGameActions.AddButton(VirtualKeyCode.VK_A);
            }
            ///////////////////////////
            //double angeleWithPoints = Helpers.GetAngleBetweenPoints(prevPoint, nextPoint);

            //if ((angleFromPlayerToDest + dopDegrees) % 360 < angeleWithPoints)
            //{
            //    listInGameActions.AddButton(VirtualKeyCode.VK_A);
            //    //Global.input.KeyDown(VirtualKeyCode.VK_A);
            //    return;
            //}

            //if ((angleFromPlayerToDest + dopDegrees) % 360 > angeleWithPoints)
            //{
            //    listInGameActions.AddButton(VirtualKeyCode.VK_D);
            //    //Global.input.KeyDown(VirtualKeyCode.VK_D);
            //    return;
            //}
        }

        public static bool MoveToBombDirectly(ListOfInGameActions listInGameActions)
        {
            Point bombPoint = InGameChecker.FindCoordOfBombOnMinimap();
            //if (Helpers.NumIsBetween(InGameChecker.PointOfMiddleOnMiniMap.X, bombPoint.X, 1) &&
            //    Helpers.NumIsBetween(InGameChecker.PointOfMiddleOnMiniMap.Y, bombPoint.Y, 1))
            //    return false;
            if (!Helpers.NumIsBetween(InGameChecker.PointOfMiddleOnMiniMap.X - 1, bombPoint.X, 0))
            {
                if (bombPoint.X > InGameChecker.PointOfMiddleOnMiniMap.X)
                    listInGameActions.AddButton(VirtualKeyCode.VK_D);
                else
                    listInGameActions.AddButton(VirtualKeyCode.VK_A);
            }
            if (!Helpers.NumIsBetween(InGameChecker.PointOfMiddleOnMiniMap.Y + 1, bombPoint.Y, 0))
            {
                if (bombPoint.Y > InGameChecker.PointOfMiddleOnMiniMap.Y)
                    listInGameActions.AddButton(VirtualKeyCode.VK_S);
                else
                    listInGameActions.AddButton(VirtualKeyCode.VK_W);
            }
            return false;
        }

        // tmp
        static public double AngleOfViewAndDestiny = 0;
        public static bool MoveToNextPoint(ListOfInGameActions listInGameActions)
        {
            if (listInGameActions.Iterator >= listInGameActions.ListActions.Count - 1)
                return false;

            //double angleFromPlayerToDest = Helpers.GetAngleBetweenPoints(ActionInGame.PlayerPos,
            //    listInGameActions.ListActions[listInGameActions.Iterator + 1].StartPoint);

            //double difrenceAngle;
            //if (ActionInGame.AngleOfPlayerView > angleFromPlayerToDest)
            //    difrenceAngle = ActionInGame.AngleOfPlayerView - angleFromPlayerToDest;
            //else
            //    difrenceAngle = angleFromPlayerToDest - ActionInGame.AngleOfPlayerView;

            Point nextPoint;
            GetNextPoint(listInGameActions, out nextPoint);

            double difrenceAngle;
            double angleFromPlayerToDest = InGameChecker.GetAngleOfPlayerAndDestiny(
                nextPoint, out difrenceAngle);

            difrenceAngle = Helpers.GetDifrenceBetweenAngels(angleFromPlayerToDest, ActionInGame.AngleOfPlayerView);
            //if (difrenceAngle <= 180)
            //    difrenceAngle = Helpers.PlusDegrees(angleFromPlayerToDest, difrenceAngle);
            //else
            //    difrenceAngle = Helpers.MiniusDegrees(angleFromPlayerToDest, 360 - difrenceAngle);

            AngleOfViewAndDestiny = difrenceAngle;

            int horizontDegreesZapas = 8;
            if (difrenceAngle >= 360 - horizontDegreesZapas || difrenceAngle <= horizontDegreesZapas)
            {
                AdditionalMovementTowards(listInGameActions, angleFromPlayerToDest);
                listInGameActions.AddButton(VirtualKeyCode.VK_W);
                //Global.input.KeyDown(VirtualKeyCode.VK_W);
                return false;
            }

            if (difrenceAngle >= 180 - horizontDegreesZapas && difrenceAngle <= 180 + horizontDegreesZapas)
            {
                AdditionalMovementTowards(listInGameActions, angleFromPlayerToDest);
                listInGameActions.AddButton(VirtualKeyCode.VK_S);
                //Global.input.KeyDown(VirtualKeyCode.VK_S);
                return false;
            }

            if (difrenceAngle < 180)
            {
                listInGameActions.AddButton(VirtualKeyCode.VK_A);
                //Global.input.KeyDown(VirtualKeyCode.VK_D);
                if (difrenceAngle < 45)
                    listInGameActions.AddButton(VirtualKeyCode.VK_W);
                //Global.input.KeyDown(VirtualKeyCode.VK_W);
                if (difrenceAngle > 135)
                    listInGameActions.AddButton(VirtualKeyCode.VK_S);
                //Global.input.KeyDown(VirtualKeyCode.VK_S);
                return false;
            }

            if (difrenceAngle > 180)
            {
                listInGameActions.AddButton(VirtualKeyCode.VK_D);
                //Global.input.KeyDown(VirtualKeyCode.VK_A);
                if (difrenceAngle > 315)
                    listInGameActions.AddButton(VirtualKeyCode.VK_W);
                //Global.input.KeyDown(VirtualKeyCode.VK_W);
                if (difrenceAngle < 225)
                    listInGameActions.AddButton(VirtualKeyCode.VK_S);
                //Global.input.KeyDown(VirtualKeyCode.VK_S);
                return false;
            }
            return false;
        }

        //testing 
        public static double rotateAngle = 0;
        public static double difrenceWithAngles = 0;
        static bool MouseMoveTo(Point destiny)
        {
            double difrenceAngle;
            InGameChecker.GetAngleOfPlayerAndDestiny(destiny, out difrenceAngle);
            difrenceWithAngles = difrenceAngle;
            if (difrenceAngle < 5 || difrenceAngle > 355)
            {
                rotateAngle = difrenceWithAngles;
                return false;
            }

            if (difrenceAngle > 180)
                difrenceAngle = -360 + difrenceAngle;

            rotateAngle = difrenceAngle;
            Global.input.MoveTowardWithRandomYInGame((int)(Convert.ToDouble(PixelsIn360d) / 360 * difrenceAngle));
            return true;
        }

        public static bool MoveMouseToNextPoint(ListOfInGameActions listInGameActions)
        {
            Point nextPoint;
            GetNextPoint(listInGameActions, out nextPoint);

            //if (MouseMoveTo(nextPoint)) return true;
            MouseMoveTo(nextPoint);
            return false;
        }

        public static bool MoveMouseToNextPointWhile(ListOfInGameActions listInGameActions)
        {
            Point nextPoint;
            GetNextPoint(listInGameActions, out nextPoint);

            if (MouseMoveTo(nextPoint)) return true;
            //MouseMoveTo(nextPoint);
            return false;
        }

        public static bool RunForward(ListOfInGameActions listInGameActions)
        {
            listInGameActions.AddButton(VirtualKeyCode.VK_W);
            listInGameActions.AddButton(VirtualKeyCode.VK_RSHIFT);
            //MouseMoveTo(nextPoint);
            return false;
        }

        public static bool MoveMouseToSomePointWhile(ListOfInGameActions listInGameActions)
        {
            if (MouseMoveTo(listInGameActions.GetCurrentActionProperties().LookedPoint)) return true;
            //MouseMoveTo(nextPoint);
            return false;
        }

        public static bool MoveMouseToSomePoint(ListOfInGameActions listInGameActions)
        {
            MouseMoveTo(listInGameActions.GetCurrentActionProperties().LookedPoint);
            return false;
        }

        public static bool PressG(ListOfInGameActions listInGameActions)
        {
            listInGameActions.AddButton(VirtualKeyCode.VK_G);
            return false;
        }

        public static bool PressShift(ListOfInGameActions listInGameActions)
        {
            listInGameActions.AddButton(VirtualKeyCode.VK_RSHIFT, 400, true);
            return false;
        }

        public static bool MoveMouseToYWhile(ListOfInGameActions listInGameActions)
        {
            if (!listInGameActions.GetCurrentActionProperties().YAlreadySet)
            {
                InGameExecutions.MoveMouseDownMax();
                InGameExecutions.MoveMouseUp(listInGameActions.GetCurrentActionProperties().Ypixels);
            }
            listInGameActions.GetCurrentActionProperties().YAlreadySet = true;
            //!listInGameActions.AddButton(VirtualKeyCode.VK_G);
            return false;
        }

        public static bool WaitOnThisPoint(ListOfInGameActions listInGameActions)
        {
            if (listInGameActions.GetCurrentActionProperties().PointWaitingTime > 0)
            {
                if (listInGameActions.GetCurrentActionProperties().ThisPointStartTime == DateTime.MinValue)
                    listInGameActions.GetCurrentActionProperties().ThisPointStartTime = DateTime.UtcNow;
                if (listInGameActions.GetCurrentActionProperties().ThisPointStartTime.AddMilliseconds(
                    listInGameActions.GetCurrentActionProperties().PointWaitingTime) >= DateTime.UtcNow)
                    return true;
            }
            return false;
        }

        public static bool MoveMouseToMainDestiny(ListOfInGameActions listInGameActions)
        {
            //if (MouseMoveTo(InGameChecker.PointOfBombOnMap)) return true;
            MouseMoveTo(InGameChecker.PointOfBombOnMap);
            return false;
        }

        public static bool MoveMouseToMainDestinyWhile(ListOfInGameActions listInGameActions)
        {
            if (MouseMoveTo(InGameChecker.PointOfBombOnMap)) return true;
            //MouseMoveTo(InGameChecker.PointOfBombOnMap);
            return false;
        }

        public static bool TryActivateSomething(ListOfInGameActions listInGameActions)
        {
            listInGameActions.AddButton(VirtualKeyCode.VK_E, 500, true);
            //if (!InGameChecker.IsPressedE())
            //    InGameExecutions.RePressEKey();
            return false;
        }

        static bool GetNextPoint(ListOfInGameActions listInGameActions, out Point nextPoint)
        {
            bool subZero = false;
            if (listInGameActions.Iterator == 0 && listInGameActions.SpawnPoit != Point.Empty)
            {
                nextPoint = listInGameActions.ListActions[listInGameActions.Iterator].StartPoint;
                subZero = true;
            }
            else
                nextPoint = listInGameActions.ListActions[listInGameActions.Iterator + 1].StartPoint;
            return subZero;
        }

        public static bool ForceSwitchToNextPoint(ListOfInGameActions listInGameActions)
        {
            if (listInGameActions.Iterator >= listInGameActions.ListActions.Count - 1)
                return false;
            listInGameActions.ToNextPoint();
            return true;
        }

        public static bool TrySwitchToNextPoint(ListOfInGameActions listInGameActions)
        {
            if (listInGameActions.Iterator >= listInGameActions.ListActions.Count - 1)
                return false;

            if (listInGameActions.Iterator == 0 && listInGameActions.SpawnPoit != Point.Empty)
            {
                if (Helpers.NumIsBetween(ActionInGame.PlayerPos.X, listInGameActions.ListActions[0].StartPoint.X, listInGameActions.ListActions[0].ToleranceX)
                    &&
                Helpers.NumIsBetween(ActionInGame.PlayerPos.Y, listInGameActions.ListActions[0].StartPoint.Y, listInGameActions.ListActions[0].ToleranceY))
                {
                    listInGameActions.SpawnPoit = Point.Empty;
                }
            }

            Point nextPoint = listInGameActions.ListActions[listInGameActions.Iterator + 1].StartPoint;
            int toleranceX = listInGameActions.ListActions[listInGameActions.Iterator + 1].ToleranceX;
            int toleranceY = listInGameActions.ListActions[listInGameActions.Iterator + 1].ToleranceY;

            if (Helpers.NumIsBetween(ActionInGame.PlayerPos.X, nextPoint.X, toleranceX) &&
                Helpers.NumIsBetween(ActionInGame.PlayerPos.Y, nextPoint.Y, toleranceY))
            {
                listInGameActions.ToNextPoint();
                return true;
            }
            return false;
        }

        public static bool EveryOneHaveExperience(ListOfInGameActions listInGameActions)
        {
            if (!InGameChecker.EveryOneHaveExperience())
                if (ActionInGame.OnRoundTime.AddSeconds(55) < DateTime.UtcNow)
                    return true;
            return false;
        }

        public static bool ShouldITakeAExperience(ListOfInGameActions listInGameActions)
        {
            if (!InGameChecker.ShouldITakeAExperience())
            {
                if (ActionInGame.OnRoundTime.AddMinutes(1) > DateTime.UtcNow)
                    return true;
            }
            //ActionsWithGameLogic.SaveImageIW("notMyExp");
            return false;
        }
    }
}
