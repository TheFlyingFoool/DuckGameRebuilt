// Decompiled with JetBrains decompiler
// Type: DuckGame.Flare
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("smallFire", 16, 16);
            _sprite.AddAnimation("burn", (0.2f + Rando.Float(0.2f)), true, 0, 1, 2, 3, 4);
            _sprite.SetAnimation("burn");
            _sprite.imageIndex = Rando.Int(4);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -2f);
            collisionSize = new Vec2(8f, 4f);
            depth = -0.5f;
            thickness = 1f;
            weight = 1f;
            breakForce = 9999999f;
            _owner = owner;
            weight = 0.5f;
            gravMultiplier = 0.7f;
            _numFlames = numFlames;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (isServerForObject)
            {
                for (int index = 0; index < _numFlames; ++index)
                    Level.Add(SmallFire.New(x - hSpeed, y - vSpeed, Rando.Float(6f) - 3f, Rando.Float(6f) - 3f, firedFrom: this));
            }
            SFX.Play("flameExplode", 0.9f, Rando.Float(0.2f) - 0.1f);
            Level.Remove(this);
            return true;
        }

        public override void Update()
        {
            if (Rando.Float(2f) < 0.3f) vSpeed += Rando.Float(3.5f) - 2f;
            if (Rando.Float(9f) < 0.1f) vSpeed += Rando.Float(3.1f) - 3f;
            if (Rando.Float(14f) < 0.1f) vSpeed += Rando.Float(4f) - 5f;
            if (Rando.Float(25f) < 0.1f) vSpeed += Rando.Float(6f) - 7f;
            if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(x, y));
            else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(x, y));
            if (hSpeed > 0f) _sprite.angleDegrees = 90f;
            else if (hSpeed < 0f) _sprite.angleDegrees = -90f;
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!isServerForObject || with == _owner || with is Gun || with.weight < 5)
                return;
            if (with is PhysicsObject)
            {
                with.hSpeed = hSpeed / 4f;
                --with.vSpeed;
            }
            Destroy(new DTImpact(null));
            with.Burn(position, this);
        }
    }
}
