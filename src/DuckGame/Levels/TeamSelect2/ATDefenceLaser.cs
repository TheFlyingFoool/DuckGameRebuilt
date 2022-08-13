// Decompiled with JetBrains decompiler
// Type: DuckGame.ATDefenceLaser
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATDefenceLaser : ATLaser
    {
        public ATDefenceLaser()
        {
            accuracy = 1f;
            range = 600f;
            penetration = 1f;
            bulletSpeed = 30f;
            bulletLength = 40f;
            bulletThickness = 0.3f;
            rangeVariation = 50f;
            bulletType = typeof(LaserBulletPurple);
            angleShot = false;
        }
    }
}
