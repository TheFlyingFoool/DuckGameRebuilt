// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPistolExplode
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPistolExplode : NMEvent
    {
        public Vec2 position;

        public NMPistolExplode()
        {
        }

        public NMPistolExplode(Vec2 pPosition) => position = pPosition;

        public override void Activate()
        {
            DuelingPistol.ExplodeEffect(position);
            base.Activate();
        }
    }
}
