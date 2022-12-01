using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Guns")]
    internal class DartGrenade : Gun // All this code is taken from normal grenades but changed as to not create shrapnel
    {
        public StateBinding _timerBinding = new StateBinding(nameof(_timer));
        public StateBinding _pinBinding = new StateBinding(nameof(_pin));
        private SpriteMap _sprite;
        public bool _pin = true;
        public float _timer = 1.2f;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;
        private bool _explosionCreated;
        private bool _localDidExplode;
        private bool _didBonus;
        public int _explodeFrames = -1;
        public const string dartGrenade = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAQCAMAAABA3o1rAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAVUExURQAAAMAgLXsPHKRkIvfgWv///wAAAECipFQAAAAHdFJOU////////wAaSwNGAAAACXBIWXMAAA7DAAAOwwHHb6hkAAAAaklEQVQoU6WNUQ7AIAhDqzDvf+TZgbFsH1uyJopPHorxkq8CUM3NUdFaU0M4hd6hhnDumCwGf0iODQYXAXBbRgiHO9/gmSFbXlzLZ2zOZBbHsFyQmcJhlX7lELAejCivIm1m863xzF9hjBOq8gmZU+pZRwAAAABJRU5ErkJggg==";

        public Duck cookThrower => _cookThrower;

        public float cookTimeOnThrow => _cookTimeOnThrow;

        public DartGrenade(float xpos, float ypos) : base(xpos, ypos)
        {
            _sprite = new SpriteMap(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(dartGrenade))), "dartgrenade"), 16, 16);
            _sprite.Namebase = "dartgrenade";
            Content.textures[_sprite.Namebase] = _sprite.texture;
            ammo = 1;
            _type = "gun";
            graphic = _sprite;
            center = new Vec2(7f, 8f);
            collisionOffset = new Vec2(-4f, -5f);
            collisionSize = new Vec2(8f, 10f);
            friction = 0.05f;
            _fireRumble = RumbleIntensity.Kick;
            _editorName = "Dart Grenade";
            _ammoType = new ATDart();
            bouncy = 0.6f;
            physicsMaterial = PhysicsMaterial.Plastic;
            editorTooltip = "New grenade made by NURF, safe for all ages!";
            _bio = "Works the same as any conventional grenade other than the fact that it sprays harmless darts everywhere!";
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            _pin = false;
            _localDidExplode = true;
            if (!_explosionCreated)
                Graphics.FlashScreen();
            CreateExplosion(pos);
        }

        public void CreateExplosion(Vec2 pos)
        {
            if (_explosionCreated)
                return;
            float x = pos.x;
            float ypos = pos.y - 2f;
            Level.Add(new ExplosionPart(x, ypos));
            _explosionCreated = true;
            SFX.Play("dartGunFire");
            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Medium, RumbleDuration.Short, RumbleFalloff.Medium));
        }

        public override void Update()
        {
            base.Update();
            if (!_pin)
            {
                _timer -= 0.01f;
                holsterable = false;
            }
            if (_timer < 0.5 && owner == null && !_didBonus)
            {
                _didBonus = true;
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
            }
            if (!_localDidExplode && _timer < 0.0)
            {
                if (_explodeFrames < 0)
                {
                    CreateExplosion(position);
                    _explodeFrames = 4;
                }
                else
                {
                    --_explodeFrames;
                    if (_explodeFrames == 0)
                    {
                        float x = this.x;
                        float y = this.y;
                        if (isServerForObject)
                        {
                            for (int index = 0; index < 10; ++index)
                            {
                                float fireAngle = (index * 45);
                                Dart dart = new Dart(x + (float)(Math.Cos(Maths.DegToRad(fireAngle)) * 15.0), y - (float)(Math.Sin(Maths.DegToRad(fireAngle)) * 15.0), owner as Duck, fireAngle);
                                Vec2 vec = Maths.AngleToVec(fireAngle);
                                dart.hSpeed = vec.x * 14f;
                                dart.hMax = 30;
                                dart.vMax = 30;
                                dart.vSpeed = vec.y * 14f;
                                Level.Add(dart);
                            }
                            if (owner != null && owner.isServerForObject)
                            {

                            }
                        }
                        Level.Remove(this);
                        _destroyed = true;
                        _explodeFrames = -1;
                    }
                }
            }
            if (prevOwner != null && _cookThrower == null)
            {
                _cookThrower = prevOwner as Duck;
                _cookTimeOnThrow = _timer;
            }
            _sprite.frame = _pin ? 0 : 1;
        }

        public override void OnPressAction()
        {
            if (!_pin)
                return;
            _pin = false;
            GrenadePin grenadePin = new GrenadePin(x, y)
            {
                hSpeed = -offDir * (1.5f + Rando.Float(0.5f)),
                vSpeed = -2f
            };
            Level.Add(grenadePin);
            if (duck != null)
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
            SFX.Play("pullPin");
        }
    }
}
