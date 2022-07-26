// Decompiled with JetBrains decompiler
// Type: DuckGame.History
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class History
    {
        private static int _actionIndex = -1;
        private static List<UndoData> _actions = new List<UndoData>();
        private static UndoData _currentAction;
        private static bool _performingAction;

        public static void BeginUndoSection()
        {
            if (History._performingAction)
                return;
            History._currentAction = new UndoData();
        }

        public static bool hasUndo => History._actionIndex != -1;

        public static bool hasRedo => History._actionIndex != History._actions.Count;

        public static void Add(UndoData pData)
        {
            if (History._performingAction)
            {
                if (pData == null || pData.apply == null)
                    return;
                pData.apply();
            }
            else
            {
                if (History._currentAction != null)
                {
                    History._currentAction.actions.Add(pData);
                }
                else
                {
                    if (History._actions.Count - 1 > History._actionIndex)
                    {
                        if (History._actionIndex == -1)
                            History._actions.Clear();
                        else
                            History._actions = History._actions.GetRange(0, History._actionIndex + 1);
                    }
                    History._actions.Add(pData);
                    ++History._actionIndex;
                }
                if (pData.apply == null)
                    return;
                pData.apply();
            }
        }

        public static void Add(Action pDo, Action pUndo) => History.Add(new UndoData()
        {
            apply = pDo,
            revert = pUndo
        });

        public static void EndUndoSection()
        {
            if (History._performingAction)
                return;
            if (History._currentAction != null)
            {
                if (History._currentAction.actions.Count > 0)
                {
                    UndoData currentAction = History._currentAction;
                    History._currentAction = (UndoData)null;
                    History.Add(currentAction);
                }
                else
                    History._currentAction = (UndoData)null;
            }
            History._currentAction = (UndoData)null;
        }

        public static void Undo()
        {
            if (History._performingAction)
                return;
            History._performingAction = true;
            if (History._actionIndex >= 0)
            {
                if (History._actions[History._actionIndex].actions.Count > 0)
                {
                    for (int index = History._actions[History._actionIndex].actions.Count - 1; index >= 0; --index)
                        History._actions[History._actionIndex].actions[index].revert();
                }
                if (History._actions[History._actionIndex].revert != null)
                    History._actions[History._actionIndex].revert();
                --History._actionIndex;
            }
            History._performingAction = false;
        }

        public static void Redo()
        {
            if (History._performingAction)
                return;
            History._performingAction = true;
            if (History._actionIndex < History._actions.Count - 1)
            {
                ++History._actionIndex;
                foreach (UndoData action in History._actions[History._actionIndex].actions)
                    action.apply();
                if (History._actions[History._actionIndex].apply != null)
                    History._actions[History._actionIndex].apply();
            }
            History._performingAction = false;
        }

        public static void Clear()
        {
            History._actions.Clear();
            History._actionIndex = -1;
        }
    }
}
