// Decompiled with JetBrains decompiler
// Type: DuckGame.HeartPuff
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class HeartPuff : Thing
    {
        private SpriteMap _sprite;

        public HeartPuff(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("heartpuff", 16, 16);
            this._sprite.AddAnimation("wither", 0.35f, false, 0, 1, 2, 3, 4);
            this._sprite.SetAnimation("wither");
            this.center = new Vec2(5f, 16f);
            this.alpha = 0.6f;
            this.depth = (Depth)0.9f;
            this.graphic = (Sprite)this._sprite;
            this._sprite.color = Color.Green;
        }

        public override void Update()
        {
            if (this.anchor != (Thing)null && this.anchor.thing != null)
            {
                this.flipHorizontal = this.anchor.thing.offDir < (sbyte)0;
                if (this.flipHorizontal)
                    this.center = new Vec2(10f, 16f);
                else
                    this.center = new Vec2(5f, 16f);
                this.angle = this.anchor.thing.angle;
            }
            if (this._sprite.finished)
                Level.Remove((Thing)this);
            base.Update();
        }
    }
}
