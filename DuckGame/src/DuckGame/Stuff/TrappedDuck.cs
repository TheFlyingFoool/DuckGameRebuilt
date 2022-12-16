// Decompiled with JetBrains decompiler
// Type: DuckGame.TrappedDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class TrappedDuck : Holdable, IPlatform, IAmADuck
    {
        public StateBinding _duckOwnerBinding = new StateBinding(nameof(_duckOwner));
        public Duck _duckOwner;
        public float _trapTime = 1f;
        public float _shakeMult;
        private float _shakeInc;
        public byte funNum;
        public bool infinite;
        private bool extinguishing;
        private float jumpCountdown;
        private bool _prevVisible;
        private int framesInvisible;
        private Vec2 _stickLerp;
        private Vec2 _stickSlowLerp;

        public Duck captureDuck => _duckOwner;

        public override bool visible
        {
            get => base.visible;
            set
            {
                if (value && _trapTime < 0.0)
                {
                    _trapTime = 1f;
                    owner = null;
                }
                base.visible = value;
            }
        }

        public TrappedDuck(float xpos, float ypos, Duck duckowner)
          : base(xpos, ypos)
        {
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            thickness = 0.5f;
            weight = 5f;
            flammable = 1f;
            burnSpeed = 0f;
            _duckOwner = duckowner;
            tapeable = false;
            InitializeStuff();
        }

        public void InitializeStuff() => _trapTime = 1f;

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (_duckOwner != null)
                _duckOwner.Burn(firePosition, litBy);
            return base.OnBurn(firePosition, litBy);
        }

        public override void Extinquish()
        {
            if (extinguishing)
                return;
            extinguishing = true;
            if (_duckOwner != null)
                _duckOwner.Extinquish();
            base.Extinquish();
            extinguishing = false;
        }

        public override void Terminate() => base.Terminate();

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (_duckOwner == null)
                return false;
            if (!destroyed)
            {
                _duckOwner.hSpeed = hSpeed;
                bool flag = type != null;
                if (!flag && jumpCountdown > 0.01f)
                    _duckOwner.vSpeed = Duck.JumpSpeed;
                else
                    _duckOwner.vSpeed = flag ? vSpeed - 1f : -3f;
                _duckOwner.x = x;
                _duckOwner.y = y - 10f;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f));
                    smallSmoke.hSpeed += hSpeed * Rando.Float(0.3f, 0.5f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
                if (duck != null)
                {
                    if (held)
                    {
                        if (duck.holdObject == this)
                            duck.holdObject = null;
                    }
                    else if (duck.holstered == this)
                        duck.holstered = null;
                }
                if (Network.isActive)
                {
                    if (!flag)
                    {
                        _duckOwner.Fondle(this);
                        authority += 30;
                    }
                    active = false;
                    visible = false;
                    owner = null;
                }
                else
                    Level.Remove(this);
                if (_duckOwner.owner == this)
                    _duckOwner.owner = null;
                if (flag && !_duckOwner.killingNet)
                {
                    _duckOwner.killingNet = true;
                    _duckOwner.Destroy(type);
                }
                _duckOwner._trapped = null;
            }
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && (_duckOwner == null || !_duckOwner.HitArmor(bullet, hitPos)))
                OnDestroy(new DTShot(bullet));
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
        }

        public override void InactiveUpdate()
        {
            if (!isServerForObject)
                return;
            y = -9999f;
            visible = false;
        }

        public override void Update()
        {
            if (Network.isActive && _prevVisible && !visible)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                {
                    SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f));
                    smallSmoke.hSpeed += hSpeed * Rando.Float(0.3f, 0.5f);
                    smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                    Level.Add(smallSmoke);
                }
            }
            if (_duckOwner == null)
                return;
            ++_framesSinceTransfer;
            base.Update();
            if (isOffBottomOfLevel)
                OnDestroy(new DTFall());
            jumpCountdown -= Maths.IncFrameTimer();
            _prevVisible = visible;
            _shakeInc += 0.8f;
            _shakeMult = Lerp.Float(_shakeMult, 0f, 0.05f);
            if (Network.isActive && _duckOwner._trapped == this && !_duckOwner.isServerForObject && _duckOwner.inputProfile.Pressed(Triggers.Jump))
                _shakeMult = 1f;
            if (_duckOwner.isServerForObject && _duckOwner._trapped == this)
            {
                if (!visible && owner == null)
                {
                    ++framesInvisible;
                    if (framesInvisible > 30)
                    {
                        framesInvisible = 0;
                        y = -9999f;
                    }
                }
                if (!infinite)
                {
                    _duckOwner.profile.stats.timeInNet += Maths.IncFrameTimer();
                    if (_duckOwner.inputProfile.Pressed(Triggers.Jump))
                    {
                        _shakeMult = 1f;
                        _trapTime -= 0.007f;
                        jumpCountdown = 0.25f;
                    }
                    if (grounded && _duckOwner.inputProfile.Pressed(Triggers.Jump))
                    {
                        _shakeMult = 1f;
                        _trapTime -= 0.028f;
                        if (owner == null)
                        {
                            if (Math.Abs(hSpeed) < 1.0 && _framesSinceTransfer > 30)
                                _duckOwner.Fondle(this);
                            vSpeed -= Rando.Float(0.8f, 1.1f);
                            if (_duckOwner.inputProfile.Down(Triggers.Left) && hSpeed > -1.0)
                                hSpeed -= Rando.Float(0.6f, 0.8f);
                            if (_duckOwner.inputProfile.Down(Triggers.Right) && hSpeed < 1.0)
                                hSpeed += Rando.Float(0.6f, 0.8f);
                        }
                    }
                    if (_duckOwner.inputProfile.Pressed(Triggers.Jump) && _duckOwner.HasEquipment(typeof(Jetpack)))
                        _duckOwner.GetEquipment(typeof(Jetpack)).PressAction();
                    if (_duckOwner.inputProfile.Released(Triggers.Jump) && _duckOwner.HasEquipment(typeof(Jetpack)))
                        _duckOwner.GetEquipment(typeof(Jetpack)).ReleaseAction();
                    _trapTime -= 0.0028f;
                    if ((_trapTime <= 0.0 || _duckOwner.dead) && !inPipe)
                        OnDestroy(null);
                }
                _duckOwner.UpdateSkeleton();
                weight = 5f;
            }
            if (_duckOwner._trapped == this)
                _duckOwner.position = position;
            if (owner != null)
                return;
            depth = _duckOwner.depth - 10;
        }

        public override void Draw()
        {
            if (_duckOwner == null)
                return;
            _duckOwner._sprite.SetAnimation("netted");
            _duckOwner._sprite.imageIndex = 14;
            _duckOwner._spriteQuack.frame = _duckOwner._sprite.frame;
            _duckOwner._sprite.depth = depth;
            _duckOwner._spriteQuack.depth = depth;
            if (Network.isActive)
                _duckOwner.DrawConnectionIndicators();
            float num1 = 0f;
            if (owner != null)
                num1 = (float)(Math.Sin(_shakeInc) * _shakeMult * 1.0);
            if (_duckOwner.quack > 0)
            {
                Vec2 tounge = _duckOwner.tounge;
                if (!_duckOwner._spriteQuack.flipH && tounge.x < 0.0)
                    tounge.x = 0f;
                if (_duckOwner._spriteQuack.flipH && tounge.x > 0.0)
                    tounge.x = 0f;
                if (tounge.y < -0.3f)
                    tounge.y = -0.3f;
                if (tounge.y > 0.04f)
                    tounge.y = 0.4f;
                _stickLerp = Lerp.Vec2Smooth(_stickLerp, tounge, 0.2f);
                _stickSlowLerp = Lerp.Vec2Smooth(_stickSlowLerp, tounge, 0.1f);
                Vec2 stickLerp = _stickLerp;
                stickLerp.y *= -1f;
                Vec2 stickSlowLerp = _stickSlowLerp;
                stickSlowLerp.y *= -1f;
                int num2 = 0;
                double length = stickLerp.length;
                if (length > 0.5)
                    num2 = 72;
                Graphics.Draw(_duckOwner._spriteQuack, _duckOwner._sprite.imageIndex + num2, x + num1, y - 8f);
                if (length > 0.05f)
                {
                    Vec2 vec2_1 = position + new Vec2(num1 + (_duckOwner._spriteQuack.flipH ? -1f : 1f), -2f);
                    List<Vec2> vec2List = Curve.Bezier(8, vec2_1, vec2_1 + stickSlowLerp * 6f, vec2_1 + stickLerp * 6f);
                    Vec2 vec2_2 = Vec2.Zero;
                    float num3 = 1f;
                    foreach (Vec2 p2 in vec2List)
                    {
                        if (vec2_2 != Vec2.Zero)
                        {
                            Vec2 vec2_3 = vec2_2 - p2;
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2, new Color(223, 30, 30), 0.15f * num3, depth + 1);
                            Graphics.DrawTexturedLine(Graphics.tounge.texture, vec2_2 + vec2_3.normalized * 0.4f, p2 - vec2_3.normalized * 0.4f, Color.Black, 0.3f * num3, depth - 1);
                        }
                        num3 -= 0.1f;
                        vec2_2 = p2;
                    }
                    if (_duckOwner._spriteQuack != null)
                    {
                        _duckOwner._spriteQuack.alpha = alpha;
                        _duckOwner._spriteQuack.angle = angle;
                        _duckOwner._spriteQuack.depth = depth + 2;
                        _duckOwner._spriteQuack.scale = scale;
                        _duckOwner._spriteQuack.frame += 36;
                        _duckOwner._spriteQuack.Draw();
                        _duckOwner._spriteQuack.frame -= 36;
                    }
                }
            }
            else
                Graphics.Draw(_duckOwner._sprite, x + num1, y - 8f);
            base.Draw();
        }
    }
}
