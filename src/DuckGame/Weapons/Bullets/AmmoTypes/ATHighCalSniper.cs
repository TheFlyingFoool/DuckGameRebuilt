// Decompiled with JetBrains decompiler
// Type: DuckGame.ATHighCalSniper
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATHighCalSniper : AmmoType
    {
        public ATHighCalSniper()
        {
            combustable = true;
            range = 1200f;
            accuracy = 1f;
            penetration = 9f;
            bulletSpeed = 96f;
            impactPower = 6f;
            bulletThickness = 1.5f;
        }

        public override void PopShell(float x, float y, int dir)
        {
            SniperShell sniperShell = new SniperShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(sniperShell);
        }
    }
}
