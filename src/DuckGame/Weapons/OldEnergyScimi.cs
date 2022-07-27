// Decompiled with JetBrains decompiler
// Type: DuckGame.OldEnergyScimi
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class OldEnergyScimi : Sword
    {
        public StateBinding _glowBinding = new StateBinding(nameof(_glow));
        private MaterialEnergyBlade _bladeMaterial;
        private Sprite _blade;
        private Sprite _bladeTrail;
        private List<EnergyBlocker> _walls = new List<EnergyBlocker>();
        private Platform _platform;
        private Sprite _whiteGlow;
        private Sprite _warpLine;
        public Color properColor = new Color(178, 220, 239);
        public Color swordColor;
        private ConstantSound _hum;
        private float _timeTillPulse;
        private List<OldEnergyScimi.RagdollDrag> _drag = new List<OldEnergyScimi.RagdollDrag>();
        private bool _airFly;
        private float _airFlyDir;
        private bool _canAirFly = true;
        private bool _airFlyVertical;
        private float _upFlyTime;
        private bool _stuck;
        private float _glow;
        private bool _longCharge;
        private float _angleWhoom;
        private bool _thrownDown;
        private bool _thrownUp;
        private float slowWait;
        private bool _slowV;
        private Duck _revertVMaxDuck;
        private float _vmaxReversion = 1f;
        private bool _playedChargeUp;
        private float _unchargeWait;
        private float _lastAngleHum;
        private float _timeSincePickedUp = 10f;
        private bool _didOwnerSwitchLogic;
        public List<WarpLine> warpLines = new List<WarpLine>();

        public OldEnergyScimi(float pX, float pY)
          : base(pX, pY)
        {
            this.graphic = new Sprite("energyScimiHilt");
            this.center = new Vec2(6f, 26f);
            this.collisionOffset = new Vec2(-2f, -24f);
            this.collisionSize = new Vec2(4f, 28f);
            this._blade = new Sprite("energyScimiBlade");
            this._bladeTrail = new Sprite("energyScimiBladeTrail");
            this._whiteGlow = new Sprite("whiteGlow")
            {
                center = new Vec2(16f, 28f),
                xscale = 0.8f,
                yscale = 1.4f
            };
            this.thickness = 0.01f;
            this._impactThreshold = 0.5f;
            this.centerHeld = new Vec2(6f, 29f);
            this.centerUnheld = new Vec2(6f, 16f);
            this._bladeMaterial = new MaterialEnergyBlade(this);
            this.additionalHoldOffset = new Vec2(0.0f, -3f);
            this._swingSound = null;
            this._enforceJabSwing = false;
            this._allowJabMotion = false;
            this._clashWithWalls = false;
            this.swordColor = this.properColor;
            this._warpLine = new Sprite("warpLine2");
        }

        public override Vec2 barrelStartPos
        {
            get
            {
                if (this._stuck)
                    return this.position - (this.Offset(this.barrelOffset) - this.position).normalized * -5f;
                if (this.owner == null)
                    return this.position - (this.Offset(this.barrelOffset) - this.position).normalized * 6f;
                return this._slamStance ? this.position + (this.Offset(this.barrelOffset) - this.position).normalized * 12f : this.position + (this.Offset(this.barrelOffset) - this.position).normalized * 2f;
            }
        }

        public override void Initialize()
        {
            for (int index = 0; index < 6; ++index)
            {
                EnergyBlocker energyBlocker = new EnergyBlocker(this)
                {
                    collisionSize = new Vec2(6f, 6f),
                    center = new Vec2(3f, 3f),
                    collisionOffset = new Vec2(-3f, -3f)
                };
                this._walls.Add(energyBlocker);
                Level.Add(energyBlocker);
            }
            this._platform = new Platform(0.0f, 0.0f, 20f, 8f)
            {
                solid = false,
                enablePhysics = false,
                center = new Vec2(10f, 4f),
                collisionOffset = new Vec2(-10f, -2f),
                thickness = 0.01f
            };
            Level.Add(_platform);
            this._hum = new ConstantSound("scimiHum")
            {
                volume = 0.0f,
                lerpSpeed = 1f
            };
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => false;

        public override void Terminate()
        {
            foreach (Thing wall in this._walls)
                Level.Remove(wall);
            this._walls.Clear();
            if (this._platform != null)
                Level.Remove(_platform);
            base.Terminate();
        }

        public void Pulse()
        {
            if (_timeTillPulse >= 0.0)
                return;
            this._timeTillPulse = 0.2f;
            SFX.Play("scimiSurge", 0.8f, Rando.Float(-0.2f, 0.2f));
            this._glow = 12f;
        }

        public override void RestoreCollisionSize(bool pHeld = false)
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
                this.collisionOffset = new Vec2(-2f, -24f);
                this.collisionSize = new Vec2(4f, 28f);
                if (!this._wasLifted)
                    return;
                this.collisionOffset = new Vec2(-4f, -2f);
                this.collisionSize = new Vec2(8f, 4f);
            }
        }

        public override DestroyType destroyType => this._airFly ? new DTImpale(this) : new DTIncinerate(this);

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this._wasLifted || this.owner != null)
                return;
            if (with is Block || with is IPlatform && from == ImpactedFrom.Bottom && (double)this.vSpeed > 0.0)
            {
                this.Shing();
                if (this._framesSinceThrown <= 5)
                    return;
                this._framesSinceThrown = 25;
            }
            else
            {
                if (!this._airFly || !(with is RagdollPart) || this._drag.FirstOrDefault<OldEnergyScimi.RagdollDrag>(x => x.part == with) != null)
                    return;
                RagdollPart ragdollPart = with as RagdollPart;
                if (ragdollPart.doll == null)
                    return;
                if (ragdollPart.doll.part1 != null)
                    this._drag.Add(new OldEnergyScimi.RagdollDrag()
                    {
                        part = ragdollPart.doll.part1,
                        offset = this.position - ragdollPart.doll.part1.position
                    });
                if (ragdollPart.doll.part2 != null)
                    this._drag.Add(new OldEnergyScimi.RagdollDrag()
                    {
                        part = ragdollPart.doll.part2,
                        offset = this.position - ragdollPart.doll.part2.position
                    });
                if (ragdollPart.doll.part3 == null)
                    return;
                this._drag.Add(new OldEnergyScimi.RagdollDrag()
                {
                    part = ragdollPart.doll.part3,
                    offset = this.position - ragdollPart.doll.part3.position
                });
            }
        }

        protected override void PerformAirSpin()
        {
            if (!this.enablePhysics)
                return;
            if (this._canAirFly && !this._airFly && this._framesSinceThrown < 15)
            {
                this._upFlyTime = 0.0f;
                if ((double)Math.Abs(this.hSpeed) > 2.0)
                {
                    if (Level.CheckLine<Block>(this.position + new Vec2(-16f, 0.0f), this.position + new Vec2(16f, 0.0f)) == null)
                    {
                        this._airFly = true;
                        this._airFlyDir = Math.Sign(this.hSpeed);
                    }
                    else
                        this._canAirFly = false;
                    this._airFlyVertical = false;
                }
                else if ((double)Math.Abs(this.vSpeed) > 2.0)
                {
                    if (Level.CheckLine<Block>(this.position + new Vec2(0.0f, -16f), this.position + new Vec2(0.0f, 16f)) == null)
                    {
                        this._airFly = true;
                        this._airFlyDir = Math.Sign(this.vSpeed);
                    }
                    else
                        this._canAirFly = false;
                    this._airFlyVertical = true;
                }
            }
            if (this._airFly)
            {
                this.hMax = 18f;
                if (this._airFlyVertical)
                {
                    this._upFlyTime += Maths.IncFrameTimer();
                    if (_upFlyTime > 2.0 && _airFlyDir < 0.0)
                        this._airFlyDir = 1f;
                    if (_airFlyDir > 0.0)
                        this._throwSpin = 90f;
                    else if (_airFlyDir < 0.0)
                        this._throwSpin = 270f;
                    this.vSpeed = this._airFlyDir * 18f;
                    this.hSpeed = 0.0f;
                }
                else
                {
                    if (_airFlyDir > 0.0)
                        this._throwSpin = 0.0f;
                    else if (_airFlyDir < 0.0)
                        this._throwSpin = 180f;
                    this.vSpeed = 0.0f;
                    this.hSpeed = this._airFlyDir * 18f;
                }
                this.angleDegrees = 90f + this._throwSpin;
                this.gravMultiplier = 0.0f;
            }
            else
            {
                this.hMax = 12f;
                this.vMax = 8f;
                base.PerformAirSpin();
            }
        }

        public override void Shing()
        {
            this.gravMultiplier = 1f;
            this.ClearDrag();
            this.Pulse();
            if (this._airFly)
            {
                bool flag = false;
                if (this._airFlyVertical)
                {
                    Block block = Level.CheckLine<Block>(this.position, this.position + new Vec2(0.0f, this.vSpeed));
                    if (block != null)
                    {
                        flag = true;
                        this.clip.Add(block);
                        if ((double)this.vSpeed > 0.0)
                        {
                            this.y = block.top - 18f;
                            this._throwSpin = 90f;
                        }
                        else
                        {
                            this.y = block.bottom + 18f;
                            this._throwSpin = 270f;
                        }
                    }
                }
                else
                {
                    Block block = Level.CheckLine<Block>(this.position, this.position + new Vec2(this.hSpeed, 0.0f));
                    if (block != null)
                    {
                        flag = true;
                        this.clip.Add(block);
                        if ((double)this.hSpeed > 0.0)
                        {
                            this.x = block.left - 18f;
                            this._throwSpin = 0.0f;
                        }
                        else
                        {
                            this.x = block.right + 18f;
                            this._throwSpin = 180f;
                        }
                    }
                }
                if (flag)
                {
                    this._longCharge = true;
                    this._stuck = true;
                    this.enablePhysics = false;
                    this.hSpeed = 0.0f;
                    this.vSpeed = 0.0f;
                }
            }
            this._airFly = false;
            base.Shing();
        }

        protected override void QuadLaserHit(QuadLaserBullet pBullet)
        {
            if (!this.isServerForObject)
                return;
            this.Fondle(pBullet);
            EnergyScimitarBlast energyScimitarBlast = new EnergyScimitarBlast(pBullet.position, new Vec2(offDir * 2000, 0.0f));
            Level.Add(energyScimitarBlast);
            Level.Remove(pBullet);
            if (!Network.isActive)
                return;
            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast.position, energyScimitarBlast._target));
        }

        protected override void UpdateCrouchStance()
        {
            if (!this._crouchStance)
            {
                this._hold = -0.3f;
                this.handOffset = new Vec2(this._addOffsetX + 4f, this._addOffsetY);
                this._holdOffset = new Vec2(2f + this._addOffsetX, 6f + this._addOffsetY) + this.additionalHoldOffset;
            }
            else if (this.duck != null && this.duck.sliding)
            {
                this._hold = 2.24159f;
                if (this.handFlip)
                    this._holdOffset = new Vec2(this._addOffsetX - 4f, this._addOffsetY - 3f) + this.additionalHoldOffset;
                else
                    this._holdOffset = new Vec2(2f + this._addOffsetX, this._addOffsetY - 3f) + this.additionalHoldOffset;
                this.handOffset = new Vec2(3f + this._addOffsetX, this._addOffsetY - 6f);
            }
            else
            {
                this._hold = 2.54159f;
                if (this.handFlip)
                    this._holdOffset = new Vec2(this._addOffsetX - 4f, this._addOffsetY - 7f) + this.additionalHoldOffset;
                else
                    this._holdOffset = new Vec2(2f + this._addOffsetX, this._addOffsetY - 7f) + this.additionalHoldOffset;
                this.handOffset = new Vec2(3f + this._addOffsetX, this._addOffsetY - 10f);
            }
        }

        protected override void UpdateJabPullback()
        {
            this.handFlip = true;
            if (this.duck != null && this.duck.sliding)
                this._swing = MathHelper.Lerp(this._swing, -4.2f, 0.36f);
            else
                this._swing = MathHelper.Lerp(this._swing, -4.8f, 0.36f);
            this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -2f, 0.45f);
            if (_addOffsetX >= -12.0)
                return;
            this._addOffsetX = -12f;
        }

        protected override void UpdateSlamPullback()
        {
            this._swing = MathHelper.Lerp(this._swing, 0.8f, 0.8f);
            this._addOffsetX = MathHelper.Lerp(this._addOffsetX, -5f, 0.45f);
            if (_addOffsetX < -4.59999990463257)
                this._addOffsetX = -5f;
            this._addOffsetY = MathHelper.Lerp(this._addOffsetY, 6f, 0.35f);
            if (_addOffsetX >= -5.5)
                return;
            this._addOffsetY = -6f;
        }

        public float angleWhoom => this._angleWhoom;

        public void ClearDrag()
        {
            int num = 1;
            foreach (OldEnergyScimi.RagdollDrag ragdollDrag in this._drag)
            {
                if (ragdollDrag.part.doll != null && ragdollDrag.part.doll.captureDuck != null && ragdollDrag.part.doll.captureDuck._cooked == null)
                {
                    if (!this._airFlyVertical)
                        ragdollDrag.part.position = this.Offset(new Vec2(-10f, 10f));
                    else if (_airFlyDir < 0.0)
                        ragdollDrag.part.position = this.Offset(new Vec2(0.0f, 0.0f));
                    else
                        ragdollDrag.part.position = this.Offset(new Vec2(0.0f, 20f));
                    ragdollDrag.part.doll.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.OnKill(new DTIncinerate(ragdollDrag.part.doll.captureDuck));
                    if (ragdollDrag.part.doll.captureDuck._cooked != null)
                        ragdollDrag.part.doll.captureDuck._cooked.vSpeed = -(2 + num);
                    ++num;
                }
            }
            this._drag.Clear();
        }

        public override void Thrown()
        {
            if (this.isServerForObject && this.duck != null)
            {
                if (this.duck.inputProfile.Down("DOWN"))
                {
                    this.x = this.duck.x;
                    this._thrownDown = true;
                    if (!this.duck.grounded)
                        this.duck.vSpeed -= 8f;
                }
                else if (this.duck.inputProfile.Down("UP"))
                {
                    this.x = this.duck.x;
                    this._thrownUp = true;
                }
            }
            base.Thrown();
        }

        protected override void OnSwing()
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
                this.duck.hSpeed = offDir * 11.25f;
                this.duck.vSpeed = -1f;
                this._slowV = false;
                this.warpLines.Add(new WarpLine()
                {
                    start = this.duck.position + new Vec2(-this.offDir * 16, 4f),
                    end = this.duck.position + new Vec2(offDir * 62, 4f),
                    lerp = 0.0f,
                    wide = 20f
                });
            }
            this.slowWait = 0.085f;
        }

        protected override void ResetTrailHistory()
        {
            this._lastAngleHum = this.angle;
            base.ResetTrailHistory();
        }

        public override void Update()
        {
            float num1 = Math.Min(this._angleWhoom, 0.5f) * 38f;
            if (this.isServerForObject)
            {
                this._stickWait -= Maths.IncFrameTimer();
                if (this._thrownDown)
                {
                    this.vSpeed = 4f;
                    this.y += 10f;
                    this._thrownDown = false;
                }
                if (this._thrownUp)
                {
                    this.vSpeed = -4f;
                    this.y -= 6f;
                    this._thrownUp = false;
                }
                if (this.duck != null && slowWait > 0.0)
                {
                    this.slowWait -= Maths.IncFrameTimer();
                    if (slowWait <= 0.0)
                    {
                        if (this._revertVMaxDuck != null)
                        {
                            this._revertVMaxDuck.vMax = this._vmaxReversion;
                            this._revertVMaxDuck = null;
                        }
                        if (this._slowV)
                            this.duck.vSpeed *= 0.25f;
                        else
                            this.duck.hSpeed *= 0.25f;
                    }
                }
                this.handFlip = false;
                foreach (OldEnergyScimi.RagdollDrag ragdollDrag in this._drag)
                {
                    ragdollDrag.part.position = this.position - ragdollDrag.offset;
                    ragdollDrag.part.hSpeed = 0.0f;
                    ragdollDrag.part.vSpeed = 0.0f;
                }
                this._timeSincePickedUp += Maths.IncFrameTimer();
                if (this.grounded)
                    this._canAirFly = true;
                this._timeTillPulse -= Maths.IncFrameTimer();
                if (this.owner != null)
                {
                    this._canAirFly = true;
                    this.ClearDrag();
                    if (this.prevOwner == null && !this._didOwnerSwitchLogic)
                    {
                        this._didOwnerSwitchLogic = true;
                        this._timeSincePickedUp = 0.0f;
                        foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(this.position, 16f))
                            physicsObject.sleeping = false;
                    }
                    float num2 = 20f + num1;
                    Vec2 vec2 = this.position + this.OffsetLocal(new Vec2(0.0f, -2f));
                    foreach (EnergyBlocker wall in this._walls)
                    {
                        vec2 += this.OffsetLocal(new Vec2(0.0f, -num2 / _walls.Count));
                        wall.position = vec2;
                        wall.solid = _glow > 0.5;
                    }
                }
                else
                {
                    this._didOwnerSwitchLogic = false;
                    Vec2 vec2 = this.position + this.OffsetLocal(new Vec2(0.0f, this._stuck ? -25f : -14f));
                    foreach (EnergyBlocker wall in this._walls)
                    {
                        vec2 += this.OffsetLocal(new Vec2(0.0f, 18f / _walls.Count));
                        wall.position = vec2;
                        wall.solid = _glow > 0.5;
                    }
                }
                if (this.duck != null && _timeSincePickedUp > 0.400000005960464 && this.held && this._swinging && Level.CheckLine<Block>(this.position, this.position + new Vec2(offDir * 16, 0.0f)) != null)
                {
                    this.duck.Swear();
                    double angle = (double)this.angle;
                    this.duck.ThrowItem();
                    this._airFly = true;
                    this._airFlyDir = offDir;
                    this.hSpeed = offDir * 16;
                    this.Shing();
                    this.angleDegrees = this._throwSpin + 90f;
                    this.ResetTrailHistory();
                }
            }
            float num3 = Math.Min(this._glow, 1f);
            float to1 = Math.Min(Math.Abs(this._lastAngleHum - this.angle), 1f);
            this._angleWhoom = Lerp.FloatSmooth(this._angleWhoom, to1, 0.2f);
            this._hum.volume = Lerp.FloatSmooth(this._hum.volume, Math.Min((float)((double)Math.Min(Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed), 5f) / 10.0 + (double)to1 * 2.0 + 0.150000005960464 + (double)num3 * 0.100000001490116) * this._glow, 0.75f), 0.2f);
            if (this.level != null)
            {
                float val2_1 = 800f;
                float val2_2 = 400f;
                this._hum.volume *= (float)(1.0 - (double)Math.Min(Math.Max((this.level.camera.position - this.position).length, val2_2) - val2_2, val2_1) / (double)val2_1);
                if (this.isServerForObject && ((double)this.x < level.topLeft.x - 1000.0 || (double)this.x > level.bottomRight.x + 1000.0))
                    Level.Remove(this);
            }
            this._extraOffset = new Vec2(0.0f, -num1);
            this._barrelOffsetTL = new Vec2(4f, 3f - num1);
            this._lastAngleHum = this.angle;
            if (_glow > 1.0)
                this._glow *= 0.85f;
            if (this.held || this._airFly)
            {
                this._stuck = false;
                this._unchargeWait = !this._longCharge ? 0.1f : 0.5f;
                this._longCharge = false;
                if (!this._playedChargeUp && this.owner != null)
                {
                    this._playedChargeUp = true;
                    SFX.Play("laserChargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                float to2 = 1f;
                if (this.duck != null && !this._swinging && !this._crouchStance)
                    to2 = 0.0f;
                this._glow = Lerp.Float(this._glow, to2, 0.1f);
            }
            else
            {
                this._unchargeWait -= Maths.IncFrameTimer();
                if (_unchargeWait < 0.0)
                {
                    if (this._playedChargeUp && this.owner == null)
                    {
                        this._playedChargeUp = false;
                        SFX.Play("laserUnchargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                    this._glow = Lerp.Float(this._glow, 0.0f, 0.2f);
                }
            }
            if (_glow > 0.100000001490116)
            {
                this._stayVolatile = true;
                this._volatile = true;
                this.heat += 3f / 500f;
            }
            else
            {
                this._stayVolatile = false;
                this._volatile = false;
                if (heat > 0.0)
                    this.heat -= 0.01f;
            }
            base.Update();
            this._platform.solid = false;
            this._platform.enablePhysics = false;
            this._platform.position = new Vec2(-99999f, -99999f);
            if (!this._stuck)
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
                float num2 = Math.Max((float)((warpLine.lerp - 0.5) * 2.0), 0.0f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start, swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(this._warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start - vec2_1 * (num1 * 1f), swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.13f;
            }
            this.warpLines.RemoveAll(v => v.lerp >= 1.0);
            base.DrawGlow();
        }

        public override void Draw()
        {
            base.Draw();
            Sword._playedShing = true;
            int num1 = DevConsole.showCollision ? 1 : 0;
            float num2 = Math.Min(this._angleWhoom, 0.5f) * 1.5f;
            Graphics.material = _bladeMaterial;
            this._bladeMaterial.glow = (float)(0.25 + _glow * 0.75);
            this._blade.center = this.center;
            this._bladeTrail.center = this.center;
            this._blade.angle = this.graphic.angle;
            this._blade.flipH = this.graphic.flipH;
            this._bladeTrail.flipH = this._blade.flipH;
            this._blade.color = Color.Lerp(Color.White, Color.Red, this.heat);
            this.swordColor = Color.Lerp(this.properColor, Color.Red, this.heat);
            if (_glow > 1.0)
                this._blade.scale = new Vec2((float)(1.0 + (_glow - 1.0) * 0.0299999993294477), 1f);
            else
                this._blade.scale = new Vec2(1f);
            this._bladeTrail.yscale = this._blade.yscale + num2;
            Graphics.Draw(this._blade, this.x, this.y, this.depth - 1);
            Graphics.material = null;
            this.alpha = 1f;
            Depth depth = this.depth;
            this._bladeTrail.color = this.swordColor;
            this.graphic.color = Color.White;
            if (_glow <= 0.5)
                return;
            float num3 = this.angle;
            float num4 = 1f;
            Vec2 vec2 = this.position;
            for (int idx = 0; idx < 8; ++idx)
            {
                Vec2 current1 = Vec2.Zero;
                float current2 = 0.0f;
                for (int index1 = 0; index1 < 4 && this._lastSize > idx; ++index1)
                {
                    int index2 = this.historyIndex(idx);
                    if (index1 == 0)
                    {
                        current1 = vec2;
                        current2 = num3;
                    }
                    num3 = Lerp.FloatSmooth(current2, this._lastAngles[index2], 0.25f * index1);
                    vec2 = Lerp.Vec2Smooth(current1, this._lastPositions[index2], 0.25f * index1);
                    if (this.owner != null)
                        vec2 += this.owner.velocity * 0.5f;
                    this._bladeTrail.angle = num3;
                    this._bladeTrail.alpha = Math.Min(Math.Max((float)(((double)this._hum.volume - 0.100000001490116) * 4.0), 0.0f), 1f) * 0.7f;
                    Graphics.Draw(this._bladeTrail, vec2.x, vec2.y, this.depth - 2);
                }
                num4 -= 0.15f;
            }
        }

        private class RagdollDrag
        {
            public RagdollPart part;
            public Vec2 offset;
        }
    }
}
