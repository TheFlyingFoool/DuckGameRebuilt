using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class LoadingAction
    {
        private bool actionInvoked;
        public string label = "";
        public bool flag;
        public bool waiting;
        public object context;
        public Queue<LoadingAction> actions = new Queue<LoadingAction>();
        public Action action;
        public Func<bool> waitAction;

        public LoadingAction()
        {
        }

        public LoadingAction(Action pAction, Func<bool> pWaitAction = null, string label = "")
        {
            action = pAction;
            waitAction = pWaitAction;
            this.label = label;
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
                    MonoMain.NloadMessage = "invoke load action " + loadingAction.label;
                    return false;
                }
                if (loadingAction.waiting)
                {
                    waiting = true;
                    MonoMain.NloadMessage = "invoke Load action waiting " + loadingAction.label; ;
                    return false;
                }
            }
            if (actions.Count > 0)
            {
                MonoMain.NloadMessage = "actions count " + actions.Count.ToString();
                return false;
            }
            if (waitAction != null)
            {
                MonoMain.NloadMessage = "invoke waitAction";
                waiting = true;
                return waitAction();
            }
            MonoMain.NloadMessage = "5 invoke";
            waiting = false;
            return true;
        }

        public static implicit operator LoadingAction(Action pAction) => new LoadingAction(pAction);
    }
}
