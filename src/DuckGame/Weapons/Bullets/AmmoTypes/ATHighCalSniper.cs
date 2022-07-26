// Decompiled with JetBrains decompiler
// Type: DuckGame.ATHighCalSniper
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATHighCalSniper : AmmoType
    {
        public ATHighCalSniper()
        {
            this.combustable = true;
            this.range = 1200f;
            this.accuracy = 1f;
            this.penetration = 9f;
            this.bulletSpeed = 96f;
            this.impactPower = 6f;
            this.bulletThickness = 1.5f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y);
            sniperShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)sniperShell);
        }
    }
}
