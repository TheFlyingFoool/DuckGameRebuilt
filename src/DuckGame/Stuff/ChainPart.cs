// Decompiled with JetBrains decompiler
// Type: DuckGame.ChainPart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("chain", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -4f);
            this.collisionSize = new Vec2(11f, 7f);
            this.weight = 0.1f;
            this.thickness = 0.1f;
            this.canPickUp = false;
            this.initLength = init;
            this.depth = - 0.5f;
            this._vinePartSprite = new Sprite("chain")
            {
                center = new Vec2(8f, 0.0f)
            };
        }
    }
}
