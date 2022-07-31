// Decompiled with JetBrains decompiler
// Type: DuckGame.OldPistol
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    public class OldPistol : Gun
    {
        public StateBinding _loadStateBinding = new StateBinding(nameof(_loadState));
        public int _loadState = -1;
        public float _angleOffset;
        private SpriteMap _sprite;

        public OldPistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = new ATOldPistol();
            this._type = "gun";
            this._sprite = new SpriteMap("oldPistol", 32, 32);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 17f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(24f, 16f);
            this._fireSound = "shotgun";
            this._kickForce = 2f;
            this._fireRumble = RumbleIntensity.Kick;
            this._manualLoad = true;
            this._holdOffset = new Vec2(2f, 0.0f);
            this.editorTooltip = "A pain in the tailfeathers to reload, but it'll get the job done.";
        }

        public override void Update()
        {
            base.Update();
            this._sprite.frame = this.ammo <= 1 ? 1 : 0;
            if (this.infinite.value)
            {
                this.UpdateLoadState();
                this.UpdateLoadState();
            }
            else
                this.UpdateLoadState();
        }

        private void UpdateLoadState()
        {
            if (this._loadState <= -1)
                return;
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
                        NetSoundEffect.Play("oldPistolSwipe");
                }
                else
                    SFX.Play("swipe", 0.6f, -0.3f);
                ++this._loadState;
            }
            else if (this._loadState == 1)
            {
                if (_angleOffset < 0.16f)
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.2f, 0.08f);
                else
                    ++this._loadState;
            }
            else if (this._loadState == 2)
            {
                this.handOffset.y -= 0.28f;
                if (handOffset.y >= -4f)
                    return;
                ++this._loadState;
                this.ammo = 2;
                this.loaded = false;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolLoad");
                }
                else
                    SFX.Play("shotgunLoad");
            }
            else if (this._loadState == 3)
            {
                this.handOffset.y += 0.15f;
                if (handOffset.y < 0.0)
                    return;
                ++this._loadState;
                this.handOffset.y = 0f;
                if (Network.isActive)
                {
                    if (!this.isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolSwipe2");
                }
                else
                    SFX.Play("swipe", 0.7f);
            }
            else
            {
                if (this._loadState != 4)
                    return;
                if (_angleOffset > 0.04f)
                {
                    this._angleOffset = MathHelper.Lerp(this._angleOffset, 0.0f, 0.08f);
                }
                else
                {
                    this._loadState = -1;
                    this.loaded = true;
                    this._angleOffset = 0.0f;
                    if (this.isServerForObject && this.duck != null && this.duck.profile != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                    if (Network.isActive)
                    {
                        if (!this.isServerForObject)
                            return;
                        SFX.PlaySynchronized("click", 1f, 0.5f, 0.0f, false, true);
                    }
                    else
                        SFX.Play("click", pitch: 0.5f);
                }
            }
        }

        public override void OnPressAction()
        {
            if (this.loaded && this.ammo > 1)
            {
                base.OnPressAction();
                for (int index = 0; index < 4; ++index)
                    Level.Add(Spark.New(this.offDir > 0 ? this.x - 9f : this.x + 9f, this.y - 6f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.05f));
                for (int index = 0; index < 4; ++index)
                    Level.Add(SmallSmoke.New(this.barrelPosition.x + offDir * 4f, this.barrelPosition.y));
                this.ammo = 1;
            }
            else
            {
                if (this._loadState != -1)
                    return;
                this._loadState = 0;
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
