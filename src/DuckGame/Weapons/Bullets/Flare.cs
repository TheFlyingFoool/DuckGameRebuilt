// Decompiled with JetBrains decompiler
// Type: DuckGame.Flare
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Flare : PhysicsObject, IPlatform
    {
        private SpriteMap _sprite;
        private FlareGun _owner;
        private int _numFlames;

        public Flare(float xpos, float ypos, FlareGun owner, int numFlames = 8)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("smallFire", 16, 16);
            this._sprite.AddAnimation("burn", (float)(0.200000002980232 + (double)Rando.Float(0.2f)), true, 0, 1, 2, 3, 4);
            this._sprite.SetAnimation("burn");
            this._sprite.imageIndex = Rando.Int(4);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -2f);
            this.collisionSize = new Vec2(8f, 4f);
            this.depth = -0.5f;
            this.thickness = 1f;
            this.weight = 1f;
            this.breakForce = 9999999f;
            this._owner = owner;
            this.weight = 0.5f;
            this.gravMultiplier = 0.7f;
            this._numFlames = numFlames;
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
            if ((double)Rando.Float(2f) < 0.300000011920929)
                this.vSpeed += Rando.Float(3.5f) - 2f;
            if ((double)Rando.Float(9f) < 0.100000001490116)
                this.vSpeed += Rando.Float(3.1f) - 3f;
            if ((double)Rando.Float(14f) < 0.100000001490116)
                this.vSpeed += Rando.Float(4f) - 5f;
            if ((double)Rando.Float(25f) < 0.100000001490116)
                this.vSpeed += Rando.Float(6f) - 7f;
            Level.Add(SmallSmoke.New(this.x, this.y));
            if ((double)this.hSpeed > 0.0)
                this._sprite.angleDegrees = 90f;
            else if ((double)this.hSpeed < 0.0)
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
