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
            ammo = 2;
            _ammoType = new ATOldPistol();
            _type = "gun";
            _sprite = new SpriteMap("oldPistol", 32, 32);
            graphic = _sprite;
            center = new Vec2(16f, 17f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(24f, 16f);
            _fireSound = "shotgun";
            _kickForce = 2f;
            _fireRumble = RumbleIntensity.Kick;
            _manualLoad = true;
            _holdOffset = new Vec2(2f, 0f);
            editorTooltip = "A pain in the tailfeathers to reload, but it'll get the job done.";
        }

        public override void Update()
        {
            base.Update();
            _sprite.frame = ammo <= 1 ? 1 : 0;
            if (infinite.value)
            {
                UpdateLoadState();
                UpdateLoadState();
            }
            else
                UpdateLoadState();
        }

        private void UpdateLoadState()
        {
            if (_loadState <= -1)
                return;
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
                        NetSoundEffect.Play("oldPistolSwipe");
                }
                else
                    SFX.Play("swipe", 0.6f, -0.3f);
                ++_loadState;
            }
            else if (_loadState == 1)
            {
                if (_angleOffset < 0.16f)
                    _angleOffset = MathHelper.Lerp(_angleOffset, 0.2f, 0.08f);
                else
                    ++_loadState;
            }
            else if (_loadState == 2)
            {
                handOffset.y -= 0.28f;
                if (handOffset.y >= -4f)
                    return;
                ++_loadState;
                ammo = 2;
                loaded = false;
                if (Network.isActive)
                {
                    if (!isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolLoad");
                }
                else
                    SFX.Play("shotgunLoad");
            }
            else if (_loadState == 3)
            {
                handOffset.y += 0.15f;
                if (handOffset.y < 0)
                    return;
                ++_loadState;
                handOffset.y = 0f;
                if (Network.isActive)
                {
                    if (!isServerForObject)
                        return;
                    NetSoundEffect.Play("oldPistolSwipe2");
                }
                else
                    SFX.Play("swipe", 0.7f);
            }
            else
            {
                if (_loadState != 4)
                    return;
                if (_angleOffset > 0.04f)
                {
                    _angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.08f);
                }
                else
                {
                    _loadState = -1;
                    loaded = true;
                    _angleOffset = 0f;
                    if (isServerForObject && duck != null && duck.profile != null)
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                    if (Network.isActive)
                    {
                        if (!isServerForObject)
                            return;
                        SFX.PlaySynchronized("click", 1f, 0.5f, 0f, false, true);
                    }
                    else
                        SFX.Play("click", pitch: 0.5f);
                }
            }
        }

        public override void OnPressAction()
        {
            if (loaded && ammo > 1)
            {
                base.OnPressAction();
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    Level.Add(Spark.New(offDir > 0 ? x - 9f : x + 9f, y - 6f, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.05f));
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    Level.Add(SmallSmoke.New(barrelPosition.x + offDir * 4f, barrelPosition.y));
                ammo = 1;
            }
            else
            {
                if (_loadState != -1)
                    return;
                _loadState = 0;
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
