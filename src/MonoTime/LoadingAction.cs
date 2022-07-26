// Decompiled with JetBrains decompiler
// Type: DuckGame.LoadingAction
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.action = pAction;
            this.waitAction = pWaitAction;
        }

        public bool Invoke()
        {
            MonoMain.currentActionQueue = this.actions;
            if (!this.actionInvoked)
            {
                this.actionInvoked = true;
                this.action();
            }
            if (this.actions.Count > 0)
            {
                LoadingAction loadingAction = this.actions.Peek();
                if (loadingAction.Invoke())
                {
                    this.actions.Dequeue();
                    return false;
                }
                if (loadingAction.waiting)
                {
                    this.waiting = true;
                    return false;
                }
            }
            if (this.actions.Count > 0)
                return false;
            if (this.waitAction != null)
            {
                this.waiting = true;
                return this.waitAction();
            }
            this.waiting = false;
            return true;
        }

        public static implicit operator LoadingAction(Action pAction) => new LoadingAction(pAction);
    }
}
