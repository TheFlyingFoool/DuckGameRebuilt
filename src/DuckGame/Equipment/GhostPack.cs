// Decompiled with JetBrains decompiler
// Type: DuckGame.GhostPack
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class GhostPack : Jetpack
    {
        public GhostPack(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("jetpack", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(11f, 12f);
            _offset = new Vec2(-3f, 3f);
            thickness = 0.1f;
        }

        public override void Draw()
        {
            _heat = 0.01f;
            if (_equippedDuck != null)
            {
                depth = -0.5f;
                Vec2 offset = _offset;
                if (duck.offDir < 0)
                    offset.x *= -1f;
                position = duck.position + offset;
            }
            else
                depth = (Depth)0f;
        }
    }
}
