namespace DuckGame
{
    public abstract partial class FFBoundary
    {
        protected static SpriteMap _borderSpriteThick;
        protected static SpriteMap _borderSpriteThin;

        public Rectangle Bounds;
        public Depth Depth;
        public float Alpha = 1f;
        
        protected BorderStyle _borderStyle;
        protected SpriteMap _borderSprite => _borderStyle == BorderStyle.Thick ? _borderSpriteThick : _borderSpriteThin;
        
        static FFBoundary()
        {
            _borderSpriteThick = new SpriteMap("ff_icons/window_thick", 4, 4);
            _borderSpriteThin = new SpriteMap("ff_icons/window_thin", 4, 4);
        }

        protected FFBoundary(Rectangle bounds, Depth depth, BorderStyle borderStyle)
        {
            Bounds = bounds;
            Depth = depth;
            _borderStyle = borderStyle;
        }

        public virtual void Update(bool focus)
        {
            
        }

        public virtual void Draw(bool focus)
        {
            DrawBorder();
        }

        public void DrawBorder()
        {
            if (_borderStyle == BorderStyle.None)
                return;
            
            _borderSprite.depth = Depth;
            _borderSprite.alpha = Alpha;

            // expand outwards by 4px so border is outside _bounds
            Rectangle rect = Bounds.Shrink(-4);

            Graphics.Draw(_borderSprite, 0, rect.x, rect.y);
            Graphics.Draw(_borderSprite, 1, rect.x + _borderSprite.w, rect.y, scaleX: (rect.width / _borderSprite.w) - 2);
            Graphics.Draw(_borderSprite, 2, rect.Right - _borderSprite.w, rect.y);
            Graphics.Draw(_borderSprite, 3, rect.x, rect.y + _borderSprite.h, scaleY: (rect.height / _borderSprite.h) - 2);
            Graphics.Draw(_borderSprite, 4, rect.x + _borderSprite.w, rect.y + _borderSprite.h, scaleX: (rect.width / _borderSprite.w) - 2, scaleY: (rect.height / _borderSprite.h) - 2);
            Graphics.Draw(_borderSprite, 5, rect.Right - _borderSprite.w, rect.y + _borderSprite.h, scaleY: (rect.height / _borderSprite.h) - 2);
            Graphics.Draw(_borderSprite, 6, rect.x, rect.Bottom - _borderSprite.h);
            Graphics.Draw(_borderSprite, 7, rect.x + _borderSprite.w, rect.Bottom - _borderSprite.h, scaleX: (rect.width / _borderSprite.w) - 2);
            Graphics.Draw(_borderSprite, 8, rect.Right - _borderSprite.w, rect.Bottom - _borderSprite.h);
        }
    }
}