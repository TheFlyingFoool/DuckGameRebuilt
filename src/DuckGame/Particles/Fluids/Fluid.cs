// Decompiled with JetBrains decompiler
// Type: DuckGame.Fluid
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Fluid : PhysicsParticle
    {
        public static FluidData Lava = new FluidData(0f, new Color((int)byte.MaxValue, 89, 5).ToVector4(), 0f, "lava", 1f, 0.8f);
        public static FluidData Gas = new FluidData(0f, new Color(246, 198, 55).ToVector4(), 1f, "gas");
        public static FluidData Water = new FluidData(0f, new Color(0, 150, 249).ToVector4(), 0f, "water");
        public static FluidData Ketchup = new FluidData(0f, Color.Red.ToVector4() * 0.8f, 0.4f, "water");
        public static FluidData Poo = new FluidData(0f, Color.SaddleBrown.ToVector4() * 0.8f, 0.5f, "water");
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

        public Fluid child
        {
            get => _child;
            set => _child = value;
        }

        public SmallFire fire
        {
            get => _fire;
            set => _fire = value;
        }

        public Fluid(
          float xpos,
          float ypos,
          Vec2 hitAngle,
          FluidData dat,
          Fluid stream = null,
          float thickMult = 1f)
          : base(xpos, ypos)
        {
            hSpeed = (-hitAngle.x * 2f * (Rando.Float(1f) + 0.3f));
            vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
            hSpeed = hitAngle.x;
            vSpeed = hitAngle.y;
            _bounceEfficiency = 0.6f;
            _stream = stream;
            if (stream != null)
                stream.child = this;
            alpha = 1f;
            _gravMult = 2f;
            depth = -0.5f;
            data = dat;
            _thickMult = thickMult;
            _thickness = Maths.Clamp(data.amount * 600f, 0.2f, 8f) * _thickMult;
            startThick = _thickness;
            _glob = new SpriteMap("bigGlob", 8, 8);
        }

        public override void Update()
        {
            if (_fire != null)
                _fire.position = position;
            _life = 1f;
            if (_thickness < 4f || Math.Abs(vSpeed) < 1.5)
                live -= 0.01f;
            _thickness = Lerp.FloatSmooth(startThick, 0.1f, 1f - live);
            if (live < 0f || _grounded && Math.Abs(vSpeed) < 0.1f)
            {
                Level.Remove(this);
                active = false;
                FluidPuddle fluidPuddle1 = null;
                foreach (FluidPuddle fluidPuddle2 in Level.current.things[typeof(FluidPuddle)])
                {
                    if (x > fluidPuddle2.left && x < fluidPuddle2.right && Math.Abs(fluidPuddle2.y - y) < 10.0)
                    {
                        fluidPuddle1 = fluidPuddle2;
                        break;
                    }
                }
                if (fluidPuddle1 == null)
                {
                    Vec2 position;
                    Block b = Level.CheckLine<AutoBlock>(this.position + new Vec2(0f, -8f), this.position + new Vec2(0f, 16f), out position);
                    if (b != null && position.y == b.top)
                    {
                        fluidPuddle1 = new FluidPuddle(position.x, position.y, b);
                        Level.Add(fluidPuddle1);
                    }
                }
                fluidPuddle1?.Feed(data);
            }
            else
            {
                base.Update();
                if (_touchedFloor && !_firstHit)
                {
                    _firstHit = true;
                    hSpeed += Rando.Float(-1f, 1f);
                    hSpeed *= Rando.Float(-1f, 1.5f);
                    vSpeed *= Rando.Float(0.3f, 1f);
                }
                if (_stream == null)
                    return;
                float num = Math.Abs(hSpeed - _stream.hSpeed);
                if (Math.Abs(x - _stream.x) * num <= 40f && Math.Abs(vSpeed - _stream.vSpeed) <= 1.9f && num <= 1.9f)
                    return;
                BreakStream();
            }
        }

        public void BreakStream()
        {
            if (_child != null)
                _child._stream = null;
            _child = null;
            if (_stream != null)
                _stream._child = null;
            _stream = null;
        }

        public override void Draw()
        {
            if (_stream != null)
            {
                ++Graphics.currentDrawIndex;
                Graphics.DrawLine(position, _stream.position, new Color(data.color) * alpha, _thickness, depth);
            }
            else
            {
                if (_child != null)
                    return;
                if (_thickness > 4.0)
                {
                    _glob.depth = depth;
                    _glob.frame = 2;
                    _glob.color = new Color(data.color) * alpha;
                    _glob.CenterOrigin();
                    _glob.angle = Maths.DegToRad((float)(-Maths.PointDirection(position, position + velocity) + 90.0));
                    Graphics.Draw(_glob, x, y);
                }
                else
                    Graphics.DrawRect(position - new Vec2(_thickness / 2f, _thickness / 2f), position + new Vec2(_thickness / 2f, _thickness / 2f), new Color(data.color) * alpha, depth);
            }
        }
    }
}
