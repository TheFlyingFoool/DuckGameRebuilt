// Decompiled with JetBrains decompiler
// Type: DuckGame.EnergyScimitar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Guns|Melee")]
    public class EnergyScimitar : Gun
    {
        public StateBinding _throwSpinBinding = new StateBinding(true, nameof(_throwSpin));
        public StateBinding _stanceBinding = new StateBinding(true, nameof(stanceInt));
        public StateBinding _lerpedAngleBinding = new StateBinding(true, nameof(_lerpedAngle));
        public StateBinding _swordAngleBinding = new StateBinding(true, nameof(_swordAngle));
        public StateBinding _stuckBinding = new StateBinding(nameof(stuck));
        public StateBinding _airFlyBinding = new StateBinding(true, nameof(_airFly));
        public StateBinding _airFlyDirBinding = new StateBinding(true, nameof(_airFlyAngle));
        public StateBinding _wasLiftedBinding = new StateBinding(nameof(_wasLifted));
        private bool _stuck;
        public float _swordAngle;
        public float _lerpedAngle;
        public bool dragSpeedBonus;
        protected Vec2 centerHeld = new Vec2(6f, 26f);
        protected Vec2 centerUnheld = new Vec2(4f, 22f);
        public EnergyScimitar.Stance _stance = EnergyScimitar.Stance.SwingUp;
        private bool _stanceReady;
        private float _dragAngle = -110f;
        private float _dragAngleDangle = -145f;
        private bool _swordFlip;
        //private bool _stanceHeld;
        private float _stanceCounter;
        private float _swingDif;
        private int _timeSinceDragJump = 11111;
        private float _lerpBoost;
        private bool _goIntermediate;
        private bool _blocking;
        public bool _spikeDrag;
        public float _dragRand;
        private int _timeSincePress;
        public StateBinding _glowBinding = new StateBinding(nameof(_glow));
        private MaterialEnergyBlade _bladeMaterial;
        private Sprite _blade;
        private Sprite _bladeTrail;
        private List<EnergyScimitar.Blocker> _walls = new List<EnergyScimitar.Blocker>();
        private Platform _platform;
        private Sprite _whiteGlow;
        private Sprite _warpLine;
        public Color properColor = new Color(178, 220, 239);
        public Color swordColor;
        private LoopingSound _hum;
        private float _timeTillPulse;
        private List<EnergyScimitar.RagdollDrag> _drag = new List<EnergyScimitar.RagdollDrag>();
        public float _throwSpin;
        public bool _airFly;
        public float _airFlyAngle;
        public bool _canAirFly = true;
        private float _upFlyTime;
        public float _airFlySpeed = 14f;
        private int timeSinceReversal;
        private bool _wasLifted;
        private bool skipThrowMove;
        public MaterialThing _stuckInto;
        private float _glow;
        private bool _longCharge;
        private float _angleWhoom;
        private float slowWait;
        private bool _slowV;
        private Duck _revertVMaxDuck;
        private float _vmaxReversion = 1f;
        protected float[] _lastAngles = new float[8];
        protected Vec2[] _lastPositions = new Vec2[8];
        protected int _lastIndex;
        protected int _lastSize;
        private bool _playedChargeUp;
        private float _unchargeWait;
        private float _lastAngleHum;
        private float _timeSincePickedUp = 10f;
        private bool _didOwnerSwitchLogic = true;
        private float _stickWait;
        private float _timeSinceBlast = 99f;
        private int _glowTime;
        private Vec2 _lastBarrelStartPos;
        private Vec2 _lastBarrelPos;
        private float _humAmount;
        public List<WarpLine> warpLines = new List<WarpLine>();

        public bool stuck
        {
            get => this.isServerForObject ? this._stuckInto != null : this._stuck;
            set => this._stuck = value;
        }

        public int stanceInt
        {
            get => (int)this._stance;
            set => this._stance = (EnergyScimitar.Stance)value;
        }

        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case TapedSword _:
                case Sword _:
                    return false;
                default:
                    return true;
            }
        }

        public override float angle
        {
            get
            {
                if (this.owner is WireMount)
                    return this._angle;
                return !this.held && this.owner != null ? this._angle + 1.570796f * (float)this.offDir : Maths.DegToRad(this._lerpedAngle) * (float)this.offDir + Maths.DegToRad(this._throwSpin);
            }
            set => this._angle = value;
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
        }

        public override void PressAction()
        {
            if (this.owner is WireMount)
            {
                WireMount owner = this.owner as WireMount;
                foreach (MaterialThing materialThing in Level.CheckRectAll<Block>(this.position + new Vec2(-8f, -8f), this.position + new Vec2(8f, 8f)))
                    this.clip.Add(materialThing);
                float pAngleDegrees = (float)(-(double)this.angleDegrees - 90.0 - 180.0);
                this.owner = (Thing)null;
                owner._containedThing = (Thing)null;
                this.StartFlying(pAngleDegrees);
            }
            base.PressAction();
        }

        private void UpdateStance()
        {
            ++this._timeSincePress;
            ++this._timeSinceDragJump;
            if (this.duck == null || !this.held)
            {
                this._stance = EnergyScimitar.Stance.None;
                this._swordAngle = 0.0f;
                this._lerpedAngle = this.owner != null ? 0.0f : (this._wasLifted ? 90f : 0.0f);
                this._swordFlip = this.offDir < (sbyte)0;
                ++this._framesSinceThrown;
                this.center = this.centerUnheld;
                this.collisionOffset = new Vec2(-2f, -16f);
                this.collisionSize = new Vec2(4f, 26f);
                if (this._wasLifted)
                {
                    if (this._airFly)
                    {
                        if ((double)this.vSpeed < -4.0)
                        {
                            this.collisionOffset = new Vec2(0.0f, -4f);
                            this.collisionSize = new Vec2(6f, 8f);
                        }
                        else if ((double)this.vSpeed > 4.0)
                        {
                            this.collisionOffset = new Vec2(-5f, -4f);
                            this.collisionSize = new Vec2(6f, 8f);
                        }
                        else
                        {
                            this.collisionOffset = new Vec2(-4f, 0.0f);
                            this.collisionSize = new Vec2(8f, 6f);
                        }
                    }
                    else
                    {
                        this.center = new Vec2(6f, 22f);
                        this.collisionOffset = new Vec2(-4f, -3f);
                        this.collisionSize = new Vec2(8f, 6f);
                    }
                }
                if (this.owner != null || this.stuck || !this._wasLifted)
                    return;
                bool flag1 = false;
                bool flag2 = false;
                if (this._airFly)
                {
                    this.PerformAirSpin();
                    flag1 = true;
                }
                else if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 2.0 || !this.grounded)
                {
                    if (!this.grounded && Level.CheckRect<Block>(this.position + new Vec2(-6f, -6f), this.position + new Vec2(6f, -2f)) != null)
                        flag2 = true;
                    if (!flag2 && !this._grounded && (Level.CheckPoint<IPlatform>(this.position + new Vec2(0.0f, 8f)) == null || (double)this.vSpeed < 0.0 || this._airFly))
                    {
                        this.PerformAirSpin();
                        flag1 = true;
                    }
                }
                if (!(!flag1 | flag2) || this._airFly)
                    return;
                this._throwSpin %= 360f;
                if (flag2)
                {
                    if ((double)Math.Abs(this._throwSpin - 90f) < (double)Math.Abs(this._throwSpin + 90f))
                        this._throwSpin = Lerp.Float(this._throwSpin, 90f, 16f);
                    else
                        this._throwSpin = Lerp.Float(-90f, 0.0f, 16f);
                }
                else if ((double)this._throwSpin > 90.0 && (double)this._throwSpin < 270.0)
                {
                    this._throwSpin = Lerp.Float(this._throwSpin, 180f, 14f);
                }
                else
                {
                    if ((double)this._throwSpin > 180.0)
                        this._throwSpin -= 360f;
                    else if ((double)this._throwSpin < -180.0)
                        this._throwSpin += 360f;
                    this._throwSpin = Lerp.Float(this._throwSpin, 0.0f, 14f);
                }
            }
            else
            {
                if (this._stance == EnergyScimitar.Stance.None)
                    this._stance = EnergyScimitar.Stance.SwingUp;
                this._framesSinceThrown = (byte)0;
                this.center = this.centerHeld;
                this.collisionOffset = new Vec2(-4f, 0.0f);
                this.collisionSize = new Vec2(4f, 4f);
                this._throwSpin = 0.0f;
                this._wasLifted = true;
                this._blocking = this.duck.crouch && (double)Math.Abs(this.duck.hSpeed) < 2.0;
                if (this.duck.inputProfile.Pressed("UP") && !this.duck.inputProfile.Pressed("JUMP") && (this._stance == EnergyScimitar.Stance.Drag || this._stance == EnergyScimitar.Stance.Intermediate) && !this.duck.sliding)
                    this._stance = EnergyScimitar.Stance.SwingUp;
                if (this.duck.crouch && !this.duck.sliding && this.duck.inputProfile.Pressed("LEFT"))
                    this.duck.offDir = (sbyte)-1;
                else if (this.duck.crouch && !this.duck.sliding && this.duck.inputProfile.Pressed("RIGHT"))
                    this.duck.offDir = (sbyte)1;
                bool flag = Level.CheckLine<IPlatform>(new Vec2(this.owner.position.x, this.owner.bottom) + new Vec2((float)((int)-this.offDir * 16), -10f), new Vec2(this.owner.position.x, this.owner.bottom) + new Vec2((float)((int)-this.offDir * 16), 2f)) == null;
                this._spikeDrag = this.duck.grounded && !flag && Level.CheckLine<Spikes>(new Vec2(this.owner.position.x, this.owner.bottom) + new Vec2((float)((int)-this.offDir * 16), -10f), new Vec2(this.owner.position.x, this.owner.bottom) + new Vec2((float)((int)-this.offDir * 16), 2f)) != null;
                this._dragRand = Lerp.FloatSmooth(this._dragRand, 0.0f, 0.1f);
                if ((double)this._dragRand > 1.0)
                    this._dragRand = 1f;
                this.dragSpeedBonus = this._stance == EnergyScimitar.Stance.Drag && !flag && this._stanceReady;
                if (this._spikeDrag && this.dragSpeedBonus)
                {
                    this._dragRand += Rando.Float(Math.Abs(this.duck.hSpeed)) * 0.1f;
                    if (Rando.Int(30) == 0 && (double)this._dragRand > 0.100000001490116)
                        this.duck.Swear();
                }
                if (!this.duck.grounded && this.duck.inputProfile.Pressed("DOWN") && this._stance != EnergyScimitar.Stance.Drag)
                    this._goIntermediate = true;
                if (this._stance == EnergyScimitar.Stance.Drag && this.duck._hovering)
                {
                    this._stance = EnergyScimitar.Stance.SwingDown;
                    this.duck.vSpeed = -6f;
                    this.duck._hovering = false;
                    this._timeSinceDragJump = 100;
                }
                if (this.duck.inputProfile.Pressed("DOWN") && this.duck.grounded)
                    this._stance = EnergyScimitar.Stance.Drag;
                if ((double)Math.Abs(this.duck.hSpeed) > 1.0)
                {
                    if (this.dragSpeedBonus)
                    {
                        Spark spark = Spark.New(this.barrelPosition.x, this.barrelPosition.y - 6f, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                        spark._color = this.swordColor;
                        spark._width = 1f;
                        this._glow = 0.3f;
                        Level.Add((Thing)spark);
                    }
                    if (this._stance == EnergyScimitar.Stance.Drag && this.duck.grounded)
                    {
                        if (this.duck.sliding || this.duck.crouch)
                        {
                            this.duck.tilt = 0.0f;
                            this.duck.verticalOffset = 0.0f;
                        }
                        else
                        {
                            float num = this.duck.hSpeed - (float)Math.Sign(this.duck.hSpeed);
                            if (Math.Sign(num) != Math.Sign(this.duck.hSpeed))
                                num = 0.0f;
                            this.duck.tilt = num;
                            this.duck.verticalOffset = Math.Abs(num);
                        }
                    }
                }
                if (this._stance == EnergyScimitar.Stance.SwingDown && this.duck.inputProfile.Pressed("JUMP"))
                    this._stance = EnergyScimitar.Stance.SwingUp;
                if (this._goIntermediate && this._stanceReady)
                {
                    this._stance = EnergyScimitar.Stance.Intermediate;
                    this._goIntermediate = false;
                }
                this.handAngle = 0.0f;
                this._holdOffset = new Vec2(0.0f, 0.0f);
                this.handOffset = new Vec2(0.0f, 0.0f);
                this.handFlip = false;
                if (this._stance == EnergyScimitar.Stance.Intermediate)
                {
                    this._swordAngle = -60f;
                    this.handAngle = Maths.DegToRad(this._swordAngle - 90f) * (float)this.offDir;
                    this._holdOffset = new Vec2(0.0f, 2f);
                    this._swordFlip = this.offDir > (sbyte)0;
                    if (this.duck.grounded)
                        this._stance = EnergyScimitar.Stance.Drag;
                }
                else if (this._stance == EnergyScimitar.Stance.Drag)
                {
                    if (this.duck._hovering)
                    {
                        this._swordAngle = -190f;
                        this.handAngle = Maths.DegToRad(this._swordAngle - 90f) * (float)this.offDir;
                        this._holdOffset = new Vec2(0.0f, 2f);
                        this._swordFlip = this.offDir > (sbyte)0;
                    }
                    else
                    {
                        this._swordAngle = !flag ? this._dragAngle : this._dragAngleDangle;
                        if (this.duck.crouch)
                            this._swordAngle += 10f;
                        if (this.duck.sliding)
                            this._swordAngle += 10f;
                        this.handAngle = Maths.DegToRad(this._swordAngle - 90f) * (float)this.offDir;
                        this._holdOffset = new Vec2(0.0f, 2f);
                        this._swordFlip = this.offDir > (sbyte)0;
                    }
                }
                else if (this._stance == EnergyScimitar.Stance.SwingUp)
                {
                    if (this.duck._hovering)
                    {
                        this._swordAngle = -25f;
                        this._swordFlip = this.offDir < (sbyte)0;
                    }
                    else if (this._blocking)
                    {
                        this._swordAngle = 130f;
                        this.handAngle = Maths.DegToRad(this._swordAngle) * (float)this.offDir;
                        this._holdOffset = new Vec2(-22f, 0.0f);
                        this.handOffset = new Vec2(7f, -7f);
                        this._swordFlip = this.offDir >= (sbyte)0;
                        this.handFlip = true;
                    }
                    else
                    {
                        this._swordAngle = 25f;
                        this.handAngle = Maths.DegToRad(this._swordAngle) * (float)this.offDir;
                        this._holdOffset = new Vec2((float)((double)this._swingDif * 0.550000011920929 - 2.0), -4f);
                        this.handOffset = new Vec2((float)(2.0 + (double)this._swingDif * 0.349999994039536), -3f);
                        this._swordFlip = this.offDir < (sbyte)0;
                    }
                }
                else if (this._stance == EnergyScimitar.Stance.SwingDown)
                {
                    if (this.duck._hovering)
                    {
                        this._swordAngle = 40f;
                        this._holdOffset = new Vec2(0.0f, 8f);
                        this.handOffset = new Vec2(2f, 0.0f);
                        this._swordFlip = this.offDir < (sbyte)0;
                    }
                    else if (this._blocking)
                    {
                        this._swordAngle = 45f;
                        this.handAngle = Maths.DegToRad(this._swordAngle) * (float)this.offDir;
                        this._holdOffset = new Vec2(3f, -3f);
                        this.handOffset = new Vec2(7f, 0.0f);
                        this._swordFlip = this.offDir < (sbyte)0;
                    }
                    else
                    {
                        this._swordAngle = 80f;
                        this.handAngle = Maths.DegToRad(this._swordAngle) * (float)this.offDir;
                        this._holdOffset = new Vec2(0.0f, (float)(-2.0 - (double)this._swingDif * 0.550000011920929));
                        this.handOffset = new Vec2((float)(0.0 + (double)this._swingDif * 0.349999994039536), 3f);
                        this._swordFlip = this.offDir < (sbyte)0;
                    }
                }
                this._lerpedAngle = Lerp.FloatSmooth(this._lerpedAngle, this._swordAngle, 0.25f + this._lerpBoost);
                this._stanceReady = (double)Math.Abs(this._lerpedAngle - this._swordAngle) < 25.0;
                this._stanceCounter += Maths.IncFrameTimer();
                this._swingDif = Math.Min(Math.Abs(this._lerpedAngle - this._swordAngle), 35f);
                if (this._timeSincePress > 25)
                    this._swingDif *= 0.25f;
                this._lerpBoost = Lerp.FloatSmooth(this._lerpBoost, 0.0f, 0.1f);
            }
        }

        public override void Ejected(Thing pFrom)
        {
            this.ResetTrailHistory();
            if (pFrom is SpawnCannon)
            {
                this.StartFlying((pFrom as SpawnCannon).direction);
            }
            else
            {
                if ((double)this.vSpeed < -0.100000001490116)
                    this.StartFlying(TileConnection.Up);
                if ((double)this.vSpeed > 0.100000001490116)
                    this.StartFlying(TileConnection.Down);
                if ((double)this.hSpeed < -0.100000001490116)
                    this.StartFlying(TileConnection.Left);
                if ((double)this.hSpeed <= 0.100000001490116)
                    return;
                this.StartFlying(TileConnection.Right);
            }
        }

        public override void OnPressAction()
        {
            if (!this.isServerForObject || this.duck == null || this.receivingPress)
                return;
            this._goIntermediate = false;
            if (this._timeSincePress > 3)
                this._timeSincePress = 0;
            if (this._stance == EnergyScimitar.Stance.Intermediate)
                this._stance = EnergyScimitar.Stance.SwingDown;
            if (!this._stanceReady)
                return;
            if (this._stance == EnergyScimitar.Stance.Drag || this._stance == EnergyScimitar.Stance.SwingUp)
            {
                if (this._stance == EnergyScimitar.Stance.Drag && this.duck != null && (!this.duck.grounded || !this.duck.crouch && !this.duck.sliding))
                {
                    this.duck.hSpeed = (float)((int)this.duck.offDir * 9);
                    this.duck.vSpeed = -2f;
                    this.duck._disarmDisable = 5;
                }
                else if (this._stance == EnergyScimitar.Stance.Drag && (this.duck.crouch || this.duck.sliding))
                    this._lerpBoost = 0.4f;
                this._stance = EnergyScimitar.Stance.SwingDown;
            }
            else
            {
                if (this._stance != EnergyScimitar.Stance.SwingDown)
                    return;
                this._stance = EnergyScimitar.Stance.SwingUp;
            }
        }

        public override void Fire()
        {
            //this._stanceHeld = true;
            this._stanceCounter = 0.0f;
        }

        public EnergyScimitar(float pX, float pY)
          : base(pX, pY)
        {
            this.graphic = new Sprite("energyScimiHilt");
            this.center = new Vec2(6f, 26f);
            this.collisionOffset = new Vec2(-2f, -24f);
            this.collisionSize = new Vec2(4f, 28f);
            this._blade = new Sprite("energyScimiBlade");
            this._bladeTrail = new Sprite("energyScimiBladeTrail");
            this._whiteGlow = new Sprite("whiteGlow");
            this._whiteGlow.center = new Vec2(16f, 28f);
            this._whiteGlow.xscale = 0.8f;
            this._whiteGlow.yscale = 1.4f;
            this._fullAuto = true;
            this._bouncy = 0.5f;
            this._impactThreshold = 0.3f;
            this.ammo = 99999;
            this._ammoType = (AmmoType)new ATLaser();
            this.thickness = 0.01f;
            this._impactThreshold = 0.5f;
            this._bladeMaterial = new MaterialEnergyBlade(this);
            this.swordColor = this.properColor;
            this._warpLine = new Sprite("warpLine2");
            this.editorTooltip = "How do you invent a sword? It uses modern technology.";
        }

        public Vec2 barrelStartPos => this.position + (this.Offset(this.barrelOffset) - this.position).normalized * 2f;

        public override void Initialize()
        {
            this._platform = (Platform)new EnergyScimitar.ScimiPlatform(0.0f, 0.0f, 20f, 8f, this);
            this._platform.solid = false;
            this._platform.enablePhysics = false;
            this._platform.center = new Vec2(10f, 4f);
            this._platform.collisionOffset = new Vec2(-10f, -2f);
            this._platform.thickness = 0.01f;
            Level.Add((Thing)this._platform);
            this._hum = new LoopingSound("scimiHum");
            this._hum.volume = 0.0f;
            this._humAmount = 0.0f;
            this._hum.lerpSpeed = 1f;
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => false;

        public override void Terminate()
        {
            if (this._hum != null)
                this._hum.Kill();
            foreach (Thing wall in this._walls)
                Level.Remove(wall);
            this._walls.Clear();
            if (this._platform != null)
                Level.Remove((Thing)this._platform);
            base.Terminate();
        }

        public void Pulse()
        {
            if ((double)this._timeTillPulse >= 0.0)
                return;
            this._timeTillPulse = 0.2f;
            SFX.Play("scimiSurge", 0.8f, Rando.Float(-0.2f, 0.2f));
            this._glow = 12f;
            Vec2 normalized = (this.position - this.barrelPosition).normalized;
            Vec2 barrelPosition = this.barrelPosition;
            for (int index = 0; index < 6; ++index)
            {
                Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                spark._color = this.swordColor;
                spark._width = 1f;
                Level.Add((Thing)spark);
                barrelPosition += normalized * 4f;
            }
        }

        public override bool Destroy(DestroyType type = null) => base.Destroy(type);

        public override void DoTerminate() => base.DoTerminate();

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.owner != null || !this.isServerForObject)
                return;
            if (this._airFly && with is EnergyScimitar && (with as EnergyScimitar)._airFly)
            {
                this.Fondle((Thing)with);
                Vec2 vec1 = Maths.AngleToVec(Maths.DegToRad(this._airFlyAngle));
                this.hSpeed = this.lastHSpeed = (float)(-(double)vec1.x * 3.0);
                this.vSpeed = this.lastVSpeed = (float)(-(double)vec1.y * 3.0);
                Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad((with as EnergyScimitar)._airFlyAngle));
                with.hSpeed = with.lastHSpeed = (float)(-(double)vec2.x * 3.0);
                with.vSpeed = with.lastVSpeed = (float)(-(double)vec2.y * 3.0);
                this.Shing();
                (with as EnergyScimitar).Shing();
            }
            else if (this._airFly && with is PhysicsObject && !(with is Gun) && !(with is Equipment) && !(with is Duck) && !(with is RagdollPart))
                with.Destroy((DestroyType)new DTIncinerate((Thing)this));
            else if (this._airFly && with is Duck && (with != this._prevOwner || this._framesSinceThrown > (byte)15))
            {
                with.Destroy((DestroyType)new DTImpale((Thing)this));
                Duck duck = with as Duck;
                if (duck.ragdoll == null || duck.ragdoll.part2 == null || this._drag.FirstOrDefault<EnergyScimitar.RagdollDrag>((Func<EnergyScimitar.RagdollDrag, bool>)(x => x.part == with)) != null)
                    return;
                this._drag.Add(new EnergyScimitar.RagdollDrag()
                {
                    part = duck.ragdoll.part2,
                    offset = this.position - duck.ragdoll.part2.position
                });
            }
            else if (with is Block || with is IPlatform && from == ImpactedFrom.Bottom && (double)this.vSpeed > 0.0)
            {
                if (with is Nubber)
                    return;
                this.Shing();
                if (this._framesSinceThrown <= (byte)5)
                    return;
                this._framesSinceThrown = (byte)25;
            }
            else
            {
                if (!this._airFly || !(with is RagdollPart))
                    return;
                RagdollPart ragdollPart = with as RagdollPart;
                if (ragdollPart.doll != null && ragdollPart.doll.captureDuck != null && ragdollPart.doll.captureDuck == this._prevOwner && this._framesSinceThrown <= (byte)15 || this._drag.FirstOrDefault<EnergyScimitar.RagdollDrag>((Func<EnergyScimitar.RagdollDrag, bool>)(x => x.part == with)) != null || ragdollPart.doll == null)
                    return;
                if (ragdollPart.doll.part1 != null)
                {
                    this._drag.Add(new EnergyScimitar.RagdollDrag()
                    {
                        part = ragdollPart.doll.part1,
                        offset = this.position - ragdollPart.doll.part1.position
                    });
                    ragdollPart.doll.part1.owner = (Thing)this;
                }
                if (ragdollPart.doll.part2 != null)
                {
                    this._drag.Add(new EnergyScimitar.RagdollDrag()
                    {
                        part = ragdollPart.doll.part2,
                        offset = this.position - ragdollPart.doll.part2.position
                    });
                    ragdollPart.doll.part2.owner = (Thing)this;
                }
                if (ragdollPart.doll.part3 == null)
                    return;
                this._drag.Add(new EnergyScimitar.RagdollDrag()
                {
                    part = ragdollPart.doll.part3,
                    offset = this.position - ragdollPart.doll.part3.position
                });
                ragdollPart.doll.part3.owner = (Thing)this;
            }
        }

        public Vec2 TravelThroughAir(float pMult = 1f)
        {
            Vec2 vec = Maths.AngleToVec(Maths.DegToRad(this._airFlyAngle));
            this.position = this.position + vec * this._airFlySpeed * pMult;
            return vec;
        }

        public void ReverseFlyDirection()
        {
            if (this.timeSinceReversal <= 10)
                return;
            this.timeSinceReversal = 0;
            this._airFlyAngle += 180f;
            this.offDir *= (sbyte)-1;
            this.TravelThroughAir();
            this.PerformAirSpin();
        }

        private void UpdateAirDirection()
        {
            if (this.offDir < (sbyte)0)
                this._throwSpin = (float)(-(double)this._airFlyAngle + 180.0);
            else
                this._throwSpin = -this._airFlyAngle;
        }

        protected void PerformAirSpin()
        {
            ++this.timeSinceReversal;
            if (!this.enablePhysics)
                return;
            if (this._airFly)
            {
                Vec2 vec = Maths.AngleToVec(Maths.DegToRad(this._airFlyAngle));
                this.offDir = (double)vec.x < -0.100000001490116 ? (sbyte)-1 : (sbyte)1;
                if ((double)Math.Abs(vec.x) < 0.200000002980232 && (double)vec.y < 0.0)
                {
                    this._upFlyTime += Maths.IncFrameTimer();
                    if ((double)this._upFlyTime > 2.0)
                        this.ReverseFlyDirection();
                }
                this.UpdateAirDirection();
                if (!this.skipThrowMove)
                {
                    this.hSpeed = vec.x * this._airFlySpeed;
                    this.vSpeed = vec.y * this._airFlySpeed;
                }
                this._impactThreshold = 0.01f;
                this._bouncy = 0.0f;
                this.hMax = this._airFlySpeed;
                this.vMax = this._airFlySpeed;
                this._lerpedAngle = 90f;
                this.gravMultiplier = 0.0f;
            }
            else
            {
                this._impactThreshold = 0.3f;
                this._bouncy = 0.5f;
                this.hMax = 12f;
                if ((double)this.hSpeed > 0.0)
                    this._throwSpin += (float)(((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed)) * 2.0 + 4.0);
                else
                    this._throwSpin -= (float)(((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed)) * 2.0 + 4.0);
            }
        }

        public override void ReturnToWorld()
        {
            if (this._airFly)
                return;
            this._throwSpin = 90f;
        }

        public void StartFlying(TileConnection pDirection, bool pThrown = false)
        {
            switch (pDirection)
            {
                case TileConnection.Left:
                    this.StartFlying(180f, pThrown);
                    break;
                case TileConnection.Right:
                    this.StartFlying(0.0f, pThrown);
                    break;
                case TileConnection.Up:
                    this.StartFlying(90f, pThrown);
                    break;
                case TileConnection.Down:
                    this.StartFlying(270f, pThrown);
                    break;
            }
        }

        public void StartFlying(float pAngleDegrees, bool pThrown = false)
        {
            if (this.owner != null)
                return;
            this._wasLifted = true;
            this._airFly = true;
            this._upFlyTime = 0.0f;
            this._airFlyAngle = pAngleDegrees;
            if (pThrown)
                this.TravelThroughAir(-0.5f);
            this.skipThrowMove = true;
            this.PerformAirSpin();
            this.skipThrowMove = false;
            this.UpdateStance();
            this.ResetTrailHistory();
        }

        public override void Thrown()
        {
            if (this.duck == null)
                return;
            this.x = this.duck.x;
            this._oldDepth = this.depth = - 0.1f;
            if (!this.isServerForObject || this.duck == null || this.duck.destroyed || !this._canAirFly || this._airFly)
                return;
            this._upFlyTime = 0.0f;
            if (!this.duck.inputProfile.Down("GRAB"))
                return;
            if (this.duck.inputProfile.Down("LEFT") && this.duck.offDir < (sbyte)0 || this.duck.inputProfile.Down("RIGHT") && this.duck.offDir > (sbyte)0)
            {
                this.y = this.duck.y;
                if (this._stance == EnergyScimitar.Stance.Drag)
                    this.y += 6f;
                this.skipThrowMove = true;
                TileConnection pDirection = this.duck.inputProfile.Down("LEFT") ? TileConnection.Left : TileConnection.Right;
                this.owner = (Thing)null;
                this.StartFlying(pDirection, true);
                this.skipThrowMove = false;
            }
            else
            {
                if (!this.duck.inputProfile.Down("UP") && !this.duck.inputProfile.Down("DOWN") || !this.duck.inputProfile.Down("UP") && this.duck.grounded)
                    return;
                int num = 1;
                if (this.duck.inputProfile.Down("UP"))
                    num = -1;
                this.x = this.duck.x + (float)this.duck.offDir * -2f;
                if (num == 1 && !this.duck.grounded)
                    this.duck.vSpeed -= 8f;
                this.skipThrowMove = true;
                TileConnection pDirection = TileConnection.Down;
                if (num < 0)
                    pDirection = TileConnection.Up;
                this.owner = (Thing)null;
                this.StartFlying(pDirection, true);
                this.skipThrowMove = false;
            }
        }

        private void UpdateStuck()
        {
            if (!this.stuck)
                return;
            this.UpdateAirDirection();
        }

        public void Shing()
        {
            if (this.stuck)
                return;
            this.gravMultiplier = 1f;
            this.ClearDrag();
            this.Pulse();
            this._timeSinceBlast = 0.0f;
            if (this._airFly && this.isServerForObject)
            {
                Vec2 vec2 = this.TravelThroughAir(-0.5f);
                Vec2 hitPos = Vec2.Zero;
                MaterialThing materialThing = (MaterialThing)Level.CheckRay<Block>(this.position, this.position + vec2 * this._airFlySpeed, out hitPos);
                if (materialThing != this._platform)
                {
                    if (materialThing is EnergyScimitar.ScimiPlatform)
                    {
                        this.hSpeed = this.lastHSpeed = (float)(-(double)vec2.x * 3.0);
                        this.vSpeed = this.lastHSpeed = (float)(-(double)vec2.y * 3.0);
                    }
                    else
                    {
                        if (materialThing != null)
                        {
                            this.clip.Add(materialThing);
                            this.position = hitPos - vec2 * 16f;
                            this.UpdateAirDirection();
                        }
                        if (materialThing != null)
                        {
                            this._stuckInto = materialThing;
                            this._longCharge = true;
                            this.enablePhysics = false;
                            this.hSpeed = 0.0f;
                            this.vSpeed = 0.0f;
                            this.lastHSpeed = this._hSpeed;
                            this.lastVSpeed = this._vSpeed;
                            this.depth = - 0.55f;
                        }
                        else
                        {
                            this.vSpeed = (float)(-(double)this.vSpeed * 0.25);
                            this.hSpeed = (float)(-(double)this.hSpeed * 0.25);
                        }
                    }
                }
            }
            this._airFly = false;
        }

        protected void QuadLaserHit(QuadLaserBullet pBullet)
        {
            if (!this.isServerForObject)
                return;
            this.Fondle((Thing)pBullet);
            EnergyScimitarBlast energyScimitarBlast = new EnergyScimitarBlast(pBullet.position, new Vec2((float)((int)this.offDir * 2000), 0.0f));
            Level.Add((Thing)energyScimitarBlast);
            Level.Remove((Thing)pBullet);
            if (!Network.isActive)
                return;
            Send.Message((NetMessage)new NMEnergyScimitarBlast(energyScimitarBlast.position, energyScimitarBlast._target));
        }

        public float angleWhoom => this._angleWhoom;

        public override void OnTeleport()
        {
            this.ResetTrailHistory();
            base.OnTeleport();
        }

        public void ClearDrag()
        {
            int num = 1;
            foreach (EnergyScimitar.RagdollDrag ragdollDrag in this._drag)
            {
                if (ragdollDrag.part.doll != null && ragdollDrag.part.doll.captureDuck != null && ragdollDrag.part.doll.captureDuck._cooked == null)
                {
                    ragdollDrag.part.position = this.Offset(Maths.AngleToVec(Maths.DegToRad(this._airFlyAngle)) * 8f);
                    ragdollDrag.part.doll.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.Cook();
                    ragdollDrag.part.doll.captureDuck.Kill((DestroyType)new DTIncinerate((Thing)this));
                    if (ragdollDrag.part.doll.captureDuck._cooked != null)
                        ragdollDrag.part.doll.captureDuck._cooked.vSpeed = (float)-(2 + num);
                    ++num;
                }
            }
            this._drag.Clear();
        }

        protected void OnSwing()
        {
            if (this.duck == null || !this.isServerForObject)
                return;
            if (this.duck._hovering)
            {
                this._revertVMaxDuck = this.duck;
                this._vmaxReversion = this.duck.vMax;
                this.duck.vMax = 13f;
                this.duck.vSpeed = -13f;
                this._slowV = true;
                this.warpLines.Add(new WarpLine()
                {
                    start = this.duck.position,
                    end = this.duck.position + new Vec2(0.0f, -80f),
                    lerp = 0.0f,
                    wide = 24f
                });
            }
            else
            {
                this.duck.hSpeed = (float)this.offDir * 11.25f;
                this.duck.vSpeed = -1f;
                this._slowV = false;
                this.warpLines.Add(new WarpLine()
                {
                    start = this.duck.position + new Vec2((float)((int)-this.offDir * 16), 4f),
                    end = this.duck.position + new Vec2((float)((int)this.offDir * 62), 4f),
                    lerp = 0.0f,
                    wide = 20f
                });
            }
            this.slowWait = 0.085f;
        }

        protected virtual void ResetTrailHistory()
        {
            this._lastAngles = new float[8];
            this._lastPositions = new Vec2[8];
            this._lastIndex = 0;
            this._lastSize = 0;
        }

        protected int historyIndex(int idx) => this._lastIndex + idx + 1 & 7;

        protected void addHistory(float angle, Vec2 position)
        {
            this._lastAngles[this._lastIndex] = angle;
            this._lastPositions[this._lastIndex] = position;
            this._lastIndex = this._lastIndex - 1 & 7;
            ++this._lastSize;
        }

        public override bool Sprung(Thing pSpringer)
        {
            this.StartFlying((float)(-(double)pSpringer.angleDegrees - 90.0 - 180.0));
            return false;
        }

        public override void Update()
        {
            if (this._hum != null)
                this._hum.Update();
            if (!this.isServerForObject)
                this.UpdateStuck();
            this._skipAutoPlatforms = this._airFly;
            this._skipPlatforms = this._airFly;
            if (this._airFly || this._stuckInto != null)
                this.depth = - 0.55f;
            else
                this.depth = - 0.1f;
            if (this._stuckInto != null && this._stuckInto is Door && (double)Math.Abs((this._stuckInto as Door)._open) > 0.5)
            {
                this._stuckInto.Fondle((Thing)this);
                this.enablePhysics = true;
                this._stuckInto = (MaterialThing)null;
            }
            this.ammo = 999;
            this.UpdateStance();
            if ((double)this._glow < 0.400000005960464)
                this._glowTime = 0;
            if (this.duck != null && ((double)this._glow > 0.400000005960464 || this._glowTime > 0 && this._glowTime < 4))
            {
                ++this._glowTime;
                foreach (Bullet bullet1 in Level.current.things[typeof(Bullet)])
                {
                    Vec2 vec2 = this.barrelStartPos + this.OffsetLocal(new Vec2(8f, 0.0f));
                    Vec2 barrelPosition = this.barrelPosition;
                    bool flag = Collision.LineIntersect(vec2 + this.velocity, barrelPosition + this.velocity, bullet1.start, bullet1.start + bullet1.travelDirNormalized * bullet1.bulletSpeed);
                    if (!flag)
                        flag = Collision.LineIntersect(vec2 + this.velocity * 0.5f, barrelPosition + this.velocity * 0.5f, bullet1.start, bullet1.start + bullet1.travelDirNormalized * bullet1.bulletSpeed);
                    if (flag && bullet1.lastReboundSource != this)
                    {
                        bullet1.lastReboundSource = (Thing)this;
                        Bullet bullet2 = bullet1.ReverseTravel();
                        if (bullet2 != null)
                            bullet2.owner = (Thing)this.duck;
                        this.Pulse();
                    }
                }
            }
            this._timeSinceBlast += Maths.IncFrameTimer();
            float num1 = Math.Min(this._angleWhoom, 0.5f) * 38f;
            if (this.isServerForObject)
            {
                this._stickWait -= Maths.IncFrameTimer();
                if (this.duck != null && (double)this.slowWait > 0.0)
                {
                    this.slowWait -= Maths.IncFrameTimer();
                    if ((double)this.slowWait <= 0.0)
                    {
                        if (this._revertVMaxDuck != null)
                        {
                            this._revertVMaxDuck.vMax = this._vmaxReversion;
                            this._revertVMaxDuck = (Duck)null;
                        }
                        if (this._slowV)
                            this.duck.vSpeed *= 0.25f;
                        else
                            this.duck.hSpeed *= 0.25f;
                    }
                }
                this.handFlip = false;
                foreach (EnergyScimitar.RagdollDrag ragdollDrag in this._drag)
                {
                    ragdollDrag.part.position = this.position - ragdollDrag.offset;
                    ragdollDrag.part.hSpeed = 0.0f;
                    ragdollDrag.part.vSpeed = 0.0f;
                }
                this._timeSincePickedUp += Maths.IncFrameTimer();
                if (this._stance == EnergyScimitar.Stance.Drag && this.duck != null)
                    this._glow = (double)Math.Abs(this.duck.hSpeed) > 1.0 ? 0.35f : 0.0f;
                if (this.grounded)
                    this._canAirFly = true;
                this._timeTillPulse -= Maths.IncFrameTimer();
                if (this.owner != null)
                {
                    this.gravMultiplier = 1f;
                    if ((double)this._glow > 0.400000005960464 && (double)this._timeSincePickedUp > 0.25 && this.duck != null)
                    {
                        Vec2 barrelStartPos = this.barrelStartPos;
                        Vec2 p2_1 = this.barrelStartPos + new Vec2(this.duck.hSpeed * 2f, this.duck.vSpeed);
                        Vec2 barrelPosition = this.barrelPosition;
                        Vec2 p2_2 = this.barrelPosition + new Vec2(this.duck.hSpeed * 2f, this.duck.vSpeed);
                        foreach (EnergyScimitar t in Level.current.things[typeof(EnergyScimitar)])
                        {
                            if (t != this && t.owner != this.duck)
                            {
                                if (t.owner == null && t._airFly && (int)t.offDir != (int)this.duck.offDir && ((double)Math.Abs(t.hSpeed) > 2.0 && Collision.Line(this.barrelStartPos, this.barrelPosition, new Rectangle(t.x + t.hSpeed, t.y - 8f, Math.Abs(t.hSpeed), 16f)) || (double)Math.Abs(t.vSpeed) > 2.0 && Collision.Line(this.barrelStartPos, this.barrelPosition, new Rectangle(t.x - 8f, t.y + t.vSpeed, 16f, Math.Abs(t.vSpeed)))))
                                {
                                    this.Fondle((Thing)t);
                                    t.ReverseFlyDirection();
                                    this.Shing();
                                }
                                if (t.owner is Duck && (double)t._glow > 0.400000005960464 && (Collision.LineIntersect(this.barrelStartPos, this.barrelPosition, t.barrelStartPos, t.barrelPosition) || Collision.LineIntersect(barrelStartPos, p2_1, t.barrelStartPos, t.barrelPosition) || Collision.LineIntersect(barrelPosition, p2_2, t.barrelStartPos, t.barrelPosition)) && (double)this._timeSinceBlast > 0.150000005960464)
                                {
                                    Duck owner = t.owner as Duck;
                                    this.duck.x -= this.duck.hSpeed;
                                    owner.x -= owner.hSpeed;
                                    this._timeSinceBlast = 0.0f;
                                    owner.hSpeed = (float)this.offDir * 5f;
                                    owner.vSpeed = -4f;
                                    this.duck.hSpeed = (float)-this.offDir * 5f;
                                    this.duck.vSpeed = -4f;
                                    this.duck.hSpeed *= 2f;
                                    this.duck.UpdatePhysics();
                                    this.duck.hSpeed /= 2f;
                                    owner.hSpeed *= 2f;
                                    owner.UpdatePhysics();
                                    owner.hSpeed /= 2f;
                                    this.duck.swordInvincibility = 10;
                                    owner.swordInvincibility = 10;
                                    this.Shing();
                                    t.Shing();
                                    if (this.isServerForObject && this.owner != null && owner != null)
                                    {
                                        EnergyScimitarBlast energyScimitarBlast1 = new EnergyScimitarBlast((owner.position + this.owner.position) / 2f + new Vec2(0.0f, -16f), new Vec2(0.0f, -2000f));
                                        if (Network.isActive)
                                            Send.Message((NetMessage)new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        EnergyScimitarBlast energyScimitarBlast2 = new EnergyScimitarBlast((owner.position + this.owner.position) / 2f + new Vec2(0.0f, 16f), new Vec2(0.0f, 2000f));
                                        if (Network.isActive)
                                            Send.Message((NetMessage)new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        Level.Add((Thing)energyScimitarBlast1);
                                        Level.Add((Thing)energyScimitarBlast2);
                                    }
                                }
                            }
                        }
                        for (int index = 0; index < 8; ++index)
                        {
                            Vec2 p1 = Lerp.Vec2Smooth(this.barrelStartPos, this._lastBarrelStartPos, (float)index / 7f);
                            Vec2 p2_3 = Lerp.Vec2Smooth(this.barrelPosition, this._lastBarrelPos, (float)index / 7f);
                            QuadLaserBullet pBullet = Level.CheckLine<QuadLaserBullet>(p1, p2_3);
                            if (pBullet != null)
                                this.QuadLaserHit(pBullet);
                            foreach (MaterialThing t in Level.CheckLineAll<MaterialThing>(p1, p2_3))
                            {
                                if (t != this.duck && t != this && t.owner != this.duck)
                                {
                                    switch (t)
                                    {
                                        case PhysicsObject _:
                                        case Icicles _:
                                            if (!(t is Duck) || (t as Duck).swordInvincibility <= 0)
                                            {
                                                if (!t.isServerForObject)
                                                    Thing.SuperFondle((Thing)t, DuckNetwork.localConnection);
                                                t.Destroy((DestroyType)new DTIncinerate((Thing)this));
                                                if (t is Duck && this.duck != null)
                                                {
                                                    this.duck._disarmDisable = 5;
                                                    continue;
                                                }
                                                continue;
                                            }
                                            continue;
                                        default:
                                            continue;
                                    }
                                }
                            }
                        }
                    }
                    this._canAirFly = true;
                    this.ClearDrag();
                    if (!this._didOwnerSwitchLogic)
                    {
                        this._didOwnerSwitchLogic = true;
                        this._timeSincePickedUp = 0.0f;
                        foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(this.position, 16f))
                            physicsObject.sleeping = false;
                    }
                    float num2 = 24f + num1;
                    Vec2 vec2 = this.position + this.OffsetLocal(new Vec2(0.0f, 4f));
                    foreach (EnergyScimitar.Blocker wall in this._walls)
                    {
                        vec2 += this.OffsetLocal(new Vec2(0.0f, -num2 / (float)this._walls.Count));
                        wall.position = vec2;
                        float num3 = 1f - Math.Min(this._stanceCounter / 0.25f, 1f);
                        wall.collisionSize = new Vec2((float)(6.0 + (double)num3 * 8.0), 6f);
                        wall.collisionOffset = new Vec2((float)(-3.0 - (double)num3 * 4.0), -3f);
                    }
                }
                else
                {
                    if (this.stuck)
                        this._didOwnerSwitchLogic = false;
                    Vec2 vec2 = this.position + this.OffsetLocal(new Vec2(0.0f, this._stuckInto != null ? -25f : -14f));
                    foreach (EnergyScimitar.Blocker wall in this._walls)
                    {
                        vec2 += this.OffsetLocal(new Vec2(0.0f, 18f / (float)this._walls.Count));
                        wall.position = vec2;
                        wall.solid = (double)this._stanceCounter < 0.150000005960464;
                    }
                }
                this._lastBarrelPos = this.barrelPosition;
                this._lastBarrelStartPos = this.barrelStartPos;
            }
            else
                this._didOwnerSwitchLogic = false;
            float num4 = Math.Min(this._glow, 1f);
            float to1 = Math.Min(Math.Abs(this._lastAngleHum - this.angle), 1f);
            this._angleWhoom = Lerp.FloatSmooth(this._angleWhoom, to1, 0.2f);
            this._humAmount = Lerp.FloatSmooth(this._humAmount, Math.Min((float)((double)Math.Min(Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed), 5f) / 10.0 + (double)to1 * 2.0 + 0.25 + (double)num4 * 0.300000011920929) * this._glow, 0.75f), 0.2f);
            this._humAmount = Math.Min(this._humAmount + this._dragRand * 0.2f, 1f);
            if (this._hum != null)
                this._hum.volume = this._humAmount;
            if (this.level != null)
            {
                float val2_1 = 800f;
                float val2_2 = 400f;
                float num5 = (float)(1.0 - (double)Math.Min(Math.Max((this.level.camera.position - this.position).length, val2_2) - val2_2, val2_1) / (double)val2_1);
                if (this._hum != null)
                    this._hum.volume *= num5;
                if (this.isServerForObject && this.visible && ((double)this.x < (double)this.level.topLeft.x - 1000.0 || (double)this.x > (double)this.level.bottomRight.x + 1000.0) && this.owner == null && !this.inPipe)
                    Level.Remove((Thing)this);
            }
            this._extraOffset = new Vec2(0.0f, -num1);
            this._barrelOffsetTL = new Vec2(4f, 3f - num1);
            this._lastAngleHum = this.angle;
            if ((double)this._glow > 1.0)
                this._glow *= 0.85f;
            if (this.owner != null)
                this._airFly = false;
            if (this.held || this._airFly)
            {
                this._stuckInto = (MaterialThing)null;
                this._unchargeWait = !this._longCharge ? 0.1f : 0.5f;
                this._longCharge = false;
                if (!this._playedChargeUp && this.owner != null)
                {
                    this._playedChargeUp = true;
                    SFX.Play("laserChargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                float to2;
                if (!this._stanceReady || this._airFly)
                {
                    to2 = 1f;
                    this._glow = 1.5f;
                }
                else
                    to2 = 0.0f;
                if (this._stance == EnergyScimitar.Stance.Drag && this.duck != null)
                    this._glow = (double)Math.Abs(this.duck.hSpeed) > 1.0 ? 0.35f : 0.0f;
                this._glow = Lerp.Float(this._glow, to2, 0.1f);
            }
            else
            {
                this._unchargeWait -= Maths.IncFrameTimer();
                if ((double)this._unchargeWait < 0.0)
                {
                    if (this._playedChargeUp && this.owner == null)
                    {
                        this._playedChargeUp = false;
                        SFX.Play("laserUnchargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                    this._glow = Lerp.Float(this._glow, 0.0f, 0.2f);
                }
            }
            double glow = (double)this._glow;
            base.Update();
            this._platform.solid = false;
            this._platform.enablePhysics = false;
            this._platform.position = new Vec2(-99999f, -99999f);
            if (!this.stuck)
                return;
            if ((double)Math.Abs(this.barrelStartPos.y - this.barrelPosition.y) < 6.0)
            {
                this._platform.solid = true;
                this._platform.enablePhysics = true;
                this._platform.position = this.Offset(new Vec2(0.0f, -10f));
            }
            this.center = new Vec2(6f, 29f);
        }

        public override void DrawGlow()
        {
            if (this.inPipe)
                return;
            this._whiteGlow.angle = this.angle;
            this._whiteGlow.color = this.swordColor;
            this._whiteGlow.alpha = this._glow * 0.5f;
            Graphics.Draw(this._whiteGlow, this.x, this.y, this.depth - 2);
            Color swordColor = this.swordColor;
            foreach (WarpLine warpLine in this.warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                float num1 = Math.Min(warpLine.lerp, 0.5f) / 0.5f;
                float num2 = Math.Max((float)(((double)warpLine.lerp - 0.5) * 2.0), 0.0f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start, swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start - vec2_1 * (num1 * 1f), swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.13f;
            }
            this.warpLines.RemoveAll((Predicate<WarpLine>)(v => (double)v.lerp >= 1.0));
            base.DrawGlow();
        }

        public override void EditorUpdate()
        {
            this._lerpedAngle = Maths.RadToDeg(this._angle);
            base.EditorUpdate();
        }

        public override void Draw()
        {
            if (this.inPipe)
                return;
            base.Draw();
            if (DevConsole.showCollision)
            {
                foreach (Thing wall in this._walls)
                    Graphics.DrawRect(wall.rectangle, Color.Red, this.depth + 10);
            }
            float num1 = Math.Min(this._angleWhoom, 0.5f) * 1.5f;
            Graphics.material = (Material)this._bladeMaterial;
            this._bladeMaterial.glow = (float)(0.25 + (double)this._glow * 0.75);
            this._blade.center = this.center;
            this._bladeTrail.center = this.center;
            this._blade.angle = this.graphic.angle;
            this._blade.flipH = this._swordFlip;
            this._bladeTrail.flipH = this._blade.flipH;
            this._blade.alpha = this.alpha;
            this._blade.color = Color.Lerp(Color.White, Color.Red, this.heat);
            this.swordColor = Color.Lerp(this.properColor, Color.Red, this.heat);
            if ((double)this._glow > 1.0)
                this._blade.scale = new Vec2((float)(1.0 + ((double)this._glow - 1.0) * 0.0299999993294477), 1f);
            else
                this._blade.scale = new Vec2(1f);
            this._bladeTrail.yscale = this._blade.yscale + num1;
            Graphics.Draw(this._blade, this.x, this.y, this.depth - 1);
            Graphics.material = (Material)null;
            Vec2 position1 = this.position;
            Depth depth = this.depth;
            this._bladeTrail.color = this.swordColor;
            this.graphic.color = Color.White;
            if ((double)this._glow > 0.5)
            {
                float num2 = this.angle;
                double angle = (double)this._angle;
                float num3 = 1f;
                Vec2 vec2 = this.position;
                Vec2 position2 = this.position;
                for (int idx = 0; idx < 7; ++idx)
                {
                    Vec2 current1 = Vec2.Zero;
                    float current2 = 0.0f;
                    for (int index1 = 0; index1 < 4 && this._lastSize > idx; ++index1)
                    {
                        int index2 = this.historyIndex(idx);
                        if (index1 == 0)
                        {
                            current1 = vec2;
                            current2 = num2;
                        }
                        num2 = Lerp.FloatSmooth(current2, this._lastAngles[index2], 0.25f * (float)index1);
                        vec2 = Lerp.Vec2Smooth(current1, this._lastPositions[index2], 0.25f * (float)index1);
                        if (this.owner != null)
                            vec2 += this.owner.velocity * 0.5f;
                        this._bladeTrail.angle = num2;
                        this._bladeTrail.alpha = Math.Min(Math.Max((float)(((double)this._humAmount - 0.100000001490116) * 4.0), 0.0f), 1f) * 0.7f;
                        Graphics.Draw(this._bladeTrail, vec2.x, vec2.y, this.depth - 2);
                    }
                    num3 -= 0.15f;
                }
            }
            this.addHistory(this.angle, this.position);
            if (this._lastSize > 2)
            {
                int index3 = this.historyIndex(0);
                int index4 = this.historyIndex(2);
                this.addHistory((float)(((double)this._lastAngles[index3] + (double)this._lastAngles[index4]) / 2.0), (this._lastPositions[index3] + this._lastPositions[index4]) / 2f);
            }
            if (this._lastSize <= 8)
                return;
            this._lastSize = 8;
        }

        public enum Stance
        {
            None,
            Drag,
            SwingUp,
            SwingDown,
            Intermediate,
        }

        public class Blocker : MaterialThing
        {
            private EnergyScimitar _parent;

            public Blocker(EnergyScimitar pParent)
              : base(0.0f, 0.0f)
            {
                this.thickness = 100f;
                this._editorCanModify = false;
                this.visible = false;
                this._parent = pParent;
                this.weight = 0.01f;
            }

            public override bool Hit(Bullet bullet, Vec2 hitPos)
            {
                if (!this._solid)
                    return false;
                if (this._parent != null)
                    this._parent.Shing();
                if (!(bullet.ammo is ATLaser) || !bullet.ammo.canBeReflected)
                    return base.Hit(bullet, hitPos);
                bullet.reboundOnce = true;
                return true;
            }
        }

        private class ScimiPlatform : Platform
        {
            public EnergyScimitar scimitar;

            public ScimiPlatform(float x, float y, float wid, float hi, EnergyScimitar pScimitar)
              : base(x, y, wid, hi)
            {
                this.scimitar = pScimitar;
            }
        }

        private class RagdollDrag
        {
            public RagdollPart part;
            public Vec2 offset;
        }
    }
}
