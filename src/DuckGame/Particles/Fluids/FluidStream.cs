// Decompiled with JetBrains decompiler
// Type: DuckGame.FluidStream
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class FluidStream : Thing
    {
        private Vec2 _sprayAngle;
        private Vec2 _startSprayAngle;
        private float _holeThickness = 1f;
        private float _sprayVelocity;
        private Vec2 _endPoint;
        private Vec2 _offset;
        private bool _onFire;
        private int _framesSinceFire;
        private Fluid _lastFluid;
        private int _framesSinceFluid;
        private float _fluctuate;
        private float _speedMul = 0.1f;
        private float _maxSpeedMul = 0.1f;
        public float streamSpeedMultiplier = 1f;

        public Vec2 sprayAngle
        {
            get => this._sprayAngle;
            set => this._sprayAngle = value;
        }

        public Vec2 startSprayAngle => this._startSprayAngle;

        public float holeThickness
        {
            get => this._holeThickness;
            set => this._holeThickness = value;
        }

        public Vec2 offset
        {
            get => this._offset;
            set => this._offset = value;
        }

        public bool onFire
        {
            get => this._onFire;
            set => this._onFire = value;
        }

        public FluidStream(float xpos, float ypos, Vec2 sprayAngleVal, float sprayVelocity, Vec2 off = default(Vec2))
          : base(xpos, ypos)
        {
            this._endPoint = new Vec2(xpos, ypos);
            this._sprayAngle = sprayAngleVal;
            this._startSprayAngle = sprayAngleVal;
            this._sprayVelocity = sprayVelocity;
            this._offset = off;
        }

        public void Feed(FluidData dat)
        {
            float to = Maths.Clamp(dat.amount * 200f, 0.1f, 2f);
            if (to > _maxSpeedMul)
                this._maxSpeedMul = Lerp.Float(this._maxSpeedMul, to, 0.1f);
            this._lastFluid = new Fluid(this.x, this.y, (this._sprayAngle * ((2f + (float)Math.Sin(_fluctuate) * 0.5f) * this._speedMul) + new Vec2(this.hSpeed * 0f, this.vSpeed * 0f)) * this.streamSpeedMultiplier, dat, this._lastFluid);
            Level.Add(_lastFluid);
            this._framesSinceFluid = 0;
            if (dat.flammable > 0.5f && this.onFire && this._framesSinceFire > 12 && Rando.Float(1f) < 0.12f * dat.flammable)
            {
                SmallFire smallFire = SmallFire.New(this._lastFluid.x, this._lastFluid.y, 0f, 0f);
                this._lastFluid.fire = smallFire;
                Level.Add(smallFire);
                this._framesSinceFire = 0;
            }
            this._fluctuate += 0.2f;
        }

        public override void Update()
        {
            ++this._framesSinceFire;
            this._maxSpeedMul = Lerp.Float(this._maxSpeedMul, 0.1f, 1f / 1000f);
            this._speedMul = Lerp.Float(this._speedMul, this._maxSpeedMul, 0.04f);
            if (this._lastFluid != null)
                ++this._framesSinceFluid;
            if (this._framesSinceFluid <= 12)
                return;
            this._framesSinceFluid = 0;
            this._lastFluid = null;
        }

        public override void Draw() => base.Draw();
    }
}
