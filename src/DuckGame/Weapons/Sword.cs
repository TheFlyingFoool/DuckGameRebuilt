// Decompiled with JetBrains decompiler
// Type: DuckGame.Sword
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Melee")]
    public class Sword : Gun
    {
        public StateBinding _swingBinding = new StateBinding(true, nameof(_swing));
        public StateBinding _holdBinding = new StateBinding(true, nameof(_hold));
        public StateBinding _stanceBinding = new SwordFlagBinding();
        public StateBinding _pullBackBinding = new StateBinding(true, nameof(_pullBack));
        public StateBinding _throwSpinBinding = new StateBinding(true, nameof(_throwSpin));
        public StateBinding _addOffsetXBinding = new StateBinding(nameof(_addOffsetX));
        public StateBinding _addOffsetYBinding = new StateBinding(nameof(_addOffsetY));
        public float _swing;
        public float _hold;
        protected bool _drawing;
        protected bool _clashWithWalls = true;
        public bool _pullBack;
        public bool _jabStance;
        public bool _crouchStance;
        public bool _slamStance;
        public bool _swinging;
        public float _addOffsetX;
        public float _addOffsetY;
        public bool _swingPress;
        public bool _shing;
        public static bool _playedShing;
        public bool _atRest = true;
        public bool _swung;
        public bool _wasLifted;
        public float _throwSpin;
        public int _framesExisting;
        public int _hitWait;
        protected bool _enforceJabSwing = true;
        protected bool _allowJabMotion = true;
        private SpriteMap _swordSwing;
        protected float _stickWait;
        protected Vec2 additionalHoldOffset = Vec2.Zero;
        private int _unslam;
        protected float _afterSwingWait;
        private float _afterSwingCounter;
        private Vec2 _tapeOffset = Vec2.Zero;
        private byte blocked;
        public bool _volatile;
        protected Vec2 centerHeld = new Vec2(4f, 21f);
        protected Vec2 centerUnheld = new Vec2(4f, 11f);
        protected bool _stayVolatile;
        private bool bayonetLethal;
        private float _prevAngle;
        private Vec2 _prevPos;
        private int _prevOffdir = -1;
        protected float[] _lastAngles = new float[8];
        protected Vec2[] _lastPositions = new Vec2[8];
        protected int _lastIndex;
        protected int _lastSize;
        private Thing _prevHistoryOwner;
        private Vec2 _lastHistoryPos = Vec2.Zero;
        private float _lastHistoryAngle;
        protected string _swingSound = "swipe";
        protected float _timeSinceSwing;

        public override float angle
        {
            get => _drawing ? _angle : base.angle + (_swing + _hold) * offDir;
            set => _angle = value;
        }

        public bool jabStance => _jabStance;

        public bool crouchStance => _crouchStance;

        public virtual Vec2 barrelStartPos
        {
            get
            {
                if (owner == null)
                    return position - (Offset(barrelOffset) - position).normalized * 6f;
                return _slamStance ? position + (Offset(barrelOffset) - position).normalized * 12f : position + (Offset(barrelOffset) - position).normalized * 2f;
            }
        }

        public Sword(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.8f;
            _type = "gun";
            graphic = new Sprite("sword");
            center = new Vec2(4f, 21f);
            collisionOffset = new Vec2(-2f, -16f);
            collisionSize = new Vec2(4f, 18f);
            _barrelOffsetTL = new Vec2(4f, 1f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(-4f, 4f);
            weight = 0.9f;
            physicsMaterial = PhysicsMaterial.Metal;
            _swordSwing = new SpriteMap("swordSwipe", 32, 32);
            _swordSwing.AddAnimation("swing", 0.6f, false, 0, 1, 1, 2);
            _swordSwing.currentAnimation = "swing";
            _swordSwing.speed = 0f;
            _swordSwing.center = new Vec2(9f, 25f);
            holsterAngle = 180f;
            tapedIndexPreference = 0;
            _bouncy = 0.5f;
            _impactThreshold = 0.3f;
            editorTooltip = "Basically a giant letter opener.";
        }

        public override void Initialize() => base.Initialize();

        public override Vec2 tapedOffset => tapedCompatriot is Gun ? (tapedCompatriot as Gun).barrelOffset + new Vec2(-14f, 2f) : new Vec2(-6f, -3f);

        public override void UpdateTapedPositioning(TapedGun pTaped)
        {
            if (pTaped.gun1 != null && pTaped.gun2 != null)
                angleDegrees = pTaped.angleDegrees - 90 * offDir;
            if (tapedCompatriot is Gun)
            {
                (tapedCompatriot as Gun).addVerticalTapeOffset = false;
                tape._holdOffset = (tapedCompatriot as Gun)._holdOffset;
                tape.handOffset = (tapedCompatriot as Gun).handOffset;
            }
            collisionOffset = new Vec2(-4f, 0f);
            collisionSize = new Vec2(4f, 4f);
            center = centerHeld;
            thickness = 0f;
        }

        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case TapedSword _:
                case EnergyScimitar _:
                    return false;
                default:
                    return true;
            }
        }

        public override Holdable BecomeTapedMonster(TapedGun pTaped) => pTaped.gun1 is Sword && pTaped.gun2 is Sword ? new TapedSword(x, y) : (Holdable)null;

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
        }

        public override bool HolsterActivate(Holster pHolster)
        {
            pHolster.EjectItem();
            return true;
        }

        public override void Thrown()
        {
        }

        public virtual void Shing()
        {
            if (_shing)
                return;
            _pullBack = false;
            _swinging = false;
            _shing = true;
            _swingPress = false;
            if (!Sword._playedShing)
            {
                Sword._playedShing = true;
                SFX.Play("swordClash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f));
            }
            Vec2 normalized = (position - this.barrelPosition).normalized;
            Vec2 barrelPosition = this.barrelPosition;
            for (int index = 0; index < 6; ++index)
            {
                Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                if (this is OldEnergyScimi)
                {
                    spark._color = (this as OldEnergyScimi).swordColor;
                    spark._width = 1f;
                }
                Level.Add(spark);
                barrelPosition += normalized * 4f;
            }
            if (duck != null)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            _swung = false;
            _swordSwing.speed = 0f;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (duck == null)
                return false;
            if (blocked == 0)
            {
                duck.AddCoolness(1);
            }
            else
            {
                ++blocked;
                if (blocked > 4)
                {
                    blocked = 1;
                    duck.AddCoolness(1);
                }
            }
            RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
            SFX.Play("ting");
            return base.Hit(bullet, hitPos);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (tape != null && tape.owner != null || !_wasLifted || owner != null)
                return;
            switch (with)
            {
                case Block _:
                label_3:
                    Shing();
                    _framesSinceThrown = 25;
                    break;
                case IPlatform _:
                    if (from != ImpactedFrom.Bottom || vSpeed <= 0.0)
                        break;
                    goto label_3;
            }
        }

        public override void ReturnToWorld()
        {
            _throwSpin = 90f;
            RestoreCollisionSize();
        }

        public virtual void RestoreCollisionSize(bool pHeld = false)
        {
            if (pHeld)
            {
                collisionOffset = new Vec2(-4f, 0f);
                collisionSize = new Vec2(4f, 4f);
                if (!_crouchStance || _jabStance)
                    return;
                collisionOffset = new Vec2(-2f, -19f);
                collisionSize = new Vec2(4f, 16f);
                thickness = 3f;
            }
            else
            {
                collisionOffset = new Vec2(-2f, -16f);
                collisionSize = new Vec2(4f, 18f);
                if (!_wasLifted)
                    return;
                collisionOffset = new Vec2(-4f, -2f);
                collisionSize = new Vec2(8f, 4f);
            }
        }

        protected virtual void PerformAirSpin()
        {
            if (this.hSpeed > 0f)
            {
                this._throwSpin += (Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed)) * 2f + 4f;
                return;
            }
            this._throwSpin -= (Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed)) * 2f + 4f;
        }

        public new bool held => duck != null && duck.holdObject == this;

        protected virtual void QuadLaserHit(QuadLaserBullet pBullet)
        {
        }

        protected virtual void UpdateCrouchStance()
        {
            if (!_crouchStance)
            {
                _hold = -0.4f;
                handOffset = new Vec2(_addOffsetX, _addOffsetY);
                _holdOffset = new Vec2(_addOffsetX - 4f, 4f + _addOffsetY) + additionalHoldOffset;
            }
            else
            {
                _hold = 0f;
                _holdOffset = new Vec2(0f + _addOffsetX, 4f + _addOffsetY) + additionalHoldOffset;
                handOffset = new Vec2(3f + _addOffsetX, _addOffsetY);
            }
        }

        public virtual DestroyType destroyType => new DTImpale(this);

        protected virtual void UpdateJabPullback()
        {
            _swing = MathHelper.Lerp(_swing, 1.75f, 0.4f);
            if (_swing > 1.55f)
            {
                _swing = 1.55f;
                _shing = false;
                _swung = false;
            }
            _addOffsetX = MathHelper.Lerp(_addOffsetX, -12f, 0.45f);
            if (_addOffsetX < -12.0)
                _addOffsetX = -12f;
            _addOffsetY = MathHelper.Lerp(_addOffsetY, -4f, 0.35f);
            if (_addOffsetX >= -3.0)
                return;
            _addOffsetY = -3f;
        }

        protected virtual void UpdateSlamPullback()
        {
            _swing = MathHelper.Lerp(_swing, 3.14f, 0.8f);
            if (_swing > 3.1f && _unslam == 0)
            {
                _swing = 3.14f;
                _shing = false;
                _swung = true;
            }
            _addOffsetX = MathHelper.Lerp(_addOffsetX, -5f, 0.45f);
            if (_addOffsetX < -4.6f)
                _addOffsetX = -5f;
            _addOffsetY = MathHelper.Lerp(_addOffsetY, -6f, 0.35f);
            if (_addOffsetX >= -5.5)
                return;
            _addOffsetY = -6f;
        }

        public override void Update()
        {
            bayonetLethal = false;
            if (tape != null && tapedCompatriot != null)
            {
                if (tapedCompatriot != null && (Math.Abs(_prevAngle - angleDegrees) > 1.0f || (_prevPos - position).length > 2.0f))
                    bayonetLethal = true;
                if (isServerForObject && bayonetLethal)
                {
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(Offset(new Vec2(4f, 10f) - center + _extraOffset), barrelPosition))
                    {
                        if (amAduck != duck && amAduck is MaterialThing materialThing)
                        {
                            bool flag = materialThing != prevOwner;
                            if (flag && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == prevOwner)
                                flag = false;
                            int num = 16;
                            if (flag || tape._framesSinceThrown > num)
                            {
                                materialThing.Destroy(destroyType);
                                if (Recorder.currentRecording != null)
                                    Recorder.currentRecording.LogBonus();
                            }
                        }
                    }
                }
                if (_prevOffdir != offDir)
                    ResetTrailHistory();
                _prevOffdir = offDir;
                _prevPos = position;
                _prevAngle = angleDegrees;
            }
            else
            {
                _tapeOffset = Vec2.Zero;
                base.Update();
                _timeSinceSwing += Maths.IncFrameTimer();
                if (_swordSwing.finished)
                    _swordSwing.speed = 0f;
                if (_hitWait > 0)
                    --_hitWait;
                ++_framesExisting;
                if (_framesExisting > 100)
                    _framesExisting = 100;
                if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 4.0f && _framesExisting > 10)
                    _wasLifted = true;
                if (owner != null)
                    _wasLifted = true;
                if (held)
                {
                    if (!(this is OldEnergyScimi))
                        _hold = -0.4f;
                    _wasLifted = true;
                    center = centerHeld;
                    _framesSinceThrown = 0;
                    _volatile = false;
                }
                else
                {
                    if (_framesSinceThrown == 1)
                    {
                        _throwSpin = Maths.RadToDeg(angle) - 90f;
                        _hold = 0f;
                        _swing = 0f;
                    }
                    if (_wasLifted)
                    {
                        if (enablePhysics)
                            angleDegrees = 90f + _throwSpin;
                        center = centerUnheld;
                        if (duck != null || owner is Holster)
                        {
                            _hold = 0f;
                            _swing = 0f;
                            angleDegrees = 0f;
                            _throwSpin = 0f;
                            return;
                        }
                    }
                    _volatile = _stayVolatile;
                    bool flag1 = false;
                    bool flag2 = false;
                    if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2.0f || !grounded)
                    {
                        if (!grounded && Level.CheckRect<Block>(position + new Vec2(-6f, -6f), position + new Vec2(6f, -2f)) != null)
                        {
                            flag2 = true;
                            if (vSpeed > 4.0f && !(this is OldEnergyScimi))
                                _volatile = true;
                        }
                        if (!flag2 && !_grounded && !this.initemspawner && (Level.CheckPoint<IPlatform>(position + new Vec2(0f, 8f)) == null || vSpeed < 0.0f))
                        {
                            PerformAirSpin();
                            flag1 = true;
                        }
                    }
                    if (_framesExisting > 15 && !(this is OldEnergyScimi) && Math.Abs(hSpeed) + Math.Abs(vSpeed) > 3.0f)
                        _volatile = true;
                    if (!flag1 | flag2)
                    {
                        _throwSpin %= 360f;
                        if (flag2)
                            _throwSpin = Math.Abs(_throwSpin - 90f) >= Math.Abs(_throwSpin + 90f) ? Lerp.Float(-90f, 0f, 16f) : Lerp.Float(_throwSpin, 90f, 16f);
                        else if (_throwSpin > 90.0f && _throwSpin < 270.0f)
                        {
                            _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                        }
                        else
                        {
                            if (_throwSpin > 180.0f)
                                _throwSpin -= 360f;
                            else if (_throwSpin < -180.0f)
                                _throwSpin += 360f;
                            _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                        }
                    }
                    if (_volatile && _hitWait == 0)
                    {
                        (Offset(barrelOffset) - position).Normalize();
                        Offset(barrelOffset);
                        bool flag3 = false;
                        foreach (Sword sword in Level.current.things[typeof(Sword)])
                        {
                            if (sword != this && sword.owner != null && sword._crouchStance && !sword._jabStance && !sword._jabStance && (hSpeed > 0.0f && sword.x > x - 4.0f || hSpeed < 0.0f && sword.x < x + 4.0f) && Collision.LineIntersect(barrelStartPos, barrelPosition, sword.barrelStartPos, sword.barrelPosition))
                            {
                                Shing();
                                sword.Shing();
                                flag3 = true;
                                _hitWait = 4;
                                sword.owner.hSpeed += offDir * 1f;
                                --sword.owner.vSpeed;
                                hSpeed = -hSpeed * 0.6f;
                            }
                        }
                        int num = 12;
                        if (_stayVolatile)
                            num = 22;
                        if (!flag3)
                        {
                            foreach (Chainsaw chainsaw in Level.current.things[typeof(Chainsaw)])
                            {
                                if (chainsaw.owner != null && chainsaw.throttle && Collision.LineIntersect(barrelStartPos, barrelPosition, chainsaw.barrelStartPos, chainsaw.barrelPosition))
                                {
                                    Shing();
                                    chainsaw.Shing(this);
                                    chainsaw.owner.hSpeed += offDir * 1f;
                                    --chainsaw.owner.vSpeed;
                                    flag3 = true;
                                    hSpeed = -hSpeed * 0.6f;
                                    _hitWait = 4;
                                    if (Recorder.currentRecording != null)
                                        Recorder.currentRecording.LogBonus();
                                }
                            }
                            if (!flag3)
                            {
                                Helmet helmet = Level.CheckLine<Helmet>(barrelStartPos, barrelPosition, null);
                                if (helmet != null && helmet.equippedDuck != null && (helmet.owner != prevOwner || _framesSinceThrown > num))
                                {
                                    hSpeed = (float)(-hSpeed * 0.6f);
                                    Shing();
                                    flag3 = true;
                                    _hitWait = 4;
                                }
                                else
                                {
                                    ChestPlate chestPlate = Level.CheckLine<ChestPlate>(barrelStartPos, barrelPosition, null);
                                    if (chestPlate != null && chestPlate.equippedDuck != null && (chestPlate.owner != prevOwner || _framesSinceThrown > num))
                                    {
                                        hSpeed = -this.hSpeed * 0.6f;
                                        Shing();
                                        flag3 = true;
                                        _hitWait = 4;
                                    }
                                }
                            }
                        }
                        if (!flag3 && isServerForObject)
                        {
                            foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition))
                            {
                                if (amAduck != duck && amAduck is MaterialThing materialThing)
                                {
                                    bool flag4 = materialThing != prevOwner;
                                    if (flag4 && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == prevOwner)
                                        flag4 = false;
                                    if (flag4 || _framesSinceThrown > num)
                                    {
                                        materialThing.Destroy(destroyType);
                                        if (Recorder.currentRecording != null)
                                            Recorder.currentRecording.LogBonus();
                                    }
                                }
                            }
                        }
                    }
                }
                if (_stayVolatile && isServerForObject)
                {
                    int num = 28;
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition))
                    {
                        if (amAduck != duck && amAduck is MaterialThing materialThing)
                        {
                            bool flag = materialThing != prevOwner;
                            if (flag && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == prevOwner)
                                flag = false;
                            if (flag || _framesSinceThrown > num)
                            {
                                materialThing.Destroy(destroyType);
                                if (Recorder.currentRecording != null)
                                    Recorder.currentRecording.LogBonus();
                            }
                        }
                    }
                }
                if (owner == null)
                {
                    _swinging = false;
                    _jabStance = false;
                    _crouchStance = false;
                    _pullBack = false;
                    _swung = false;
                    _shing = false;
                    _swing = 0f;
                    _swingPress = false;
                    _slamStance = false;
                    _unslam = 0;
                }
                _afterSwingCounter += Maths.IncFrameTimer();
                if (isServerForObject)
                {
                    if (_unslam > 1)
                    {
                        --_unslam;
                        _slamStance = true;
                    }
                    else if (_unslam == 1)
                    {
                        _unslam = 0;
                        _slamStance = false;
                    }
                    if (_pullBack)
                    {
                        if (duck != null)
                        {
                            if (_jabStance)
                            {
                                _pullBack = false;
                                _swinging = true;
                            }
                            else
                            {
                                _swinging = true;
                                _pullBack = false;
                            }
                        }
                    }
                    else if (_swinging)
                    {
                        if (_jabStance)
                        {
                            _addOffsetX = MathHelper.Lerp(_addOffsetX, 3f, 0.4f);
                            if (_addOffsetX > 2.0f && !action)
                                _swinging = false;
                        }
                        else if (raised)
                        {
                            _swing = MathHelper.Lerp(_swing, -2.8f, 0.2f);
                            if (_swing < -2.4f && !action)
                            {
                                _swinging = false;
                                _swing = 1.8f;
                            }
                        }
                        else
                        {
                            _swing = MathHelper.Lerp(_swing, 2.1f, 0.4f);
                            if (_swing > 1.8f && !action)
                            {
                                _swinging = false;
                                _swing = 1.8f;
                            }
                        }
                    }
                    else
                    {
                        if (_wasLifted && !_swinging && (!_swingPress || _shing || _jabStance && _addOffsetX < 1.0f || !_jabStance && _swing < 1.6f))
                        {
                            if (_jabStance)
                                UpdateJabPullback();
                            else if (_slamStance)
                                UpdateSlamPullback();
                            else if (_afterSwingWait < _afterSwingCounter)
                            {
                                float amount = 0.36f;
                                if (this is OldEnergyScimi && (duck == null || !duck.grounded || !duck.crouch))
                                    amount = 0.36f;
                                _swing = MathHelper.Lerp(_swing, -0.22f, amount);
                                _addOffsetX = MathHelper.Lerp(_addOffsetX, 1f, 0.2f);
                                if (_addOffsetX > 0.0f)
                                    _addOffsetX = 0f;
                                _addOffsetY = MathHelper.Lerp(_addOffsetY, 1f, 0.2f);
                                if (_addOffsetY > 0.0f)
                                    _addOffsetY = 0f;
                            }
                        }
                        if ((_swing < 0.0f || _jabStance) && _swing < 0.0f && _enforceJabSwing)
                        {
                            _swing = 0f;
                            _shing = false;
                            _swung = false;
                        }
                    }
                }
                if (duck != null)
                {
                    RestoreCollisionSize(true);
                    _swingPress = false;
                    if (!_pullBack && !_swinging)
                    {
                        _crouchStance = false;
                        _jabStance = false;
                        if (duck.crouch)
                        {
                            if (!_pullBack && !_swinging && duck.inputProfile.Down(offDir > 0 ? "LEFT" : "RIGHT"))
                                _jabStance = true;
                            _crouchStance = true;
                        }
                        if (!_crouchStance || _jabStance)
                            _slamStance = false;
                    }
                    UpdateCrouchStance();
                }
                else
                {
                    RestoreCollisionSize();
                    thickness = 0f;
                }
                if ((!(this is OldEnergyScimi) && _swung || _swinging) && !_shing)
                {
                    (Offset(barrelOffset) - position).Normalize();
                    Offset(barrelOffset);
                    IEnumerable<IAmADuck> amAducks = Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition);
                    Block block = Level.CheckLine<Block>(barrelStartPos, barrelPosition);
                    Level.CheckRect<Icicles>(barrelStartPos, barrelPosition)?.Hurt(100f);
                    if (!(this is OldEnergyScimi) && block != null && !_slamStance)
                    {
                        if (offDir < 0 && block.x > x)
                            block = null;
                        else if (offDir > 0 && block.x < x)
                            block = null;
                    }
                    bool flag = false;
                    if (block != null && _clashWithWalls)
                    {
                        if (_slamStance)
                            owner.vSpeed = -5f;
                        Shing();
                        if (_slamStance)
                        {
                            _swung = false;
                            _unslam = 20;
                        }
                        if (block is Window)
                            block.Destroy(new DTImpact(this));
                    }
                    else if (!_jabStance && !_slamStance && isServerForObject)
                    {
                        Thing ignore = null;
                        if (duck != null)
                            ignore = duck.GetEquipment(typeof(Helmet));
                        Vec2 vec2_1 = barrelPosition + barrelVector * 3f;
                        QuadLaserBullet quadLaserBullet = Level.CheckRect<QuadLaserBullet>(new Vec2(position.x < vec2_1.x ? position.x : vec2_1.x, position.y < vec2_1.y ? position.y : vec2_1.y), new Vec2(position.x > vec2_1.x ? position.x : vec2_1.x, position.y > vec2_1.y ? position.y : vec2_1.y));
                        if (quadLaserBullet != null)
                        {
                            Shing();
                            Fondle(quadLaserBullet);
                            quadLaserBullet.safeFrames = 8;
                            quadLaserBullet.safeDuck = duck;
                            Vec2 vec2_2 = quadLaserBullet.travel;
                            float length = vec2_2.length;
                            float num = 1.5f;
                            vec2_2 = offDir <= 0 ? new Vec2(-length * num, 0f) : new Vec2(length * num, 0f);
                            quadLaserBullet.travel = vec2_2;
                            QuadLaserHit(quadLaserBullet);
                        }
                        else
                        {
                            Helmet helmet = Level.CheckLine<Helmet>(barrelStartPos, barrelPosition, ignore);
                            if (helmet != null && helmet.equippedDuck != null && helmet.owner != null)
                            {
                                Shing();
                                helmet.owner.hSpeed += offDir * 3f;
                                helmet.owner.vSpeed -= 2f;
                                if (helmet.duck != null)
                                    helmet.duck.crippleTimer = 1f;
                                helmet.Hurt(0.53f);
                                flag = true;
                            }
                            else
                            {
                                if (duck != null)
                                    ignore = duck.GetEquipment(typeof(ChestPlate));
                                ChestPlate chestPlate = Level.CheckLine<ChestPlate>(barrelStartPos, barrelPosition, ignore);
                                if (chestPlate != null && chestPlate.equippedDuck != null && chestPlate.owner != null)
                                {
                                    Shing();
                                    chestPlate.owner.hSpeed += offDir * 3f;
                                    chestPlate.owner.vSpeed -= 2f;
                                    if (chestPlate.duck != null)
                                        chestPlate.duck.crippleTimer = 1f;
                                    chestPlate.Hurt(0.53f);
                                    flag = true;
                                }
                            }
                        }
                    }
                    if (!flag && isServerForObject)
                    {
                        foreach (Sword sword in Level.current.things[typeof(Sword)])
                        {
                            if (sword != this && sword.held && sword.owner != null && !_jabStance && !sword._jabStance && duck != null && Collision.LineIntersect(barrelStartPos, barrelPosition, sword.barrelStartPos, sword.barrelPosition))
                            {
                                Shing();
                                sword.Shing();
                                if (this is OldEnergyScimi)
                                {
                                    sword.owner.hSpeed += offDir * 5f;
                                    sword.owner.vSpeed -= 4f;
                                    duck.hSpeed += -offDir * 5f;
                                    duck.vSpeed -= 4f;
                                    if (isServerForObject)
                                    {
                                        EnergyScimitarBlast energyScimitarBlast1 = new EnergyScimitarBlast((sword.owner.position + owner.position) / 2f + new Vec2(0f, -16f), new Vec2(0f, -2000f));
                                        Level.Add(energyScimitarBlast1);
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        EnergyScimitarBlast energyScimitarBlast2 = new EnergyScimitarBlast((sword.owner.position + owner.position) / 2f + new Vec2(0f, 16f), new Vec2(0f, 2000f));
                                        Level.Add(energyScimitarBlast2);
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast2.position, energyScimitarBlast2._target));
                                    }
                                }
                                else
                                {
                                    sword.owner.hSpeed += offDir * 3f;
                                    sword.owner.vSpeed -= 2f;
                                    duck.hSpeed += -offDir * 3f;
                                    duck.vSpeed -= 2f;
                                }
                                if (sword.duck != null && duck != null)
                                {
                                    sword.duck.crippleTimer = 1f;
                                    duck.crippleTimer = 1f;
                                    flag = true;
                                }
                            }
                        }
                    }
                    if (flag || !isServerForObject)
                        return;
                    foreach (IAmADuck amAduck in amAducks)
                    {
                        if (amAduck != duck && amAduck != prevOwner && amAduck is MaterialThing materialThing)
                        {
                            if (materialThing is Duck duck && !duck.destroyed && this.duck != null)
                            {
                                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                ++Global.data.swordKills.valueInt;
                            }
                            materialThing.Destroy(destroyType);
                        }
                    }
                }
                else
                {
                    if (!_crouchStance || duck == null)
                        return;
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(barrelStartPos, barrelPosition))
                    {
                        if (amAduck != duck && amAduck is MaterialThing t)
                        {
                            if (t.vSpeed > 0.5f && t.bottom < position.y - 8.0f && t.left < barrelPosition.x && t.right > barrelPosition.x)
                            {
                                if (t is Duck duck && !duck.destroyed)
                                {
                                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                    ++Global.data.swordKills.valueInt;
                                }
                                t.Destroy(destroyType);
                            }
                            else if (!_jabStance && !t.destroyed && (offDir > 0 && t.x > duck.x || offDir < 0 && t.x < duck.x))
                            {
                                if (t is Duck)
                                    (t as Duck).crippleTimer = 1f;
                                else if (duck.x > t.x && t.hSpeed > 1.5f || duck.x < t.x && t.hSpeed < -1.5f)
                                {
                                    if (t is Duck duck && !duck.destroyed)
                                    {
                                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                        ++Global.data.swordKills.valueInt;
                                    }
                                    t.Destroy(destroyType);
                                }
                                Fondle(t);
                                t.hSpeed = offDir * 3f;
                                t.vSpeed = -2f;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void ResetTrailHistory()
        {
            _lastAngles = new float[8];
            _lastPositions = new Vec2[8];
            _lastIndex = 0;
            _lastSize = 0;
            _lastHistoryPos = Vec2.Zero;
        }

        protected int historyIndex(int idx)
        {
            int num = _lastIndex - idx - 1;
            if (num < 0)
                num += 8;
            return num;
        }

        protected void addHistory(float angle, Vec2 position)
        {
            if (_lastHistoryPos != Vec2.Zero)
            {
                _lastAngles[_lastIndex] = (float)((angle + _lastHistoryAngle) / 2.0);
                _lastPositions[_lastIndex] = (position + _lastHistoryPos) / 2f;
                _lastIndex = (_lastIndex + 1) % 8;
                ++_lastSize;
            }
            _lastAngles[_lastIndex] = angle;
            _lastPositions[_lastIndex] = position;
            _lastIndex = (_lastIndex + 1) % 8;
            ++_lastSize;
            _lastHistoryPos = position;
            _lastHistoryAngle = angle;
        }

        public override void Draw()
        {
            Sword._playedShing = false;
            if (owner != _prevHistoryOwner)
            {
                _prevHistoryOwner = owner;
                ResetTrailHistory();
            }
            if (DevConsole.showCollision)
                Graphics.DrawLine(barrelStartPos, barrelPosition, Color.Red, depth: ((Depth)1f));
            if (_swordSwing.speed > 0.0)
            {
                if (duck != null)
                    _swordSwing.flipH = duck.offDir <= 0;
                _swordSwing.alpha = 0.4f;
                _swordSwing.position = this.position;
                _swordSwing.depth = this.depth + 1;
                _swordSwing.Draw();
            }
            Vec2 position = this.position;
            Depth depth = this.depth;
            graphic.color = Color.White;
            if (owner == null && velocity.length > 1.0 || _swing != 0.0 || tape != null && bayonetLethal)
            {
                float alpha = this.alpha;
                this.alpha = 1f;
                float angle1 = angle;
                _drawing = true;
                float angle2 = _angle;
                angle = angle1;
                for (int idx = 0; idx < 7; ++idx)
                {
                    base.Draw();
                    if (_lastSize > idx)
                    {
                        int index = historyIndex(idx);
                        _angle = _lastAngles[index];
                        this.position = _lastPositions[index];
                        this.depth -= 2;
                        this.alpha -= 0.15f;
                        graphic.color = Color.Red;
                    }
                    else
                        break;
                }
                this.position = position;
                this.depth = depth;
                this.alpha = alpha;
                _angle = angle2;
                xscale = 1f;
                _drawing = false;
            }
            else
                base.Draw();
            addHistory(angle, this.position);
        }

        protected virtual void OnSwing()
        {
        }

        public override void OnPressAction()
        {
            if (_crouchStance && _jabStance && !_swinging || !_crouchStance && !_swinging && _swing < 0.1f)
            {
                if (_jabStance && !_allowJabMotion)
                    return;
                _afterSwingCounter = 0f;
                _pullBack = true;
                _swung = true;
                _shing = false;
                _timeSinceSwing = 0f;
                OnSwing();
                if (_swingSound != null)
                    SFX.Play(_swingSound, Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f));
                if (_jabStance)
                    return;
                _swordSwing.speed = 1f;
                _swordSwing.frame = 0;
            }
            else
            {
                if (!_crouchStance || _jabStance || duck == null || duck.grounded)
                    return;
                _slamStance = true;
            }
        }

        public override void Fire()
        {
        }
    }
}
