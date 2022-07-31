// Decompiled with JetBrains decompiler
// Type: DuckGame.ATPellet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ATPellet : AmmoType
    {
        public ATPellet()
        {
            this.accuracy = 1f;
            this.range = 4000f;
            this.penetration = 0.4f;
            this.bulletSpeed = 18f;
            this.gravityMultiplier = 2f;
            this.affectedByGravity = true;
            this.speedVariation = 0f;
            this.rebound = true;
            this.softRebound = true;
            this.weight = 5f;
            this.bulletThickness = 1f;
            this.bulletColor = Color.White;
            this.sprite = new Sprite("pellet")
            {
                center = new Vec2(1f, 1f)
            };
            this.bulletType = typeof(PelletBullet);
            this.flawlessPipeTravel = true;
        }
    }
}
