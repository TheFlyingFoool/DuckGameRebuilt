// Decompiled with JetBrains decompiler
// Type: DuckGame.EyeCloseWing
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EyeCloseWing : Thing
    {
        private SpriteMap _sprite;
        private float _move;
        private int _dir;
        private Duck _closer;

        public EyeCloseWing(float xpos, float ypos, int dir, SpriteMap s, Duck own, Duck closer)
          : base(xpos, ypos)
        {
            _sprite = s.CloneMap();
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _dir = dir;
            depth = (Depth)0.9f;
            if (_dir < 0)
                angleDegrees = 70f;
            else
                angleDegrees = 120f;
            owner = own;
            _closer = closer;
            if (_dir >= 0)
                return;
            x += 14f;
        }

        public override void Update()
        {
            float num = 0.3f;
            x += _dir * num;
            _move += num;
            if (_dir < 0)
                angleDegrees += 2f;
            else
                angleDegrees -= 2f;
            if (_move > 4)
                _closer.eyesClosed = true;
            if (_move <= 8)
                return;
            Level.Remove(this);
            (_owner as Duck).closingEyes = false;
        }

        public override void Draw()
        {
            int frame = _sprite.frame;
            _sprite.flipV = _dir <= 0;
            _sprite.flipH = false;
            _sprite.frame = 18;
            base.Draw();
            _sprite.frame = frame;
        }
    }
}
