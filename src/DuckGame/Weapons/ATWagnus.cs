// Decompiled with JetBrains decompiler
// Type: DuckGame.ATWagnus
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATWagnus : AmmoType
    {
        public ATWagnus()
        {
            this.accuracy = 1f;
            this.range = 128f;
            this.penetration = 2f;
            this.bulletSpeed = 25f;
            this.bulletLength = 40f;
            this.bulletThickness = 0.3f;
            this.rangeVariation = 0.0f;
            this.barrelAngleDegrees = 180f;
            this.bulletType = typeof(LaserBulletPurple);
            this.canBeReflected = false;
            this.canTeleport = false;
        }
    }
}
