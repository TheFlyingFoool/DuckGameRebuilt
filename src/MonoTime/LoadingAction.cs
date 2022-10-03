// Decompiled with JetBrains decompiler
// Type: DuckGame.LoadingAction
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class LoadingAction
    {
        private bool actionInvoked;
        public bool flag;
        public bool waiting;
        public object context;
        public Queue<LoadingAction> actions = new Queue<LoadingAction>();
        public Action action;
        public Func<bool> waitAction;

        public LoadingAction()
        {
        }

        public LoadingAction(Action pAction, Func<bool> pWaitAction = null)
        {
            action = pAction;
            waitAction = pWaitAction;
        }

        public bool Invoke()
        {
            MonoMain.currentActionQueue = actions;
            if (!actionInvoked)
            {
                actionInvoked = true;
                if (MonoMain.logLoading && action.Method != null)
                {
                    string methodfullname = "";
                    methodfullname = action.Method.Name;
                    if (action.Method.DeclaringType != null)
                    {
                        methodfullname = action.Method.DeclaringType.FullName + "." + methodfullname;
                    }
                    DevConsole.Log("LoadingAction Invoke " + methodfullname);
                }
                action();
            }
            if (actions.Count > 0)
            {
                LoadingAction loadingAction = actions.Peek();
                if (loadingAction.Invoke())
                {
                    actions.Dequeue();
                    return false;
                }
                if (loadingAction.waiting)
                {
                    waiting = true;
                    return false;
                }
            }
            if (actions.Count > 0)
                return false;
            if (waitAction != null)
            {
                waiting = true;
                return waitAction();
            }
            waiting = false;
            return true;
        }

        public static implicit operator LoadingAction(Action pAction) => new LoadingAction(pAction);
    }
}
