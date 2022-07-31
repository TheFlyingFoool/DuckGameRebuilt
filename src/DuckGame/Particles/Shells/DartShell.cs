// Decompiled with JetBrains decompiler
// Type: DuckGame.DartShell
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("dart", 16, 16)
            {
                flipH = flip
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this._bounceSound = "";
            this._rotSpeed = rotSpeed;
            this.depth = (Depth)0.3f;
        }

        public override void Update()
        {
            base.Update();
            this.angle += this._rotSpeed;
            if (this.vSpeed < 0.0 || this._grounded)
                this._die = true;
            if (this._die)
                this.alpha -= 0.05f;
            if (this.alpha > 0.0)
                return;
            Level.Remove(this);
        }
    }
}
