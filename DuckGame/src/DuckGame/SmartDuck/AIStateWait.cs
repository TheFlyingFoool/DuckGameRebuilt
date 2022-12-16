//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIStateWait
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class AIStateWait : AIState
    {
        private float _wait;

        public AIStateWait(float wait) => _wait = wait;

        public override AIState Update(Duck duck, DuckAI ai)
        {
            _wait -= 0.016f;
            return _wait <= 0.0 ? null : (AIState)this;
        }
    }
}
