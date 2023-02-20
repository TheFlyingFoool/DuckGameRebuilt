// Decompiled with JetBrains decompiler
// Type: DuckGame.ATShotgun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATShotgun : AmmoType
    {
        public ATShotgun()
        {
            accuracy = 0.6f;
            range = 115f;
            penetration = 1f;
            rangeVariation = 10f;
            combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            ShotgunShell shotgunShell = new ShotgunShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(shotgunShell);
        }
    }
}
