// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMagnum
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [ClientOnly]
    public class ATWumpMagnum : AmmoType
    {
        public float angle;

        public ATWumpMagnum()
        {
            accuracy = 1f;
            range = 400f;
            penetration = 2f;
            bulletSpeed = 36f;
            combustable = true;
            rebound = true;
            bulletThickness = 2;
            bulletType = typeof(WumpMagnumbullet);
            flawlessPipeTravel = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            MagnumShell magnumShell = new MagnumShell(x, y)
            {
                hSpeed = dir * (1.5f + Rando.Float(1f))
            };
            Level.Add(magnumShell);
        }
    }
}
