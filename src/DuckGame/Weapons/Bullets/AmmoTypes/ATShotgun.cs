// Decompiled with JetBrains decompiler
// Type: DuckGame.ATShotgun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATShotgun : AmmoType
    {
        public ATShotgun()
        {
            this.accuracy = 0.6f;
            this.range = 115f;
            this.penetration = 1f;
            this.rangeVariation = 10f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y);
            shotgunShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)shotgunShell);
        }
    }
}
