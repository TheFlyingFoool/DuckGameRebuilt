namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Wump|Shotguns")]
    public class WumpBlunderbuss : TampingWeapon
    {
        public WumpBlunderbuss(float xval, float yval) : base(xval, yval)
        {
            wideBarrel = true;
            ammo = 99;
            _ammoType = new ATShrapnel
            {
                range = 180f,
                rangeVariation = 40f,
                accuracy = 0.01f
            };
            _numBulletsPerFire = 6;
            _ammoType.penetration = 1;
            _type = "gun";
            graphic = new Sprite("wumpblunderbuss");
            Content.textures[graphic.Namebase] = graphic.texture;
            center = new Vec2(19f, 8f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(34f, 4f);
            _fireSound = "shotgun";
            _kickForce = 2f;
            _fireRumble = RumbleIntensity.Light;
            _holdOffset = new Vec2(4f, 1f);
            _editorName = "Blunderbuss";
            editorTooltip = "A new and frosty blunderbuss, and yet it still takes 150 years to reload.";
        }
        public override void Fire()
        {
            if (!loaded)
            {
                return;
            }
            if (ammo > 0 && _wait == 0f)
            {
                firedBullets.Clear();
                if (duck != null)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
                }
                ApplyKick();
                for (int i = 0; i < _numBulletsPerFire; i++)
                {
                    IceSpike ss = new IceSpike(barrelPosition.x, barrelPosition.y)
                    {
                        velocity = barrelVector.Rotate(Rando.Float(-0.3f, 0.3f), Vec2.Zero) * Rando.Float(7, 10)
                    };
                    if (duck != null) ss.velocity += duck.velocity;
                    Level.Add(ss);
                }
                _smokeWait = 3f;
                loaded = false;
                _flareAlpha = 1.5f;
                if (!_manualLoad)
                {
                    Reload(true);
                }
                firing = true;
                _wait = _fireWait;
                PlayFireSound();
                if (owner == null)
                {
                    Vec2 vec3 = barrelVector * Rando.Float(1f, 3f);
                    vec3.y += Rando.Float(2f);
                    hSpeed -= vec3.x;
                    vSpeed -= vec3.y;
                }
                _accuracyLost += loseAccuracy;
                if (_accuracyLost > maxAccuracyLost)
                {
                    _accuracyLost = maxAccuracyLost;
                    return;
                }
            }
            else if (ammo <= 0 && _wait == 0f)
            {
                firedBullets.Clear();
                DoAmmoClick();
                _wait = _fireWait;
            }
        }
        public override void OnPressAction()
        {
            if (_tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int i = 0; i < 18; i++)
                {
                    FreezeSmoke smoke = new FreezeSmoke(barrelPosition.x - 16f + Rando.Float(32f), barrelPosition.y - 16f + Rando.Float(32f))
                    {
                        depth = 0.9f + i * 0.001f
                    };
                    smoke.graphic.color = Color.LightBlue;
                    if (num < 6)
                    {
                        smoke.move -= barrelVector * Rando.Float(0.1f);
                    }
                    if (num > 5 && num < 16)
                    {
                        smoke.fly += barrelVector * (3f + Rando.Float(9f));
                    }
                    Level.Add(smoke);
                    num++;
                }
                _tampInc = 0f;
                _tampTime = infinite.value ? 0.5f : 0f;
                _tamped = false;
                return;
            }
            if (!_raised)
            {
                Duck duck = owner as Duck;
                if (duck != null && duck.grounded)
                {
                    duck.immobilized = true;
                    duck.sliding = false;
                    _rotating = true;
                }
            }
        }
    }
}
