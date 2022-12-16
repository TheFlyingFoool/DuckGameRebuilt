//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIState
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class AIState
    {
        protected Stack<AIState> _state = new Stack<AIState>();

        public virtual AIState DoUpdate(Duck duck, DuckAI ai)
        {
            if (_state.Count <= 0)
                return Update(duck, ai);
            AIState aiState = _state.Peek().DoUpdate(duck, ai);
            if (aiState == null)
                _state.Pop();
            else if (aiState != _state.Peek())
            {
                _state.Pop();
                _state.Push(aiState);
            }
            return this;
        }

        public virtual AIState Update(Duck duck, DuckAI ai) => this;
    }
}
