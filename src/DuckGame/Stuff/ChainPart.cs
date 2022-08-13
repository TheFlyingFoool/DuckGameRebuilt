// Decompiled with JetBrains decompiler
// Type: DuckGame.ChainPart
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class ChainPart : Vine
    {
        public ChainPart(float xpos, float ypos, float init)
          : base(xpos, ypos, init)
        {
            _sprite = new SpriteMap("chain", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(11f, 7f);
            weight = 0.1f;
            thickness = 0.1f;
            canPickUp = false;
            initLength = init;
            depth = -0.5f;
            _vinePartSprite = new Sprite("chain")
            {
                center = new Vec2(8f, 0f)
            };
        }
    }
}
