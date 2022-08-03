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
            depth = (Depth)deep;
            _sprite = spr;
            _sprite.CenterOrigin();
            layer = Layer.Foreground;
        }

        public override void Update()
        {
            if (_sprite.finished)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            _sprite.scale = scale;
            _sprite.alpha = alpha;
            _sprite.color = color;
            _sprite.depth = depth;
            _sprite.flipH = flipHorizontal;
            Graphics.Draw(_sprite, x, y);
            base.Draw();
        }
    }
}
