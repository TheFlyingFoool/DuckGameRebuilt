// Decompiled with JetBrains decompiler
// Type: DuckGame.NMEyeCloseWing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMEyeCloseWing : NMEvent
    {
        public Vec2 position;
        public Duck closer;
        public Duck closee;

        public NMEyeCloseWing()
        {
        }

        public NMEyeCloseWing(Vec2 pPosition, Duck pCloser, Duck pClosee)
        {
            this.position = pPosition;
            this.closer = pCloser;
            this.closee = pClosee;
        }

        public override void Activate()
        {
            if (this.closer != null && this.closee != null && this.closee.ragdoll != null && this.closee.ragdoll.part1 != null)
                Level.Add((Thing)new EyeCloseWing((double)this.closee.ragdoll.part1.angle < 0.0 ? this.position.x - 4f : this.position.x - 11f, this.position.y + 7f, (double)this.closee.ragdoll.part1.angle < 0.0 ? 1 : -1, this.closer._spriteArms, this.closer, this.closee));
            base.Activate();
        }
    }
}
