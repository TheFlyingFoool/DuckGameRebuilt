// Decompiled with JetBrains decompiler
// Type: DuckGame.OpenPresent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class OpenPresent : PhysicsParticle
    {
        private SpriteMap _sprite;

        public OpenPresent(float xpos, float ypos, int frame)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("presents", 16, 16);
            this._sprite.frame = frame + 8;
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 13f);
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            this._bounceEfficiency = 0.0f;
            this.depth = (Depth)0.9f;
            this._life = 5f;
        }

        public override void Update() => base.Update();
    }
}
