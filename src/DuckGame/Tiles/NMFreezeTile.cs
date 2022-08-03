// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFreezeTile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMFreezeTile : NMEvent
    {
        public short x;
        public short y;

        public NMFreezeTile()
        {
        }

        public NMFreezeTile(Vec2 pPosition)
        {
            x = (short)pPosition.x;
            y = (short)pPosition.y;
        }

        public override void Activate()
        {
            Level.CheckPoint<SnowTileset>(new Vec2(x, y))?.Freeze(false, true);
            base.Activate();
        }
    }
}
