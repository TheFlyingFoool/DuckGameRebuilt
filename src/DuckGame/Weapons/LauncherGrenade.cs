// Decompiled with JetBrains decompiler
// Type: DuckGame.LauncherGrenade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class LauncherGrenade : PhysicsObject
    {
        private List<Vec2> _trail = new List<Vec2>();
        private float _isVolatile = 1f;
        private Vec2 _prevPosition;
        private bool _fade;
        private int _numTrail;
        private float _fadeVal = 1f;
        private bool _blowUp;
        private float _startWait = 0.2f;

        public LauncherGrenade(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("launcherGrenade");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(8f, 6f);
            this.collisionOffset = new Vec2(-4f, -3f);
            for (int index = 0; index < 17; ++index)
                this._trail.Add(new Vec2(0.0f, 0.0f));
            this._prevPosition = new Vec2(this.position);
            this.bouncy = 1f;
            this.friction = 0.0f;
            this._dontCrush = true;
        }

        public override void Initialize()
        {
            if (Level.CheckPoint<Block>(this.position) != null)
                this._blowUp = true;
            base.Initialize();
        }

        public override void Update()
        {
            if (this._fade)
                this.enablePhysics = false;
            base.Update();
            this._startWait -= 0.1f;
            this.angle = -Maths.DegToRad(Maths.PointDirection(this.x, this.y, this._prevPosition.x, this._prevPosition.y));
            this._isVolatile -= 0.06f;
            for (int index = 15; index >= 0; --index)
                this._trail[index + 1] = new Vec2(this._trail[index].x, this._trail[index].y);
            if (!this._fade)
            {
                this._trail[0] = new Vec2(this.x, this.y);
                ++this._numTrail;
            }
            else
            {
                --this._numTrail;
                this._fadeVal -= 0.1f;
                if ((double)this._fadeVal <= 0.0)
                    Level.Remove((Thing)this);
            }
            this._prevPosition.x = this.position.x;
            this._prevPosition.y = this.position.y;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._fade || with is Gun || (with is AutoPlatform || with is Nubber) && (double)this.vSpeed <= 0.0)
                return;
            if (with is PhysicsObject)
                this._isVolatile = -1f;
            if ((double)this._startWait <= 0.0 && !this._fade && ((double)this.totalImpactPower > 2.0 && ((double)this._isVolatile <= 0.0 || !(with is Block)) || this._blowUp))
            {
                int num1 = 0;
                for (int index = 0; index < 1; ++index)
                {
                    ExplosionPart explosionPart = new ExplosionPart(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                    explosionPart.xscale *= 0.7f;
                    explosionPart.yscale *= 0.7f;
                    Level.Add((Thing)explosionPart);
                    ++num1;
                }
                SFX.Play("explode");
                RumbleManager.AddRumbleEvent(this.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
                for (int index = 0; index < 12; ++index)
                {
                    float num2 = (float)((double)index * 30.0 - 10.0) + Rando.Float(20f);
                    ATShrapnel type = new ATShrapnel();
                    type.range = 25f + Rando.Float(10f);
                    Level.Add((Thing)new Bullet(this.x + (float)(Math.Cos((double)Maths.DegToRad(num2)) * 8.0), this.y - (float)(Math.Sin((double)Maths.DegToRad(num2)) * 8.0), (AmmoType)type, num2)
                    {
                        firedFrom = (Thing)this
                    });
                }
                this._fade = true;
                this.y += 10000f;
            }
            else
            {
                if (with is IPlatform)
                    return;
                if (from == ImpactedFrom.Left || from == ImpactedFrom.Right)
                    this.BounceH();
                if (from != ImpactedFrom.Top && from != ImpactedFrom.Bottom)
                    return;
                this.BounceV();
            }
        }

        public override void Draw()
        {
            if (!this._fade)
                base.Draw();
            for (int index = 1; index < 16; ++index)
            {
                if (index < this._numTrail)
                {
                    float num = (float)((1.0 - (double)index / 16.0) * (double)this._fadeVal * 0.800000011920929);
                    Graphics.DrawLine(new Vec2(this._trail[index - 1].x, this._trail[index - 1].y), new Vec2(this._trail[index].x, this._trail[index].y), Color.White * num);
                }
            }
        }
    }
}
