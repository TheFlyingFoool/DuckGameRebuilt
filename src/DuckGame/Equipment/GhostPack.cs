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
            this._sprite = new SpriteMap("jetpack", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(11f, 12f);
            this._offset = new Vec2(-3f, 3f);
            this.thickness = 0.1f;
        }

        public override void Draw()
        {
            this._heat = 0.01f;
            if (this._equippedDuck != null)
            {
                this.depth = - 0.5f;
                Vec2 offset = this._offset;
                if (this.duck.offDir < 0)
                    offset.x *= -1f;
                this.position = this.duck.position + offset;
            }
            else
                this.depth = (Depth)0.0f;
        }
    }
}
