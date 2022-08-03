// Decompiled with JetBrains decompiler
// Type: DuckGame.ATReboundLaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATReboundLaser : ATLaser
    {
        public ATReboundLaser()
        {
            accuracy = 0.8f;
            range = 220f;
            penetration = 1f;
            bulletSpeed = 20f;
            bulletThickness = 0.3f;
            rebound = true;
            bulletType = typeof(LaserBullet);
            angleShot = true;
        }
    }
}
