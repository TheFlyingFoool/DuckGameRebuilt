// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRessurect
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMRessurect : NMEvent
    {
        public Vec2 position;
        public Duck duck;
        public byte lifeIndex;

        public NMRessurect()
        {
        }

        public NMRessurect(Vec2 pPosition, Duck pDuck, byte pLifeChangeIndex)
        {
            position = pPosition;
            duck = pDuck;
            lifeIndex = pLifeChangeIndex;
        }

        public override void Activate()
        {
            if (duck != null && duck.profile != null && duck.WillAcceptLifeChange(lifeIndex))
            {
                Duck.ResurrectEffect(position);
                duck.ResetNonServerDeathState();
            }
            base.Activate();
        }
    }
}
