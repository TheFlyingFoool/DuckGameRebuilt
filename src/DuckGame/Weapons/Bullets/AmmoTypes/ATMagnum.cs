// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMagnum
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATMagnum : AmmoType
    {
        public float angle;

        public ATMagnum()
        {
            this.accuracy = 1f;
            this.range = 300f;
            this.penetration = 2f;
            this.bulletSpeed = 36f;
            this.combustable = true;
        }

        public override void PopShell(float x, float y, int dir)
        {
            MagnumShell magnumShell = new MagnumShell(x, y);
            magnumShell.hSpeed = (float)dir * (1.5f + Rando.Float(1f));
            Level.Add((Thing)magnumShell);
        }
    }
}
