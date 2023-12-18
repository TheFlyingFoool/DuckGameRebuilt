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
