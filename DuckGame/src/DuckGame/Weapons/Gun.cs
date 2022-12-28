// Decompiled with JetBrains decompiler
// Type: DuckGame.Gun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        protected Tex2D _laserTex;
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

        public AmmoType ammoType => _ammoType;

        public sbyte netAmmo
        {
            get => (sbyte)ammo;
            set => ammo = value;
        }

        public bool lowerOnFire => _lowerOnFire;

        public string bio => _bio;

        public Vec2 barrelPosition => Offset(barrelOffset);

        public Vec2 barrelOffset => _barrelOffsetTL - center + _extraOffset;

        public Vec2 laserOffset => _laserOffsetTL - center;

        public Vec2 barrelVector => Offset(barrelOffset) - Offset(barrelOffset + new Vec2(-1f, 0f));

        public override float angle
        {
            get => _angle + (float)_accuracyWave * (_accuracyLost * 0.5f);
            set => _angle = value;
        }

        public float barrelAngleOffset => _barrelAngleOffset;

        public float barrelAngle => Maths.DegToRad(Maths.PointDirection(Vec2.Zero, barrelVector) + _barrelAngleOffset * offDir);

        public bool CanSpin() => weight <= 5.0f;

        public override void EditorPropertyChanged(object property)
        {
            infiniteAmmoVal = infinite.value;
            UpdateMaterial();
        }

        public virtual void OnNetworkBulletsFired(Vec2 pos)
        {
        }

        public override void UpdateMaterial()
        {
            if (infinite.value)
                infiniteAmmoVal = true;
            if (infiniteAmmoVal)
            {
                if (material != null)
                    return;
                material = new MaterialGold(this);
            }
            else
                base.UpdateMaterial();
        }

        public bool fullAuto => _fullAuto;

        public Gun(float xval, float yval)
          : base(xval, yval)
        {
            _flare = new SpriteMap("smallFlare", 11, 10)
            {
                center = new Vec2(0f, 5f)
            };
            _barrelSmoke = new SpriteMap("barrelSmoke", 8, 8)
            {
                center = new Vec2(1f, 8f)
            };
            _barrelSmoke.ClearAnimations();
            _barrelSmoke.AddAnimation("puff", 1f, false, 0, 1, 2);
            _barrelSmoke.AddAnimation("loop", 1f, true, 3, 4, 5, 6, 7, 8);
            _barrelSmoke.AddAnimation("finish", 1f, false, 9, 10, 11, 12);
            _barrelSmoke.SetAnimation("puff");
            _barrelSmoke.speed = 0f;
            _translucent = true;
            physicsMaterial = PhysicsMaterial.Metal;
            _dontCrush = true;
            _clickPuff = new SpriteMap("clickPuff", 16, 16);
            _clickPuff.AddAnimation("puff", 0.3f, false, 0, 1, 2, 3);
            _clickPuff.center = new Vec2(0f, 12f);
            _sightHit = new Sprite("laserSightHit");
            _sightHit.CenterOrigin();
            depth = -0.1f;
            infinite = new EditorProperty<bool>(false, this)
            {
                _tooltip = "Makes gun have infinite ammo."
            };
            collideSounds.Add("smallMetalCollide");
            impactVolume = 0.3f;
            holsterAngle = 90f;
            coolingFactor = 1f / 500f;
        }

        public void DoAmmoClick()
        {
            _doPuff = true;
            _clickPuff.frame = 0;
            _clickPuff.SetAnimation("puff");
            _barrelHeat = 0f;
            _barrelSmoke.SetAnimation("finish");
            SFX.Play(_clickSound);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(barrelPosition.x, barrelPosition.y);
                smallSmoke.scale = new Vec2(0.3f, 0.3f);
                smallSmoke.hSpeed = Rando.Float(-0.1f, 0.1f);
                smallSmoke.vSpeed = -Rando.Float(0.05f, 0.2f);
                smallSmoke.alpha = 0.6f;
                Level.Add(smallSmoke);
            }
        }

        public override void HeatUp(Vec2 location)
        {
            if (_ammoType == null || !_ammoType.combustable || ammo <= 0 || heat <= 1f || Rando.Float(1f) <= 0.8f)
                return;
            int bulletFireIndex1 = bulletFireIndex;
            heat -= 0.05f;
            PressAction();
            int bulletFireIndex2 = bulletFireIndex;
            if (bulletFireIndex1 == bulletFireIndex2 || Rando.Float(1f) <= 0.4f)
                return;
            SFX.Play("bulletPop", Rando.Float(0.5f, 1f), Rando.Float(-1f, 1f));
        }

        public override void DoUpdate()
        {
            if (laserSight && _laserTex == null)
            {
                //this._additiveMaterial = new Material("Shaders/basicAdd");
                _laserTex = Content.Load<Tex2D>("pointerLaser");
            }
            base.DoUpdate();
        }

        public override void Update()
        {
            if (infiniteAmmoVal)
                ammo = 99;
            if (TeamSelect2.Enabled("INFAMMO"))
            {
                infinite.value = true;
                infiniteAmmoVal = true;
            }
            base.Update();
            if (_clickPuff.finished)
                _doPuff = false;
            _accuracyLost = Maths.CountDown(_accuracyLost, 0.015f);
            if (_flareAlpha > 0f)
                _flareAlpha -= 0.5f;
            else
                _flareAlpha = 0f;
            if (_barrelHeat > 0f)
                _barrelHeat -= 0.01f;
            else
                _barrelHeat = 0f;
            if (_barrelHeat > 10f)
                _barrelHeat = 10f;
            if (_smokeWait > 0f)
            {
                _smokeWait -= 0.1f;
            }
            else
            {
                if (_barrelHeat > 0.1f && _barrelSmoke.speed == 0f)
                {
                    _barrelSmoke.SetAnimation("puff");
                    _barrelSmoke.speed = 0.1f;
                }
                if (_barrelSmoke.speed > 0f && _barrelSmoke.currentAnimation == "puff" && _barrelSmoke.finished)
                    _barrelSmoke.SetAnimation("loop");
                if (_barrelSmoke.speed > 0f && _barrelSmoke.currentAnimation == "loop" && _barrelSmoke.frame == 5 && _barrelHeat < 0.1f)
                    _barrelSmoke.SetAnimation("finish");
            }
            if (_smokeWait > 0.0 && _barrelSmoke.speed > 0.0)
                _barrelSmoke.SetAnimation("finish");
            if (_barrelSmoke.currentAnimation == "finish" && _barrelSmoke.finished)
                _barrelSmoke.speed = 0f;
            if (owner != null)
            {
                if (owner.hSpeed > 0.1f)
                    _smokeAngle -= 0.1f;
                else if (owner.hSpeed < -0.1f)
                    _smokeAngle += 0.1f;
                if (_smokeAngle > 0.4f)
                    _smokeAngle = 0.4f;
                if (_smokeAngle < -0.4f)
                    _smokeAngle = -0.4f;
                if (owner.vSpeed > 0.1f)
                    _smokeFlatten -= 0.1f;
                else if (owner.vSpeed < -0.1f)
                    _smokeFlatten += 0.1f;
                if (_smokeFlatten > 0.5f)
                    _smokeFlatten = 0.5f;
                if (_smokeFlatten < -0.5f)
                    _smokeFlatten = -0.5f;
                _framesSinceThrown = 0;
            }
            else
            {
                ++_framesSinceThrown;
                if (_framesSinceThrown > 25)
                    _framesSinceThrown = 25;
            }
            if (!(this is Sword) && owner == null && CanSpin() && Level.current.simulatePhysics)
            {
                bool flag1 = false;
                bool flag2 = false;
                if ((Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2f || !grounded) && gravMultiplier > 0f && !flag2 && !_grounded)
                {
                    if (offDir > 0)
                        angleDegrees += (float)((Math.Abs(hSpeed * 2f) + Math.Abs(vSpeed)) * 1f + 5f);
                    else
                        angleDegrees -= (float)((Math.Abs(hSpeed * 2f) + Math.Abs(vSpeed)) * 1f + 5f);
                    flag1 = true;
                }
                if (!flag1 | flag2)
                {
                    angleDegrees %= 360f;
                    if (angleDegrees < 0f)
                        angleDegrees += 360f;
                    if (flag2)
                    {
                        if (Math.Abs(angleDegrees - 90f) < Math.Abs(angleDegrees + 90f))
                            angleDegrees = Lerp.Float(angleDegrees, 90f, 16f);
                        else
                            angleDegrees = Lerp.Float(-90f, 0f, 16f);
                    }
                    else if (angleDegrees > 90.0 && angleDegrees < 270.0)
                    {
                        angleDegrees = Lerp.Float(angleDegrees, 180f, 14f);
                    }
                    else
                    {
                        if (angleDegrees > 180.0)
                            angleDegrees -= 360f;
                        else if (angleDegrees < -180.0)
                            angleDegrees += 360f;
                        angleDegrees = Lerp.Float(angleDegrees, 0f, 14f);
                    }
                }
            }
            float num = (float)(1f - (Math.Sin(Maths.DegToRad(angleDegrees + 90f)) + 1f) / 2f);
            if (_owner == null)
                _extraOffset.y = num * (_collisionOffset.y + _collisionSize.y + _collisionOffset.y);
            else
                _extraOffset.y = 0f;
            if (owner == null || owner.hSpeed > -0.1f && owner.hSpeed < 0.1f)
            {
                if (_smokeAngle >= 0.1f)
                    _smokeAngle -= 0.1f;
                else if (_smokeAngle <= -0.1f)
                    _smokeAngle += 0.1f;
                else
                    _smokeAngle = 0f;
            }
            if (owner == null || owner.vSpeed > -0.1f && owner.vSpeed < 0.1f)
            {
                if (_smokeFlatten >= 0.1f)
                    _smokeFlatten -= 0.1f;
                else if (_smokeFlatten <= -0.1f)
                    _smokeFlatten += 0.1f;
                else
                    _smokeFlatten = 0f;
            }
            if (kick > 0f)
                kick -= 0.2f;
            else
                kick = 0f;
            if (owner == null)
            {
                if (ammo <= 0 && (alpha < 0.99f || grounded && Math.Abs(hSpeed) + Math.Abs(vSpeed) < 0.3f))
                {
                    canPickUp = false;
                    alpha -= 10.2f;
                    weight = 0.01f;
                }
                if (alpha < 0f)
                    Level.Remove(this);
            }
            if (owner != null && owner.graphic != null)
                graphic.flipH = owner.graphic.flipH;
            if (_wait > 0f)
                _wait -= 0.15f;
            if (_wait >= 0f)
                return;
            _wait = 0f;
        }

        public override void Terminate()
        {
            if (!(Level.current is Editor) && DGRSettings.S_ParticleMultiplier != 0)
            {
                Level.Add(SmallSmoke.New(x, y));
                Level.Add(SmallSmoke.New(x + 4f, y));
                Level.Add(SmallSmoke.New(x - 4f, y));
                Level.Add(SmallSmoke.New(x, y + 4f));
                Level.Add(SmallSmoke.New(x, y - 4f));
            }
            base.Terminate();
        }

        public override void PressAction()
        {
            if (isServerForObject && (TeamSelect2.Enabled("GUNEXPL") && ammo <= 0 || explode))
            {
                if (duck == null)
                    return;
                if (this is Warpgun)
                    duck.Kill(new DTImpale(this));
                else
                    duck.ThrowItem();
                Level.Remove(this);
                for (int index = 0; index < 1; ++index)
                {
                    ExplosionPart explosionPart = new ExplosionPart(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
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
                    // Precision probably matters here + dnspy decompiles to the same thing so don't remove  and (float).
                    Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 8f), y - (float)(Math.Sin(Maths.DegToRad(num)) * 8f), type, num)
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
            if (_fullAuto)
                return;
            if (isServerForObject)
                _fireActivated = true;
            Fire();
        }

        public override void OnHoldAction()
        {
            if (!_fullAuto)
                return;
            Fire();
        }

        public virtual void ApplyKick()
        {
            if (owner == null || !isServerForObject)
                return;
            if (_kickForce != 0.0)
            {
                Duck owner = this.owner as Duck;
                Thing thing = this.owner;
                if (owner != null && owner._trapped != null)
                    thing = owner._trapped;
                if (owner != null && owner.ragdoll != null && owner.ragdoll.part2 != null && owner.ragdoll.part1 != null && owner.ragdoll.part3 != null)
                {
                    Vec2 vec2 = -barrelVector * (_kickForce / 2f);
                    owner.ragdoll.part1.hSpeed += vec2.x;
                    owner.ragdoll.part1.vSpeed += vec2.y;
                    owner.ragdoll.part2.hSpeed += vec2.x;
                    owner.ragdoll.part2.vSpeed += vec2.y;
                    owner.ragdoll.part3.hSpeed += vec2.x;
                    owner.ragdoll.part3.vSpeed += vec2.y;
                }
                else
                {
                    Vec2 vec2 = -barrelVector * _kickForce;
                    if (Math.Sign(thing.hSpeed) != Math.Sign(vec2.x) || Math.Abs(vec2.x) > Math.Abs(thing.hSpeed))
                        thing.hSpeed = vec2.x;
                    if (owner != null)
                    {
                        if (owner.crouch)
                            owner.sliding = true;
                        thing.vSpeed += vec2.y - _kickForce * 0.333f;
                    }
                    else
                        thing.vSpeed += vec2.y - _kickForce * 0.333f;
                }
            }
            kick = 1f;
        }

        public virtual void Fire()
        {
            if (!loaded)
                return;
            if (ammo > 0 && _wait == 0.0)
            {
                firedBullets.Clear();
                if (duck != null)
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                ApplyKick();
                for (int index = 0; index < _numBulletsPerFire; ++index)
                {
                    float accuracy = _ammoType.accuracy;
                    _ammoType.accuracy *= 1f - _accuracyLost;
                    _ammoType.bulletColor = _bulletColor;
                    float angleDegrees = this.angleDegrees;
                    float angle = offDir >= 0 ? angleDegrees + _ammoType.barrelAngleDegrees : angleDegrees + 180f - _ammoType.barrelAngleDegrees;
                    if (!receivingPress)
                    {
                        if (_ammoType is ATDart)
                        {
                            if (isServerForObject)
                            {
                                Vec2 vec2 = Offset(barrelOffset);
                                Dart dart = new Dart(vec2.x, vec2.y, owner as Duck, -angle);
                                Fondle(dart);
                                if (onFire || _barrelHeat > 6.0)
                                {
                                    Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: dart, firedFrom: this));
                                    dart.burning = true;
                                    dart.onFire = true;
                                    Burn(position, this);
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
                            Bullet bullet = _ammoType.FireBullet(Offset(barrelOffset), owner, angle, this);
                            if (Network.isActive && isServerForObject)
                            {
                                firedBullets.Add(bullet);
                                if (duck != null && duck.profile.connection != null)
                                    bullet.connection = duck.profile.connection;
                            }
                            if (isServerForObject)
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
                    ++bulletFireIndex;
                    _ammoType.accuracy = accuracy;
                    _barrelHeat += 0.3f;
                }
                _smokeWait = 3f;
                loaded = false;
                _flareAlpha = 1.5f;
                if (!_manualLoad)
                    Reload();
                firing = true;
                _wait = _fireWait;
                PlayFireSound();
                if (owner == null)
                {
                    Vec2 vec2 = barrelVector * Rando.Float(1f, 3f);
                    vec2.y += Rando.Float(2f);
                    hSpeed -= vec2.x;
                    vSpeed -= vec2.y;
                }
                _accuracyLost += loseAccuracy;
                if (_accuracyLost <= maxAccuracyLost)
                    return;
                _accuracyLost = maxAccuracyLost;
            }
            else
            {
                if (ammo > 0 || _wait != 0.0)
                    return;
                firedBullets.Clear();
                DoAmmoClick();
                _wait = _fireWait;
            }
        }

        protected virtual void PlayFireSound() => SFX.Play(_fireSound, pitch: (Rando.Float(0.2f) - 0.1f + _fireSoundPitch));

        public void PopShell(bool isMessage = false)
        {
            if (!(isServerForObject | isMessage) || _ammoType == null) return;
            int iters = (int)Math.Ceiling(DGRSettings.ActualParticleMultiplier);
            for (int i = 0; i < iters; i++)
            {
                _ammoType.PopShell(x, y, -offDir);
            }
            if (isMessage) return;
            Send.Message(new NMPopShell(this), NetMessagePriority.UnreliableUnordered);
        }

        public virtual void Reload(bool shell = true)
        {
            if (ammo != 0)
            {
                if (shell)
                    PopShell();
                --ammo;
            }
            loaded = true;
        }

        public override void Draw()
        {
            if (laserSight && held)
            {
                ATTracer type = new ATTracer
                {
                    range = 2000f
                };
                float ang = angleDegrees * -1f;
                if (offDir < 0)
                    ang += 180f;
                Vec2 vec2 = Offset(laserOffset);
                type.penetration = 0.4f;
                _wallPoint = new Bullet(vec2.x, vec2.y, type, ang, owner, tracer: true).end;
                _laserInit = true;
            }
            Material material = Graphics.material;
            if (graphic != null)
            {
                if (owner != null && owner.graphic != null)
                    graphic.flipH = owner.graphic.flipH;
                else
                    graphic.flipH = offDir <= 0;
            }
            if (_doPuff)
            {
                _clickPuff.alpha = 0.6f;
                _clickPuff.angle = angle + _smokeAngle;
                _clickPuff.flipH = offDir < 0;
                Draw(_clickPuff, barrelOffset);
            }
            if (!VirtualTransition.active && Graphics.material == null)
                Graphics.material = this.material;
            base.Draw();
            Graphics.material = null;
            if (_flareAlpha > 0.0)
                Draw(_flare, barrelOffset);
            if (_barrelSmoke.speed > 0.0 && !raised)
            {
                _barrelSmoke.alpha = 0.7f;
                _barrelSmoke.angle = _smokeAngle;
                _barrelSmoke.flipH = offDir < 0;
                if (offDir > 0 && angleDegrees > 90.0 && angleDegrees < 270.0)
                    _barrelSmoke.flipH = true;
                if (offDir < 0 && angleDegrees > 90.0 && angleDegrees < 270.0)
                    _barrelSmoke.flipH = false;
                _barrelSmoke.yscale = 1f - _smokeFlatten;
                DrawIgnoreAngle(_barrelSmoke, barrelOffset);
            }
            if (!Options.Data.fireGlow)
                DrawGlow();
            Graphics.material = material;
            int num = DevConsole.showCollision ? 1 : 0;
        }

        public override void DrawGlow()
        {
            if (laserSight && held && _laserTex != null && _laserInit)
            {
                float num = 1f;
                if (!Options.Data.fireGlow)
                    num = 0.4f;
                Vec2 p1 = Offset(laserOffset);
                float length = (p1 - _wallPoint).length;
                float val1 = 100f;
                if (ammoType != null)
                    val1 = ammoType.range;
                Vec2 normalized = (_wallPoint - p1).normalized;
                Vec2 vec2 = p1 + normalized * Math.Min(val1, length);
                Graphics.DrawTexturedLine(_laserTex, p1, vec2, Color.Red * num, 0.5f, depth - 1);
                if (length > val1)
                {
                    for (int index = 1; index < 4; ++index)
                    {
                        Graphics.DrawTexturedLine(_laserTex, vec2, vec2 + normalized * 2f, Color.Red * (1f - index * 0.2f) * num, 0.5f, depth - 1);
                        vec2 += normalized * 2f;
                    }
                }
                if (_sightHit != null && length < val1)
                {
                    _sightHit.alpha = num;
                    _sightHit.color = Color.Red * num;
                    Graphics.Draw(_sightHit, _wallPoint.x, _wallPoint.y);
                }
            }
            base.DrawGlow();
        }
    }
}
