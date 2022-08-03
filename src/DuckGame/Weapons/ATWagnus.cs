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
            accuracy = 1f;
            range = 128f;
            penetration = 2f;
            bulletSpeed = 25f;
            bulletLength = 40f;
            bulletThickness = 0.3f;
            rangeVariation = 0f;
            barrelAngleDegrees = 180f;
            bulletType = typeof(LaserBulletPurple);
            canBeReflected = false;
            canTeleport = false;
        }
    }
}
