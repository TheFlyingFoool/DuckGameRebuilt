// Decompiled with JetBrains decompiler
// Type: DuckGame.AIStateDeathmatchBot
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class AIStateDeathmatchBot : AIState
    {
        public override AIState Update(Duck duck, DuckAI ai)
        {
            if (Network.InLobby() && !duck.pickedHat)
            {
                duck.pickedHat = true;
                this._state.Push((AIState)new AIStatePickHat());
                return (AIState)this;
            }
            if (duck.holdObject == null || !(duck.holdObject is Gun))
            {
                this._state.Push((AIState)new AIStateFindGun());
                return (AIState)this;
            }
            this._state.Push((AIState)new AIStateFindTarget());
            return (AIState)this;
        }
    }
}
