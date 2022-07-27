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
            this._sprite = new SpriteMap("waterCoolerJug", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 10f);
            this.depth = - 0.5f;
            this._editorName = "Water Cooler";
            this.editorTooltip = "Looking for all the latest hot gossip? This is the place to hang.";
            this.thickness = 2f;
            this.weight = 5f;
            this._jugLine = new SpriteMap("waterCoolerJugLine", 16, 16);
            this._jugLine.CenterOrigin();
            this.flammable = 0.3f;
            this._bottom = new Sprite("waterCoolerBottom");
            this._bottom.CenterOrigin();
            this.editorOffset = new Vec2(0.0f, -8f);
            this._fluid = Fluid.Water;
        }

        protected override bool OnDestroy(DestroyType type = null) => true;

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            hitPos += bullet.travelDirNormalized * 2f;
            if (1.0 - (hitPos.y - (double)this.top) / ((double)this.bottom - (double)this.top) < _fluidLevel)
            {
                this.thickness = 2f;
                Vec2 off = hitPos - this.position;
                bool flag = false;
                foreach (FluidStream hole in this._holes)
                {
                    if ((double)(hole.offset - off).length < 2.0)
                    {
                        hole.offset = off;
                        hole.holeThickness += 0.5f;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    FluidStream fluidStream = new FluidStream(0.0f, 0.0f, (-bullet.travelDirNormalized).Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off);
                    this._holes.Add(fluidStream);
                    fluidStream.streamSpeedMultiplier = 2f;
                }
                this._shakeMult = 1f;
                SFX.Play("bulletHitWater", pitch: Rando.Float(-0.2f, 0.2f));
                return base.Hit(bullet, hitPos);
            }
            this.thickness = 1f;
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            exitPos -= bullet.travelDirNormalized * 2f;
            Vec2 off = exitPos - this.position;
            bool flag = false;
            foreach (FluidStream hole in this._holes)
            {
                if ((double)(hole.offset - off).length < 2.0)
                {
                    hole.offset = off;
                    hole.holeThickness += 0.5f;
                    flag = true;
                    break;
                }
            }
            if (flag)
                return;
            this._holes.Add(new FluidStream(0.0f, 0.0f, bullet.travelDirNormalized.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero), 1f, off));
        }

        public override void Update()
        {
            base.Update();
            this._shakeInc += 0.8f;
            this._shakeMult = Lerp.Float(this._shakeMult, 0.0f, 0.05f);
            if (this._alternate == 0)
            {
                foreach (FluidStream hole in this._holes)
                {
                    hole.onFire = this.onFire;
                    hole.hSpeed = this.hSpeed;
                    hole.vSpeed = this.vSpeed;
                    hole.DoUpdate();
                    hole.position = this.Offset(hole.offset);
                    hole.sprayAngle = this.OffsetLocal(hole.startSprayAngle);
                    float num1 = (float)(1.0 - (hole.offset.y - (double)this.topLocal) / ((double)this.bottomLocal - (double)this.topLocal));
                    if ((double)hole.x > (double)this.left - 2.0 && (double)hole.x < (double)this.right + 2.0 && (double)num1 < _fluidLevel)
                    {
                        float num2 = Maths.Clamp(this._fluidLevel - num1, 0.1f, 1f) * 0.0012f * hole.holeThickness;
                        FluidData fluid = this._fluid;
                        fluid.amount = num2;
                        hole.Feed(fluid);
                        this._fluidLevel -= num2;
                    }
                }
            }
            this.weight = this._fluidLevel * 10f;
            ++this._alternate;
            if (this._alternate <= 4)
                return;
            this._alternate = 0;
        }

        public override void Draw()
        {
            this._sprite.frame = (int)((1.0 - _fluidLevel) * 10.0);
            Vec2 position = this.position;
            float num = (float)(Math.Sin(_shakeInc) * _shakeMult * 1.0);
            this.position.x += num;
            base.Draw();
            this.position = position;
            this._bottom.depth = this.depth + 1;
            Graphics.Draw(this._bottom, this.x, this.y + 9f);
            this._jugLine.depth = this.depth + 1;
            this._jugLine.imageIndex = this._sprite.imageIndex;
            this._jugLine.alpha = (float)(_fluidLevel * 10.0 % 1.0);
            Graphics.Draw(_jugLine, this.x + num, this.y);
        }
    }
}
