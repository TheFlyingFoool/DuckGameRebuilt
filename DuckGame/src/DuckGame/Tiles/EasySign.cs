using System;

namespace DuckGame
{
    [EditorGroup("Details|Signs")]
    public class EasySign : Thing
    {
        private SpriteMap _sprite;

        public EasySign(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("easySign", 32, 32);
            graphic = _sprite;
            center = new Vec2(16f, 24f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = -0.5f;
            _editorName = "Easy Sign";
            hugWalls = WallHug.Floor;
        }

        public override Type TabRotate(bool control)
        {
            if (control)
                return typeof(FishinSign);
            return base.TabRotate(control);
        }

        public override void Draw()
        {
            _sprite.frame = offDir > 0 ? 1 : 0;
            base.Draw();
        }
    }
}
