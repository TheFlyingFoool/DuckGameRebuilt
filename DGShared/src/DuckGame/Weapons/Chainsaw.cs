// Decompiled with JetBrains decompiler
// Type: DuckGame.Chainsaw
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Melee")]
    [BaggedProperty("isSuperWeapon", true)]
    public class Chainsaw : Gun
    {
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_hold));
        public StateBinding _throwSpinBinding = new StateBinding(nameof(_throwSpin));
        public StateBinding _gasBinding = new StateBinding(nameof(_gas));
        public StateBinding _floodBinding = new StateBinding(nameof(_flood));
        public StateBinding _chainsawStateBinding = new ChainsawFlagBinding();
        public EditorProperty<bool> souped = new EditorProperty<bool>(false);
        private float _hold;
        private bool _shing;
        private static bool _playedShing;
        public float _throwSpin;
        private int _framesExisting;
        private int _hitWait;
        private SpriteMap _swordSwing;
        private SpriteMap _sprite;
        private float _rotSway;
        public bool _started;
        private int _pullState = -1;
        private float _animRot;
        private float _upWait;
        private float _engineSpin;
        private float _bladeSpin;
        private float _engineResistance = 1f;
        private SinWave _idleWave = (SinWave)0.6f;
        private SinWave _spinWave = (SinWave)1f;
        private bool _puffClick;
        private float _warmUp;
        public bool _flooded;
        private int _gasDripFrames;
        public float _flood;
        private bool _releasePull;
        public float _gas = 1f;
        private bool _struggling;
        private bool _throttle;
        private float _throttleWait;
        private bool _releasedSincePull;
        private int _skipDebris;
        private bool _resetDuck;
        private int _skipSpark;
        private LoopingSound _sound;
        private LoopingSound _bladeSound;
        private LoopingSound _bladeSoundLow;
        private bool _smokeFlipper;
        private float _fireTrailWait;
        private bool _skipSmoke;
        private Vec2 _idleOffset = Vec2.Zero;

        public override float angle
        {
            get => base.angle + _hold * offDir + _animRot * offDir + _rotSway * offDir;
            set => _angle = value;
        }

        public Vec2 barrelStartPos => position + (Offset(barrelOffset) - position).normalized * 2f;

        public override Vec2 tapedOffset => !tapedIsGun1 ? Vec2.Zero : new Vec2(6f, 0f);

        public bool throttle => _throttle;

        public Chainsaw(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            _type = "gun";
            _sprite = new SpriteMap("chainsaw", 29, 13);
            graphic = _sprite;
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-8f, -6f);
            collisionSize = new Vec2(20f, 11f);
            _barrelOffsetTL = new Vec2(27f, 8f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-4f, 4f);
            weight = 5f;
            physicsMaterial = PhysicsMaterial.Metal;
            _swordSwing = new SpriteMap("swordSwipe", 32, 32);
            _swordSwing.AddAnimation("swing", 0.6f, false, 0, 1, 1, 2);
            _swordSwing.currentAnimation = "swing";
            _swordSwing.speed = 0f;
            _swordSwing.center = new Vec2(9f, 25f);
            throwSpeedMultiplier = 0.5f;
            _bouncy = 0.5f;
            _impactThreshold = 0.3f;
            collideSounds.Add("landTV");
            holsterAngle = -10f;
            editorTooltip = "The perfect tool for cutting wood or carving decorative ice sculptures.";
        }

        public override void Initialize()
        {
            _sound = new LoopingSound("chainsawIdle", multiSound: "chainsawIdleMulti");
            _bladeSound = new LoopingSound("chainsawBladeLoop", multiSound: "chainsawBladeLoopMulti");
            _bladeSoundLow = new LoopingSound("chainsawBladeLoopLow", multiSound: "chainsawBladeLoopLowMulti");
            _sprite = new SpriteMap("chainsaw", 29, 13);
            if ((bool)souped)
                _sprite = new SpriteMap("turbochainsaw", 29, 13);
            graphic = _sprite;
            base.Initialize();
        }

        public override void Terminate()
        {
            _sound.Kill();
            _bladeSound.Kill();
            _bladeSoundLow.Kill();
        }

        public void Shing(Thing wall)
        {
            if (_shing)
                return;
            _struggling = true;
            _shing = true;
            if (!_playedShing)
            {
                _playedShing = true;
                SFX.Play("chainsawClash", Rando.Float(0.4f, 0.55f), Rando.Float(-0.2f, 0.2f), Rando.Float(-0.1f, 0.1f));
            }
            Vec2 normalized = (position - this.barrelPosition).normalized;
            Vec2 barrelPosition = this.barrelPosition;

            int ix = (int)(DGRSettings.ActualParticleMultiplier * 6);
            float f = 24f / ix;
            for (int index = 0; index < ix; ++index)
            {
                Level.Add(Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                barrelPosition += normalized * f;
            }
            _swordSwing.speed = 0f;
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogAction(7);
            if (this.duck == null)
                return;
            Duck duck = this.duck;
            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Pulse, RumbleFalloff.None));
            if (wall.bottom < duck.top)
            {
                duck.vSpeed += 2f;
            }
            else
            {
                if (duck.sliding)
                    duck.sliding = false;
                if (wall.x > duck.x)
                    duck.hSpeed -= 5f;
                else
                    duck.hSpeed += 5f;
                duck.vSpeed -= 2f;
            }
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            SFX.Play("ting");
            return base.Hit(bullet, hitPos);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (owner != null || !(with is Block))
                return;
            Shing(with);
            if (totalImpactPower <= 3f)
                return;
            _started = false;
        }

        public override void ReturnToWorld() => _throwSpin = 90f;

        public void PullEngine()
        {
            float pitch = (bool)souped ? 0.3f : 0f;
            if (!_flooded && _gas > 0f && (_warmUp > 0.5f || _engineResistance < 1f))
            {
                SFX.Play("chainsawFire");
                _started = true;
                _engineSpin = 1.5f;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                    Level.Add(SmallSmoke.New(x + offDir * 4, y + 5f));
                _flooded = false;
                _flood = 0f;
            }
            else
            {
                if (_flooded && _gas > 0f)
                {
                    SFX.Play("chainsawFlooded", 0.9f, Rando.Float(-0.2f, 0.2f));
                    _engineSpin = 1.6f;
                }
                else
                {
                    if (_gas == 0f || Rando.Float(1f) > 0.3f)
                        SFX.Play("chainsawPull", pitch: pitch);
                    else
                        SFX.Play("chainsawFire", pitch: pitch);
                    _engineSpin = 0.8f;
                }
                if (Rando.Float(1f) > 0.8f)
                {
                    _flooded = false;
                    _flood = 0f;
                }
            }
            _engineResistance -= 0.5f;
            if (_gas <= 0f)
                return;
            int num = _flooded ? 4 : 2;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * num; ++index)
                Level.Add(SmallSmoke.New(x + offDir * 4, y + 5f));
        }

        public override void PreUpdateTapedPositioning(TapedGun pTaped)
        {
            base.UpdateTapedPositioning(pTaped);
            if (pTaped.gun1 != this)
                return;
            if (pTaped.duck != null && pTaped.duck.crouch)
            {
                pTaped._holdOffset = new Vec2(0f, -3f);
                pTaped.handOffset = new Vec2(0f, -3f);
            }
            else
            {
                pTaped._holdOffset = Vec2.Zero;
                pTaped.handOffset = new Vec2(0f, 0f);
            }
        }

        public override void UpdateTapedPositioning(TapedGun pTaped)
        {
            base.UpdateTapedPositioning(pTaped);
            if (pTaped.gun1 != this)
                return;
            offDir = pTaped.offDir;
            if (offDir < 0)
                angleDegrees -= 200f;
            else
                angleDegrees -= 160f;
        }

        public override void Update()
        {
            base.Update();
            float num1 = 1f;
            if ((bool)souped)
                num1 = 1.3f;
            if (_swordSwing.finished)
                _swordSwing.speed = 0f;
            if (_hitWait > 0)
                --_hitWait;
            ++_framesExisting;
            if (_framesExisting > 100)
                _framesExisting = 100;
            float pitch = (bool)souped ? 0.3f : 0f;
            _sound.lerpVolume = !_started || _throttle ? 0f : 0.6f;
            _sound.pitch = pitch;
            if (isServerForObject && duck != null)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent((float)(_engineSpin / 4f / 12f + (_started ? 0.02f : 0f)), 0.05f, 0f));
            if (_started)
            {
                _warmUp += 0.001f;
                if (_warmUp > 1f)
                    _warmUp = 1f;
                if (!_puffClick && _idleWave > 0.9f)
                {
                    _skipSmoke = !_skipSmoke;
                    if (_throttle || !_skipSmoke)
                    {
                        if (DGRSettings.S_ParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.S_ParticleMultiplier; i++) Level.Add(SmallSmoke.New(x + offDir * 4, y + 5f, _smokeFlipper ? -0.1f : 0.8f, 0.7f));
                        else if (Rando.Int(DGRSettings.S_ParticleMultiplier) > 0) Level.Add(SmallSmoke.New(x + offDir * 4, y + 5f, _smokeFlipper ? -0.1f : 0.8f, 0.7f));
                        _smokeFlipper = !_smokeFlipper;
                        _puffClick = true;
                    }
                }
                else if (_puffClick && (float)_idleWave < 0f)
                    _puffClick = false;
                if (_pullState < 0)
                {
                    float extraShake = 1f + Maths.NormalizeSection(_engineSpin, 1f, 2f) * 2f;
                    float wave = _idleWave;
                    if (extraShake > 1f)
                        wave = (float)_spinWave;
                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(0f, 2f + wave * extraShake), 0.23f);
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f + wave * extraShake), 0.23f);
                    _rotSway = _idleWave.normalized * (Maths.NormalizeSection(_engineSpin, 1f, 2f) * 3f) * 0.03f;
                }
                else
                    _rotSway = 0f;
                if (!infinite.value)
                {
                    _gas -= 0.00003f;
                    if (_throttle)
                        _gas -= 0.0002f;
                    if (_gas < 0f)
                    {
                        _gas = 0f;
                        _started = false;
                        _throttle = false;
                    }
                }
                if (_triggerHeld)
                {
                    if (_releasedSincePull)
                    {
                        if (!_throttle)
                        {
                            _throttle = true;
                            SFX.Play("chainsawBladeRevUp", 0.5f, pitch);
                        }
                        _engineSpin = Lerp.FloatSmooth(_engineSpin, 4f, 0.1f);
                    }
                }
                else
                {
                    if (_throttle)
                    {
                        _throttle = false;
                        if (_engineSpin > 1.7f)
                            SFX.Play("chainsawBladeRevDown", 0.5f, pitch);
                    }
                    _engineSpin = Lerp.FloatSmooth(_engineSpin, 0f, 0.1f);
                    _releasedSincePull = true;
                }
            }
            else
            {
                _warmUp -= 0.001f;
                if (_warmUp < 0f)
                    _warmUp = 0f;
                _releasedSincePull = false;
                _throttle = false;
            }
            _bladeSound.lerpSpeed = 0.1f;
            _throttleWait = Lerp.Float(_throttleWait, _throttle ? 1f : 0f, 0.07f);
            _bladeSound.lerpVolume = _throttleWait > 0.96f ? 0.6f : 0f;
            if (_struggling)
                _bladeSound.lerpVolume = 0f;
            _bladeSoundLow.lerpVolume = (_throttleWait > 0.96f && _struggling) ? 0.6f : 0f;
            _bladeSound.pitch = pitch;
            _bladeSoundLow.pitch = pitch;
            if (owner == null)
            {
                collisionOffset = new Vec2(-8f, -6f);
                collisionSize = new Vec2(13f, 11f);
            }
            else if (duck != null && (duck.sliding || duck.crouch))
            {
                collisionOffset = new Vec2(-8f, -6f);
                collisionSize = new Vec2(6f, 11f);
            }
            else
            {
                collisionOffset = new Vec2(-8f, -6f);
                collisionSize = new Vec2(10f, 11f);
            }
            if (owner != null)
            {
                _resetDuck = false;
                if (_pullState == -1)
                {
                    if (!_started)
                    {
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(0f, 2f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f), 0.23f);
                    }
                    _upWait = 0f;
                }
                else if (_pullState == 0)
                {
                    _animRot = Lerp.FloatSmooth(_animRot, -0.4f, 0.15f);
                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(-2f, -2f), 0.25f);
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(-4f, 4f), 0.23f);
                    if (_animRot <= -0.35f)
                    {
                        _animRot = -0.4f;
                        _pullState = 1;
                        PullEngine();
                    }
                    _upWait = 0f;
                }
                else if (_pullState == 1)
                {
                    _releasePull = false;
                    _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(2f, 3f), 0.23f);
                    handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(-4f, -2f), 0.23f);
                    _animRot = Lerp.FloatSmooth(_animRot, -0.5f, 0.07f);
                    if (_animRot < -0.45f)
                    {
                        _animRot = -0.5f;
                        _pullState = 2;
                    }
                    _upWait = 0f;
                }
                else if (_pullState == 2)
                {
                    if (_releasePull || !_triggerHeld)
                    {
                        _releasePull = true;
                        if (_started)
                        {
                            handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(0f, 2f + _idleWave.normalized), 0.23f);
                            _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f + _idleWave.normalized), 0.23f);
                            _animRot = Lerp.FloatSmooth(_animRot, 0f, 0.1f);
                            if (_animRot > -0.07f)
                            {
                                _animRot = 0f;
                                _pullState = -1;
                            }
                        }
                        else
                        {
                            _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(-4f, 4f), 0.24f);
                            handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(-2f, -2f), 0.24f);
                            _animRot = Lerp.FloatSmooth(_animRot, -0.4f, 0.12f);
                            if (_animRot > -0.44f)
                            {
                                _releasePull = false;
                                _animRot = -0.4f;
                                _pullState = 3;
                                _holdOffset = new Vec2(-4f, 4f);
                                handOffset = new Vec2(-2f, -2f);
                            }
                        }
                    }
                    _upWait = 0f;
                }
                else if (_pullState == 3)
                {
                    _releasePull = false;
                    _upWait += 0.1f;
                    if (_upWait > 6.0)
                        _pullState = -1;
                }
                _bladeSpin += _engineSpin;
                while (_bladeSpin >= 1.0)
                {
                    --_bladeSpin;
                    int num4 = _sprite.frame + 1;
                    if (num4 > 15)
                        num4 = 0;
                    _sprite.frame = num4;
                }
                _engineSpin = Lerp.FloatSmooth(_engineSpin, 0f, 0.1f);
                _engineResistance = Lerp.FloatSmooth(_engineResistance, 1f, 0.01f);
                _hold = -0.4f;
                center = new Vec2(8f, 7f);
                _framesSinceThrown = 0;
            }
            else
            {
                _rotSway = 0f;
                _shing = false;
                _animRot = Lerp.FloatSmooth(_animRot, 0f, 0.18f);
                if (_framesSinceThrown == 1)
                    _throwSpin = angleDegrees;
                _hold = 0f;
                angleDegrees = _throwSpin;
                center = new Vec2(8f, 7f);
                bool flag1 = false;
                bool flag2 = false;
                if ((Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2f || !grounded) && gravMultiplier > 0f)
                {
                    if (!grounded && Level.CheckRect<Block>(position + new Vec2(-8f, -6f), position + new Vec2(8f, -2f)) != null)
                        flag2 = true;
                    if (!flag2 && !_grounded && Level.CheckPoint<IPlatform>(position + new Vec2(0f, 8f)) == null)
                    {
                        if (offDir > 0)
                            _throwSpin += (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 1f + 5f;
                        else
                            _throwSpin -= (Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 1f + 5f;
                        flag1 = true;
                    }
                }
                if (!flag1 | flag2)
                {
                    _throwSpin %= 360f;
                    if (_throwSpin < 0f)
                        _throwSpin += 360f;
                    if (flag2)
                        _throwSpin = Math.Abs(_throwSpin - 90f) >= Math.Abs(_throwSpin + 90f) ? Lerp.Float(-90f, 0f, 16f) : Lerp.Float(_throwSpin, 90f, 16f);
                    else if (_throwSpin > 90f && _throwSpin < 270f)
                    {
                        _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                    }
                    else
                    {
                        if (_throwSpin > 180.0)
                            _throwSpin -= 360f;
                        else if (_throwSpin < -180.0)
                            _throwSpin += 360f;
                        _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                    }
                }
            }
            if (Math.Abs(angleDegrees) > 90f && Math.Abs(angleDegrees) < 270f && !infinite.value)
            {
                if (isServerForObject)
                {
                    _flood += 0.005f;
                    if (_flood > 1f)
                    {
                        _flooded = true;
                        _started = false;
                    }
                }
                ++_gasDripFrames;
                if (_gas > 0f && _flooded && _gasDripFrames > 2)
                {
                    FluidData gas = Fluid.Gas;
                    gas.amount = 0.003f;
                    _gas -= 0.005f;
                    if (_gas < 0.0)
                        _gas = 0f;
                    Level.Add(new Fluid(x, y, Vec2.Zero, gas));
                    _gasDripFrames = 0;
                }
                if (_gas <= 0f && isServerForObject)
                    _started = false;
            }
            else if (isServerForObject)
            {
                _flood -= 0.008f;
                if (_flood < 0f)
                    _flood = 0f;
            }
            if (duck != null)
            {
                duck.frictionMult = 1f;
                if (_skipSpark > 0)
                {
                    ++_skipSpark;
                    if (_skipSpark > 2)
                        _skipSpark = 0;
                }
                if (duck.sliding && _throttle && !tapedIsGun2 && _skipSpark == 0)
                {
                    if (Level.CheckLine<Block>(barrelStartPos + new Vec2(0f, 8f), barrelPosition + new Vec2(0f, 8f)) != null)
                    {
                        _skipSpark = 1;
                        Vec2 vec2 = position + barrelVector * 5f;
                        for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                        {
                            Level.Add(Spark.New(vec2.x, vec2.y, new Vec2(offDir * Rando.Float(0f, 2f), Rando.Float(0.5f, 1.5f))));
                        }
                        for (int index = 0; index < 2; ++index)
                        {
                            vec2 += barrelVector * 2f;
                            _fireTrailWait -= 0.5f;
                            if ((bool)souped && _fireTrailWait <= 0.0)
                            {
                                _fireTrailWait = 1f;
                                SmallFire smallFire = SmallFire.New(vec2.x, vec2.y, offDir * Rando.Float(0f, 2f), Rando.Float(0.5f, 1.5f));
                                smallFire.waitToHurt = Rando.Float(1f, 2f);
                                smallFire.whoWait = owner as Duck;
                                Level.Add(smallFire);
                            }
                        }
                        if (offDir > 0 && owner.hSpeed < offDir * 6 * num1)
                            owner.hSpeed = offDir * 6 * num1;
                        else if (offDir < 0 && owner.hSpeed > offDir * 6 * num1)
                            owner.hSpeed = offDir * 6 * num1;
                    }
                    else if (offDir > 0 && owner.hSpeed < offDir * 3 * num1)
                        owner.hSpeed = offDir * 3 * num1;
                    else if (offDir < 0 && owner.hSpeed > offDir * 3 * num1)
                        owner.hSpeed = offDir * 3 * num1;
                }
                if (_pullState == -1)
                {
                    if (!_throttle)
                    {
                        _animRot = MathHelper.Lerp(_animRot, 0.3f, 0.2f);
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(-2f, 2f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(-3f, 4f), 0.23f);
                    }
                    else if (_shing)
                    {
                        _animRot = MathHelper.Lerp(_animRot, -1.8f, 0.4f);
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(1f, 0f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f), 0.23f);
                        if (_animRot < -1.5)
                            _shing = false;
                    }
                    else if (duck.crouch)
                    {
                        _animRot = tape == null ? MathHelper.Lerp(_animRot, 0.4f, 0.2f) : (tape.gun1 != this ? MathHelper.Lerp(_animRot, 0.4f, 0.2f) : MathHelper.Lerp(_animRot, 0.2f, 0.2f));
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(1f, 0f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f), 0.23f);
                    }
                    else if (duck.inputProfile.Down(Triggers.Up))
                    {
                        _animRot = tape == null ? MathHelper.Lerp(_animRot, -0.9f, 0.2f) : (tape.gun1 != this ? MathHelper.Lerp(_animRot, -0.6f, 0.2f) : MathHelper.Lerp(_animRot, -0.4f, 0.2f));
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(1f, 0f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f), 0.23f);
                    }
                    else
                    {
                        _animRot = MathHelper.Lerp(_animRot, 0f, 0.2f);
                        handOffset = Lerp.Vec2Smooth(handOffset, new Vec2(1f, 0f), 0.25f);
                        _holdOffset = Lerp.Vec2Smooth(_holdOffset, new Vec2(1f, 2f), 0.23f);
                    }
                }
            }
            else if (!_resetDuck && prevOwner != null)
            {
                if (this.prevOwner is PhysicsObject prevOwner)
                    prevOwner.frictionMult = 1f;
                _resetDuck = true;
            }
            if (_skipDebris > 0)
                ++_skipDebris;
            if (_skipDebris > 3)
                _skipDebris = 0;
            _struggling = false;
            if (owner != null && _started && _throttle && !_shing)
            {
                (Offset(barrelOffset) - position).Normalize();
                Offset(barrelOffset);
                IEnumerable<IAmADuck> amAducks = Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition);
                Block wall1 = Level.CheckLine<Block>(barrelStartPos, barrelPosition);
                if (owner != null)
                {
                    foreach (MaterialThing materialThing in Level.CheckLineAll<MaterialThing>(barrelStartPos, barrelPosition))
                    {
                        if (materialThing.Hurt(materialThing is Door ? 1.8f : 0.5f))
                        {
                            if (duck != null && duck.sliding && materialThing is Door && (materialThing as Door)._jammed)
                            {
                                materialThing.Destroy(new DTImpale(this));
                            }
                            else
                            {
                                _struggling = true;
                                if (duck != null)
                                    duck.frictionMult = 4f;
                                if (_skipDebris == 0)
                                {
                                    _skipDebris = 1;
                                    Vec2 vec2_1 = Collision.LinePoint(barrelStartPos, barrelPosition, materialThing.rectangle);
                                    if (vec2_1 != Vec2.Zero)
                                    {
                                        Vec2 vec2_2 = vec2_1 + barrelVector * Rando.Float(0f, 3f);
                                        Vec2 vec2_3 = -barrelVector.Rotate(Rando.Float(-0.2f, 0.2f), Vec2.Zero);
                                        if (DGRSettings.S_ParticleMultiplier >= 1)
                                        {
                                            for (int i = 0; i < DGRSettings.S_ParticleMultiplier; i++)//once again bad code someone else fix it -NiK0
                                            {
                                                if (materialThing.physicsMaterial == PhysicsMaterial.Wood)
                                                {
                                                    WoodDebris woodDebris = WoodDebris.New(vec2_2.x, vec2_2.y);
                                                    woodDebris.hSpeed = vec2_3.x * 3f;
                                                    woodDebris.vSpeed = vec2_3.y * 3f;
                                                    Level.Add(woodDebris);
                                                }
                                                else if (materialThing.physicsMaterial == PhysicsMaterial.Metal)
                                                {
                                                    Spark spark = Spark.New(vec2_2.x, vec2_2.y, Vec2.Zero);
                                                    spark.hSpeed = vec2_3.x * 3f;
                                                    spark.vSpeed = vec2_3.y * 3f;
                                                    Level.Add(spark);
                                                }
                                                else if (materialThing.physicsMaterial == PhysicsMaterial.Glass)
                                                {
                                                    GlassParticle glassParticle = new GlassParticle(vec2_2.x, vec2_2.y, Vec2.Zero)
                                                    {
                                                        hSpeed = vec2_3.x * 3f,
                                                        vSpeed = vec2_3.y * 3f
                                                    };
                                                    Level.Add(glassParticle);
                                                }
                                            }
                                        }
                                        else if (Rando.Int(DGRSettings.S_ParticleMultiplier) > 0)
                                        {
                                            if (materialThing.physicsMaterial == PhysicsMaterial.Wood)
                                            {
                                                WoodDebris woodDebris = WoodDebris.New(vec2_2.x, vec2_2.y);
                                                woodDebris.hSpeed = vec2_3.x * 3f;
                                                woodDebris.vSpeed = vec2_3.y * 3f;
                                                Level.Add(woodDebris);
                                            }
                                            else if (materialThing.physicsMaterial == PhysicsMaterial.Metal)
                                            {
                                                Spark spark = Spark.New(vec2_2.x, vec2_2.y, Vec2.Zero);
                                                spark.hSpeed = vec2_3.x * 3f;
                                                spark.vSpeed = vec2_3.y * 3f;
                                                Level.Add(spark);
                                            }
                                            else if (materialThing.physicsMaterial == PhysicsMaterial.Glass)
                                            {
                                                GlassParticle glassParticle = new GlassParticle(vec2_2.x, vec2_2.y, Vec2.Zero)
                                                {
                                                    hSpeed = vec2_3.x * 3f,
                                                    vSpeed = vec2_3.y * 3f
                                                };
                                                Level.Add(glassParticle);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                bool flag = false;
                switch (wall1)
                {
                    case null:
                    case Door _:
                        foreach (Sword wall2 in Level.current.things[typeof(Sword)])
                        {
                            if (wall2.owner != null && wall2.crouchStance && !wall2.jabStance && Collision.LineIntersect(barrelStartPos, barrelPosition, wall2.barrelStartPos, wall2.barrelPosition))
                            {
                                Shing(wall2);
                                wall2.Shing();
                                wall2.owner.hSpeed += offDir * 3f;
                                wall2.owner.vSpeed -= 2f;
                                duck.hSpeed += -offDir * 3f;
                                duck.vSpeed -= 2f;
                                if (wall2.duck != null)
                                    wall2.duck.crippleTimer = 1f;
                                duck.crippleTimer = 1f;
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            Thing ignore = null;
                            if (duck != null)
                                ignore = duck.GetEquipment(typeof(Helmet));
                            QuadLaserBullet wall3 = Level.CheckLine<QuadLaserBullet>(position, barrelPosition);
                            if (wall3 != null)
                            {
                                Shing(wall3);
                                Vec2 vec2 = wall3.travel;
                                float length = vec2.length;
                                float num5 = 1f;
                                if (offDir > 0 && vec2.x < 0.0)
                                    num5 = 1.5f;
                                else if (offDir < 0 && vec2.x > 0.0)
                                    num5 = 1.5f;
                                vec2 = offDir <= 0 ? new Vec2(-length * num5, 0f) : new Vec2(length * num5, 0f);
                                wall3.travel = vec2;
                                break;
                            }
                            Helmet wall4 = Level.CheckLine<Helmet>(barrelStartPos, barrelPosition, ignore);
                            if (wall4 != null && wall4.equippedDuck != null && wall4.owner != null)
                            {
                                Shing(wall4);
                                if (wall4.owner != null)
                                {
                                    wall4.owner.hSpeed += offDir * 3f;
                                    wall4.owner.vSpeed -= 2f;
                                    if (wall4.duck != null)
                                        wall4.duck.crippleTimer = 1f;
                                }
                                wall4.Hurt(0.53f);
                                flag = true;
                                break;
                            }
                            if (duck != null)
                                ignore = duck.GetEquipment(typeof(ChestPlate));
                            ChestPlate wall5 = Level.CheckLine<ChestPlate>(barrelStartPos, barrelPosition, ignore);
                            if (wall5 != null && wall5.equippedDuck != null && wall5.owner != null)
                            {
                                Shing(wall5);
                                if (wall5.owner != null)
                                {
                                    wall5.owner.hSpeed += offDir * 3f;
                                    wall5.owner.vSpeed -= 2f;
                                    if (wall5.duck != null)
                                        wall5.duck.crippleTimer = 1f;
                                }
                                wall5.Hurt(0.53f);
                                flag = true;
                                break;
                            }
                            break;
                        }
                        break;
                    default:
                        Shing(wall1);
                        if (wall1 is Window)
                        {
                            wall1.Destroy(new DTImpact(this));
                            break;
                        }
                        break;
                }
                if (!flag)
                {
                    foreach (Chainsaw wall6 in Level.current.things[typeof(Chainsaw)])
                    {
                        if (wall6 != this && wall6.owner != null && duck != null && wall6 != tapedCompatriot && Collision.LineIntersect(barrelStartPos, barrelPosition, wall6.barrelStartPos, wall6.barrelPosition))
                        {
                            Shing(wall6);
                            wall6.Shing(this);
                            wall6.owner.hSpeed += offDir * 2f;
                            wall6.owner.vSpeed -= 1.5f;
                            duck.hSpeed += -offDir * 2f;
                            duck.vSpeed -= 1.5f;
                            if (wall6.duck != null)
                                wall6.duck.crippleTimer = 1f;
                            duck.crippleTimer = 1f;
                            flag = true;
                            if (Recorder.currentRecording != null)
                                Recorder.currentRecording.LogBonus();
                        }
                    }
                }
                if (!flag)
                {
                    foreach (IAmADuck amAduck in amAducks)
                    {
                        if (amAduck != duck)
                        {
                            if (amAduck is Duck && duck != null)
                                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                            if (amAduck is MaterialThing materialThing1)
                            {
                                MaterialThing materialThing = materialThing1;
                                materialThing.velocity += new Vec2(offDir * 0.8f, -0.8f);
                                materialThing1.Destroy(new DTImpale(this));
                                if (duck != null)
                                    duck._timeSinceChainKill = 0;
                            }
                        }
                    }
                }
            }
            _sound.Update();
            _bladeSound.Update();
            _bladeSoundLow.Update();
        }

        public override void HolsterUpdate(Holster pHolster)
        {
            holsterOffset = Vec2.Zero;
            if (pHolster is PowerHolster)
            {
                if (duck != null && duck.sliding)
                {
                    holsterAngle = 90f;
                    holsterOffset = new Vec2(6f, 0f);
                }
                else
                    holsterAngle = -10f;
            }
            else
                holsterAngle = 90f;
            _flood = 0f;
        }

        public override void Draw()
        {
            _playedShing = false;
            if (_swordSwing.speed > 0f)
            {
                if (duck != null)
                    _swordSwing.flipH = duck.offDir <= 0;
                _swordSwing.alpha = 0.4f;
                _swordSwing.position = position;
                _swordSwing.depth = depth + 1;
                _swordSwing.Draw();
            }
            if (duck != null && (_pullState == 1 || _pullState == 2))
                Graphics.DrawLine(Offset(new Vec2(-2f, -2f)), duck.armPosition + new Vec2(handOffset.x * offDir, handOffset.y), Color.White, depth: duck.depth + 11 - 1);
            _idleOffset = duck != null && tape == null || !_started ? Vec2.Zero : Lerp.Vec2Smooth(handOffset, new Vec2(0f, 2f + _idleWave.normalized), 0.23f);
            position += _idleOffset;
            base.Draw();
            position -= _idleOffset;
        }

        public override void OnPressAction()
        {
            if (_started)
                return;
            if (duck != null)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
            if (_pullState == -1)
            {
                _pullState = 0;
            }
            else
            {
                if (_pullState != 3)
                    return;
                _pullState = 1;
                PullEngine();
            }
        }

        public override void Fire()
        {
        }

        public class ChainsawFlagBinding : StateFlagBase
        {
            public override ushort ushortValue
            {
                get
                {
                    _value = 0;
                    Chainsaw thing = _thing as Chainsaw;
                    if (thing._flooded)
                        _value |= 4;
                    if (thing._started)
                        _value |= 2;
                    if (thing._throttle)
                        _value |= 1;
                    return _value;
                }
                set
                {
                    _value = value;
                    Chainsaw thing = _thing as Chainsaw;
                    thing._flooded = (_value & 4U) > 0U;
                    thing._started = (_value & 2U) > 0U;
                    thing._throttle = (_value & 1U) > 0U;
                }
            }

            public ChainsawFlagBinding(GhostPriority p = GhostPriority.Normal)
              : base(p, 3)
            {
            }
        }
    }
}
