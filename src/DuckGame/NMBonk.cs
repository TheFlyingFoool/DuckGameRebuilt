// Decompiled with JetBrains decompiler
// Type: DuckGame.NMBonk
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMBonk : NMEvent
    {
        public Vec2 position;
        public Vec2 velocity;

        public NMBonk()
        {
        }

        public NMBonk(Vec2 pPosition, Vec2 pVelocity)
        {
            position = pPosition;
            velocity = pVelocity;
        }

        public override void Activate()
        {
            Duck.MakeStars(position, velocity);
            base.Activate();
        }
    }
}
