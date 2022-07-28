//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIState
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System.Collections.Generic;

//namespace DuckGame
//{
//    public class AIState
//    {
//        protected Stack<AIState> _state = new Stack<AIState>();

//        public virtual AIState DoUpdate(Duck duck, DuckAI ai)
//        {
//            if (this._state.Count <= 0)
//                return this.Update(duck, ai);
//            AIState aiState = this._state.Peek().DoUpdate(duck, ai);
//            if (aiState == null)
//                this._state.Pop();
//            else if (aiState != this._state.Peek())
//            {
//                this._state.Pop();
//                this._state.Push(aiState);
//            }
//            return this;
//        }

//        public virtual AIState Update(Duck duck, DuckAI ai) => this;
//    }
//}
