// Decompiled with JetBrains decompiler
// Type: DuckGame.Sword
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._drawing ? this._angle : base.angle + (this._swing + this._hold) * offDir;
            set => this._angle = value;
        }

        public bool jabStance => this._jabStance;

        public bool crouchStance => this._crouchStance;

        public virtual Vec2 barrelStartPos
        {
            get
            {
                if (this.owner == null)
                    return this.position - (this.Offset(this.barrelOffset) - this.position).normalized * 6f;
                return this._slamStance ? this.position + (this.Offset(this.barrelOffset) - this.position).normalized * 12f : this.position + (this.Offset(this.barrelOffset) - this.position).normalized * 2f;
            }
        }

        public Sword(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this.graphic = new Sprite("sword");
            this.center = new Vec2(4f, 21f);
            this.collisionOffset = new Vec2(-2f, -16f);
            this.collisionSize = new Vec2(4f, 18f);
            this._barrelOffsetTL = new Vec2(4f, 1f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(-4f, 4f);
            this.weight = 0.9f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._swordSwing = new SpriteMap("swordSwipe", 32, 32);
            this._swordSwing.AddAnimation("swing", 0.6f, false, 0, 1, 1, 2);
            this._swordSwing.currentAnimation = "swing";
            this._swordSwing.speed = 0.0f;
            this._swordSwing.center = new Vec2(9f, 25f);
            this.holsterAngle = 180f;
            this.tapedIndexPreference = 0;
            this._bouncy = 0.5f;
            this._impactThreshold = 0.3f;
            this.editorTooltip = "Basically a giant letter opener.";
        }

        public override void Initialize() => base.Initialize();

        public override Vec2 tapedOffset => this.tapedCompatriot is Gun ? (this.tapedCompatriot as Gun).barrelOffset + new Vec2(-14f, 2f) : new Vec2(-6f, -3f);

        public override void UpdateTapedPositioning(TapedGun pTaped)
        {
            if (pTaped.gun1 != null && pTaped.gun2 != null)
                this.angleDegrees = pTaped.angleDegrees - 90 * offDir;
            if (this.tapedCompatriot is Gun)
            {
                (this.tapedCompatriot as Gun).addVerticalTapeOffset = false;
                this.tape._holdOffset = (this.tapedCompatriot as Gun)._holdOffset;
                this.tape.handOffset = (this.tapedCompatriot as Gun).handOffset;
            }
            this.collisionOffset = new Vec2(-4f, 0.0f);
            this.collisionSize = new Vec2(4f, 4f);
            this.center = this.centerHeld;
            this.thickness = 0.0f;
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

        public override Holdable BecomeTapedMonster(TapedGun pTaped) => pTaped.gun1 is Sword && pTaped.gun2 is Sword ? new TapedSword(this.x, this.y) : (Holdable)null;

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
            if (this._shing)
                return;
            this._pullBack = false;
            this._swinging = false;
            this._shing = true;
            this._swingPress = false;
            if (!Sword._playedShing)
            {
                Sword._playedShing = true;
                SFX.Play("swordClash", Rando.Float(0.6f, 0.7f), Rando.Float(-0.1f, 0.1f), Rando.Float(-0.1f, 0.1f));
            }
            Vec2 normalized = (this.position - this.barrelPosition).normalized;
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
            if (this.duck != null)
                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
            this._swung = false;
            this._swordSwing.speed = 0.0f;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this.duck == null)
                return false;
            if (this.blocked == 0)
            {
                this.duck.AddCoolness(1);
            }
            else
            {
                ++this.blocked;
                if (this.blocked > 4)
                {
                    this.blocked = 1;
                    this.duck.AddCoolness(1);
                }
            }
            RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
            SFX.Play("ting");
            return base.Hit(bullet, hitPos);
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.tape != null && this.tape.owner != null || !this._wasLifted || this.owner != null)
                return;
            switch (with)
            {
                case Block _:
                label_3:
                    this.Shing();
                    this._framesSinceThrown = 25;
                    break;
                case IPlatform _:
                    if (from != ImpactedFrom.Bottom || (double)this.vSpeed <= 0.0)
                        break;
                    goto label_3;
            }
        }

        public override void ReturnToWorld()
        {
            this._throwSpin = 90f;
            this.RestoreCollisionSize();
        }

        public virtual void RestoreCollisionSize(bool pHeld = false)
        {
            if (pHeld)
            {
                this.collisionOffset = new Vec2(-4f, 0.0f);
                this.collisionSize = new Vec2(4f, 4f);
                if (!this._crouchStance || this._jabStance)
                    return;
                this.collisionOffset = new Vec2(-2f, -19f);
                this.collisionSize = new Vec2(4f, 16f);
                this.thickness = 3f;
            }
            else
            {
                this.collisionOffset = new Vec2(-2f, -16f);
                this.collisionSize = new Vec2(4f, 18f);
                if (!this._wasLifted)
                    return;
                this.collisionOffset = new Vec2(-4f, -2f);
                this.collisionSize = new Vec2(8f, 4f);
            }
        }

        protected virtual void PerformAirSpin()
        {
            if ((double)this.hSpeed > 0.0)
                this._throwSpin += (float)(((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed)) * 2.0 + 4.0);
            else
                this._throwSpin -= (float)(((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed)) * 2.0 + 4.0);
        }

        public new bool held => this.duck != null && this.duck.holdObject == this;

        protected virtual void QuadLaserHit(QuadLaserBullet pBullet)
        {
        }

        protected virtual void UpdateCrouchStance()
        {
            if (!this._crouchStance)
            {
                this._hold = -0.4f;
                this.handOffset = new Vec2(this._addOffsetX, this._addOffsetY);
                this._holdOffset = new Vec2(this._addOffsetX - 4f, 4f + this._addOffsetY) + this.additionalHoldOffset;
            }
            else
            {
                this._hold = 0.0f;
                this._holdOffset = new Vec2(0.0f + this._addOffsetX, 4f + this._addOffsetY) + this.additionalHoldOffset;
                this.handOffset = new Vec2(3f + this._addOffsetX, this._addOffsetY);
            }
        }

        public virtual DestroyType destroyType => new DTImpale(this);

        protected virtual void UpdateJabPullback()
        {
            this._swing = MathHelper.Lerp(this._swing, 1.75f, 0.4f);
            if (_swing > 1.54999995231628)
            {
                this._swing = 1.55f;
                this._shing = false;
                this._swung = false;
            }
            this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -12f, 0.45f);
            if (_addOffsetX < -12.0)
                this._addOffsetX = -12f;
            this._addOffsetY = MathHelper.Lerp(this._addOffsetY, -4f, 0.35f);
            if (_addOffsetX >= -3.0)
                return;
            this._addOffsetY = -3f;
        }

        protected virtual void UpdateSlamPullback()
        {
            this._swing = MathHelper.Lerp(this._swing, 3.14f, 0.8f);
            if (_swing > 3.09999990463257 && this._unslam == 0)
            {
                this._swing = 3.14f;
                this._shing = false;
                this._swung = true;
            }
            this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -5f, 0.45f);
            if (_addOffsetX < -4.59999990463257)
                this._addOffsetX = -5f;
            this._addOffsetY = MathHelper.Lerp(this._addOffsetY, -6f, 0.35f);
            if (_addOffsetX >= -5.5)
                return;
            this._addOffsetY = -6f;
        }

        public override void Update()
        {
            this.bayonetLethal = false;
            if (this.tape != null && this.tapedCompatriot != null)
            {
                if (this.tapedCompatriot != null && ((double)Math.Abs(this._prevAngle - this.angleDegrees) > 1.0 || (double)(this._prevPos - this.position).length > 2.0))
                    this.bayonetLethal = true;
                if (this.isServerForObject && this.bayonetLethal)
                {
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(this.Offset(new Vec2(4f, 10f) - this.center + this._extraOffset), this.barrelPosition))
                    {
                        if (amAduck != this.duck && amAduck is MaterialThing materialThing)
                        {
                            bool flag = materialThing != this.prevOwner;
                            if (flag && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == this.prevOwner)
                                flag = false;
                            int num = 16;
                            if (flag || tape._framesSinceThrown > num)
                            {
                                materialThing.Destroy(this.destroyType);
                                if (Recorder.currentRecording != null)
                                    Recorder.currentRecording.LogBonus();
                            }
                        }
                    }
                }
                if (this._prevOffdir != offDir)
                    this.ResetTrailHistory();
                this._prevOffdir = offDir;
                this._prevPos = this.position;
                this._prevAngle = this.angleDegrees;
            }
            else
            {
                this._tapeOffset = Vec2.Zero;
                base.Update();
                this._timeSinceSwing += Maths.IncFrameTimer();
                if (this._swordSwing.finished)
                    this._swordSwing.speed = 0.0f;
                if (this._hitWait > 0)
                    --this._hitWait;
                ++this._framesExisting;
                if (this._framesExisting > 100)
                    this._framesExisting = 100;
                if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 4.0 && this._framesExisting > 10)
                    this._wasLifted = true;
                if (this.owner != null)
                    this._wasLifted = true;
                if (this.held)
                {
                    if (!(this is OldEnergyScimi))
                        this._hold = -0.4f;
                    this._wasLifted = true;
                    this.center = this.centerHeld;
                    this._framesSinceThrown = 0;
                    this._volatile = false;
                }
                else
                {
                    if (this._framesSinceThrown == 1)
                    {
                        this._throwSpin = Maths.RadToDeg(this.angle) - 90f;
                        this._hold = 0.0f;
                        this._swing = 0.0f;
                    }
                    if (this._wasLifted)
                    {
                        if (this.enablePhysics)
                            this.angleDegrees = 90f + this._throwSpin;
                        this.center = this.centerUnheld;
                        if (this.duck != null || this.owner is Holster)
                        {
                            this._hold = 0.0f;
                            this._swing = 0.0f;
                            this.angleDegrees = 0.0f;
                            this._throwSpin = 0.0f;
                            return;
                        }
                    }
                    this._volatile = this._stayVolatile;
                    bool flag1 = false;
                    bool flag2 = false;
                    if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 2.0 || !this.grounded)
                    {
                        if (!this.grounded && Level.CheckRect<Block>(this.position + new Vec2(-6f, -6f), this.position + new Vec2(6f, -2f)) != null)
                        {
                            flag2 = true;
                            if ((double)this.vSpeed > 4.0 && !(this is OldEnergyScimi))
                                this._volatile = true;
                        }
                        if (!flag2 && !this._grounded && (Level.CheckPoint<IPlatform>(this.position + new Vec2(0.0f, 8f)) == null || (double)this.vSpeed < 0.0))
                        {
                            this.PerformAirSpin();
                            flag1 = true;
                        }
                    }
                    if (this._framesExisting > 15 && !(this is OldEnergyScimi) && (double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 3.0)
                        this._volatile = true;
                    if (!flag1 | flag2)
                    {
                        this._throwSpin %= 360f;
                        if (flag2)
                            this._throwSpin = (double)Math.Abs(this._throwSpin - 90f) >= (double)Math.Abs(this._throwSpin + 90f) ? Lerp.Float(-90f, 0.0f, 16f) : Lerp.Float(this._throwSpin, 90f, 16f);
                        else if (_throwSpin > 90.0 && _throwSpin < 270.0)
                        {
                            this._throwSpin = Lerp.Float(this._throwSpin, 180f, 14f);
                        }
                        else
                        {
                            if (_throwSpin > 180.0)
                                this._throwSpin -= 360f;
                            else if (_throwSpin < -180.0)
                                this._throwSpin += 360f;
                            this._throwSpin = Lerp.Float(this._throwSpin, 0.0f, 14f);
                        }
                    }
                    if (this._volatile && this._hitWait == 0)
                    {
                        (this.Offset(this.barrelOffset) - this.position).Normalize();
                        this.Offset(this.barrelOffset);
                        bool flag3 = false;
                        foreach (Sword sword in Level.current.things[typeof(Sword)])
                        {
                            if (sword != this && sword.owner != null && sword._crouchStance && !sword._jabStance && !sword._jabStance && ((double)this.hSpeed > 0.0 && (double)sword.x > (double)this.x - 4.0 || (double)this.hSpeed < 0.0 && (double)sword.x < (double)this.x + 4.0) && Collision.LineIntersect(this.barrelStartPos, this.barrelPosition, sword.barrelStartPos, sword.barrelPosition))
                            {
                                this.Shing();
                                sword.Shing();
                                flag3 = true;
                                this._hitWait = 4;
                                sword.owner.hSpeed += offDir * 1f;
                                --sword.owner.vSpeed;
                                this.hSpeed = (float)(-(double)this.hSpeed * 0.600000023841858);
                            }
                        }
                        int num = 12;
                        if (this._stayVolatile)
                            num = 22;
                        if (!flag3)
                        {
                            foreach (Chainsaw chainsaw in Level.current.things[typeof(Chainsaw)])
                            {
                                if (chainsaw.owner != null && chainsaw.throttle && Collision.LineIntersect(this.barrelStartPos, this.barrelPosition, chainsaw.barrelStartPos, chainsaw.barrelPosition))
                                {
                                    this.Shing();
                                    chainsaw.Shing(this);
                                    chainsaw.owner.hSpeed += offDir * 1f;
                                    --chainsaw.owner.vSpeed;
                                    flag3 = true;
                                    this.hSpeed = (float)(-(double)this.hSpeed * 0.600000023841858);
                                    this._hitWait = 4;
                                    if (Recorder.currentRecording != null)
                                        Recorder.currentRecording.LogBonus();
                                }
                            }
                            if (!flag3)
                            {
                                Helmet helmet = Level.CheckLine<Helmet>(this.barrelStartPos, this.barrelPosition, null);
                                if (helmet != null && helmet.equippedDuck != null && (helmet.owner != this.prevOwner || _framesSinceThrown > num))
                                {
                                    this.hSpeed = (float)(-(double)this.hSpeed * 0.600000023841858);
                                    this.Shing();
                                    flag3 = true;
                                    this._hitWait = 4;
                                }
                                else
                                {
                                    ChestPlate chestPlate = Level.CheckLine<ChestPlate>(this.barrelStartPos, this.barrelPosition, null);
                                    if (chestPlate != null && chestPlate.equippedDuck != null && (chestPlate.owner != this.prevOwner || _framesSinceThrown > num))
                                    {
                                        this.hSpeed = (float)(-(double)this.hSpeed * 0.600000023841858);
                                        this.Shing();
                                        flag3 = true;
                                        this._hitWait = 4;
                                    }
                                }
                            }
                        }
                        if (!flag3 && this.isServerForObject)
                        {
                            foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(this.barrelStartPos, this.barrelPosition))
                            {
                                if (amAduck != this.duck && amAduck is MaterialThing materialThing)
                                {
                                    bool flag4 = materialThing != this.prevOwner;
                                    if (flag4 && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == this.prevOwner)
                                        flag4 = false;
                                    if (flag4 || _framesSinceThrown > num)
                                    {
                                        materialThing.Destroy(this.destroyType);
                                        if (Recorder.currentRecording != null)
                                            Recorder.currentRecording.LogBonus();
                                    }
                                }
                            }
                        }
                    }
                }
                if (this._stayVolatile && this.isServerForObject)
                {
                    int num = 28;
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(this.barrelStartPos, this.barrelPosition))
                    {
                        if (amAduck != this.duck && amAduck is MaterialThing materialThing)
                        {
                            bool flag = materialThing != this.prevOwner;
                            if (flag && materialThing is RagdollPart && (materialThing as RagdollPart).doll != null && (materialThing as RagdollPart).doll.captureDuck == this.prevOwner)
                                flag = false;
                            if (flag || _framesSinceThrown > num)
                            {
                                materialThing.Destroy(this.destroyType);
                                if (Recorder.currentRecording != null)
                                    Recorder.currentRecording.LogBonus();
                            }
                        }
                    }
                }
                if (this.owner == null)
                {
                    this._swinging = false;
                    this._jabStance = false;
                    this._crouchStance = false;
                    this._pullBack = false;
                    this._swung = false;
                    this._shing = false;
                    this._swing = 0.0f;
                    this._swingPress = false;
                    this._slamStance = false;
                    this._unslam = 0;
                }
                this._afterSwingCounter += Maths.IncFrameTimer();
                if (this.isServerForObject)
                {
                    if (this._unslam > 1)
                    {
                        --this._unslam;
                        this._slamStance = true;
                    }
                    else if (this._unslam == 1)
                    {
                        this._unslam = 0;
                        this._slamStance = false;
                    }
                    if (this._pullBack)
                    {
                        if (this.duck != null)
                        {
                            if (this._jabStance)
                            {
                                this._pullBack = false;
                                this._swinging = true;
                            }
                            else
                            {
                                this._swinging = true;
                                this._pullBack = false;
                            }
                        }
                    }
                    else if (this._swinging)
                    {
                        if (this._jabStance)
                        {
                            this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 3f, 0.4f);
                            if (_addOffsetX > 2.0 && !this.action)
                                this._swinging = false;
                        }
                        else if (this.raised)
                        {
                            this._swing = MathHelper.Lerp(this._swing, -2.8f, 0.2f);
                            if (_swing < -2.40000009536743 && !this.action)
                            {
                                this._swinging = false;
                                this._swing = 1.8f;
                            }
                        }
                        else
                        {
                            this._swing = MathHelper.Lerp(this._swing, 2.1f, 0.4f);
                            if (_swing > 1.79999995231628 && !this.action)
                            {
                                this._swinging = false;
                                this._swing = 1.8f;
                            }
                        }
                    }
                    else
                    {
                        if (this._wasLifted && !this._swinging && (!this._swingPress || this._shing || this._jabStance && _addOffsetX < 1.0 || !this._jabStance && _swing < 1.60000002384186))
                        {
                            if (this._jabStance)
                                this.UpdateJabPullback();
                            else if (this._slamStance)
                                this.UpdateSlamPullback();
                            else if (_afterSwingWait < (double)this._afterSwingCounter)
                            {
                                float amount = 0.36f;
                                if (this is OldEnergyScimi && (this.duck == null || !this.duck.grounded || !this.duck.crouch))
                                    amount = 0.36f;
                                this._swing = MathHelper.Lerp(this._swing, -0.22f, amount);
                                this._addOffsetX = MathHelper.Lerp(this._addOffsetX, 1f, 0.2f);
                                if (_addOffsetX > 0.0)
                                    this._addOffsetX = 0.0f;
                                this._addOffsetY = MathHelper.Lerp(this._addOffsetY, 1f, 0.2f);
                                if (_addOffsetY > 0.0)
                                    this._addOffsetY = 0.0f;
                            }
                        }
                        if ((_swing < 0.0 || this._jabStance) && _swing < 0.0 && this._enforceJabSwing)
                        {
                            this._swing = 0.0f;
                            this._shing = false;
                            this._swung = false;
                        }
                    }
                }
                if (this.duck != null)
                {
                    this.RestoreCollisionSize(true);
                    this._swingPress = false;
                    if (!this._pullBack && !this._swinging)
                    {
                        this._crouchStance = false;
                        this._jabStance = false;
                        if (this.duck.crouch)
                        {
                            if (!this._pullBack && !this._swinging && this.duck.inputProfile.Down(this.offDir > 0 ? "LEFT" : "RIGHT"))
                                this._jabStance = true;
                            this._crouchStance = true;
                        }
                        if (!this._crouchStance || this._jabStance)
                            this._slamStance = false;
                    }
                    this.UpdateCrouchStance();
                }
                else
                {
                    this.RestoreCollisionSize();
                    this.thickness = 0.0f;
                }
                if ((!(this is OldEnergyScimi) && this._swung || this._swinging) && !this._shing)
                {
                    (this.Offset(this.barrelOffset) - this.position).Normalize();
                    this.Offset(this.barrelOffset);
                    IEnumerable<IAmADuck> amAducks = Level.CheckLineAll<IAmADuck>(this.barrelStartPos, this.barrelPosition);
                    Block block = Level.CheckLine<Block>(this.barrelStartPos, this.barrelPosition);
                    Level.CheckRect<Icicles>(this.barrelStartPos, this.barrelPosition)?.Hurt(100f);
                    if (!(this is OldEnergyScimi) && block != null && !this._slamStance)
                    {
                        if (this.offDir < 0 && (double)block.x > (double)this.x)
                            block = null;
                        else if (this.offDir > 0 && (double)block.x < (double)this.x)
                            block = null;
                    }
                    bool flag = false;
                    if (block != null && this._clashWithWalls)
                    {
                        if (this._slamStance)
                            this.owner.vSpeed = -5f;
                        this.Shing();
                        if (this._slamStance)
                        {
                            this._swung = false;
                            this._unslam = 20;
                        }
                        if (block is Window)
                            block.Destroy(new DTImpact(this));
                    }
                    else if (!this._jabStance && !this._slamStance && this.isServerForObject)
                    {
                        Thing ignore = null;
                        if (this.duck != null)
                            ignore = this.duck.GetEquipment(typeof(Helmet));
                        Vec2 vec2_1 = this.barrelPosition + this.barrelVector * 3f;
                        QuadLaserBullet quadLaserBullet = Level.CheckRect<QuadLaserBullet>(new Vec2(position.x < (double)vec2_1.x ? this.position.x : vec2_1.x, position.y < (double)vec2_1.y ? this.position.y : vec2_1.y), new Vec2(position.x > (double)vec2_1.x ? this.position.x : vec2_1.x, position.y > (double)vec2_1.y ? this.position.y : vec2_1.y));
                        if (quadLaserBullet != null)
                        {
                            this.Shing();
                            this.Fondle(quadLaserBullet);
                            quadLaserBullet.safeFrames = 8;
                            quadLaserBullet.safeDuck = this.duck;
                            Vec2 vec2_2 = quadLaserBullet.travel;
                            float length = vec2_2.length;
                            float num = 1.5f;
                            vec2_2 = this.offDir <= 0 ? new Vec2(-length * num, 0.0f) : new Vec2(length * num, 0.0f);
                            quadLaserBullet.travel = vec2_2;
                            this.QuadLaserHit(quadLaserBullet);
                        }
                        else
                        {
                            Helmet helmet = Level.CheckLine<Helmet>(this.barrelStartPos, this.barrelPosition, ignore);
                            if (helmet != null && helmet.equippedDuck != null && helmet.owner != null)
                            {
                                this.Shing();
                                helmet.owner.hSpeed += offDir * 3f;
                                helmet.owner.vSpeed -= 2f;
                                if (helmet.duck != null)
                                    helmet.duck.crippleTimer = 1f;
                                helmet.Hurt(0.53f);
                                flag = true;
                            }
                            else
                            {
                                if (this.duck != null)
                                    ignore = this.duck.GetEquipment(typeof(ChestPlate));
                                ChestPlate chestPlate = Level.CheckLine<ChestPlate>(this.barrelStartPos, this.barrelPosition, ignore);
                                if (chestPlate != null && chestPlate.equippedDuck != null && chestPlate.owner != null)
                                {
                                    this.Shing();
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
                    if (!flag && this.isServerForObject)
                    {
                        foreach (Sword sword in Level.current.things[typeof(Sword)])
                        {
                            if (sword != this && sword.held && sword.owner != null && !this._jabStance && !sword._jabStance && this.duck != null && Collision.LineIntersect(this.barrelStartPos, this.barrelPosition, sword.barrelStartPos, sword.barrelPosition))
                            {
                                this.Shing();
                                sword.Shing();
                                if (this is OldEnergyScimi)
                                {
                                    sword.owner.hSpeed += offDir * 5f;
                                    sword.owner.vSpeed -= 4f;
                                    this.duck.hSpeed += -this.offDir * 5f;
                                    this.duck.vSpeed -= 4f;
                                    if (this.isServerForObject)
                                    {
                                        EnergyScimitarBlast energyScimitarBlast1 = new EnergyScimitarBlast((sword.owner.position + this.owner.position) / 2f + new Vec2(0.0f, -16f), new Vec2(0.0f, -2000f));
                                        Level.Add(energyScimitarBlast1);
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        EnergyScimitarBlast energyScimitarBlast2 = new EnergyScimitarBlast((sword.owner.position + this.owner.position) / 2f + new Vec2(0.0f, 16f), new Vec2(0.0f, 2000f));
                                        Level.Add(energyScimitarBlast2);
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast2.position, energyScimitarBlast2._target));
                                    }
                                }
                                else
                                {
                                    sword.owner.hSpeed += offDir * 3f;
                                    sword.owner.vSpeed -= 2f;
                                    this.duck.hSpeed += -this.offDir * 3f;
                                    this.duck.vSpeed -= 2f;
                                }
                                if (sword.duck != null && this.duck != null)
                                {
                                    sword.duck.crippleTimer = 1f;
                                    this.duck.crippleTimer = 1f;
                                    flag = true;
                                }
                            }
                        }
                    }
                    if (flag || !this.isServerForObject)
                        return;
                    foreach (IAmADuck amAduck in amAducks)
                    {
                        if (amAduck != this.duck && amAduck != this.prevOwner && amAduck is MaterialThing materialThing)
                        {
                            if (materialThing is Duck duck && !duck.destroyed && this.duck != null)
                            {
                                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                ++Global.data.swordKills.valueInt;
                            }
                            materialThing.Destroy(this.destroyType);
                        }
                    }
                }
                else
                {
                    if (!this._crouchStance || this.duck == null)
                        return;
                    foreach (IAmADuck amAduck in Level.CheckLineAll<IAmADuck>(this.barrelStartPos, this.barrelPosition))
                    {
                        if (amAduck != this.duck && amAduck is MaterialThing t)
                        {
                            if ((double)t.vSpeed > 0.5 && (double)t.bottom < position.y - 8.0 && (double)t.left < barrelPosition.x && (double)t.right > barrelPosition.x)
                            {
                                if (t is Duck duck && !duck.destroyed)
                                {
                                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                    ++Global.data.swordKills.valueInt;
                                }
                                t.Destroy(this.destroyType);
                            }
                            else if (!this._jabStance && !t.destroyed && (this.offDir > 0 && (double)t.x > (double)this.duck.x || this.offDir < 0 && (double)t.x < (double)this.duck.x))
                            {
                                if (t is Duck)
                                    (t as Duck).crippleTimer = 1f;
                                else if ((double)this.duck.x > (double)t.x && (double)t.hSpeed > 1.5 || (double)this.duck.x < (double)t.x && (double)t.hSpeed < -1.5)
                                {
                                    if (t is Duck duck && !duck.destroyed)
                                    {
                                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                                        ++Global.data.swordKills.valueInt;
                                    }
                                    t.Destroy(this.destroyType);
                                }
                                this.Fondle(t);
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
            this._lastAngles = new float[8];
            this._lastPositions = new Vec2[8];
            this._lastIndex = 0;
            this._lastSize = 0;
            this._lastHistoryPos = Vec2.Zero;
        }

        protected int historyIndex(int idx)
        {
            int num = this._lastIndex - idx - 1;
            if (num < 0)
                num += 8;
            return num;
        }

        protected void addHistory(float angle, Vec2 position)
        {
            if (this._lastHistoryPos != Vec2.Zero)
            {
                this._lastAngles[this._lastIndex] = (float)(((double)angle + _lastHistoryAngle) / 2.0);
                this._lastPositions[this._lastIndex] = (position + this._lastHistoryPos) / 2f;
                this._lastIndex = (this._lastIndex + 1) % 8;
                ++this._lastSize;
            }
            this._lastAngles[this._lastIndex] = angle;
            this._lastPositions[this._lastIndex] = position;
            this._lastIndex = (this._lastIndex + 1) % 8;
            ++this._lastSize;
            this._lastHistoryPos = position;
            this._lastHistoryAngle = angle;
        }

        public override void Draw()
        {
            Sword._playedShing = false;
            if (this.owner != this._prevHistoryOwner)
            {
                this._prevHistoryOwner = this.owner;
                this.ResetTrailHistory();
            }
            if (DevConsole.showCollision)
                Graphics.DrawLine(this.barrelStartPos, this.barrelPosition, Color.Red, depth: ((Depth)1f));
            if ((double)this._swordSwing.speed > 0.0)
            {
                if (this.duck != null)
                    this._swordSwing.flipH = this.duck.offDir <= 0;
                this._swordSwing.alpha = 0.4f;
                this._swordSwing.position = this.position;
                this._swordSwing.depth = this.depth + 1;
                this._swordSwing.Draw();
            }
            Vec2 position = this.position;
            Depth depth = this.depth;
            this.graphic.color = Color.White;
            if (this.owner == null && (double)this.velocity.length > 1.0 || _swing != 0.0 || this.tape != null && this.bayonetLethal)
            {
                float alpha = this.alpha;
                this.alpha = 1f;
                float angle1 = this.angle;
                this._drawing = true;
                float angle2 = this._angle;
                this.angle = angle1;
                for (int idx = 0; idx < 7; ++idx)
                {
                    base.Draw();
                    if (this._lastSize > idx)
                    {
                        int index = this.historyIndex(idx);
                        this._angle = this._lastAngles[index];
                        this.position = this._lastPositions[index];
                        this.depth -= 2;
                        this.alpha -= 0.15f;
                        this.graphic.color = Color.Red;
                    }
                    else
                        break;
                }
                this.position = position;
                this.depth = depth;
                this.alpha = alpha;
                this._angle = angle2;
                this.xscale = 1f;
                this._drawing = false;
            }
            else
                base.Draw();
            this.addHistory(this.angle, this.position);
        }

        protected virtual void OnSwing()
        {
        }

        public override void OnPressAction()
        {
            if (this._crouchStance && this._jabStance && !this._swinging || !this._crouchStance && !this._swinging && _swing < 0.100000001490116)
            {
                if (this._jabStance && !this._allowJabMotion)
                    return;
                this._afterSwingCounter = 0.0f;
                this._pullBack = true;
                this._swung = true;
                this._shing = false;
                this._timeSinceSwing = 0.0f;
                this.OnSwing();
                if (this._swingSound != null)
                    SFX.Play(this._swingSound, Rando.Float(0.8f, 1f), Rando.Float(-0.1f, 0.1f));
                if (this._jabStance)
                    return;
                this._swordSwing.speed = 1f;
                this._swordSwing.frame = 0;
            }
            else
            {
                if (!this._crouchStance || this._jabStance || this.duck == null || this.duck.grounded)
                    return;
                this._slamStance = true;
            }
        }

        public override void Fire()
        {
        }
    }
}
