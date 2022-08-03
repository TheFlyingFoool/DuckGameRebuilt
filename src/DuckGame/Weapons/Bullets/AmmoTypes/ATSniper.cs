// Decompiled with JetBrains decompiler
// Type: DuckGame.ATSniper
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATSniper : AmmoType
    {
        public ATSniper()
        {
            accuracy = 0.75f;
            range = 250f;
            penetration = 1f;
            combustable = true;
            range = 1000f;
            accuracy = 1f;
            penetration = 2f;
            bulletSpeed = 48f;
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
