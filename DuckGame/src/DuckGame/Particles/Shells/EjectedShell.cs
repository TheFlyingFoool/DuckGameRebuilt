// Decompiled with JetBrains decompiler
// Type: DuckGame.EjectedShell
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class EjectedShell : PhysicsParticle
    {
        private SpriteMap _sprite;

        protected EjectedShell(float xpos, float ypos, string shellSprite, string bounceSound = "metalBounce")
          : base(xpos, ypos)
        {
            hSpeed = -4f - Rando.Float(3f);
            vSpeed = (float)-(Rando.Float(1.5f) + 1);
            _sprite = new SpriteMap(shellSprite, 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _bounceSound = bounceSound;
            depth = (Depth)(0.3f + Rando.Float(0f, 0.1f));
        }

        public override void Update()
        {
            base.Update();
            _angle = Maths.DegToRad(-_spinAngle);
        }
    }
}
