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
            graphic = new Sprite("launcherGrenade");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(8f, 6f);
            collisionOffset = new Vec2(-4f, -3f);
            for (int index = 0; index < 17; ++index)
                _trail.Add(new Vec2(0f, 0f));
            _prevPosition = new Vec2(position);
            bouncy = 1f;
            friction = 0f;
            _dontCrush = true;
        }

        public override void Initialize()
        {
            if (Level.CheckPoint<Block>(position) != null)
                _blowUp = true;
            base.Initialize();
        }

        public override void Update()
        {
            if (_fade)
                enablePhysics = false;
            base.Update();
            _startWait -= 0.1f;
            angle = -Maths.DegToRad(Maths.PointDirection(x, y, _prevPosition.x, _prevPosition.y));
            _isVolatile -= 0.06f;
            for (int index = 15; index >= 0; --index)
                _trail[index + 1] = new Vec2(_trail[index].x, _trail[index].y);
            if (!_fade)
            {
                _trail[0] = new Vec2(x, y);
                ++_numTrail;
            }
            else
            {
                --_numTrail;
                _fadeVal -= 0.1f;
                if (_fadeVal <= 0.0)
                    Level.Remove(this);
            }
            _prevPosition.x = position.x;
            _prevPosition.y = position.y;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_fade || with is Gun || (with is AutoPlatform || with is Nubber) && vSpeed <= 0.0)
                return;
            if (with is PhysicsObject)
                _isVolatile = -1f;
            if (_startWait <= 0.0 && !_fade && (totalImpactPower > 2.0 && (_isVolatile <= 0.0 || !(with is Block)) || _blowUp))
            {
                int num1 = 0;
                for (int index = 0; index < 1; ++index)
                {
                    ExplosionPart explosionPart = new ExplosionPart(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                    explosionPart.xscale *= 0.7f;
                    explosionPart.yscale *= 0.7f;
                    Level.Add(explosionPart);
                    ++num1;
                }
                SFX.Play("explode");
                RumbleManager.AddRumbleEvent(position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
                for (int index = 0; index < 12; ++index)
                {
                    float num2 = (index * 30f - 10f) + Rando.Float(20f);
                    ATShrapnel type = new ATShrapnel
                    {
                        range = 25f + Rando.Float(10f)
                    };
                    Level.Add(new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num2)) * 8f), y - (float)(Math.Sin(Maths.DegToRad(num2)) * 8f), type, num2)
                    {
                        firedFrom = this
                    });
                }
                _fade = true;
                y += 10000f;
            }
            else
            {
                if (with is IPlatform)
                    return;
                if (from == ImpactedFrom.Left || from == ImpactedFrom.Right)
                    BounceH();
                if (from != ImpactedFrom.Top && from != ImpactedFrom.Bottom)
                    return;
                BounceV();
            }
        }

        public override void Draw()
        {
            if (!_fade)
                base.Draw();
            for (int index = 1; index < 16; ++index)
            {
                if (index < _numTrail)
                {
                    float num = ((1f - index / 16f) * _fadeVal * 0.8f);
                    Graphics.DrawLine(new Vec2(_trail[index - 1].x, _trail[index - 1].y), new Vec2(_trail[index].x, _trail[index].y), Color.White * num);
                }
            }
        }
    }
}
