using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class InGameActionsProperties
    {
        public int ToleranceX { get; set; } = 4;
        public int ToleranceY { get; set; } = 4;
        public int Ypixels { get; set; } = 0;
        public Point StartPoint { get; set; }
        public List<Func<ListOfInGameActions, bool>> Actions { get; set; }
        public int MaxTimePressed { get; set; } = 1000;
        public bool NeedReleaseButton { get; set; } = false;
        public bool YAlreadySet { get; set; } = false;
        public int PointWaitingTime { get; set; } = 0;
        public DateTime ThisPointStartTime { get; set; } = DateTime.MinValue;
        public Point LookedPoint { get; set; } = Point.Empty;
    }

    abstract class ListOfInGameActions
    {
        class PressKeyInfo
        {
            public VirtualKeyCode Key { get; set; }
            public int MaxTimePressed { get; set; } = 1000;
            public DateTime TimeStartPressed { get; set; }
            public bool NeedReleaseButton { get; set; } = false;
        }

        ConcurrentDictionary<VirtualKeyCode, PressKeyInfo> shouldPressButtons = new ConcurrentDictionary<VirtualKeyCode, PressKeyInfo>();
        System.Timers.Timer checkKeyDownTimer = new System.Timers.Timer(33);

        public int Iterator { get; private set; } = 0;
        public List<InGameActionsProperties> ListActions { get; set; } = new List<InGameActionsProperties>();
        public Point SpawnPoit { get; set; } = Point.Empty;
        bool spawnUsed = false;
        public ListOfInGameActions()
        {
            FillMoveButtons();
            checkKeyDownTimer.Elapsed += CheckKeyDownTimer_Elapsed;
            checkKeyDownTimer.Start();
        }

        public InGameActionsProperties GetCurrentActionProperties()
        {
            return ListActions[Iterator];
        }

        private void CheckKeyDownTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkKeyDownTimer.Stop();
            try
            {
                PressKeyInfo pressKeyInfo;
                foreach (var item in allMoveButtons)
                {
                    if (shouldPressButtons.TryGetValue(item, out pressKeyInfo))
                    {
                        if (pressKeyInfo.TimeStartPressed.AddMilliseconds(pressKeyInfo.MaxTimePressed) < DateTime.UtcNow)
                        {
                            if (item == VirtualKeyCode.VK_E)
                            {
                                ActionsWithGameLogic.RefreshScreenOnGame();
                                if (!InGameChecker.IsPressedE())
                                {
                                    //ActionsWithGameLogic.SaveImageIW("tmp");
                                    //InGameExecutions.RePressEKey();
                                    Global.input.KeyUp(item);
                                    //pressKeyInfo.TimeStartPressed = DateTime.UtcNow;
                                    shouldPressButtons.TryRemove(item, out pressKeyInfo);
                                }
                            }
                            else
                            {
                                shouldPressButtons.TryRemove(item, out pressKeyInfo);
                                Global.input.KeyUp(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Global.UIdata.ErrororsUI.AddRecord(new UIMessages(ex.Message));
            }
            checkKeyDownTimer.Start();
        }
        List<PressKeyInfo> needPressButton = new List<PressKeyInfo>();
        public void AddButton(VirtualKeyCode key, int maxTimePressed = 0, bool? needReleaseButton = null)
        {
            needPressButton.Add(new PressKeyInfo() {
                Key = key,
                MaxTimePressed = maxTimePressed == 0 ? ListActions[Iterator].MaxTimePressed : maxTimePressed,
                TimeStartPressed = DateTime.MinValue,
                NeedReleaseButton = needReleaseButton == null ? ListActions[Iterator].NeedReleaseButton : (bool)needReleaseButton
            });
        }
        public void ToNextPoint()
        {
            SpawnPoit = Point.Empty;
            if (Iterator < ListActions.Count - 1)
                Iterator++;
        }
        public void ResetData()
        {
            spawnUsed = false;
            Iterator = 0;
            SpawnPoit = Point.Empty;
            ResetPointsData();
        }

        void ResetPointsData()
        {
            foreach (var item in ListActions)
            {
                if (item.PointWaitingTime > 0)
                    item.ThisPointStartTime = DateTime.MinValue;
                item.YAlreadySet = false;
            }
        }

        public void DoActions()
        {
            needPressButton = new List<PressKeyInfo>();
            if (!spawnUsed && SpawnPoit == Point.Empty)
            {
                SpawnPoit = ActionInGame.PlayerPos;
                spawnUsed = true;
            }
            foreach (var item in ListActions[Iterator].Actions)
            {
                if (item(this))
                    break;
            }
            PressMoveButtons();
        }

        void FillMoveButtons()
        {
            allMoveButtons = new List<VirtualKeyCode>();
            allMoveButtons.Add(VirtualKeyCode.VK_W);
            allMoveButtons.Add(VirtualKeyCode.VK_A);
            allMoveButtons.Add(VirtualKeyCode.VK_S);
            allMoveButtons.Add(VirtualKeyCode.VK_D);
            allMoveButtons.Add(VirtualKeyCode.VK_E);
            allMoveButtons.Add(VirtualKeyCode.VK_G);
            allMoveButtons.Add(VirtualKeyCode.VK_RSHIFT);
        }

        List<VirtualKeyCode> allMoveButtons = null;
        void PressMoveButtons()
        {
            PressKeyInfo pressKeyInfo;
            foreach (var item in allMoveButtons)
            {
                int i = needPressButton.FindIndex(x => x.Key == item);
                if (i < 0)
                {
                    shouldPressButtons.TryRemove(item, out pressKeyInfo);
                    Global.input.KeyUp(item);
                }
                else
                {
                    PressKeyInfo pki = new PressKeyInfo()
                    {
                        Key = item,
                        MaxTimePressed = needPressButton[i].MaxTimePressed,
                        NeedReleaseButton = needPressButton[i].NeedReleaseButton,
                        TimeStartPressed = DateTime.UtcNow
                    };
                    pressKeyInfo = null;
                    shouldPressButtons.TryRemove(item, out pressKeyInfo);
                    if (pressKeyInfo != null)
                    {
                        if (pressKeyInfo.NeedReleaseButton)
                        {
                            pki.TimeStartPressed = pressKeyInfo.TimeStartPressed;
                        }
                    }
                    else
                        Global.input.KeyDown(item);

                    shouldPressButtons.TryAdd(item, pki);
                }
            }
        }
    }

    class ListOfInGameActionsWF : ListOfInGameActions
    {
        public ListOfInGameActionsWF()
        {
            // point 0
            List<Func<ListOfInGameActions, bool>> actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(59, 217),
            });
            //
            // point 1
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(51, 152),
            });
            //
            //
            // point 2
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToSomePointWhile);
            actions.Add(InGameActionsLogic.ForceSwitchToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(60, 128),
                LookedPoint = new Point(5, 151)
            });
            //
            // point 3
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.ShouldITakeAExperience);
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToSomePoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(85, 90),
                LookedPoint = new Point(5, 151),
            });
            //
            // point 4
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToSomePointWhile);
            actions.Add(InGameActionsLogic.MoveMouseToYWhile);
            actions.Add(InGameActionsLogic.ForceSwitchToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(37, 140),
                LookedPoint = new Point(69, 79),
                ToleranceX = 8,
                ToleranceY = 6
            });
            //
            // point 5
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.PressG);
            actions.Add(InGameActionsLogic.WaitOnThisPoint);
            actions.Add(InGameActionsLogic.ForceSwitchToNextPoint);

            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(37, 140),
                PointWaitingTime = 1000,
                MaxTimePressed = 200,
                NeedReleaseButton = true,
            });
            //
            // point 6
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.PressShift);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(37, 140),
            });
            //
            // point 7
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToMainDestinyWhile);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(52, 111)
            });
            //
        }
    }

    class ListOfInGameActionsBW : ListOfInGameActions
    {
        public ListOfInGameActionsBW()
        {
            // point 0
            List<Func<ListOfInGameActions, bool>> actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.TryActivateSomething);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 47),
                ToleranceX = 6,
                ToleranceY = 6
            });
            //
            // point 1
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.PressShift);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 64),
                ToleranceX = 7,
                ToleranceY = 7
            });
            //
            // point 2
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToSomePointWhile);
            actions.Add(InGameActionsLogic.ForceSwitchToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 96),
                LookedPoint = new Point(14, 162),
                ToleranceX = 7,
                ToleranceY = 7,
            });
            //
            // point 2
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.PressShift);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 96),
                ToleranceX = 7,
            });
            //
            // point 3
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToMainDestinyWhile);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(14, 164),
            });
            //
        }
    }

    class ListOfInGameActionsBWDefuse : ListOfInGameActions
    {
        public ListOfInGameActionsBWDefuse()
        {
            // point 0
            List<Func<ListOfInGameActions, bool>> actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.TryActivateSomething);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 47),
                ToleranceX = 6,
                ToleranceY = 6
            });
            //
            // point 1
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            actions.Add(InGameActionsLogic.PressShift);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 64),
                ToleranceX = 7,
                ToleranceY = 7
            });
            //
            // point 2
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(82, 120),
            });
            //
            // point 3
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TrySwitchToNextPoint);
            actions.Add(InGameActionsLogic.MoveMouseToNextPoint);
            actions.Add(InGameActionsLogic.MoveToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(106, 120),
            });
            //
            // point 4
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.MoveMouseToMainDestinyWhile);
            actions.Add(InGameActionsLogic.ForceSwitchToNextPoint);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(108, 168),
            });
            //
            // point 5
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TryActivateSomething);
            //actions.Add(InGameActionsLogic.MoveMouseToMainDestiny);
            actions.Add(InGameActionsLogic.MoveToBombDirectly);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = new Point(120, 168)
            });
            //
            // point 6
            actions = new List<Func<ListOfInGameActions, bool>>();
            actions.Add(InGameActionsLogic.TryActivateSomething);
            ListActions.Add(new InGameActionsProperties()
            {
                Actions = actions,
                StartPoint = InGameChecker.PointOfBombOnMap,
                ToleranceX = 2,
                ToleranceY = 2
            });
            //
        }
    }
}
