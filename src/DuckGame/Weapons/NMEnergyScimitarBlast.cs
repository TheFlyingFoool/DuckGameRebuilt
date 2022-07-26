// Decompiled with JetBrains decompiler
// Type: DuckGame.NMEnergyScimitarBlast
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMEnergyScimitarBlast : NMEvent
    {
        public Vec2 position;
        public Vec2 target;

        public NMEnergyScimitarBlast(Vec2 pPosition, Vec2 pTarget)
        {
            this.position = pPosition;
            this.target = pTarget;
        }

        public NMEnergyScimitarBlast()
        {
        }

        public override void Activate()
        {
            if (Level.current == null)
                return;
            Level.Add((Thing)new EnergyScimitarBlast(this.position, this.target));
        }
    }
}
