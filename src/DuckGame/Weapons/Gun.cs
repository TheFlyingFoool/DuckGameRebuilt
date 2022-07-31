// Decompiled with JetBrains decompiler
// Type: DuckGame.Gun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public abstract class Gun : Holdable
    {
        protected AmmoType _ammoType;
        public StateBinding _ammoBinding = new StateBinding(nameof(netAmmo));
        public StateBinding _waitBinding = new StateBinding(nameof(_wait));
        public StateBinding _loadedBinding = new StateBinding(nameof(loaded));
        public StateBinding _bulletFireIndexBinding = new StateBinding(nameof(bulletFireIndex));
        public StateBinding _infiniteAmmoValBinding = new StateBinding(nameof(infiniteAmmoVal));
        public byte bulletFireIndex;
        public bool wideBarrel;
        public Vec2 barrelInsertOffset;
        public float kick;
        protected float _kickForce = 3f;
        protected RumbleIntensity _fireRumble;
        public int ammo;
        public bool firing;
        public bool tamping;
        public float tampPos;
        public float loseAccuracy;
        public float _accuracyLost;
        public float maxAccuracyLost;
        protected Color _bulletColor = Color.White;
        protected bool _lowerOnFire = true;
        public bool receivingPress;
        public bool hasFireEvents;
        public bool onlyFireAction;
        protected float _fireSoundPitch;
        public EditorProperty<bool> infinite;
        public bool infiniteAmmoVal;
        protected string _bio = "No Info.";
        protected Vec2 _barrelOffsetTL;
        protected Vec2 _laserOffsetTL;
        protected float _barrelAngleOffset;
        public bool plugged;
        protected string _fireSound = "pistol";
        protected string _clickSound = "click";
        public float _wait;
        public float _fireWait = 1f;
        public bool loaded = true;
        //private Sprite _bayonetSprite;
        //private Sprite _tapeSprite;
        private bool _laserInit;
        protected bool _fullAuto;
        protected int _numBulletsPerFire = 1;
        protected bool _manualLoad;
        public bool laserSight;
        protected SpriteMap _flare;
        protected float _flareAlpha;
        private SpriteMap _barrelSmoke;
        public float _barrelHeat;
        protected float _smokeWait;
        protected float _smokeAngle;
        protected float _smokeFlatten;
        private SinWave _accuracyWave = (SinWave)0.3f;
        private SpriteMap _clickPuff;
        private Tex2D _laserTex;
        public bool isFatal = true;
        protected Vec2 _wallPoint;
        protected Sprite _sightHit;
        private bool _doPuff;
        public byte _framesSinceThrown;
        public bool explode;
        public List<Bullet> firedBullets = new List<Bullet>();
        //private Material _additiveMaterial;

        public bool CanSpawnInfinite()
        {
            return !(this is FlareGun) && !(this is QuadLaser) && !(this is RomanCandle) && !(this is Matchbox) && !(this is FireCrackers) && !(this is NetGun);
        }

        public AmmoType ammoType => this._ammoType;

        public sbyte netAmmo
        {
            get => (sbyte)this.ammo;
            set => this.ammo = value;
        }

        public bool lowerOnFire => this._lowerOnFire;

        public string bio => this._bio;

        public Vec2 barrelPosition => this.Offset(this.barrelOffset);

        public Vec2 barrelOffset => this._barrelOffsetTL - this.center + this._extraOffset;

        public Vec2 laserOffset => this._laserOffsetTL - this.center;

        public Vec2 barrelVector => this.Offset(this.barrelOffset) - this.Offset(this.barrelOffset + new Vec2(-1f, 0f));

        public override float angle
        {
            get => this._angle + (float)this._accuracyWave * (this._accuracyLost * 0.5f);
            set => this._angle = value;
        }

        public float barrelAngleOffset => this._barrelAngleOffset;

        public float barrelAngle => Maths.DegToRad(Maths.PointDirection(Vec2.Zero, this.barrelVector) + this._barrelAngleOffset * offDir);

        public bool CanSpin() => (double)this.weight <= 5.0;

        public override void EditorPropertyChanged(object property)
        {
            this.infiniteAmmoVal = this.infinite.value;
            this.UpdateMaterial();
        }

        public virtual void OnNetworkBulletsFired(Vec2 pos)
        {
        }

        public override void UpdateMaterial()
        {
            if (this.infinite.value)
                this.infiniteAmmoVal = true;
            if (this.infiniteAmmoVal)
            {
                if (this.material != null)
                    return;
                this.material = new MaterialGold(this);
            }
            else
                base.UpdateMaterial();
        }

        public bool fullAuto => this._fullAuto;

        public Gun(float xval, float yval)
          : base(xval, yval)
        {
            this._flare = new SpriteMap("smallFlare", 11, 10)
            {
                center = new Vec2(0f, 5f)
            };
            this._barrelSmoke = new SpriteMap("barrelSmoke", 8, 8)
            {
                center = new Vec2(1f, 8f)
            };
            this._barrelSmoke.ClearAnimations();
            this._barrelSmoke.AddAnimation("puff", 1f, false, 0, 1, 2);
            this._barrelSmoke.AddAnimation("loop", 1f, true, 3, 4, 5, 6, 7, 8);
            this._barrelSmoke.AddAnimation("finish", 1f, false, 9, 10, 11, 12);
            this._barrelSmoke.SetAnimation("puff");
            this._barrelSmoke.speed = 0f;
            this._translucent = true;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._dontCrush = true;
            this._clickPuff = new SpriteMap("clickPuff", 16, 16);
            this._clickPuff.AddAnimation("puff", 0.3f, false, 0, 1, 2, 3);
            this._clickPuff.center = new Vec2(0f, 12f);
            this._sightHit = new Sprite("laserSightHit");
            this._sightHit.CenterOrigin();
            this.depth = -0.1f;
            this.infinite = new EditorProperty<bool>(false, this)
            {
                _tooltip = "Makes gun have infinite ammo."
            };
            this.collideSounds.Add("smallMetalCollide");
            this.impactVolume = 0.3f;
            this.holsterAngle = 90f;
            this.coolingFactor = 1f / 500f;
        }

        public void DoAmmoClick()
        {
            this._doPuff = true;
            this._clickPuff.frame = 0;
            this._clickPuff.SetAnimation("puff");
            this._barrelHeat = 0f;
            this._barrelSmoke.SetAnimation("finish");
            SFX.Play(this._clickSound);
            for (int index = 0; index < 2; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.barrelPosition.x, this.barrelPosition.y);
                smallSmoke.scale = new Vec2(0.3f, 0.3f);
                smallSmoke.hSpeed = Rando.Float(-0.1f, 0.1f);
                smallSmoke.vSpeed = -Rando.Float(0.05f, 0.2f);
                smallSmoke.alpha = 0.6f;
                Level.Add(smallSmoke);
            }
        }

        public override void HeatUp(Vec2 location)
        {
            if (this._ammoType == null || !this._ammoType.combustable || this.ammo <= 0 || heat <= 1f || (double)Rando.Float(1f) <= 0.8f)
                return;
            int bulletFireIndex1 = bulletFireIndex;
            this.heat -= 0.05f;
            this.PressAction();
            int bulletFireIndex2 = bulletFireIndex;
            if (bulletFireIndex1 == bulletFireIndex2 || (double)Rando.Float(1f) <= 0.4f)
                return;
            SFX.Play("bulletPop", Rando.Float(0.5f, 1f), Rando.Float(-1f, 1f));
        }

        public override void DoUpdate()
        {
            if (this.laserSight && this._laserTex == null)
            {
                //this._additiveMaterial = new Material("Shaders/basicAdd");
                this._laserTex = Content.Load<Tex2D>("pointerLaser");
            }
            base.DoUpdate();
        }

        public override void Update()
        {
            if (this.infiniteAmmoVal)
                this.ammo = 99;
            if (TeamSelect2.Enabled("INFAMMO"))
            {
                this.infinite.value = true;
                this.infiniteAmmoVal = true;
            }
            base.Update();
            if (this._clickPuff.finished)
                this._doPuff = false;
            this._accuracyLost = Maths.CountDown(this._accuracyLost, 0.015f);
            if (_flareAlpha > 0f)
                this._flareAlpha -= 0.5f;
            else
                this._flareAlpha = 0f;
            if (_barrelHeat > 0f)
                this._barrelHeat -= 0.01f;
            else
                this._barrelHeat = 0f;
            if (_barrelHeat > 10f)
                this._barrelHeat = 10f;
            if (_smokeWait > 0f)
            {
                this._smokeWait -= 0.1f;
            }
            else
            {
                if (_barrelHeat > 0.1f && (double)this._barrelSmoke.speed == 0f)
                {
                    this._barrelSmoke.SetAnimation("puff");
                    this._barrelSmoke.speed = 0.1f;
                }
                if ((double)this._barrelSmoke.speed > 0f && this._barrelSmoke.currentAnimation == "puff" && this._barrelSmoke.finished)
                    this._barrelSmoke.SetAnimation("loop");
                if ((double)this._barrelSmoke.speed > 0f && this._barrelSmoke.currentAnimation == "loop" && this._barrelSmoke.frame == 5 && _barrelHeat < 0.1f)
                    this._barrelSmoke.SetAnimation("finish");
            }
            if (_smokeWait > 0.0 && (double)this._barrelSmoke.speed > 0.0)
                this._barrelSmoke.SetAnimation("finish");
            if (this._barrelSmoke.currentAnimation == "finish" && this._barrelSmoke.finished)
                this._barrelSmoke.speed = 0f;
            if (this.owner != null)
            {
                if (this.owner.hSpeed > 0.1f)
                    this._smokeAngle -= 0.1f;
                else if (this.owner.hSpeed < -0.1f)
                    this._smokeAngle += 0.1f;
                if (_smokeAngle > 0.4f)
                    this._smokeAngle = 0.4f;
                if (_smokeAngle < -0.4f)
                    this._smokeAngle = -0.4f;
                if (this.owner.vSpeed > 0.1f)
                    this._smokeFlatten -= 0.1f;
                else if (this.owner.vSpeed < -0.1f)
                    this._smokeFlatten += 0.1f;
                if (_smokeFlatten > 0.5f)
                    this._smokeFlatten = 0.5f;
                if (_smokeFlatten < -0.5f)
                    this._smokeFlatten = -0.5f;
                this._framesSinceThrown = 0;
            }
            else
            {
                ++this._framesSinceThrown;
                if (this._framesSinceThrown > 25)
                    this._framesSinceThrown = 25;
            }
            if (!(this is Sword) && this.owner == null && this.CanSpin() && Level.current.simulatePhysics)
            {
                bool flag1 = false;
                bool flag2 = false;
                if ((Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed) > 2f || !this.grounded) && gravMultiplier > 0f && !flag2 && !this._grounded)
                {
                    if (this.offDir > 0)
                        this.angleDegrees += (float)(((double)Math.Abs(this.hSpeed * 2f) + (double)Math.Abs(this.vSpeed)) * 1f + 5f);
                    else
                        this.angleDegrees -= (float)(((double)Math.Abs(this.hSpeed * 2f) + (double)Math.Abs(this.vSpeed)) * 1f + 5f);
                    flag1 = true;
                }
                if (!flag1 | flag2)
                {
                    this.angleDegrees %= 360f;
                    if ((double)this.angleDegrees < 0f)
                        this.angleDegrees += 360f;
                    if (flag2)
                    {
                        if ((double)Math.Abs(this.angleDegrees - 90f) < (double)Math.Abs(this.angleDegrees + 90f))
                            this.angleDegrees = Lerp.Float(this.angleDegrees, 90f, 16f);
                        else
                            this.angleDegrees = Lerp.Float(-90f, 0f, 16f);
                    }
                    else if ((double)this.angleDegrees > 90.0 && (double)this.angleDegrees < 270.0)
                    {
                        this.angleDegrees = Lerp.Float(this.angleDegrees, 180f, 14f);
                    }
                    else
                    {
                        if ((double)this.angleDegrees > 180.0)
                            this.angleDegrees -= 360f;
                        else if ((double)this.angleDegrees < -180.0)
                            this.angleDegrees += 360f;
                        this.angleDegrees = Lerp.Float(this.angleDegrees, 0f, 14f);
                    }
                }
            }
            float num = (float)(1f - (Math.Sin((double)Maths.DegToRad(this.angleDegrees + 90f)) + 1f) / 2f);
            if (this._owner == null)
                this._extraOffset.y = num * (this._collisionOffset.y + this._collisionSize.y + this._collisionOffset.y);
            else
                this._extraOffset.y = 0f;
            if (this.owner == null || (double)this.owner.hSpeed > -0.1f && (double)this.owner.hSpeed < 0.1f)
            {
                if (_smokeAngle >= 0.1f)
                    this._smokeAngle -= 0.1f;
                else if (_smokeAngle <= -0.1f)
                    this._smokeAngle += 0.1f;
                else
                    this._smokeAngle = 0f;
            }
            if (this.owner == null || this.owner.vSpeed > -0.1f && this.owner.vSpeed < 0.1f)
            {
                if (_smokeFlatten >= 0.1f)
                    this._smokeFlatten -= 0.1f;
                else if (_smokeFlatten <= -0.1f)
                    this._smokeFlatten += 0.1f;
                else
                    this._smokeFlatten = 0f;
            }
            if (kick > 0f)
                this.kick -= 0.2f;
            else
                this.kick = 0f;
            if (this.owner == null)
            {
                if (this.ammo <= 0 && (this.alpha < 0.99f || this.grounded && Math.Abs(this.hSpeed) + Math.Abs(this.vSpeed) < 0.3f))
                {
                    this.canPickUp = false;
                    this.alpha -= 10.2f;
                    this.weight = 0.01f;
                }
                if ((double)this.alpha < 0f)
                    Level.Remove(this);
            }
            if (this.owner != null && this.owner.graphic != null)
                this.graphic.flipH = this.owner.graphic.flipH;
            if (_wait > 0f)
                this._wait -= 0.15f;
            if (_wait >= 0f)
                return;
            this._wait = 0f;
        }

        public override void Terminate()
        {
            if (!(Level.current is Editor))
            {
                Level.Add(SmallSmoke.New(this.x, this.y));
                Level.Add(SmallSmoke.New(this.x + 4f, this.y));
                Level.Add(SmallSmoke.New(this.x - 4f, this.y));
                Level.Add(SmallSmoke.New(this.x, this.y + 4f));
                Level.Add(SmallSmoke.New(this.x, this.y - 4f));
            }
            base.Terminate();
        }

        public override void PressAction()
        {
            if (this.isServerForObject && (TeamSelect2.Enabled("GUNEXPL") && this.ammo <= 0 || this.explode))
            {
                if (this.duck == null)
                    return;
                if (this is Warpgun)
                    this.duck.Kill(new DTImpale(this));
                else
                    this.duck.ThrowItem();
                Level.Remove(this);
                for (int index = 0; index < 1; ++index)
                {
                    ExplosionPart explosionPart = new ExplosionPart(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f));
                    explosionPart.xscale *= 0.7f;
                    explosionPart.yscale *= 0.7f;
                    Level.Add(explosionPart);
                }
                SFX.Play("explode");
                List<Bullet> varBullets = new List<Bullet>();
                for (int index = 0; index < 12; ++index)
                {
                    float num = (index * 30f - 10f) + Rando.Float(20f);
                    ATShrapnel type = new ATShrapnel
                    {
                        range = 25f + Rando.Float(10f)
                    };
                    // Precision probably matters here + dnspy decompiles to the same thing so don't remove (double) and (float).
                    Bullet bullet = new Bullet(this.x + (float)(Math.Cos((double)Maths.DegToRad(num)) * 8f), this.y - (float)(Math.Sin((double)Maths.DegToRad(num)) * 8f), type, num)
                    {
                        firedFrom = this
                    };
                    varBullets.Add(bullet);
                    Level.Add(bullet);
                }
                if (!Network.isActive)
                    return;
                Send.Message(new NMExplodingProp(varBullets), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            else
                base.PressAction();
        }

        public override void OnPressAction()
        {
            if (this._fullAuto)
                return;
            if (this.isServerForObject)
                this._fireActivated = true;
            this.Fire();
        }

        public override void OnHoldAction()
        {
            if (!this._fullAuto)
                return;
            this.Fire();
        }

        public virtual void ApplyKick()
        {
            if (this.owner == null || !this.isServerForObject)
                return;
            if (_kickForce != 0.0)
            {
                Duck owner = this.owner as Duck;
                Thing thing = this.owner;
                if (owner != null && owner._trapped != null)
                    thing = owner._trapped;
                if (owner != null && owner.ragdoll != null && owner.ragdoll.part2 != null && owner.ragdoll.part1 != null && owner.ragdoll.part3 != null)
                {
                    Vec2 vec2 = -this.barrelVector * (this._kickForce / 2f);
                    owner.ragdoll.part1.hSpeed += vec2.x;
                    owner.ragdoll.part1.vSpeed += vec2.y;
                    owner.ragdoll.part2.hSpeed += vec2.x;
                    owner.ragdoll.part2.vSpeed += vec2.y;
                    owner.ragdoll.part3.hSpeed += vec2.x;
                    owner.ragdoll.part3.vSpeed += vec2.y;
                }
                else
                {
                    Vec2 vec2 = -this.barrelVector * this._kickForce;
                    if (Math.Sign(thing.hSpeed) != Math.Sign(vec2.x) || (double)Math.Abs(vec2.x) > (double)Math.Abs(thing.hSpeed))
                        thing.hSpeed = vec2.x;
                    if (owner != null)
                    {
                        if (owner.crouch)
                            owner.sliding = true;
                        thing.vSpeed += vec2.y - this._kickForce * 0.333f;
                    }
                    else
                        thing.vSpeed += vec2.y - this._kickForce * 0.333f;
                }
            }
            this.kick = 1f;
        }

        public virtual void Fire()
        {
            if (!this.loaded)
                return;
            if (this.ammo > 0 && _wait == 0.0)
            {
                this.firedBullets.Clear();
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                this.ApplyKick();
                for (int index = 0; index < this._numBulletsPerFire; ++index)
                {
                    float accuracy = this._ammoType.accuracy;
                    this._ammoType.accuracy *= 1f - this._accuracyLost;
                    this._ammoType.bulletColor = this._bulletColor;
                    float angleDegrees = this.angleDegrees;
                    float angle = this.offDir >= 0 ? angleDegrees + this._ammoType.barrelAngleDegrees : angleDegrees + 180f - this._ammoType.barrelAngleDegrees;
                    if (!this.receivingPress)
                    {
                        if (this._ammoType is ATDart)
                        {
                            if (this.isServerForObject)
                            {
                                Vec2 vec2 = this.Offset(this.barrelOffset);
                                Dart dart = new Dart(vec2.x, vec2.y, this.owner as Duck, -angle);
                                this.Fondle(dart);
                                if (this.onFire || _barrelHeat > 6.0)
                                {
                                    Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: dart, firedFrom: this));
                                    dart.burning = true;
                                    dart.onFire = true;
                                    this.Burn(this.position, this);
                                }
                                Vec2 vec = Maths.AngleToVec(Maths.DegToRad(-angle));
                                dart.hSpeed = vec.x * 10f;
                                dart.vSpeed = vec.y * 10f;
                                dart.vSpeed -= Rando.Float(2f);
                                Level.Add(dart);
                            }
                        }
                        else
                        {
                            Bullet bullet = this._ammoType.FireBullet(this.Offset(this.barrelOffset), this.owner, angle, this);
                            if (Network.isActive && this.isServerForObject)
                            {
                                this.firedBullets.Add(bullet);
                                if (this.duck != null && this.duck.profile.connection != null)
                                    bullet.connection = this.duck.profile.connection;
                            }
                            if (this.isServerForObject)
                            {
                                switch (this)
                                {
                                    case LaserRifle _:
                                    case PewPewLaser _:
                                    case Phaser _:
                                        ++Global.data.laserBulletsFired.valueInt;
                                        break;
                                }
                            }
                        }
                    }
                    ++this.bulletFireIndex;
                    this._ammoType.accuracy = accuracy;
                    this._barrelHeat += 0.3f;
                }
                this._smokeWait = 3f;
                this.loaded = false;
                this._flareAlpha = 1.5f;
                if (!this._manualLoad)
                    this.Reload();
                this.firing = true;
                this._wait = this._fireWait;
                this.PlayFireSound();
                if (this.owner == null)
                {
                    Vec2 vec2 = this.barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    this.hSpeed -= vec2.x;
                    this.vSpeed -= vec2.y;
                }
                this._accuracyLost += this.loseAccuracy;
                if (_accuracyLost <= (double)this.maxAccuracyLost)
                    return;
                this._accuracyLost = this.maxAccuracyLost;
            }
            else
            {
                if (this.ammo > 0 || _wait != 0.0)
                    return;
                this.firedBullets.Clear();
                this.DoAmmoClick();
                this._wait = this._fireWait;
            }
        }

        protected virtual void PlayFireSound() => SFX.Play(this._fireSound, pitch: (Rando.Float(0.2f) - 0.1f + this._fireSoundPitch));

        public void PopShell(bool isMessage = false)
        {
            if (!(this.isServerForObject | isMessage) || this._ammoType == null)
                return;
            this._ammoType.PopShell(this.x, this.y, -this.offDir);
            if (isMessage)
                return;
            Send.Message(new NMPopShell(this), NetMessagePriority.UnreliableUnordered);
        }

        public virtual void Reload(bool shell = true)
        {
            if (this.ammo != 0)
            {
                if (shell)
                    this.PopShell();
                --this.ammo;
            }
            this.loaded = true;
        }

        public override void Draw()
        {
            if (this.laserSight && this.held)
            {
                ATTracer type = new ATTracer
                {
                    range = 2000f
                };
                float ang = this.angleDegrees * -1f;
                if (this.offDir < 0)
                    ang += 180f;
                Vec2 vec2 = this.Offset(this.laserOffset);
                type.penetration = 0.4f;
                this._wallPoint = new Bullet(vec2.x, vec2.y, type, ang, this.owner, tracer: true).end;
                this._laserInit = true;
            }
            Material material = Graphics.material;
            if (this.graphic != null)
            {
                if (this.owner != null && this.owner.graphic != null)
                    this.graphic.flipH = this.owner.graphic.flipH;
                else
                    this.graphic.flipH = this.offDir <= 0;
            }
            if (this._doPuff)
            {
                this._clickPuff.alpha = 0.6f;
                this._clickPuff.angle = this.angle + this._smokeAngle;
                this._clickPuff.flipH = this.offDir < 0;
                this.Draw(_clickPuff, this.barrelOffset);
            }
            if (!VirtualTransition.active && Graphics.material == null)
                Graphics.material = this.material;
            base.Draw();
            Graphics.material = null;
            if (_flareAlpha > 0.0)
                this.Draw(_flare, this.barrelOffset);
            if ((double)this._barrelSmoke.speed > 0.0 && !this.raised)
            {
                this._barrelSmoke.alpha = 0.7f;
                this._barrelSmoke.angle = this._smokeAngle;
                this._barrelSmoke.flipH = this.offDir < 0;
                if (this.offDir > 0 && (double)this.angleDegrees > 90.0 && (double)this.angleDegrees < 270.0)
                    this._barrelSmoke.flipH = true;
                if (this.offDir < 0 && (double)this.angleDegrees > 90.0 && (double)this.angleDegrees < 270.0)
                    this._barrelSmoke.flipH = false;
                this._barrelSmoke.yscale = 1f - this._smokeFlatten;
                this.DrawIgnoreAngle(_barrelSmoke, this.barrelOffset);
            }
            if (!Options.Data.fireGlow)
                this.DrawGlow();
            Graphics.material = material;
            int num = DevConsole.showCollision ? 1 : 0;
        }

        public override void DrawGlow()
        {
            if (this.laserSight && this.held && this._laserTex != null && this._laserInit)
            {
                float num = 1f;
                if (!Options.Data.fireGlow)
                    num = 0.4f;
                Vec2 p1 = this.Offset(this.laserOffset);
                float length = (p1 - this._wallPoint).length;
                float val1 = 100f;
                if (this.ammoType != null)
                    val1 = this.ammoType.range;
                Vec2 normalized = (this._wallPoint - p1).normalized;
                Vec2 vec2 = p1 + normalized * Math.Min(val1, length);
                Graphics.DrawTexturedLine(this._laserTex, p1, vec2, Color.Red * num, 0.5f, this.depth - 1);
                if ((double)length > (double)val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(this._laserTex, vec2, vec2 + normalized * 2f, Color.Red * (1f - index * 0.2f) * num, 0.5f, this.depth - 1);
                        vec2 += normalized * 2f;
                    }
                }
                if (this._sightHit != null && (double)length < (double)val1)
                {
                    this._sightHit.alpha = num;
                    this._sightHit.color = Color.Red * num;
                    Graphics.Draw(this._sightHit, this._wallPoint.x, this._wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}
