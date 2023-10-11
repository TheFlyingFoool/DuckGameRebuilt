using System;

namespace DuckGame
{
    public class Fluid : PhysicsParticle
    {
        public Fluid child
        {
            get
            {
                return _child;
            }
            set
            {
                _child = value;
            }
        }

        public SmallFire fire
        {
            get
            {
                return _fire;
            }
            set
            {
                _fire = value;
                fireset = false;
                if (_fire != null)
                {
                    fireset = true;
                }
            }
        }

        public Fluid(float xpos, float ypos, Vec2 hitAngle, FluidData dat, Fluid stream = null, float thickMult = 1f) : base(xpos, ypos)
        {
            hSpeed = -hitAngle.x * 2f * (Rando.Float(1f) + 0.3f);
            vSpeed = -hitAngle.y * 2f * (Rando.Float(1f) + 0.3f) - Rando.Float(2f);
            hSpeed = hitAngle.x;
            vSpeed = hitAngle.y;
            _bounceEfficiency = 0.6f;
            _stream = stream;
            if (stream != null)
            {
                stream.child = this;
            }
            alpha = 1f;
            _gravMult = 2f;
            depth = -0.5f;
            data = dat;
            _thickMult = thickMult;
            _thickness = Maths.Clamp(data.amount * 600f, 0.2f, 8f) * _thickMult;
            startThick = _thickness;
            _glob = new SpriteMap("bigGlob", 8, 8, false);
        }

        public override void Update()
        {
            if (fireset)
            {
                _fire.position = position;
            }
            _life = 1f;
            if (_thickness < 4f || Math.Abs(vSpeed) < 1.5f)
            {
                live -= 0.01f;
            }
            _thickness = Lerp.FloatSmooth(startThick, 0.1f, 1f - live, 1f);
            if (live < 0f || (_grounded && Math.Abs(vSpeed) < 0.1f))
            {
                Level.Remove(this);
                active = false;
                FluidPuddle p = null;
                foreach (FluidPuddle puddle in Level.current.things[typeof(FluidPuddle)])
                {
                    if (x > puddle.left && x < puddle.right && Math.Abs(puddle.y - y) < 10f)
                    {
                        p = puddle;
                        break;
                    }
                }
                if (p == null)
                {
                    Vec2 hitPos;
                    Block b = Level.CheckLine<AutoBlock>(position + new Vec2(0f, -8f), position + new Vec2(0f, 16f), out hitPos);
                    if (b != null && hitPos.y == b.top)
                    {
                        p = new FluidPuddle(hitPos.x, hitPos.y, b);
                        Level.Add(p);
                    }
                }
                if (p != null)
                {
                    p.Feed(data);
                }
                return;
            }
            base.Update();
            if (_touchedFloor && !_firstHit)
            {
                _firstHit = true;
                hSpeed += Rando.Float(-1f, 1f);
                hSpeed *= Rando.Float(-1f, 1.5f);
                vSpeed *= Rando.Float(0.3f, 1f);
            }
            if (_stream != null)
            {
                float hDif = Math.Abs(hSpeed - _stream.hSpeed);
                if (Math.Abs(x - _stream.x) * hDif > 40f || Math.Abs(vSpeed - _stream.vSpeed) > 1.9f || hDif > 1.9f)
                {
                    BreakStream();
                }
            }
        }

        public void BreakStream()
        {
            if (_child != null)
            {
                _child._stream = null;
            }
            _child = null;
            if (_stream != null)
            {
                _stream._child = null;
            }
            _stream = null;
        }

        public override void Draw()
        {
            if (_stream != null)
            {
                Graphics.currentDrawIndex++;
                Graphics.DrawLine(position, _stream.position, new Color(data.color) * alpha, _thickness, depth);
                return;
            }
            if (_child == null)
            {
                if (_thickness > 4f)
                {
                    _glob.depth = depth;
                    _glob.frame = 2;
                    _glob.color = new Color(data.color) * alpha;
                    _glob.CenterOrigin();
                    _glob.angle = Maths.DegToRad(-Maths.PointDirection(position, position + velocity) + 90f);
                    Graphics.Draw(ref _glob, x, y);
                    return;
                }
                Graphics.DrawRect(position - new Vec2(_thickness / 2f, _thickness / 2f), position + new Vec2(_thickness / 2f, _thickness / 2f), new Color(data.color) * alpha, depth, true, 1f);
            }
        }

        // Note: this type is marked as 'beforefieldinit'.
        static Fluid()
        {
        }

        public static FluidData Lava = new FluidData(0f, new Color(255, 89, 5).ToVector4(), 0f, "lava", 1f, 0.8f);

        public static FluidData Gas = new FluidData(0f, new Color(246, 198, 55).ToVector4(), 1f, "gas", 0f, 0.7f);

        public static FluidData Water = new FluidData(0f, new Color(0, 150, 249).ToVector4(), 0f, "water", 0f, 0.7f);

        public static FluidData Ketchup = new FluidData(0f, Color.Red.ToVector4() * 0.8f, 0.4f, "water", 0f, 0.7f);

        public static FluidData Poo = new FluidData(0f, Color.SaddleBrown.ToVector4() * 0.8f, 0.5f, "water", 0f, 0.7f);

        private Fluid _stream;

        private Fluid _child;

        private bool _firstHit;

        private float _thickness;

        public FluidData data;

        private float _thickMult;

        private SmallFire _fire;

        public SpriteMap _glob;

        private float startThick;

        private float live = 1f;

        private bool fireset;
    }
}
