// Decompiled with JetBrains decompiler
// Type: DuckGame.Sniper
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 3;
            this._ammoType = new ATSniper();
            this._type = "gun";
            this.graphic = new Sprite("sniper");
            this.center = new Vec2(16f, 4f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 3f);
            this._fireSound = "sniper";
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Light;
            this.laserSight = true;
            this._laserOffsetTL = new Vec2(32f, 3.5f);
            this._manualLoad = true;
            this.editorTooltip = "Long range rifle - equipped with a laser sight so you look real cool.";
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
                    this._angleOffset = 0.0f;
                    this.handOffset = Vec2.Zero;
                }
                if (this._loadState == 0)
                {
                    if (Network.isActive)
                    {
                        if (this.isServerForObject)
                            this._netLoad.Play();
                    }
                    else
                        SFX.Play("loadSniper");
                    ++this._loadState;
                }
                else if (this._loadState == 1)
                {
                    if (_angleOffset < 0.159999996423721)
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.15f);
                    else
                        ++this._loadState;
                }
                else if (this._loadState == 2)
                {
                    this.handOffset.x += 0.4f;
                    if (handOffset.x > 4.0)
                    {
                        ++this._loadState;
                        this.Reload();
                        this.loaded = false;
                    }
                }
                else if (this._loadState == 3)
                {
                    this.handOffset.x -= 0.4f;
                    if (handOffset.x <= 0.0)
                    {
                        ++this._loadState;
                        this.handOffset.x = 0.0f;
                    }
                }
                else if (this._loadState == 4)
                {
                    if (_angleOffset > 0.0399999991059303)
                    {
                        this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.15f);
                    }
                    else
                    {
                        this._loadState = -1;
                        this.loaded = true;
                        this._angleOffset = 0.0f;
                    }
                }
            }
            if (this.loaded && this.owner != null && this._loadState == -1)
                this.laserSight = true;
            else
                this.laserSight = false;
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
