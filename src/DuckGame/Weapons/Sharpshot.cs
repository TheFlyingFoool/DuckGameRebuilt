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
            this.ammo = 6;
            this._ammoType = new ATHighCalSniper();
            this._type = "gun";
            this.graphic = new Sprite("highPowerRifle");
            this.center = new Vec2(16f, 7f);
            this.collisionOffset = new Vec2(-8f, -5f);
            this.collisionSize = new Vec2(16f, 9f);
            this._holdOffset = new Vec2(3f, 0f);
            this._barrelOffsetTL = new Vec2(35f, 5f);
            this._fireSound = "sniper";
            this._fireSoundPitch = -0.2f;
            this._kickForce = 8f;
            this._fireRumble = RumbleIntensity.Light;
            this.laserSight = true;
            this._laserOffsetTL = new Vec2(35f, 5f);
            this._manualLoad = true;
            this.editorTooltip = "Like a sniper rifle, but even cooler.";
        }

        public override void Update()
        {
            base.Update();
            if (this._loadState > -1)
            {
                if (this.owner == null)
                {
                    if (this._loadState == 3)
                        this.loaded = true;
                    this._loadState = -1;
                    this._angleOffset = 0f;
                    this.handOffset = Vec2.Zero;
                }
                if (this._loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            NetSoundEffect.Play("sniperLoad");
                    }
                    else
                        SFX.Play("loadSniper");
                    ++this._loadState;
                }
                else if (this._loadState == 1)
                {
                    if (_angleOffset < 0.159999996423721)
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.25f, 0.25f);
                    else
                        ++this._loadState;
                }
                else if (this._loadState == 2)
                {
                    this.handOffset.x += 0.8f;
                    if (handOffset.x > 4.0)
                    {
                        ++this._loadState;
                        this.Reload();
                        this.loaded = false;
                    }
                }
                else if (this._loadState == 3)
                {
                    this.handOffset.x -= 0.8f;
                    if (handOffset.x <= 0.0)
                    {
                        ++this._loadState;
                        this.handOffset.x = 0f;
                    }
                }
                else if (this._loadState == 4)
                {
                    if (_angleOffset > 0.0399999991059303)
                    {
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0f, 0.25f);
                    }
                    else
                    {
                        this._loadState = -1;
                        this.loaded = true;
                        this._angleOffset = 0f;
                    }
                }
            }
            if (this.loaded && this.owner != null && this._loadState == -1)
                this.laserSight = true;
            else
                this.laserSight = false;
        }

        public override void ApplyKick()
        {
            base.ApplyKick();
            if (this.duck == null || !this.duck._hovering)
                return;
            this.duck.vSpeed *= 0.5f;
        }

        public override void OnPressAction()
        {
            if (this.loaded)
            {
                base.OnPressAction();
            }
            else
            {
                if (this.ammo <= 0 || this._loadState != -1)
                    return;
                this._loadState = 0;
                this._loadAnimation = 0;
            }
        }

        public override void Draw()
        {
            float angle = this.angle;
            if (this.offDir > 0)
                this.angle -= this._angleOffset;
            else
                this.angle += this._angleOffset;
            base.Draw();
            this.angle = angle;
        }
    }
}
