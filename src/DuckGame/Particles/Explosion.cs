// Decompiled with JetBrains decompiler
// Type: DuckGame.ExplosionPart
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ExplosionPart : Thing
    {
        private bool _created;
        private SpriteMap _sprite;
        private float _wait;
        private int _smokeFrame;
        private bool _smoked;

        public ExplosionPart(float xpos, float ypos, bool doWait = true)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("explosion", 64, 64);
            switch (Rando.ChooseInt(0, 1, 2))
            {
                case 0:
                    this._sprite.AddAnimation("explode", 1f, false, 0, 0, 2, 3, 4, 5, 6, 7, 8, 9, 10);
                    break;
                case 1:
                    this._sprite.AddAnimation("explode", 1.2f, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                    break;
                case 2:
                    this._sprite.AddAnimation("explode", 0.9f, false, 3, 4, 5, 6, 7, 8, 9);
                    break;
            }
            this._sprite.SetAnimation("explode");
            this.graphic = _sprite;
            this._sprite.speed = 0.4f + Rando.Float(0.2f);
            this.xscale = 0.5f + Rando.Float(0.5f);
            this.yscale = this.xscale;
            this.center = new Vec2(32f, 32f);
            this._wait = Rando.Float(1f);
            this._smokeFrame = Rando.Int(1, 3);
            this.depth = (Depth)1f;
            this.vSpeed = Rando.Float(-0.2f, -0.4f);
            if (doWait)
                return;
            this._wait = 0.0f;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (!this._created)
                this._created = true;
            if (this._sprite.frame > this._smokeFrame && !this._smoked)
            {
                int num1 = Graphics.effectsLevel == 2 ? Rando.Int(1, 4) : 1;
                for (int index = 0; index < num1; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-5f, 5f), this.y + Rando.Float(-5f, 5f));
                    smallSmoke.vSpeed = Rando.Float(0.0f, -0.5f);
                    double num2;
                    float num3 = (float)(num2 = (double)Rando.Float(0.2f, 0.7f));
                    smallSmoke.yscale = (float)num2;
                    smallSmoke.xscale = num3;
                    Level.Add(smallSmoke);
                }
                this._smoked = true;
            }
            if (_wait <= 0.0)
                this.y += this.vSpeed;
            if (!this._sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw()
        {
            if (_wait > 0.0)
                this._wait -= 0.2f;
            else
                base.Draw();
        }
    }
}
