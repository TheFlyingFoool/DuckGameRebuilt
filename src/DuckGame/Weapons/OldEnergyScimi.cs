// Decompiled with JetBrains decompiler
// Type: DuckGame.OldEnergyScimi
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            thickness = 0.01f;
            _impactThreshold = 0.5f;
            centerHeld = new Vec2(6f, 29f);
            centerUnheld = new Vec2(6f, 16f);
            _bladeMaterial = new MaterialEnergyBlade(this);
            additionalHoldOffset = new Vec2(0f, -3f);
            _swingSound = null;
            _enforceJabSwing = false;
            _allowJabMotion = false;
            _clashWithWalls = false;
            swordColor = properColor;
            _warpLine = new Sprite("warpLine2");
        }

        public override Vec2 barrelStartPos
        {
            get
            {
                if (_stuck)
                    return position - (Offset(barrelOffset) - position).normalized * -5f;
                if (owner == null)
                    return position - (Offset(barrelOffset) - position).normalized * 6f;
                return _slamStance ? position + (Offset(barrelOffset) - position).normalized * 12f : position + (Offset(barrelOffset) - position).normalized * 2f;
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
                _walls.Add(energyBlocker);
                Level.Add(energyBlocker);
            }
            _platform = new Platform(0f, 0f, 20f, 8f)
            {
                solid = false,
                enablePhysics = false,
                center = new Vec2(10f, 4f),
                collisionOffset = new Vec2(-10f, -2f),
                thickness = 0.01f
            };
            Level.Add(_platform);
            _hum = new ConstantSound("scimiHum")
            {
                volume = 0f,
                lerpSpeed = 1f
            };
            base.Initialize();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos) => false;

        public override void Terminate()
        {
            foreach (Thing wall in _walls)
                Level.Remove(wall);
            _walls.Clear();
            if (_platform != null)
                Level.Remove(_platform);
            base.Terminate();
        }

        public void Pulse()
        {
            if (_timeTillPulse >= 0.0)
                return;
            _timeTillPulse = 0.2f;
            SFX.Play("scimiSurge", 0.8f, Rando.Float(-0.2f, 0.2f));
            _glow = 12f;
        }

        public override void RestoreCollisionSize(bool pHeld = false)
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
                collisionOffset = new Vec2(-2f, -24f);
                collisionSize = new Vec2(4f, 28f);
                if (!_wasLifted)
                    return;
                collisionOffset = new Vec2(-4f, -2f);
                collisionSize = new Vec2(8f, 4f);
            }
        }

        public override DestroyType destroyType => _airFly ? new DTImpale(this) : new DTIncinerate(this);

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!_wasLifted || owner != null)
                return;
            if (with is Block || with is IPlatform && from == ImpactedFrom.Bottom && vSpeed > 0.0)
            {
                Shing();
                if (_framesSinceThrown <= 5)
                    return;
                _framesSinceThrown = 25;
            }
            else
            {
                if (!_airFly || !(with is RagdollPart) || _drag.FirstOrDefault<OldEnergyScimi.RagdollDrag>(x => x.part == with) != null)
                    return;
                RagdollPart ragdollPart = with as RagdollPart;
                if (ragdollPart.doll == null)
                    return;
                if (ragdollPart.doll.part1 != null)
                    _drag.Add(new OldEnergyScimi.RagdollDrag()
                    {
                        part = ragdollPart.doll.part1,
                        offset = position - ragdollPart.doll.part1.position
                    });
                if (ragdollPart.doll.part2 != null)
                    _drag.Add(new OldEnergyScimi.RagdollDrag()
                    {
                        part = ragdollPart.doll.part2,
                        offset = position - ragdollPart.doll.part2.position
                    });
                if (ragdollPart.doll.part3 == null)
                    return;
                _drag.Add(new OldEnergyScimi.RagdollDrag()
                {
                    part = ragdollPart.doll.part3,
                    offset = position - ragdollPart.doll.part3.position
                });
            }
        }

        protected override void PerformAirSpin()
        {
            if (!enablePhysics)
                return;
            if (_canAirFly && !_airFly && _framesSinceThrown < 15)
            {
                _upFlyTime = 0f;
                if (Math.Abs(hSpeed) > 2.0)
                {
                    if (Level.CheckLine<Block>(position + new Vec2(-16f, 0f), position + new Vec2(16f, 0f)) == null)
                    {
                        _airFly = true;
                        _airFlyDir = Math.Sign(hSpeed);
                    }
                    else
                        _canAirFly = false;
                    _airFlyVertical = false;
                }
                else if (Math.Abs(vSpeed) > 2.0)
                {
                    if (Level.CheckLine<Block>(position + new Vec2(0f, -16f), position + new Vec2(0f, 16f)) == null)
                    {
                        _airFly = true;
                        _airFlyDir = Math.Sign(vSpeed);
                    }
                    else
                        _canAirFly = false;
                    _airFlyVertical = true;
                }
            }
            if (_airFly)
            {
                hMax = 18f;
                if (_airFlyVertical)
                {
                    _upFlyTime += Maths.IncFrameTimer();
                    if (_upFlyTime > 2.0 && _airFlyDir < 0.0)
                        _airFlyDir = 1f;
                    if (_airFlyDir > 0.0)
                        _throwSpin = 90f;
                    else if (_airFlyDir < 0.0)
                        _throwSpin = 270f;
                    vSpeed = _airFlyDir * 18f;
                    hSpeed = 0f;
                }
                else
                {
                    if (_airFlyDir > 0.0)
                        _throwSpin = 0f;
                    else if (_airFlyDir < 0.0)
                        _throwSpin = 180f;
                    vSpeed = 0f;
                    hSpeed = _airFlyDir * 18f;
                }
                angleDegrees = 90f + _throwSpin;
                gravMultiplier = 0f;
            }
            else
            {
                hMax = 12f;
                vMax = 8f;
                base.PerformAirSpin();
            }
        }

        public override void Shing()
        {
            gravMultiplier = 1f;
            ClearDrag();
            Pulse();
            if (_airFly)
            {
                bool flag = false;
                if (_airFlyVertical)
                {
                    Block block = Level.CheckLine<Block>(position, position + new Vec2(0f, vSpeed));
                    if (block != null)
                    {
                        flag = true;
                        clip.Add(block);
                        if (vSpeed > 0.0)
                        {
                            y = block.top - 18f;
                            _throwSpin = 90f;
                        }
                        else
                        {
                            y = block.bottom + 18f;
                            _throwSpin = 270f;
                        }
                    }
                }
                else
                {
                    Block block = Level.CheckLine<Block>(position, position + new Vec2(hSpeed, 0f));
                    if (block != null)
                    {
                        flag = true;
                        clip.Add(block);
                        if (hSpeed > 0.0)
                        {
                            x = block.left - 18f;
                            _throwSpin = 0f;
                        }
                        else
                        {
                            x = block.right + 18f;
                            _throwSpin = 180f;
                        }
                    }
                }
                if (flag)
                {
                    _longCharge = true;
                    _stuck = true;
                    enablePhysics = false;
                    hSpeed = 0f;
                    vSpeed = 0f;
                }
            }
            _airFly = false;
            base.Shing();
        }

        protected override void QuadLaserHit(QuadLaserBullet pBullet)
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

        protected override void UpdateCrouchStance()
        {
            if (!_crouchStance)
            {
                _hold = -0.3f;
                handOffset = new Vec2(_addOffsetX + 4f, _addOffsetY);
                _holdOffset = new Vec2(2f + _addOffsetX, 6f + _addOffsetY) + additionalHoldOffset;
            }
            else if (duck != null && duck.sliding)
            {
                _hold = 2.24159f;
                if (handFlip)
                    _holdOffset = new Vec2(_addOffsetX - 4f, _addOffsetY - 3f) + additionalHoldOffset;
                else
                    _holdOffset = new Vec2(2f + _addOffsetX, _addOffsetY - 3f) + additionalHoldOffset;
                handOffset = new Vec2(3f + _addOffsetX, _addOffsetY - 6f);
            }
            else
            {
                _hold = 2.54159f;
                if (handFlip)
                    _holdOffset = new Vec2(_addOffsetX - 4f, _addOffsetY - 7f) + additionalHoldOffset;
                else
                    _holdOffset = new Vec2(2f + _addOffsetX, _addOffsetY - 7f) + additionalHoldOffset;
                handOffset = new Vec2(3f + _addOffsetX, _addOffsetY - 10f);
            }
        }

        protected override void UpdateJabPullback()
        {
            handFlip = true;
            if (duck != null && duck.sliding)
                _swing = MathHelper.Lerp(_swing, -4.2f, 0.36f);
            else
                _swing = MathHelper.Lerp(_swing, -4.8f, 0.36f);
            _addOffsetX = MathHelper.Lerp(_addOffsetX, -2f, 0.45f);
            if (_addOffsetX >= -12.0)
                return;
            _addOffsetX = -12f;
        }

        protected override void UpdateSlamPullback()
        {
            _swing = MathHelper.Lerp(_swing, 0.8f, 0.8f);
            _addOffsetX = MathHelper.Lerp(_addOffsetX, -5f, 0.45f);
            if (_addOffsetX < -4.6f)
                _addOffsetX = -5f;
            _addOffsetY = MathHelper.Lerp(_addOffsetY, 6f, 0.35f);
            if (_addOffsetX >= -5.5)
                return;
            _addOffsetY = -6f;
        }

        public float angleWhoom => _angleWhoom;

        public void ClearDrag()
        {
            int num = 1;
            foreach (OldEnergyScimi.RagdollDrag ragdollDrag in _drag)
            {
                if (ragdollDrag.part.doll != null && ragdollDrag.part.doll.captureDuck != null && ragdollDrag.part.doll.captureDuck._cooked == null)
                {
                    if (!_airFlyVertical)
                        ragdollDrag.part.position = Offset(new Vec2(-10f, 10f));
                    else if (_airFlyDir < 0.0)
                        ragdollDrag.part.position = Offset(new Vec2(0f, 0f));
                    else
                        ragdollDrag.part.position = Offset(new Vec2(0f, 20f));
                    ragdollDrag.part.doll.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.position = ragdollDrag.part.position;
                    ragdollDrag.part.doll.captureDuck.OnKill(new DTIncinerate(ragdollDrag.part.doll.captureDuck));
                    if (ragdollDrag.part.doll.captureDuck._cooked != null)
                        ragdollDrag.part.doll.captureDuck._cooked.vSpeed = -(2 + num);
                    ++num;
                }
            }
            _drag.Clear();
        }

        public override void Thrown()
        {
            if (isServerForObject && duck != null)
            {
                if (duck.inputProfile.Down("DOWN"))
                {
                    x = duck.x;
                    _thrownDown = true;
                    if (!duck.grounded)
                        duck.vSpeed -= 8f;
                }
                else if (duck.inputProfile.Down("UP"))
                {
                    x = duck.x;
                    _thrownUp = true;
                }
            }
            base.Thrown();
        }

        protected override void OnSwing()
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

        protected override void ResetTrailHistory()
        {
            _lastAngleHum = angle;
            base.ResetTrailHistory();
        }

        public override void Update()
        {
            float num1 = Math.Min(_angleWhoom, 0.5f) * 38f;
            if (isServerForObject)
            {
                _stickWait -= Maths.IncFrameTimer();
                if (_thrownDown)
                {
                    vSpeed = 4f;
                    y += 10f;
                    _thrownDown = false;
                }
                if (_thrownUp)
                {
                    vSpeed = -4f;
                    y -= 6f;
                    _thrownUp = false;
                }
                if (duck != null && slowWait > 0.0)
                {
                    slowWait -= Maths.IncFrameTimer();
                    if (slowWait <= 0.0)
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
                foreach (OldEnergyScimi.RagdollDrag ragdollDrag in _drag)
                {
                    ragdollDrag.part.position = position - ragdollDrag.offset;
                    ragdollDrag.part.hSpeed = 0f;
                    ragdollDrag.part.vSpeed = 0f;
                }
                _timeSincePickedUp += Maths.IncFrameTimer();
                if (grounded)
                    _canAirFly = true;
                _timeTillPulse -= Maths.IncFrameTimer();
                if (owner != null)
                {
                    _canAirFly = true;
                    ClearDrag();
                    if (prevOwner == null && !_didOwnerSwitchLogic)
                    {
                        _didOwnerSwitchLogic = true;
                        _timeSincePickedUp = 0f;
                        foreach (PhysicsObject physicsObject in Level.CheckCircleAll<PhysicsObject>(position, 16f))
                            physicsObject.sleeping = false;
                    }
                    float num2 = 20f + num1;
                    Vec2 vec2 = position + OffsetLocal(new Vec2(0f, -2f));
                    foreach (EnergyBlocker wall in _walls)
                    {
                        vec2 += OffsetLocal(new Vec2(0f, -num2 / _walls.Count));
                        wall.position = vec2;
                        wall.solid = _glow > 0.5;
                    }
                }
                else
                {
                    _didOwnerSwitchLogic = false;
                    Vec2 vec2 = position + OffsetLocal(new Vec2(0f, _stuck ? -25f : -14f));
                    foreach (EnergyBlocker wall in _walls)
                    {
                        vec2 += OffsetLocal(new Vec2(0f, 18f / _walls.Count));
                        wall.position = vec2;
                        wall.solid = _glow > 0.5;
                    }
                }
                if (duck != null && _timeSincePickedUp > 0.04f && held && _swinging && Level.CheckLine<Block>(position, position + new Vec2(offDir * 16, 0f)) != null)
                {
                    duck.Swear();
                    double angle = this.angle;
                    duck.ThrowItem();
                    _airFly = true;
                    _airFlyDir = offDir;
                    hSpeed = offDir * 16;
                    Shing();
                    angleDegrees = _throwSpin + 90f;
                    ResetTrailHistory();
                }
            }
            float num3 = Math.Min(_glow, 1f);
            float to1 = Math.Min(Math.Abs(_lastAngleHum - angle), 1f);
            _angleWhoom = Lerp.FloatSmooth(_angleWhoom, to1, 0.2f);
            _hum.volume = Lerp.FloatSmooth(_hum.volume, Math.Min((float)(Math.Min(Math.Abs(hSpeed) + Math.Abs(vSpeed), 5f) / 10.0 + to1 * 2.0f + 0.15f + num3 * 0.1f) * _glow, 0.75f), 0.2f);
            if (level != null)
            {
                float val2_1 = 800f;
                float val2_2 = 400f;
                _hum.volume *= (float)(1.0 - Math.Min(Math.Max((level.camera.position - position).length, val2_2) - val2_2, val2_1) / val2_1);
                if (isServerForObject && (x < level.topLeft.x - 1000.0 || x > level.bottomRight.x + 1000.0))
                    Level.Remove(this);
            }
            _extraOffset = new Vec2(0f, -num1);
            _barrelOffsetTL = new Vec2(4f, 3f - num1);
            _lastAngleHum = angle;
            if (_glow > 1.0)
                _glow *= 0.85f;
            if (held || _airFly)
            {
                _stuck = false;
                _unchargeWait = !_longCharge ? 0.1f : 0.5f;
                _longCharge = false;
                if (!_playedChargeUp && owner != null)
                {
                    _playedChargeUp = true;
                    SFX.Play("laserChargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                }
                float to2 = 1f;
                if (duck != null && !_swinging && !_crouchStance)
                    to2 = 0f;
                _glow = Lerp.Float(_glow, to2, 0.1f);
            }
            else
            {
                _unchargeWait -= Maths.IncFrameTimer();
                if (_unchargeWait < 0.0)
                {
                    if (_playedChargeUp && owner == null)
                    {
                        _playedChargeUp = false;
                        SFX.Play("laserUnchargeShort", pitch: Rando.Float(-0.1f, 0.1f));
                    }
                    _glow = Lerp.Float(_glow, 0f, 0.2f);
                }
            }
            if (_glow > 0.1f)
            {
                _stayVolatile = true;
                _volatile = true;
                heat += 3f / 500f;
            }
            else
            {
                _stayVolatile = false;
                _volatile = false;
                if (heat > 0.0)
                    heat -= 0.01f;
            }
            base.Update();
            _platform.solid = false;
            _platform.enablePhysics = false;
            _platform.position = new Vec2(-99999f, -99999f);
            if (!_stuck)
                return;
            if (Math.Abs(barrelStartPos.y - barrelPosition.y) < 6.0)
            {
                _platform.solid = true;
                _platform.enablePhysics = true;
                _platform.position = Offset(new Vec2(0f, -10f));
            }
            center = new Vec2(6f, 29f);
        }

        public override void DrawGlow()
        {
            _whiteGlow.angle = angle;
            _whiteGlow.color = this.swordColor;
            _whiteGlow.alpha = _glow * 0.5f;
            Graphics.Draw(_whiteGlow, x, y, depth - 2);
            Color swordColor = this.swordColor;
            foreach (WarpLine warpLine in warpLines)
            {
                Vec2 vec2_1 = warpLine.start - warpLine.end;
                Vec2 vec2_2 = warpLine.end - warpLine.start;
                float num1 = Math.Min(warpLine.lerp, 0.5f) / 0.5f;
                float num2 = Math.Max((float)((warpLine.lerp - 0.5) * 2.0), 0f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start, swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                Graphics.DrawTexturedLine(_warpLine.texture, warpLine.start - vec2_1 * (num1 * 0.5f), warpLine.start - vec2_1 * (num1 * 1f), swordColor * (1f - num2), warpLine.wide / 32f, (Depth)0.9f);
                warpLine.lerp += 0.13f;
            }
            warpLines.RemoveAll(v => v.lerp >= 1.0);
            base.DrawGlow();
        }

        public override void Draw()
        {
            base.Draw();
            Sword._playedShing = true;
            int num1 = DevConsole.showCollision ? 1 : 0;
            float num2 = Math.Min(_angleWhoom, 0.5f) * 1.5f;
            Graphics.material = _bladeMaterial;
            _bladeMaterial.glow = (float)(0.25 + _glow * 0.75);
            _blade.center = center;
            _bladeTrail.center = center;
            _blade.angle = graphic.angle;
            _blade.flipH = graphic.flipH;
            _bladeTrail.flipH = _blade.flipH;
            _blade.color = Color.Lerp(Color.White, Color.Red, heat);
            swordColor = Color.Lerp(properColor, Color.Red, heat);
            if (_glow > 1.0)
                _blade.scale = new Vec2(1f + (_glow - 1f) * 0.03f, 1f);
            else
                _blade.scale = new Vec2(1f);
            _bladeTrail.yscale = _blade.yscale + num2;
            Graphics.Draw(_blade, x, y, this.depth - 1);
            Graphics.material = null;
            alpha = 1f;
            Depth depth = this.depth;
            _bladeTrail.color = swordColor;
            graphic.color = Color.White;
            if (_glow <= 0.5)
                return;
            float num3 = angle;
            float num4 = 1f;
            Vec2 vec2 = position;
            for (int idx = 0; idx < 8; ++idx)
            {
                Vec2 current1 = Vec2.Zero;
                float current2 = 0f;
                for (int index1 = 0; index1 < 4 && _lastSize > idx; ++index1)
                {
                    int index2 = historyIndex(idx);
                    if (index1 == 0)
                    {
                        current1 = vec2;
                        current2 = num3;
                    }
                    num3 = Lerp.FloatSmooth(current2, _lastAngles[index2], 0.25f * index1);
                    vec2 = Lerp.Vec2Smooth(current1, _lastPositions[index2], 0.25f * index1);
                    if (owner != null)
                        vec2 += owner.velocity * 0.5f;
                    _bladeTrail.angle = num3;
                    _bladeTrail.alpha = Math.Min(Math.Max((float)((_hum.volume - 0.1f) * 4.0), 0f), 1f) * 0.7f;
                    Graphics.Draw(_bladeTrail, vec2.x, vec2.y, this.depth - 2);
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
