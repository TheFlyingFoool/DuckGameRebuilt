// Decompiled with JetBrains decompiler
// Type: DuckGame.History
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            if (_performingAction)
                return;
            _currentAction = new UndoData();
        }

        public static bool hasUndo => _actionIndex != -1;

        public static bool hasRedo => _actionIndex != _actions.Count;

        public static void Add(UndoData pData)
        {
            if (_performingAction)
            {
                if (pData == null || pData.apply == null)
                    return;
                pData.apply();
            }
            else
            {
                if (_currentAction != null)
                {
                    _currentAction.actions.Add(pData);
                }
                else
                {
                    if (_actions.Count - 1 > _actionIndex)
                    {
                        if (_actionIndex == -1)
                            _actions.Clear();
                        else
                            _actions = _actions.GetRange(0, _actionIndex + 1);
                    }
                    _actions.Add(pData);
                    ++_actionIndex;
                }
                if (pData.apply == null)
                    return;
                pData.apply();
            }
        }

        public static void Add(Action pDo, Action pUndo) => Add(new UndoData()
        {
            apply = pDo,
            revert = pUndo
        });

        public static void EndUndoSection()
        {
            if (_performingAction)
                return;
            if (_currentAction != null)
            {
                if (_currentAction.actions.Count > 0)
                {
                    UndoData currentAction = _currentAction;
                    _currentAction = null;
                    Add(currentAction);
                }
                else
                    _currentAction = null;
            }
            _currentAction = null;
        }

        public static void Undo()
        {
            if (_performingAction)
                return;
            _performingAction = true;
            if (_actionIndex >= 0)
            {
                if (_actions[_actionIndex].actions.Count > 0)
                {
                    for (int index = _actions[_actionIndex].actions.Count - 1; index >= 0; --index)
                        _actions[_actionIndex].actions[index].revert();
                }
                if (_actions[_actionIndex].revert != null)
                    _actions[_actionIndex].revert();
                --_actionIndex;
            }
            _performingAction = false;
        }

        public static void Redo()
        {
            if (_performingAction)
                return;
            _performingAction = true;
            if (_actionIndex < _actions.Count - 1)
            {
                ++_actionIndex;
                foreach (UndoData action in _actions[_actionIndex].actions)
                    action.apply();
                if (_actions[_actionIndex].apply != null)
                    _actions[_actionIndex].apply();
            }
            _performingAction = false;
        }

        public static void Clear()
        {
            _actions.Clear();
            _actionIndex = -1;
        }
    }
}
