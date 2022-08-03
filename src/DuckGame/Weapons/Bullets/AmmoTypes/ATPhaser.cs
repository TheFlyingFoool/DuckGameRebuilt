// Decompiled with JetBrains decompiler
// Type: DuckGame.ATPhaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATPhaser : ATLaser
    {
        public ATPhaser()
        {
            accuracy = 0.8f;
            range = 600f;
            penetration = 1f;
            bulletSpeed = 10f;
            bulletThickness = 0.3f;
            bulletLength = 40f;
            rangeVariation = 50f;
            bulletType = typeof(LaserBullet);
            angleShot = false;
        }

        public override void WriteAdditionalData(BitBuffer b)
        {
            base.WriteAdditionalData(b);
            b.Write(bulletThickness);
            b.Write(penetration);
        }

        public override void ReadAdditionalData(BitBuffer b)
        {
            base.ReadAdditionalData(b);
            bulletThickness = b.ReadFloat();
            penetration = b.ReadFloat();
        }
    }
}
