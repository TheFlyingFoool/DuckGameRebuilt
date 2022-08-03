// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterCooler
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details")]
    public class WaterCooler : MaterialThing, IPlatform
    {
        private SpriteMap _sprite;
        private SpriteMap _jugLine;
        private Sprite _bottom;
        private SinWave _colorFlux = (SinWave)0.1f;
        protected float _fluidLevel = 1f;
        protected int _alternate;
        private List<FluidStream> _holes = new List<FluidStream>();
        protected FluidData _fluid;
        public float _shakeMult;
        private float _shakeInc;

        public WaterCooler(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("waterCoolerJug", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(10f, 10f);
            depth = -0.5f;
            _editorName = "Water Cooler";
            editorTooltip = "Looking for all the latest hot gossip? This is the place to hang.";
            thickness = 2f;
            weight = 5f;
            _jugLine = new SpriteMap("waterCoolerJugLine", 16, 16);
            _jugLine.CenterOrigin();
            flammable = 0.3f;
            _bottom = new Sprite("waterCoolerBottom");
            _bottom.CenterOrigin();
            editorOffset = new Vec2(0f, -8f);
            _fluid = Fluid.Water;
        }

        protected override bool OnDestroy(DestroyType type = null) => true;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            hitPos += bullet.travelDirNormalized * 2f;
            if (1.0 - (hitPos.y - top) / (bottom - top) < _fluidLevel)
            {
                thickness = 2f;
                Vec2 off = hitPos - position;
                bool flag = false;
                foreach (FluidStream hole in _holes)
                {
                    if ((hole.offset - off).length < 2.0)
                    {
                        hole.offset = off;
                        hole.holeThickness += 0.5f;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    FluidStream fluidStream = new FluidStream(0f, 0f, (-bullet.travelDirNormalized).Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off);
                    _holes.Add(fluidStream);
                    fluidStream.streamSpeedMultiplier = 2f;
                }
                _shakeMult = 1f;
                SFX.Play("bulletHitWater", pitch: Rando.Float(-0.2f, 0.2f));
                return base.Hit(bullet, hitPos);
            }
            thickness = 1f;
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos -= bullet.travelDirNormalized * 2f;
            Vec2 off = exitPos - position;
            bool flag = false;
            foreach (FluidStream hole in _holes)
            {
                if ((hole.offset - off).length < 2.0)
                {
                    hole.offset = off;
                    hole.holeThickness += 0.5f;
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            _holes.Add(new FluidStream(0f, 0f, bullet.travelDirNormalized.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void Update()
        {
            base.Update();
            _shakeInc += 0.8f;
            _shakeMult = Lerp.Float(_shakeMult, 0f, 0.05f);
            if (_alternate == 0)
            {
                foreach (FluidStream hole in _holes)
                {
                    hole.onFire = onFire;
                    hole.hSpeed = hSpeed;
                    hole.vSpeed = vSpeed;
                    hole.DoUpdate();
                    hole.position = Offset(hole.offset);
                    hole.sprayAngle = OffsetLocal(hole.startSprayAngle);
                    float num1 = (float)(1.0 - (hole.offset.y - topLocal) / (bottomLocal - topLocal));
                    if (hole.x > left - 2.0 && hole.x < right + 2.0 && num1 < _fluidLevel)
                    {
                        float num2 = Maths.Clamp(_fluidLevel - num1, 0.1f, 1f) * 0.0012f * hole.holeThickness;
                        FluidData fluid = _fluid;
                        fluid.amount = num2;
                        hole.Feed(fluid);
                        _fluidLevel -= num2;
                    }
                }
            }
            weight = _fluidLevel * 10f;
            ++_alternate;
            if (_alternate <= 4)
                return;
            _alternate = 0;
        }

        public override void Draw()
        {
            _sprite.frame = (int)((1.0 - _fluidLevel) * 10.0);
            Vec2 position = this.position;
            float num = (float)(Math.Sin(_shakeInc) * _shakeMult * 1.0);
            this.position.x += num;
            base.Draw();
            this.position = position;
            _bottom.depth = depth + 1;
            Graphics.Draw(_bottom, x, y + 9f);
            _jugLine.depth = depth + 1;
            _jugLine.imageIndex = _sprite.imageIndex;
            _jugLine.alpha = (float)(_fluidLevel * 10.0 % 1.0);
            Graphics.Draw(_jugLine, x + num, y);
        }
    }
}
