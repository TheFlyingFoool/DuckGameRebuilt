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
            accuracy = 1f;
            range = 4000f;
            penetration = 0.4f;
            bulletSpeed = 18f;
            gravityMultiplier = 2f;
            affectedByGravity = true;
            speedVariation = 0f;
            rebound = true;
            softRebound = true;
            weight = 5f;
            bulletThickness = 1f;
            bulletColor = Color.White;
            sprite = new Sprite("pellet")
            {
                center = new Vec2(1f, 1f)
            };
            bulletType = typeof(PelletBullet);
            flawlessPipeTravel = true;
        }
    }
}
