// Decompiled with JetBrains decompiler
// Type: DuckGame.NMLightDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMLightDuck : NMEvent
    {
        public Duck duck;

        public NMLightDuck(Duck pDuck) => duck = pDuck;

        public NMLightDuck()
        {
        }

        public override void Activate()
        {
            if (duck == null)
                return;
            duck.isBurnMessage = true;
            duck.Burn(duck.position, null);
            duck.isBurnMessage = false;
        }
    }
}
