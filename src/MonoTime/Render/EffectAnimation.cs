// Decompiled with JetBrains decompiler
// Type: DuckGame.EffectAnimation
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EffectAnimation : Thing
    {
        protected SpriteMap _sprite;
        public Color color = Color.White;

        public EffectAnimation(Vec2 pos, SpriteMap spr, float deep)
          : base(pos.x, pos.y)
        {
            this.depth = (Depth)deep;
            this._sprite = spr;
            this._sprite.CenterOrigin();
            this.layer = Layer.Foreground;
        }

        public override void Update()
        {
            if (this._sprite.finished)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            this._sprite.scale = this.scale;
            this._sprite.alpha = this.alpha;
            this._sprite.color = this.color;
            this._sprite.depth = this.depth;
            this._sprite.flipH = this.flipHorizontal;
            Graphics.Draw(_sprite, this.x, this.y);
            base.Draw();
        }
    }
}
