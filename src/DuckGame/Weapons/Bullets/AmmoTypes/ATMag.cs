// Decompiled with JetBrains decompiler
// Type: DuckGame.ATMag
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATMag : AmmoType
    {
        public bool angleShot = true;

        public ATMag()
        {
            accuracy = 0.95f;
            range = 250f;
            penetration = 1f;
            bulletSpeed = 40f;
            bulletThickness = 0.3f;
            bulletType = typeof(MagBullet);
            combustable = true;
        }
    }
}
