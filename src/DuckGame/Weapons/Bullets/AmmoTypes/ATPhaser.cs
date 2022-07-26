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
            this.accuracy = 0.8f;
            this.range = 600f;
            this.penetration = 1f;
            this.bulletSpeed = 10f;
            this.bulletThickness = 0.3f;
            this.bulletLength = 40f;
            this.rangeVariation = 50f;
            this.bulletType = typeof(LaserBullet);
            this.angleShot = false;
        }

        public override void WriteAdditionalData(BitBuffer b)
        {
            base.WriteAdditionalData(b);
            b.Write(this.bulletThickness);
            b.Write(this.penetration);
        }

        public override void ReadAdditionalData(BitBuffer b)
        {
            base.ReadAdditionalData(b);
            this.bulletThickness = b.ReadFloat();
            this.penetration = b.ReadFloat();
        }
    }
}
