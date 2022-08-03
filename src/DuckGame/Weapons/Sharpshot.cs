// Decompiled with JetBrains decompiler
// Type: DuckGame.Sharpshot
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Rifles")]
    public class Sharpshot : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        public int _loadState = -1;
        public int _loadAnimation = -1;
        public float _angleOffset;

        public Sharpshot(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 6;
            _ammoType = new ATHighCalSniper();
            _type = "gun";
            graphic = new Sprite("highPowerRifle");
            center = new Vec2(16f, 7f);
            collisionOffset = new Vec2(-8f, -5f);
            collisionSize = new Vec2(16f, 9f);
            _holdOffset = new Vec2(3f, 0f);
            _barrelOffsetTL = new Vec2(35f, 5f);
            _fireSound = "sniper";
            _fireSoundPitch = -0.2f;
            _kickForce = 8f;
            _fireRumble = RumbleIntensity.Light;
            laserSight = true;
            _laserOffsetTL = new Vec2(35f, 5f);
            _manualLoad = true;
            editorTooltip = "Like a sniper rifle, but even cooler.";
        }

        public override void Update()
        {
            base.Update();
            if (_loadState > -1)
            {
                if (owner == null)
                {
                    if (_loadState == 3)
                        loaded = true;
                    _loadState = -1;
                    _angleOffset = 0f;
                    handOffset = Vec2.Zero;
                }
                if (_loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (isServerForObject)
                            NetSoundEffect.Play("sniperLoad");
                    }
                    else
                        SFX.Play("loadSniper");
                    ++_loadState;
                }
                else if (_loadState == 1)
                {
                    if (_angleOffset < 0.159999996423721)
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0.25f, 0.25f);
                    else
                        ++_loadState;
                }
                else if (_loadState == 2)
                {
                    handOffset.x += 0.8f;
                    if (handOffset.x > 4.0)
                    {
                        ++_loadState;
                        Reload();
                        loaded = false;
                    }
                }
                else if (_loadState == 3)
                {
                    handOffset.x -= 0.8f;
                    if (handOffset.x <= 0.0)
                    {
                        ++_loadState;
                        handOffset.x = 0f;
                    }
                }
                else if (_loadState == 4)
                {
                    if (_angleOffset > 0.0399999991059303)
                    {
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.25f);
                    }
                    else
                    {
                        _loadState = -1;
                        loaded = true;
                        _angleOffset = 0f;
                    }
                }
            }
            if (loaded && owner != null && _loadState == -1)
                laserSight = true;
            else
                laserSight = false;
        }

        public override void ApplyKick()
        {
            base.ApplyKick();
            if (duck == null || !duck._hovering)
                return;
            duck.vSpeed *= 0.5f;
        }

        public override void OnPressAction()
        {
            if (loaded)
            {
                base.OnPressAction();
            }
            else
            {
                if (ammo <= 0 || _loadState != -1)
                    return;
                _loadState = 0;
                _loadAnimation = 0;
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if (offDir > 0)
                this.angle -= _angleOffset;
            else
                this.angle += _angleOffset;
            base.Draw();
            this.angle = angle;
        }
    }
}
