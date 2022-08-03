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
            position = pPosition;
            closer = pCloser;
            closee = pClosee;
        }

        public override void Activate()
        {
            if (closer != null && closee != null && closee.ragdoll != null && closee.ragdoll.part1 != null)
                Level.Add(new EyeCloseWing(closee.ragdoll.part1.angle < 0.0 ? position.x - 4f : position.x - 11f, position.y + 7f, closee.ragdoll.part1.angle < 0.0 ? 1 : -1, closer._spriteArms, closer, closee));
            base.Activate();
        }
    }
}
