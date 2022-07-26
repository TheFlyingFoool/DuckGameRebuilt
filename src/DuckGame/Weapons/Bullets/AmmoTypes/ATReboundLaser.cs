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
            this.accuracy = 0.8f;
            this.range = 220f;
            this.penetration = 1f;
            this.bulletSpeed = 20f;
            this.bulletThickness = 0.3f;
            this.rebound = true;
            this.bulletType = typeof(LaserBullet);
            this.angleShot = true;
        }
    }
}
