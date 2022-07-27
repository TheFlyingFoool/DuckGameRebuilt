// Decompiled with JetBrains decompiler
// Type: DuckGame.Elevator
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Elevator : MaterialThing, IPlatform
    {
        private SpriteMap _sprite;

        public Elevator(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("elevator", 32, 37);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 13f);
            this.depth = -0.5f;
            this.thickness = 4f;
            this.weight = 7f;
            this.flammable = 0.3f;
            this.collideSounds.Add("rockHitGround2");
        }
    }
}
