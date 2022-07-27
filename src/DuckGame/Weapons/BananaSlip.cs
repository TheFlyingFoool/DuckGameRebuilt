// Decompiled with JetBrains decompiler
// Type: DuckGame.BananaSlip
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BananaSlip : Thing
    {
        private SpriteMap _sprite;

        public BananaSlip(float xpos, float ypos, bool flip)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("slip", 32, 32);
            this._sprite.AddAnimation("slip", 0.45f, false, 0, 1, 2, 3);
            this._sprite.SetAnimation("slip");
            this.center = new Vec2(19f, 31f);
            this.graphic = _sprite;
            this._sprite.flipH = flip;
        }

        public override void Update()
        {
            if (!this._sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
