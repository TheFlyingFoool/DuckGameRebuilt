// Decompiled with JetBrains decompiler
// Type: DuckGame.Fluid
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._child;
            set => this._child = value;
        }

        public SmallFire fire
        {
            get => this._fire;
            set => this._fire = value;
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
            this.hSpeed = (-hitAngle.x * 2f * (Rando.Float(1f) + 0.3f));
            this.vSpeed = (-hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
            this.hSpeed = hitAngle.x;
            this.vSpeed = hitAngle.y;
            this._bounceEfficiency = 0.6f;
            this._stream = stream;
            if (stream != null)
                stream.child = this;
            this.alpha = 1f;
            this._gravMult = 2f;
            this.depth = -0.5f;
            this.data = dat;
            this._thickMult = thickMult;
            this._thickness = Maths.Clamp(this.data.amount * 600f, 0.2f, 8f) * this._thickMult;
            this.startThick = this._thickness;
            this._glob = new SpriteMap("bigGlob", 8, 8);
        }

        public override void Update()
        {
            if (this._fire != null)
                this._fire.position = this.position;
            this._life = 1f;
            if (_thickness < 4f || Math.Abs(this.vSpeed) < 1.5)
                this.live -= 0.01f;
            this._thickness = Lerp.FloatSmooth(this.startThick, 0.1f, 1f - this.live);
            if (live < 0f || this._grounded && Math.Abs(this.vSpeed) < 0.1f)
            {
                Level.Remove(this);
                this.active = false;
                FluidPuddle fluidPuddle1 = null;
                foreach (FluidPuddle fluidPuddle2 in Level.current.things[typeof(FluidPuddle)])
                {
                    if (this.x > fluidPuddle2.left && this.x < fluidPuddle2.right && Math.Abs(fluidPuddle2.y - this.y) < 10.0)
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
                fluidPuddle1?.Feed(this.data);
            }
            else
            {
                base.Update();
                if (this._touchedFloor && !this._firstHit)
                {
                    this._firstHit = true;
                    this.hSpeed += Rando.Float(-1f, 1f);
                    this.hSpeed *= Rando.Float(-1f, 1.5f);
                    this.vSpeed *= Rando.Float(0.3f, 1f);
                }
                if (this._stream == null)
                    return;
                float num = Math.Abs(this.hSpeed - this._stream.hSpeed);
                if (Math.Abs(this.x - this._stream.x) * num <= 40f && Math.Abs(this.vSpeed - this._stream.vSpeed) <= 1.9f && num <= 1.9f)
                    return;
                this.BreakStream();
            }
        }

        public void BreakStream()
        {
            if (this._child != null)
                this._child._stream = null;
            this._child = null;
            if (this._stream != null)
                this._stream._child = null;
            this._stream = null;
        }

        public override void Draw()
        {
            if (this._stream != null)
            {
                ++Graphics.currentDrawIndex;
                Graphics.DrawLine(this.position, this._stream.position, new Color(this.data.color) * this.alpha, this._thickness, this.depth);
            }
            else
            {
                if (this._child != null)
                    return;
                if (_thickness > 4.0)
                {
                    this._glob.depth = this.depth;
                    this._glob.frame = 2;
                    this._glob.color = new Color(this.data.color) * this.alpha;
                    this._glob.CenterOrigin();
                    this._glob.angle = Maths.DegToRad((float)(-Maths.PointDirection(this.position, this.position + this.velocity) + 90.0));
                    Graphics.Draw(_glob, this.x, this.y);
                }
                else
                    Graphics.DrawRect(this.position - new Vec2(this._thickness / 2f, this._thickness / 2f), this.position + new Vec2(this._thickness / 2f, this._thickness / 2f), new Color(this.data.color) * this.alpha, this.depth);
            }
        }
    }
}
