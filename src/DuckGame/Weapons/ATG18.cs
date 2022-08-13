// Decompiled with JetBrains decompiler
// Type: DuckGame.ATG18
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATG18 : AmmoType
    {
        public ATG18()
        {
            accuracy = 0.5f;
            range = 90f;
            penetration = 1f;
            rangeVariation = 40f;
            combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            PistolShell pistolShell = new PistolShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(pistolShell);
        }
    }
}
