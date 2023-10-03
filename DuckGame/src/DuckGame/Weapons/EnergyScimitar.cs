// Decompiled with JetBrains decompiler
// Type: DuckGame.EnergyScimitar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public bool _stuck;
        public float _swordAngle;
        public float _lerpedAngle;
        public bool dragSpeedBonus;
        protected Vec2 centerHeld = new Vec2(6f, 26f);
        protected Vec2 centerUnheld = new Vec2(4f, 22f);
        public Stance _stance = Stance.SwingUp;
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
        public Sprite _blade;
        private Sprite _bladeTrail;
        private List<Blocker> _walls = new List<Blocker>();
        private Platform _platform;
        private Sprite _whiteGlow;
        private Sprite _warpLine;
        public Color properBladeColor = Color.White;
        public Color properColor = new Color(178, 220, 239);
        public Color swordColor;
        private LoopingSound _hum;
        private float _timeTillPulse;
        private List<RagdollDrag> _drag = new List<RagdollDrag>();
        public float _throwSpin;
        public bool _airFly;
        public float _airFlyAngle;
        public bool _canAirFly = true;
        private float _upFlyTime;
        public float _airFlySpeed = 14f;
        private int timeSinceReversal;
        public bool _wasLifted;
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
            get => isServerForObject ? _stuckInto != null : _stuck;
            set => _stuck = value;
        }

        public int stanceInt
        {
            get => (int)_stance;
            set => _stance = (Stance)value;
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
                if (owner is WireMount)
                    return _angle;
                return !held && owner != null ? _angle + 1.570796f * offDir : Maths.DegToRad(_lerpedAngle) * offDir + Maths.DegToRad(_throwSpin);
            }
            set => _angle = value;
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
        }

        public override void PressAction()
        {
            if (owner is WireMount)
            {
                WireMount owner = this.owner as WireMount;
                foreach (MaterialThing materialThing in Level.CheckRectAll<Block>(position + new Vec2(-8f, -8f), position + new Vec2(8f, 8f)))
                    clip.Add(materialThing);
                float pAngleDegrees = (float)(-angleDegrees - 90 - 180);
                this.owner = null;
                owner._containedThing = null;
                StartFlying(pAngleDegrees);
            }
            base.PressAction();
        }

        private void UpdateStance()
        {
            ++_timeSincePress;
            ++_timeSinceDragJump;
            if (duck == null || !held)
            {
                _stance = Stance.None;
                _swordAngle = 0f;
                _lerpedAngle = owner != null ? 0f : (_wasLifted ? 90f : 0f);
                _swordFlip = offDir < 0;
                ++_framesSinceThrown;
                center = centerUnheld;
                collisionOffset = new Vec2(-2f, -16f);
                collisionSize = new Vec2(4f, 26f);
                if (_wasLifted)
                {
                    if (_airFly)
                    {
                        if (vSpeed < -4)
                        {
                            collisionOffset = new Vec2(0f, -4f);
                            collisionSize = new Vec2(6f, 8f);
                        }
                        else if (vSpeed > 4)
                        {
                            collisionOffset = new Vec2(-5f, -4f);
                            collisionSize = new Vec2(6f, 8f);
                        }
                        else
                        {
                            collisionOffset = new Vec2(-4f, 0f);
                            collisionSize = new Vec2(8f, 6f);
                        }
                    }
                    else
                    {
                        center = new Vec2(6f, 22f);
                        collisionOffset = new Vec2(-4f, -3f);
                        collisionSize = new Vec2(8f, 6f);
                    }
                }
                if (owner != null || stuck || !_wasLifted)
                    return;
                bool flag1 = false;
                bool flag2 = false;
                if (_airFly)
                {
                    PerformAirSpin();
                    flag1 = true;
                }
                else if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2 || !grounded)
                {
                    if (!grounded && Level.CheckRect<Block>(position + new Vec2(-6f, -6f), position + new Vec2(6f, -2f)) != null)
                        flag2 = true;
                    if (!flag2 && !_grounded && (Level.CheckPoint<IPlatform>(position + new Vec2(0f, 8f)) == null || vSpeed < 0 || _airFly))
                    {
                        PerformAirSpin();
                        flag1 = true;
                    }
                }
                if (!(!flag1 | flag2) || _airFly)
                    return;
                _throwSpin %= 360f;
                if (flag2)
                {
                    if (Math.Abs(_throwSpin - 90f) < Math.Abs(_throwSpin + 90f))
                        _throwSpin = Lerp.Float(_throwSpin, 90f, 16f);
                    else
                        _throwSpin = Lerp.Float(-90f, 0f, 16f);
                }
                else if (_throwSpin > 90 && _throwSpin < 270)
                {
                    _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                }
                else
                {
                    if (_throwSpin > 180)
                        _throwSpin -= 360f;
                    else if (_throwSpin < -180)
                        _throwSpin += 360f;
                    _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                }
            }
            else
            {
                if (_stance == Stance.None)
                    _stance = Stance.SwingUp;
                _framesSinceThrown = 0;
                center = centerHeld;
                collisionOffset = new Vec2(-4f, 0f);
                collisionSize = new Vec2(4f, 4f);
                _throwSpin = 0f;
                _wasLifted = true;
                _blocking = duck.crouch && Math.Abs(duck.hSpeed) < 2;
                if (duck.inputProfile.Pressed(Triggers.Up) && !duck.inputProfile.Pressed(Triggers.Jump) && (_stance == Stance.Drag || _stance == Stance.Intermediate) && !duck.sliding)
                    _stance = Stance.SwingUp;
                if (duck.crouch && !duck.sliding && duck.inputProfile.Pressed(Triggers.Left))
                    duck.offDir = -1;
                else if (duck.crouch && !duck.sliding && duck.inputProfile.Pressed(Triggers.Right))
                    duck.offDir = 1;
                bool flag = Level.CheckLine<IPlatform>(new Vec2(owner.position.x, owner.bottom) + new Vec2(-offDir * 16, -10f), new Vec2(owner.position.x, owner.bottom) + new Vec2(-offDir * 16, 2f)) == null;
                _spikeDrag = duck.grounded && !flag && Level.CheckLine<Spikes>(new Vec2(owner.position.x, owner.bottom) + new Vec2(-offDir * 16, -10f), new Vec2(owner.position.x, owner.bottom) + new Vec2(-offDir * 16, 2f)) != null;
                _dragRand = Lerp.FloatSmooth(_dragRand, 0f, 0.1f);
                if (_dragRand > 1)
                    _dragRand = 1f;
                dragSpeedBonus = _stance == Stance.Drag && !flag && _stanceReady;
                if (_spikeDrag && dragSpeedBonus)
                {
                    _dragRand += Rando.Float(Math.Abs(duck.hSpeed)) * 0.1f;
                    if (Rando.Int(30) == 0 && _dragRand > 0.1f)
                        duck.Swear();
                }
                if (!duck.grounded && duck.inputProfile.Pressed(Triggers.Down) && _stance != Stance.Drag)
                    _goIntermediate = true;
                if (_stance == Stance.Drag && duck._hovering)
                {
                    _stance = Stance.SwingDown;
                    duck.vSpeed = -6f;
                    duck._hovering = false;
                    _timeSinceDragJump = 100;
                }
                if (duck.inputProfile.Pressed(Triggers.Down) && duck.grounded)
                    _stance = Stance.Drag;
                if (Math.Abs(duck.hSpeed) > 1)
                {
                    if (dragSpeedBonus)
                    {
                        if (DGRSettings.ActualParticleMultiplier >= 1)
                        {
                            for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++)
                            {
                                Spark spark = Spark.New(barrelPosition.x, barrelPosition.y - 6f, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                                spark._color = swordColor;
                                spark._width = 1f;
                                _glow = 0.3f;
                                Level.Add(spark);
                            }
                        }
                        else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier)
                        {
                            Spark spark = Spark.New(barrelPosition.x, barrelPosition.y - 6f, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                            spark._color = swordColor;
                            spark._width = 1f;
                            _glow = 0.3f;
                            Level.Add(spark);
                        }
                    }
                    if (_stance == Stance.Drag && duck.grounded)
                    {
                        if (duck.sliding || duck.crouch)
                        {
                            duck.tilt = 0f;
                            duck.verticalOffset = 0f;
                        }
                        else
                        {
                            float num = duck.hSpeed - Math.Sign(duck.hSpeed);
                            if (Math.Sign(num) != Math.Sign(duck.hSpeed))
                                num = 0f;
                            duck.tilt = num;
                            duck.verticalOffset = Math.Abs(num);
                        }
                    }
                }

                if (_stance == Stance.SwingDown && duck.inputProfile.Pressed(Triggers.Jump))
                    _stance = Stance.SwingUp;
                if (_goIntermediate && _stanceReady)
                {
                    _stance = Stance.Intermediate;
                    _goIntermediate = false;
                }
                handAngle = 0f;
                _holdOffset = new Vec2(0f, 0f);
                handOffset = new Vec2(0f, 0f);
                handFlip = false;
                if (_stance == Stance.Intermediate)
                {
                    _swordAngle = -60f;
                    handAngle = Maths.DegToRad(_swordAngle - 90f) * offDir;
                    _holdOffset = new Vec2(0f, 2f);
                    _swordFlip = offDir > 0;
                    if (duck.grounded)
                        _stance = Stance.Drag;
                }
                else if (_stance == Stance.Drag)
                {
                    if (duck._hovering)
                    {
                        _swordAngle = -190f;
                        handAngle = Maths.DegToRad(_swordAngle - 90f) * offDir;
                        _holdOffset = new Vec2(0f, 2f);
                        _swordFlip = offDir > 0;
                    }
                    else
                    {
                        _swordAngle = !flag ? _dragAngle : _dragAngleDangle;
                        if (duck.crouch)
                            _swordAngle += 10f;
                        if (duck.sliding)
                            _swordAngle += 10f;
                        handAngle = Maths.DegToRad(_swordAngle - 90f) * offDir;
                        _holdOffset = new Vec2(0f, 2f);
                        _swordFlip = offDir > 0;
                    }
                }
                else if (_stance == Stance.SwingUp)
                {
                    if (duck._hovering || (duck.cordHover))
                    {
                        _swordAngle = -25f;
                        _swordFlip = offDir < 0;
                    }
                    else if (_blocking)
                    {
                        _swordAngle = 130f;
                        handAngle = Maths.DegToRad(_swordAngle) * offDir;
                        _holdOffset = new Vec2(-22f, 0f);
                        handOffset = new Vec2(7f, -7f);
                        _swordFlip = offDir >= 0;
                        handFlip = true;
                    }
                    else
                    {
                        _swordAngle = 25f;
                        handAngle = Maths.DegToRad(_swordAngle) * offDir;
                        _holdOffset = new Vec2((_swingDif * 0.55f - 2f), -4f);
                        handOffset = new Vec2((2f + _swingDif * 0.35f), -3f);
                        _swordFlip = offDir < 0;
                    }
                }
                else if (_stance == Stance.SwingDown)
                {
                    if (duck._hovering)
                    {
                        _swordAngle = 40f;
                        _holdOffset = new Vec2(0f, 8f);
                        handOffset = new Vec2(2f, 0f);
                        _swordFlip = offDir < 0;
                    }
                    else if (_blocking)
                    {
                        _swordAngle = 45f;
                        handAngle = Maths.DegToRad(_swordAngle) * offDir;
                        _holdOffset = new Vec2(3f, -3f);
                        handOffset = new Vec2(7f, 0f);
                        _swordFlip = offDir < 0;
                    }
                    else
                    {
                        _swordAngle = 80f;
                        handAngle = Maths.DegToRad(_swordAngle) * offDir;
                        _holdOffset = new Vec2(0f, (float)(-2 - _swingDif * 0.55f));
                        handOffset = new Vec2((float)(0 + _swingDif * 0.35f), 3f);
                        _swordFlip = offDir < 0;
                    }
                }
                _lerpedAngle = Lerp.FloatSmooth(_lerpedAngle, _swordAngle, 0.25f + _lerpBoost);
                _stanceReady = Math.Abs(_lerpedAngle - _swordAngle) < 25;
                _stanceCounter += Maths.IncFrameTimer();
                _swingDif = Math.Min(Math.Abs(_lerpedAngle - _swordAngle), 35f);
                if (_timeSincePress > 25)
                    _swingDif *= 0.25f;
                _lerpBoost = Lerp.FloatSmooth(_lerpBoost, 0f, 0.1f);
            }
        }

        public override void Ejected(Thing pFrom)
        {
            ResetTrailHistory();
            if (pFrom is SpawnCannon)
            {
                StartFlying((pFrom as SpawnCannon).direction);
            }
            else
            {
                if (vSpeed < -0.1f)
                    StartFlying(TileConnection.Up);
                if (vSpeed > 0.1f)
                    StartFlying(TileConnection.Down);
                if (hSpeed < -0.1f)
                    StartFlying(TileConnection.Left);
                if (hSpeed <= 0.1f)
                    return;
                StartFlying(TileConnection.Right);
            }
        }

        public override void OnPressAction()
        {
            if (!isServerForObject || duck == null || receivingPress)
                return;
            _goIntermediate = false;
            if (_timeSincePress > 3)
                _timeSincePress = 0;
            if (_stance == Stance.Intermediate)
                _stance = Stance.SwingDown;
            if (!_stanceReady)
                return;
            if (_stance == Stance.Drag || _stance == Stance.SwingUp)
            {
                if (_stance == Stance.Drag && duck != null && (!duck.grounded || !duck.crouch && !duck.sliding))
                {
                    duck.hSpeed = duck.offDir * 9;
                    duck.vSpeed = -2f;
                    duck._disarmDisable = 5;
                }
                else if (_stance == Stance.Drag && (duck.crouch || duck.sliding))
                    _lerpBoost = 0.4f;
                _stance = Stance.SwingDown;
            }
            else
            {
                if (_stance != Stance.SwingDown)
                    return;
                _stance = Stance.SwingUp;
            }
        }

        public override void Fire()
        {
            //this._stanceHeld = true;
            _stanceCounter = 0f;
        }

        public EnergyScimitar(float pX, float pY)
          : base(pX, pY)
        {
            graphic = new Sprite("energyScimiHilt");
            center = new Vec2(6f, 26f);
            collisionOffset = new Vec2(-2f, -24f);
            collisionSize = new Vec2(4f, 28f);
            _blade = new Sprite("energyScimiBlade");
            _bladeTrail = new Sprite("energyScimiBladeTrail");
            _whiteGlow = new Sprite("whiteGlow")
            {
                center = new Vec2(16f, 28f),
                xscale = 0.8f,
                yscale = 1.4f
            };
            _fullAuto = true;
            _bouncy = 0.5f;
            _impactThreshold = 0.3f;
            ammo = 99999;
            _ammoType = new ATLaser();
            thickness = 0.01f;
            _impactThreshold = 0.5f;
            _bladeMaterial = new MaterialEnergyBlade(this);

            swordColor = properColor;
            _warpLine = new Sprite("warpLine2");
            editorTooltip = "How do you invent a sword? It uses modern technology.";
        }

        public Vec2 barrelStartPos => position + (Offset(barrelOffset) - position).normalized * 2f;
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is EnergyScimitar && pTaped.gun2 is Chainsaw ? new EnergyChainsaw(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Initialize()
        {
            if (material is MaterialGold)
            {
                // _blade.color = Color.Lerp(properBladeColor, Color.Red, heat);
                //swordColor = Color.Lerp(properColor, Color.Red, heat);
                properBladeColor = new Color(255, 216, 0);
                properColor = new Color(255, 216, 0); //255, 216, 24
            }
            _platform = new ScimiPlatform(0f, 0f, 20f, 8f, this)
            {
                solid = false,
                enablePhysics = false,
                center = new Vec2(10f, 4f),
                collisionOffset = new Vec2(-10f, -2f),
                thickness = 0.01f
            };
            Level.Add(_platform);
            _hum = new LoopingSound("scimiHum")
            {
                volume = 0f
            };
            _humAmount = 0f;
            _hum.lerpSpeed = 1f;
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => false;

        public override void Terminate()
        {
            if (_hum != null)
                _hum.Kill();
            foreach (Thing wall in _walls)
                Level.Remove(wall);
            _walls.Clear();
            if (_platform != null)
                Level.Remove(_platform);
            base.Terminate();
        }

        public void Pulse()
        {
            if (_timeTillPulse >= 0f)
                return;
            _timeTillPulse = 0.2f;
            SFX.Play("scimiSurge", 0.8f, Rando.Float(-0.2f, 0.2f));
            _glow = 12f;
            Vec2 normalized = (position - this.barrelPosition).normalized;
            Vec2 barrelPosition = this.barrelPosition;
            int ix = (int)(DGRSettings.ActualParticleMultiplier * 6);
            float f = 24f / ix;
            for (int index = 0; index < ix; ++index)
            {
                Spark spark = Spark.New(barrelPosition.x, barrelPosition.y, new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f)));
                spark._color = swordColor;
                spark._width = 1f;
                Level.Add(spark);
                barrelPosition += normalized * f;
            }
        }

        public override bool Destroy(DestroyType type = null) => base.Destroy(type);

        public override void DoTerminate() => base.DoTerminate();

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (owner != null || !isServerForObject)
                return;
            if (_airFly && with is EnergyScimitar && (with as EnergyScimitar)._airFly)
            {
                Fondle(with);
                Vec2 vec1 = Maths.AngleToVec(Maths.DegToRad(_airFlyAngle));
                hSpeed = lastHSpeed = (float)(-vec1.x * 3);
                vSpeed = lastVSpeed = (float)(-vec1.y * 3);
                Vec2 vec2 = Maths.AngleToVec(Maths.DegToRad((with as EnergyScimitar)._airFlyAngle));
                with.hSpeed = with.lastHSpeed = (float)(-vec2.x * 3);
                with.vSpeed = with.lastVSpeed = (float)(-vec2.y * 3);
                Shing();
                (with as EnergyScimitar).Shing();
            }
            else if (_airFly && with is Coin c)
            {
                Duck d = null;
                if (_lastThrownBy != null) d = (Duck)_lastThrownBy;
                Fondle(c);
                Vec2 v = c.TargetNear(d, true)[0];
                _airFlyAngle = Maths.PointDirection(position, v);
            }
            else if (_airFly && with is PhysicsObject && !(with is Gun) && !(with is Equipment) && !(with is Duck) && !(with is RagdollPart))
                with.Destroy(new DTIncinerate(this));
            else if (_airFly && with is Duck && (with != _prevOwner || _framesSinceThrown > 15))
            {
                with.Destroy(new DTImpale(this));
                Duck duck = with as Duck;
                if (duck.ragdoll == null || duck.ragdoll.part2 == null || _drag.FirstOrDefault(x => x.part == with) != null)
                    return;
                _drag.Add(new RagdollDrag()
                {
                    part = duck.ragdoll.part2,
                    offset = position - duck.ragdoll.part2.position
                });
            }
            else if (with is Block || with is IPlatform && from == ImpactedFrom.Bottom && vSpeed > 0)
            {
                if (with is Nubber)
                    return;
                Shing();
                if (_framesSinceThrown <= 5)
                    return;
                _framesSinceThrown = 25;
            }
            else
            {
                if (!_airFly || !(with is RagdollPart))
                    return;
                RagdollPart ragdollPart = with as RagdollPart;
                if (ragdollPart.doll != null && ragdollPart.doll.captureDuck != null && ragdollPart.doll.captureDuck == _prevOwner && _framesSinceThrown <= 15 || _drag.FirstOrDefault(x => x.part == with) != null || ragdollPart.doll == null)
                    return;
                if (ragdollPart.doll.part1 != null)
                {
                    _drag.Add(new RagdollDrag()
                    {
                        part = ragdollPart.doll.part1,
                        offset = position - ragdollPart.doll.part1.position
                    });
                    ragdollPart.doll.part1.owner = this;
                }
                if (ragdollPart.doll.part2 != null)
                {
                    _drag.Add(new RagdollDrag()
                    {
                        part = ragdollPart.doll.part2,
                        offset = position - ragdollPart.doll.part2.position
                    });
                    ragdollPart.doll.part2.owner = this;
                }
                if (ragdollPart.doll.part3 == null)
                    return;
                _drag.Add(new RagdollDrag()
                {
                    part = ragdollPart.doll.part3,
                    offset = position - ragdollPart.doll.part3.position
                });
                ragdollPart.doll.part3.owner = this;
            }
        }

        public Vec2 TravelThroughAir(float pMult = 1f)
        {
            Vec2 vec = Maths.AngleToVec(Maths.DegToRad(_airFlyAngle));
            position += vec * _airFlySpeed * pMult;
            return vec;
        }

        public void ReverseFlyDirection()
        {
            if (timeSinceReversal <= 10)
                return;
            timeSinceReversal = 0;
            _airFlyAngle += 180f;
            offDir *= -1;
            TravelThroughAir();
            PerformAirSpin();
        }

        private void UpdateAirDirection()
        {
            if (offDir < 0)
                _throwSpin = (float)(-_airFlyAngle + 180);
            else
                _throwSpin = -_airFlyAngle;
        }

        protected void PerformAirSpin()
        {
            ++timeSinceReversal;
            if (!enablePhysics)
                return;
            if (_airFly)
            {
                Vec2 vec = Maths.AngleToVec(Maths.DegToRad(_airFlyAngle));
                offDir = vec.x < -0.1f ? (sbyte)-1 : (sbyte)1;
                if (Math.Abs(vec.x) < 0.2f && vec.y < 0f)
                {
                    _upFlyTime += Maths.IncFrameTimer();
                    if (_upFlyTime > 2)
                        ReverseFlyDirection();
                }
                UpdateAirDirection();
                if (!skipThrowMove)
                {
                    hSpeed = vec.x * _airFlySpeed;
                    vSpeed = vec.y * _airFlySpeed;
                }
                _impactThreshold = 0.01f;
                _bouncy = 0f;
                hMax = _airFlySpeed;
                vMax = _airFlySpeed;
                _lerpedAngle = 90f;
                gravMultiplier = 0f;
            }
            else
            {
                _impactThreshold = 0.3f;
                _bouncy = 0.5f;
                hMax = 12f;
                if (hSpeed > 0f)
                    _throwSpin += ((Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f);
                else
                    _throwSpin -= ((Math.Abs(hSpeed) + Math.Abs(vSpeed)) * 2f + 4f);
            }
        }

        public override void ReturnToWorld()
        {
            if (_airFly)
                return;
            _throwSpin = 90f;
        }

        public void StartFlying(TileConnection pDirection, bool pThrown = false)
        {
            switch (pDirection)
            {
                case TileConnection.Left:
                    StartFlying(180f, pThrown);
                    break;
                case TileConnection.Right:
                    StartFlying(0f, pThrown);
                    break;
                case TileConnection.Up:
                    StartFlying(90f, pThrown);
                    break;
                case TileConnection.Down:
                    StartFlying(270f, pThrown);
                    break;
            }
        }

        public void StartFlying(float pAngleDegrees, bool pThrown = false)
        {
            if (owner != null)
                return;
            _wasLifted = true;
            _airFly = true;
            _upFlyTime = 0f;
            _airFlyAngle = pAngleDegrees;
            if (pThrown)
                TravelThroughAir(-0.5f);
            skipThrowMove = true;
            PerformAirSpin();
            skipThrowMove = false;
            UpdateStance();
            ResetTrailHistory();
        }

        public override void Thrown()
        {
            if (duck == null)
                return;
            x = duck.x;
            _oldDepth = depth = -0.1f;
            if (!isServerForObject || duck == null || duck.destroyed || !_canAirFly || _airFly)
                return;
            _upFlyTime = 0f;
            if (!duck.inputProfile.Down(Triggers.Grab))
                return;
            if (duck.inputProfile.Down(Triggers.Left) && duck.offDir < 0 || duck.inputProfile.Down(Triggers.Right) && duck.offDir > 0)
            {
                y = duck.y;
                if (_stance == Stance.Drag)
                    y += 6f;
                skipThrowMove = true;
                TileConnection pDirection = duck.inputProfile.Down(Triggers.Left) ? TileConnection.Left : TileConnection.Right;
                owner = null;
                StartFlying(pDirection, true);
                skipThrowMove = false;
            }
            else
            {
                if (!duck.inputProfile.Down(Triggers.Up) && !duck.inputProfile.Down(Triggers.Down) || !duck.inputProfile.Down(Triggers.Up) && duck.grounded)
                    return;
                int num = 1;
                if (duck.inputProfile.Down(Triggers.Up))
                    num = -1;
                x = duck.x + duck.offDir * -2f;
                if (num == 1 && !duck.grounded)
                    duck.vSpeed -= 8f;
                skipThrowMove = true;
                TileConnection pDirection = TileConnection.Down;
                if (num < 0)
                    pDirection = TileConnection.Up;
                owner = null;
                StartFlying(pDirection, true);
                skipThrowMove = false;
            }
        }

        private void UpdateStuck()
        {
            if (!stuck)
                return;
            UpdateAirDirection();
        }

        public void Shing()
        {
            if (stuck)
                return;
            gravMultiplier = 1f;
            ClearDrag();
            Pulse();
            _timeSinceBlast = 0f;
            if (_airFly && isServerForObject)
            {
                Vec2 vec2 = TravelThroughAir(-0.5f);
                Vec2 hitPos = Vec2.Zero;
                MaterialThing materialThing = Level.CheckRay<Block>(position, position + vec2 * _airFlySpeed, out hitPos);
                if (materialThing != _platform)
                {
                    if (materialThing is ScimiPlatform)
                    {
                        hSpeed = lastHSpeed = (float)(-vec2.x * 3);
                        vSpeed = lastHSpeed = (float)(-vec2.y * 3);
                    }
                    else
                    {
                        if (materialThing != null)
                        {
                            clip.Add(materialThing);
                            position = hitPos - vec2 * 16f;
                            UpdateAirDirection();
                        }
                        if (materialThing != null)
                        {
                            _stuckInto = materialThing;
                            _longCharge = true;
                            enablePhysics = false;
                            hSpeed = 0f;
                            vSpeed = 0f;
                            lastHSpeed = _hSpeed;
                            lastVSpeed = _vSpeed;
                            depth = -0.55f;
                        }
                        else
                        {
                            vSpeed = (float)(-vSpeed * 0.25);
                            hSpeed = (float)(-hSpeed * 0.25);
                        }
                    }
                }
            }
            _airFly = false;
        }

        protected void QuadLaserHit(QuadLaserBullet pBullet)
        {
            if (!isServerForObject)
                return;
            Fondle(pBullet);
            EnergyScimitarBlast energyScimitarBlast = new EnergyScimitarBlast(pBullet.position, new Vec2(offDir * 2000, 0f));
            Level.Add(energyScimitarBlast);
            Level.Remove(pBullet);
            if (!Network.isActive)
                return;
            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast.position, energyScimitarBlast._target));
        }

        public float angleWhoom => _angleWhoom;

        public override void OnTeleport()
        {
            ResetTrailHistory();
            base.OnTeleport();
        }

        public void ClearDrag()
        {
            int num = 1;
            foreach (RagdollDrag ragdollDrag in _drag)
            {
                if (ragdollDrag.part.doll != null && ragdollDrag.part.doll.captureDuck != null && ragdollDrag.part.doll.captureDuck._cooked == null)
                {
                    ragdollDrag.part.position = Offset(Maths.AngleToVec(Maths.DegToRad(_airFlyAngle)) * 8f);
                    ragdollDrag.part.doll.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.Cook();
                    ragdollDrag.part.doll.captureDuck.Kill(new DTIncinerate(this));
                    if (ragdollDrag.part.doll.captureDuck._cooked != null)
                        ragdollDrag.part.doll.captureDuck._cooked.vSpeed = -(2 + num);
                    ++num;
                }
            }
            _drag.Clear();
        }

        protected void OnSwing()
        {
            if (duck == null || !isServerForObject)
                return;
            if (duck._hovering)
            {
                _revertVMaxDuck = duck;
                _vmaxReversion = duck.vMax;
                duck.vMax = 13f;
                duck.vSpeed = -13f;
                _slowV = true;
                warpLines.Add(new WarpLine()
                {
                    start = duck.position,
                    end = duck.position + new Vec2(0f, -80f),
                    lerp = 0f,
                    wide = 24f
                });
            }
            else
            {
                duck.hSpeed = offDir * 11.25f;
                duck.vSpeed = -1f;
                _slowV = false;
                warpLines.Add(new WarpLine()
                {
                    start = duck.position + new Vec2(-offDir * 16, 4f),
                    end = duck.position + new Vec2(offDir * 62, 4f),
                    lerp = 0f,
                    wide = 20f
                });
            }
            slowWait = 0.085f;
        }

        protected virtual void ResetTrailHistory()
        {
            _lastAngles = new float[8];
            _lastPositions = new Vec2[8];
            _lastIndex = 0;
            _lastSize = 0;
        }

        protected int historyIndex(int idx) => _lastIndex + idx + 1 & 7;

        protected void addHistory(float angle, Vec2 position)
        {
            _lastAngles[_lastIndex] = angle;
            _lastPositions[_lastIndex] = position;
            _lastIndex = _lastIndex - 1 & 7;
            ++_lastSize;
        }

        public override bool Sprung(Thing pSpringer)
        {
            StartFlying((float)(-pSpringer.angleDegrees - 90 - 180));
            return false;
        }

        public override void Update()
        {
            if (_hum != null)
                _hum.Update();
            if (!isServerForObject)
                UpdateStuck();
            _skipAutoPlatforms = _airFly;
            _skipPlatforms = _airFly;
            if (_airFly || _stuckInto != null)
                depth = -0.55f;
            else
                depth = -0.1f;
            if (_stuckInto != null && _stuckInto is Door && Math.Abs((_stuckInto as Door)._open) > 0.5)
            {
                _stuckInto.Fondle(this);
                enablePhysics = true;
                _stuckInto = null;
            }
            ammo = 999;
            UpdateStance();
            if (_glow < 0.4f)
                _glowTime = 0;
            if (duck != null && (_glow > 0.4f || _glowTime > 0 && _glowTime < 4))
            {
                ++_glowTime;
                foreach (Bullet bullet1 in Level.current.things[typeof(Bullet)])
                {
                    Vec2 vec2 = barrelStartPos + OffsetLocal(new Vec2(8f, 0f));
                    Vec2 barrelPosition = this.barrelPosition;
                    bool flag = Collision.LineIntersect(vec2 + velocity, barrelPosition + velocity, bullet1.start, bullet1.start + bullet1.travelDirNormalized * bullet1.bulletSpeed);
                    if (!flag)
                        flag = Collision.LineIntersect(vec2 + velocity * 0.5f, barrelPosition + velocity * 0.5f, bullet1.start, bullet1.start + bullet1.travelDirNormalized * bullet1.bulletSpeed);
                    if (flag && bullet1.lastReboundSource != this)
                    {
                        bullet1.lastReboundSource = this;
                        Bullet bullet2 = bullet1.ReverseTravel();
                        if (bullet2 != null)
                            bullet2.owner = duck;
                        Pulse();
                    }
                }
            }
            _timeSinceBlast += Maths.IncFrameTimer();
            float num1 = Math.Min(_angleWhoom, 0.5f) * 38f;
            if (isServerForObject)
            {
                _stickWait -= Maths.IncFrameTimer();
                if (duck != null && slowWait > 0)
                {
                    slowWait -= Maths.IncFrameTimer();
                    if (slowWait <= 0)
                    {
                        if (_revertVMaxDuck != null)
                        {
                            _revertVMaxDuck.vMax = _vmaxReversion;
                            _revertVMaxDuck = null;
                        }
                        if (_slowV)
                            duck.vSpeed *= 0.25f;
                        else
                            duck.hSpeed *= 0.25f;
                    }
                }
                handFlip = false;
                foreach (RagdollDrag ragdollDrag in _drag)
                {
                    ragdollDrag.part.position = position - ragdollDrag.offset;
                    ragdollDrag.part.hSpeed = 0f;
                    ragdollDrag.part.vSpeed = 0f;
                }
                _timeSincePickedUp += Maths.IncFrameTimer();
                if (_stance == Stance.Drag && duck != null)
                    _glow = Math.Abs(duck.hSpeed) > 1 ? 0.35f : 0f;
                if (grounded)
                    _canAirFly = true;
                _timeTillPulse -= Maths.IncFrameTimer();
                if (owner != null)
                {
                    gravMultiplier = 1f;
                    if (_glow > 0.4f && _timeSincePickedUp > 0.25f && duck != null)
                    {
                        Vec2 barrelStartPos = this.barrelStartPos;
                        Vec2 p2_1 = this.barrelStartPos + new Vec2(duck.hSpeed * 2f, duck.vSpeed);
                        Vec2 barrelPosition = this.barrelPosition;
                        Vec2 p2_2 = this.barrelPosition + new Vec2(duck.hSpeed * 2f, duck.vSpeed);
                        foreach (EnergyScimitar t in Level.current.things[typeof(EnergyScimitar)])
                        {
                            if (t != this && t.owner != duck)
                            {
                                if (t.owner == null && t._airFly && t.offDir != duck.offDir && (Math.Abs(t.hSpeed) > 2f && Collision.Line(this.barrelStartPos, this.barrelPosition, new Rectangle(t.x + t.hSpeed, t.y - 8f, Math.Abs(t.hSpeed), 16f)) || Math.Abs(t.vSpeed) > 2f && Collision.Line(this.barrelStartPos, this.barrelPosition, new Rectangle(t.x - 8f, t.y + t.vSpeed, 16f, Math.Abs(t.vSpeed)))))
                                {
                                    Fondle(t);
                                    t.ReverseFlyDirection();
                                    Shing();
                                }
                                if (t.owner is Duck && t._glow > 0.4f && (Collision.LineIntersect(this.barrelStartPos, this.barrelPosition, t.barrelStartPos, t.barrelPosition) || Collision.LineIntersect(barrelStartPos, p2_1, t.barrelStartPos, t.barrelPosition) || Collision.LineIntersect(barrelPosition, p2_2, t.barrelStartPos, t.barrelPosition)) && _timeSinceBlast > 0.15f)
                                {
                                    Duck owner = t.owner as Duck;
                                    duck.x -= duck.hSpeed;
                                    owner.x -= owner.hSpeed;
                                    _timeSinceBlast = 0f;
                                    owner.hSpeed = offDir * 5f;
                                    owner.vSpeed = -4f;
                                    duck.hSpeed = -offDir * 5f;
                                    duck.vSpeed = -4f;
                                    duck.hSpeed *= 2f;
                                    duck.UpdatePhysics();
                                    duck.hSpeed /= 2f;
                                    owner.hSpeed *= 2f;
                                    owner.UpdatePhysics();
                                    owner.hSpeed /= 2f;
                                    duck.swordInvincibility = 10;
                                    owner.swordInvincibility = 10;
                                    Shing();
                                    t.Shing();
                                    if (isServerForObject && this.owner != null && owner != null)
                                    {
                                        EnergyScimitarBlast energyScimitarBlast1 = new EnergyScimitarBlast((owner.position + this.owner.position) / 2f + new Vec2(0f, -16f), new Vec2(0f, -2000f));
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        EnergyScimitarBlast energyScimitarBlast2 = new EnergyScimitarBlast((owner.position + this.owner.position) / 2f + new Vec2(0f, 16f), new Vec2(0f, 2000f));
                                        if (Network.isActive)
                                            Send.Message(new NMEnergyScimitarBlast(energyScimitarBlast1.position, energyScimitarBlast1._target));
                                        Level.Add(energyScimitarBlast1);
                                        Level.Add(energyScimitarBlast2);
                                    }
                                }
                            }
                        }
                        for (int index = 0; index < 8; ++index)
                        {
                            Vec2 p1 = Lerp.Vec2Smooth(this.barrelStartPos, _lastBarrelStartPos, index / 7f);
                            Vec2 p2_3 = Lerp.Vec2Smooth(this.barrelPosition, _lastBarrelPos, index / 7f);
                            QuadLaserBullet pBullet = Level.CheckLine<QuadLaserBullet>(p1, p2_3);
                            if (pBullet != null)
                                QuadLaserHit(pBullet);
                            foreach (MaterialThing t in Level.CheckLineAll<MaterialThing>(p1, p2_3))
                            {
                                if (t != duck && t != this && t.owner != duck)
                                {
                                    switch (t)
                                    {
                                        case PhysicsObject _:
                                        case Icicles _:
                                            if (!(t is Duck) || (t as Duck).swordInvincibility <= 0)
                                            {
                                                if (!t.isServerForObject)
                                                    SuperFondle(t, DuckNetwork.localConnection);
                                                t.Destroy(new DTIncinerate(this));
                                                if (t is Duck && duck != null)
                                                {
                                                    duck._disarmDisable = 5;
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
                    _canAirFly = true;
                    ClearDrag();
                    if (!_didOwnerSwitchLogic)
                    {
                        _didOwnerSwitchLogic = true;
                        _timeSincePickedUp = 0f;
                        foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 16f))
                            physicsObject.sleeping = false;
                    }
                    float num2 = 24f + num1;
                    Vec2 vec2 = position + OffsetLocal(new Vec2(0f, 4f));
                    foreach (Blocker wall in _walls)
                    {
                        vec2 += OffsetLocal(new Vec2(0f, -num2 / _walls.Count));
                        wall.position = vec2;
                        float num3 = 1f - Math.Min(_stanceCounter / 0.25f, 1f);
                        wall.collisionSize = new Vec2((float)(6 + num3 * 8), 6f);
                        wall.collisionOffset = new Vec2((float)(-3 - num3 * 4), -3f);
                    }
                }
                else
                {
                    if (stuck)
                        _didOwnerSwitchLogic = false;
                    Vec2 vec2 = position + OffsetLocal(new Vec2(0f, _stuckInto != null ? -25f : -14f));
                    foreach (Blocker wall in _walls)
                    {
                        vec2 += OffsetLocal(new Vec2(0f, 18f / _walls.Count));
                        wall.position = vec2;
                        wall.solid = _stanceCounter < 0.15f;
                    }
                }
                _lastBarrelPos = barrelPosition;
                _lastBarrelStartPos = barrelStartPos;
            }
            else
                _didOwnerSwitchLogic = false;
            float num4 = Math.Min(_glow, 1f);
            float to1 = Math.Min(Math.Abs(_lastAngleHum - angle), 1f);
            _angleWhoom = Lerp.FloatSmooth(_angleWhoom, to1, 0.2f);
            _humAmount = Lerp.FloatSmooth(_humAmount, Math.Min((float)(Math.Min(Math.Abs(hSpeed) + Math.Abs(vSpeed), 5f) / 10f + to1 * 2f + 0.25f + num4 * 0.3f) * _glow, 0.75f), 0.2f);
            _humAmount = Math.Min(_humAmount + _dragRand * 0.2f, 1f);
            if (_hum != null)
                _hum.volume = _humAmount;
            if (level != null)
            {
                float val2_1 = 800f;
                float val2_2 = 400f;
                float num5 = (float)(1 - Math.Min(Math.Max((level.camera.position - position).length, val2_2) - val2_2, val2_1) / val2_1);
                if (_hum != null)
                    _hum.volume *= num5;
                if (isServerForObject && visible && (x < level.topLeft.x - 1000 || x > level.bottomRight.x + 1000) && owner == null && !inPipe)
                    Level.Remove(this);
            }
            _extraOffset = new Vec2(0f, -num1);
            _barrelOffsetTL = new Vec2(4f, 3f - num1);
            _lastAngleHum = angle;
            if (_glow > 1)
                _glow *= 0.85f;
            if (owner != null)
                _airFly = false;
            if (held || _airFly)
            {
                _stuckInto = null;
                _unchargeWait = !_longCharge ? 0.1f : 0.5f;
                _longCharge = false;
                if (!_playedChargeUp && owner != null)
                {
                    _playedChargeUp = true;
                    SFX.DontSave = 1;
                    SFX.Play("laserChargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                float to2;
                if (!_stanceReady || _airFly)
                {
                    to2 = 1f;
                    _glow = 1.5f;
                }
                else
                    to2 = 0f;
                if (_stance == Stance.Drag && duck != null)
                    _glow = Math.Abs(duck.hSpeed) > 1 ? 0.35f : 0f;
                _glow = Lerp.Float(_glow, to2, 0.1f);
            }
            else
            {
                _unchargeWait -= Maths.IncFrameTimer();
                if (_unchargeWait < 0)
                {
                    if (_playedChargeUp && owner == null)
                    {
                        _playedChargeUp = false;
                        SFX.DontSave = 1;
                        SFX.Play("laserUnchargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                    _glow = Lerp.Float(_glow, 0f, 0.2f);
                }
            }

            base.Update();
            _platform.solid = false;
            _platform.enablePhysics = false;
            _platform.position = new Vec2(-99999f, -99999f);
            if (!stuck)
                return;
            if (Math.Abs(barrelStartPos.y - barrelPosition.y) < 6)
            {
                _platform.solid = true;
                _platform.enablePhysics = true;
                _platform.position = Offset(new Vec2(0f, -10f));
            }
            center = new Vec2(6f, 29f);
        }

        public override void DrawGlow()
        {
            if (inPipe)
                return;
            _whiteGlow.angle = angle;
            _whiteGlow.color = this.swordColor;
            _whiteGlow.alpha = _glow * 0.5f;
            Graphics.Draw(ref _whiteGlow, x, y, depth - 2);
            Color swordColor = this.swordColor;
            foreach (WarpLine warpLine in warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                float num1 = Math.Min(warpLine.lerp, 0.5f) / 0.5f;
                float num2 = Math.Max((float)((warpLine.lerp - 0.5f) * 2), 0f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start, swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start - vec2_1 * (num1 * 1f), swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.13f;
            }
            warpLines.RemoveAll(v => v.lerp >= 1);
            base.DrawGlow();
        }

        public override void EditorUpdate()
        {
            _lerpedAngle = Maths.RadToDeg(_angle);
            base.EditorUpdate();
        }

        public override void Draw()
        {
            if (inPipe)
                return;
            base.Draw();
            if (DevConsole.showCollision)
            {
                foreach (Thing wall in _walls)
                    Graphics.DrawRect(wall.rectangle, Color.Red, this.depth + 10);
            }
            float num1 = Math.Min(_angleWhoom, 0.5f) * 1.5f;
            Graphics.material = _bladeMaterial;
            _bladeMaterial.glow = (float)(0.25 + _glow * 0.75);
            _blade.center = center;
            _bladeTrail.center = center;
            _blade.angle = graphic.angle;
            _blade.flipH = _swordFlip;
            _bladeTrail.flipH = _blade.flipH;
            _blade.alpha = alpha;
            _blade.color = Color.Lerp(properBladeColor, Color.Red, heat);
            swordColor = Color.Lerp(properColor, Color.Red, heat);
            if (_glow > 1f)
                _blade.scale = new Vec2((1f + (_glow - 1f) * 0.03f), 1f);
            else
                _blade.scale = new Vec2(1f);
            _bladeTrail.yscale = _blade.yscale + num1;
            Graphics.Draw(ref _blade, x, y, this.depth - 1);
            Graphics.material = null;
            Depth depth = this.depth;
            _bladeTrail.color = swordColor;
            graphic.color = Color.White;
            if (_glow > 0.5f)
            {
                float num2 = angle;
                float num3 = 1f;
                Vec2 vec2 = position;
                for (int idx = 0; idx < 7; ++idx)
                {
                    Vec2 current1 = Vec2.Zero;
                    float current2 = 0f;
                    for (int index1 = 0; index1 < 4 && _lastSize > idx; ++index1)
                    {
                        int index2 = historyIndex(idx);
                        if (index1 == 0)
                        {
                            current1 = vec2;
                            current2 = num2;
                        }
                        num2 = Lerp.FloatSmooth(current2, _lastAngles[index2], 0.25f * index1);
                        vec2 = Lerp.Vec2Smooth(current1, _lastPositions[index2], 0.25f * index1);
                        if (owner != null)
                            vec2 += owner.velocity * 0.5f;
                        _bladeTrail.angle = num2;
                        _bladeTrail.alpha = Math.Min(Math.Max((float)((_humAmount - 0.1f) * 4f), 0f), 1f) * 0.7f;
                        Graphics.Draw(ref _bladeTrail, vec2.x, vec2.y, this.depth - 2);
                    }
                    num3 -= 0.15f;
                }
            }
            addHistory(angle, position);
            if (_lastSize > 2)
            {
                int index3 = historyIndex(0);
                int index4 = historyIndex(2);
                addHistory((float)((_lastAngles[index3] + _lastAngles[index4]) / 2), (_lastPositions[index3] + _lastPositions[index4]) / 2f);
            }
            if (_lastSize <= 8)
                return;
            _lastSize = 8;
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
              : base(0f, 0f)
            {
                thickness = 100f;
                _editorCanModify = false;
                visible = false;
                _parent = pParent;
                weight = 0.01f;
            }

            public override bool Hit(Bullet bullet, Vec2 hitPos)
            {
                if (!_solid)
                    return false;
                if (_parent != null)
                    _parent.Shing();
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
                scimitar = pScimitar;
            }
        }

        private class RagdollDrag
        {
            public RagdollPart part;
            public Vec2 offset;
        }
    }
}
