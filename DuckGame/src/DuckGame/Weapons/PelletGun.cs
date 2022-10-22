// Decompiled with JetBrains decompiler
// Type: DuckGame.PelletGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Rifles")]
    public class PelletGun : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public StateBinding _firesTillFailBinding = new StateBinding(nameof(firesTillFail));
        public StateBinding _aimAngleBinding = new StateBinding(nameof(_aimAngle));
        private SpriteMap _sprite;
        private Sprite _spring;
        public int _loadState = -1;
        public float _angleOffset;
        public float _angleOffset2;
        public float _aimAngle;
        public int _aimWait;
        private Vec2 _posOffset;
        public int firesTillFail = 8;
        private Vec2 springPos = Vec2.Zero;
        private Vec2 springVel = Vec2.Zero;
        private bool _rising;

        public override float angle
        {
            get => base.angle - Math.Max(_aimAngle, -0.2f) * offDir;
            set => base.angle = value;
        }

        public PelletGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 2;
            _ammoType = new ATPellet();
            _type = "gun";
            _sprite = new SpriteMap("pelletGun", 31, 7);
            graphic = _sprite;
            center = new Vec2(15f, 2f);
            collisionOffset = new Vec2(-8f, -2f);
            collisionSize = new Vec2(16f, 5f);
            _spring = new Sprite("dandiSpring");
            _barrelOffsetTL = new Vec2(30f, 2f);
            _fireSound = "pelletgun";
            _kickForce = 0f;
            _manualLoad = true;
            _holdOffset = new Vec2(-2f, -1f);
            editorTooltip = "Careful with that thing, you'll lose an eye!";
            _editorName = "Dandylion";
        }

        public override Vec2 Offset(Vec2 pos) => position + _posOffset + OffsetLocal(pos);

        public override void Update()
        {
            if ((bool)infinite)
                firesTillFail = 100;
            if (firesTillFail == 1)
                _fireSound = "pelletgunFail";
            if (firesTillFail <= 0)
            {
                _fireSound = "pelletgunBad";
                if (!(ammoType is ATFailedPellet))
                    _ammoType = new ATFailedPellet();
                springVel += (Offset(new Vec2(0f, -8f)) - springPos) * 0.15f;
                springVel *= 0.9f;
                springPos += springVel;
            }
            else
                springPos = position;
            ++_aimWait;
            if (_aimWait > 0)
            {
                _aimAngle = Lerp.Float(_aimAngle, _rising ? 0.4f : 0f, 0.05f);
                _aimWait = 0;
            }
            if (_rising && _aimAngle > 0.345f)
                OnReleaseAction();
            if (held)
                center = new Vec2(11f, 2f);
            else
                center = new Vec2(15f, 2f);
            if (_loadState > -1)
            {
                if (owner == null)
                {
                    if (_loadState == 3)
                        loaded = true;
                    _loadState = -1;
                    _angleOffset = 0f;
                    _posOffset = Vec2.Zero;
                    handOffset = Vec2.Zero;
                    _aimAngle = 0f;
                    _angleOffset2 = 0f;
                }
                _posOffset = _loadState <= 0 || _loadState >= 4 ? Lerp.Vec2(_posOffset, new Vec2(0f, 0f), 0.24f) : Lerp.Vec2(_posOffset, new Vec2(2f, 2f), 0.2f);
                _angleOffset2 = _loadState < 2 || _loadState >= 3 ? Lerp.Float(_angleOffset2, 0f, 0.02f) : Lerp.Float(_angleOffset2, -0.17f, 0.04f);
                if (_loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            NetSoundEffect.Play("pelletGunSwipe");
                    }
                    else
                        SFX.Play("swipe", 0.4f, 0.3f);
                    ++_loadState;
                }
                else if (_loadState == 1)
                {
                    if (_angleOffset < 0.16f)
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0.2f, 0.11f);
                    else
                        ++_loadState;
                }
                else if (_loadState == 2)
                {
                    handOffset.x += 0.31f;
                    if (handOffset.x > 4.0)
                    {
                        ++_loadState;
                        ammo = 2;
                        loaded = false;
                        if (Network.isActive)
                        {
                            if (isServerForObject)
                                NetSoundEffect.Play("pelletGunLoad");
                        }
                        else
                            SFX.Play("loadLow", 0.7f, Rando.Float(-0.05f, 0.05f));
                    }
                }
                else if (_loadState == 3)
                {
                    handOffset.x -= 0.2f;
                    if (handOffset.x <= 0.0)
                    {
                        ++_loadState;
                        handOffset.x = 0f;
                        if (Network.isActive)
                        {
                            if (isServerForObject)
                                NetSoundEffect.Play("pelletGunSwipe2");
                        }
                        else
                            SFX.Play("swipe", 0.5f, 0.4f);
                    }
                }
                else if (_loadState == 4)
                {
                    if (_angleOffset > 0.03f)
                    {
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.09f);
                    }
                    else
                    {
                        _loadState = -1;
                        loaded = true;
                        _angleOffset = 0f;
                        if (Network.isActive)
                        {
                            if (isServerForObject)
                                NetSoundEffect.Play("pelletGunClick");
                        }
                        else
                            SFX.Play("click", pitch: 0.5f);
                    }
                }
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (isServerForObject)
            {
                if (loaded && ammo > 1)
                {
                    _rising = true;
                    _aimAngle = -0.3f;
                    _aimWait = 0;
                }
                else
                {
                    if (_loadState != -1)
                        return;
                    _loadState = 0;
                }
            }
            else
                base.OnPressAction();
        }

        private void RunFireCode()
        {
            base.OnPressAction();
            if (DGRSettings.S_ParticleMultiplier != 0)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    Level.Add(SmallSmoke.New(barrelPosition.x + offDir * 4f, barrelPosition.y));
                Level.Add(SmallSmoke.New(position.x, position.y));
            }
        }

        public override void OnReleaseAction()
        {
            if (receivingPress)
            {
                RunFireCode();
            }
            else
            {
                if (!_rising)
                    return;
                if (loaded && ammo > 1)
                {
                    RunFireCode();
                    --firesTillFail;
                    ammo = 1;
                }
                _rising = false;
            }
        }

        public override void Draw()
        {
            _sprite.center = center;
            _sprite.depth = depth;
            _sprite.angle = angle;
            _sprite.frame = 0;
            _sprite.alpha = alpha;
            if (owner != null && owner.graphic != null && (duck == null || !(duck.holdObject is TapedGun)))
                _sprite.flipH = owner.graphic.flipH;
            else
                _sprite.flipH = offDir <= 0;
            if (offDir > 0)
                _sprite.angle = angle - _angleOffset - _angleOffset2;
            else
                _sprite.angle = angle + _angleOffset + _angleOffset2;
            Vec2 vec2 = Offset(_posOffset);
            Graphics.Draw(_sprite, vec2.x, vec2.y);
            _sprite.frame = 1;
            if (offDir > 0)
                _sprite.angle = angle + _angleOffset * 3f - _angleOffset2;
            else
                _sprite.angle = angle - _angleOffset * 3f + _angleOffset2;
            Graphics.Draw(_sprite, vec2.x, vec2.y);
            if (firesTillFail > 0)
                return;
            _spring.depth = depth - 5;
            _spring.center = new Vec2(4f, 7f);
            _spring.angleDegrees = Maths.PointDirection(position + _posOffset, springPos) - 90f;
            _spring.yscale = (float)((position.y + _posOffset.y - springPos.y) / 8.0);
            _spring.flipH = offDir < 0;
            if (_spring.yscale > 1.2f)
                _spring.yscale = 1.2f;
            if (_spring.yscale < -1.2f)
                _spring.yscale = -1.2f;
            _spring.alpha = alpha;
            Graphics.Draw(_spring, vec2.x, vec2.y);
        }
    }
}
