// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLightDuck
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLightDuck : NMEvent
    {
        public Duck duck;

        public NMLightDuck(Duck pDuck) => this.duck = pDuck;

        public NMLightDuck()
        {
        }

        public override void Activate()
        {
            if (this.duck == null)
                return;
            this.duck.isBurnMessage = true;
            this.duck.Burn(this.duck.position, null);
            this.duck.isBurnMessage = false;
        }
    }
}
