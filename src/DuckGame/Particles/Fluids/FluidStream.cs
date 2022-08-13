// Decompiled with JetBrains decompiler
// Type: DuckGame.FluidStream
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _sprayAngle;
            set => _sprayAngle = value;
        }

        public Vec2 startSprayAngle => _startSprayAngle;

        public float holeThickness
        {
            get => _holeThickness;
            set => _holeThickness = value;
        }

        public Vec2 offset
        {
            get => _offset;
            set => _offset = value;
        }

        public bool onFire
        {
            get => _onFire;
            set => _onFire = value;
        }

        public FluidStream(float xpos, float ypos, Vec2 sprayAngleVal, float sprayVelocity, Vec2 off = default(Vec2))
          : base(xpos, ypos)
        {
            _endPoint = new Vec2(xpos, ypos);
            _sprayAngle = sprayAngleVal;
            _startSprayAngle = sprayAngleVal;
            _sprayVelocity = sprayVelocity;
            _offset = off;
        }

        public void Feed(FluidData dat)
        {
            float to = Maths.Clamp(dat.amount * 200f, 0.1f, 2f);
            if (to > _maxSpeedMul)
                _maxSpeedMul = Lerp.Float(_maxSpeedMul, to, 0.1f);
            _lastFluid = new Fluid(x, y, (_sprayAngle * ((2f + (float)Math.Sin(_fluctuate) * 0.5f) * _speedMul) + new Vec2(hSpeed * 0f, vSpeed * 0f)) * streamSpeedMultiplier, dat, _lastFluid);
            Level.Add(_lastFluid);
            _framesSinceFluid = 0;
            if (dat.flammable > 0.5f && onFire && _framesSinceFire > 12 && Rando.Float(1f) < 0.12f * dat.flammable)
            {
                SmallFire smallFire = SmallFire.New(_lastFluid.x, _lastFluid.y, 0f, 0f);
                _lastFluid.fire = smallFire;
                Level.Add(smallFire);
                _framesSinceFire = 0;
            }
            _fluctuate += 0.2f;
        }

        public override void Update()
        {
            ++_framesSinceFire;
            _maxSpeedMul = Lerp.Float(_maxSpeedMul, 0.1f, 1f / 1000f);
            _speedMul = Lerp.Float(_speedMul, _maxSpeedMul, 0.04f);
            if (_lastFluid != null)
                ++_framesSinceFluid;
            if (_framesSinceFluid <= 12)
                return;
            _framesSinceFluid = 0;
            _lastFluid = null;
        }

        public override void Draw() => base.Draw();
    }
}
