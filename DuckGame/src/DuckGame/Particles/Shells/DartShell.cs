// Decompiled with JetBrains decompiler
// Type: DuckGame.DartShell
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DartShell : PhysicsParticle
    {
        private SpriteMap _sprite;
        private float _rotSpeed;
        private bool _die;

        public DartShell(float xpos, float ypos, float rotSpeed, bool flip)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("dart", 16, 16)
            {
                flipH = flip
            };
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _bounceSound = "";
            _rotSpeed = rotSpeed;
            depth = (Depth)0.3f;
        }

        public override void Update()
        {
            base.Update();
            angle += _rotSpeed;
            if (vSpeed < 0f || _grounded) _die = true;
            if (_die) alpha -= 0.05f;
            if (alpha > 0f) return;
            Level.Remove(this);
        }
    }
}
