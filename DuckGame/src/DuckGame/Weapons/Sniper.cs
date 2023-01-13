// Decompiled with JetBrains decompiler
// Type: DuckGame.Sniper
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Rifles")]
    public class Sniper : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        public StateBinding _netLoadBinding = new NetSoundBinding(nameof(_netLoad));
        public NetSoundEffect _netLoad = new NetSoundEffect(new string[1]
        {
            "loadSniper"
        });
        public int _loadState = -1;
        public int _loadAnimation = -1;
        public float _angleOffset;

        public Sniper(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 3;
            _ammoType = new ATSniper();
            _type = "gun";
            graphic = new Sprite("sniper");
            center = new Vec2(16f, 4f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(30f, 3f);
            _fireSound = "sniper";
            _kickForce = 2f;
            _fireRumble = RumbleIntensity.Light;
            laserSight = true;
            _laserOffsetTL = new Vec2(32f, 3.5f);
            _manualLoad = true;
            editorTooltip = "Long range rifle - equipped with a laser sight so you look real cool.";
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
                            _netLoad.Play();
                    }
                    else
                        SFX.Play("loadSniper");
                    ++_loadState;
                }
                else if (_loadState == 1)
                {
                    if (_angleOffset < 0.16f)
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0.2f, 0.15f);
                    else
                        ++_loadState;
                }
                else if (_loadState == 2)
                {
                    handOffset.x += 0.4f;
                    if (handOffset.x > 4.0)
                    {
                        ++_loadState;
                        Reload();
                        loaded = false;
                    }
                }
                else if (_loadState == 3)
                {
                    handOffset.x -= 0.4f;
                    if (handOffset.x <= 0.0)
                    {
                        ++_loadState;
                        handOffset.x = 0f;
                    }
                }
                else if (_loadState == 4)
                {
                    if (_angleOffset > 0.04f)
                    {
                        _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.15f);
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
