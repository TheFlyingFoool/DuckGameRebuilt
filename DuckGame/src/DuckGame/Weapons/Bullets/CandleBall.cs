// Decompiled with JetBrains decompiler
// Type: DuckGame.CandleBall
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("candleBall", 16, 16);
            _sprite.AddAnimation("burn", (0.4f + Rando.Float(0.2f)), true, 0, 1);
            _sprite.SetAnimation("burn");
            _sprite.imageIndex = Rando.Int(4);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -2f);
            collisionSize = new Vec2(9f, 4f);
            depth = (Depth)0.9f;
            thickness = 1f;
            weight = 1f;
            breakForce = 1E+08f;
            _owner = owner;
            weight = 0.5f;
            gravMultiplier = 0.7f;
            _numFlames = numFlames;
            Color[] source = new Color[4]
            {
        Color.Red,
        Color.Green,
        Color.Blue,
        Color.Orange
            };
            _sprite.color = source[Rando.Int(source.Count<Color>() - 1)];
            xscale = yscale = Rando.Float(0.4f, 0.8f);
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
            if (Rando.Float(2f) < 0.3f)
                vSpeed += Rando.Float(3.5f) - 2f;
            if (Rando.Float(9f) < 0.1f)
                vSpeed += Rando.Float(3.1f) - 3f;
            if (Rando.Float(14f) < 0.1f)
                vSpeed += Rando.Float(4f) - 5f;
            if (Rando.Float(25f) < 0.1f)
                vSpeed += Rando.Float(6f) - 7f;

            if (DGRSettings.S_ParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.S_ParticleMultiplier; i++) Level.Add(SmallSmoke.New(x, y));
            else if (Rando.Int(DGRSettings.S_ParticleMultiplier) > 0) Level.Add(SmallSmoke.New(x, y));
            if (hSpeed > 0f)
                _sprite.angleDegrees = 90f;
            else if (hSpeed < 0f)
                _sprite.angleDegrees = -90f;
            base.Update();
        }

        public override void OnImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!isServerForObject || with == _owner || with is Gun || with.weight < 5f)
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
