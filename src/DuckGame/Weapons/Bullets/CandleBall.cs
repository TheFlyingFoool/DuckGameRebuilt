// Decompiled with JetBrains decompiler
// Type: DuckGame.CandleBall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    public class CandleBall : PhysicsObject, IPlatform
    {
        private SpriteMap _sprite;
        private FlareGun _owner;
        private int _numFlames;

        public CandleBall(float xpos, float ypos, FlareGun owner, int numFlames = 8)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("candleBall", 16, 16);
            this._sprite.AddAnimation("burn", (0.4f + Rando.Float(0.2f)), true, 0, 1);
            this._sprite.SetAnimation("burn");
            this._sprite.imageIndex = Rando.Int(4);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -2f);
            this.collisionSize = new Vec2(9f, 4f);
            this.depth = (Depth)0.9f;
            this.thickness = 1f;
            this.weight = 1f;
            this.breakForce = 1E+08f;
            this._owner = owner;
            this.weight = 0.5f;
            this.gravMultiplier = 0.7f;
            this._numFlames = numFlames;
            Color[] source = new Color[4]
            {
        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Orange
            };
            this._sprite.color = source[Rando.Int(source.Count<Color>() - 1)];
            this.xscale = this.yscale = Rando.Float(0.4f, 0.8f);
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this.isServerForObject)
            {
                for (int index = 0; index < this._numFlames; ++index)
                    Level.Add(SmallFire.New(this.x - this.hSpeed, this.y - this.vSpeed, Rando.Float(6f) - 3f, Rando.Float(6f) - 3f, firedFrom: this));
            }
            SFX.Play("flameExplode", 0.9f, Rando.Float(0.2f) - 0.1f);
            Level.Remove(this);
            return true;
        }

        public override void Update()
        {
            if (Rando.Float(2f) < 0.3f)
                this.vSpeed += Rando.Float(3.5f) - 2f;
            if (Rando.Float(9f) < 0.1f)
                this.vSpeed += Rando.Float(3.1f) - 3f;
            if (Rando.Float(14f) < 0.1f)
                this.vSpeed += Rando.Float(4f) - 5f;
            if (Rando.Float(25f) < 0.1f)
                this.vSpeed += Rando.Float(6f) - 7f;
            Level.Add(SmallSmoke.New(this.x, this.y));
            if (this.hSpeed > 0f)
                this._sprite.angleDegrees = 90f;
            else if (this.hSpeed < 0f)
                this._sprite.angleDegrees = -90f;
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this.isServerForObject || with == this._owner || with is Gun || (double)with.weight < 5.0)
                return;
            if (with is PhysicsObject)
            {
                with.hSpeed = this.hSpeed / 4f;
                --with.vSpeed;
            }
            this.Destroy(new DTImpact(null));
            with.Burn(this.position, this);
        }
    }
}
