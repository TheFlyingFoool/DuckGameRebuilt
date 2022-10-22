// Decompiled with JetBrains decompiler
// Type: DuckGame.Grenade
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isInDemo", true)]
    public class Grenade : Gun
    {
        public StateBinding _timerBinding = new StateBinding(nameof(_timer));
        public StateBinding _pinBinding = new StateBinding(nameof(_pin));
        private SpriteMap _sprite;
        public bool _pin = true;
        public float _timer = 1.2f;
        private Duck _cookThrower;
        private float _cookTimeOnThrow;
        public bool pullOnImpact;
        private bool _explosionCreated;
        private bool _localDidExplode;
        private bool _didBonus;
        private static int grenade;
        public int gr;
        public int _explodeFrames = -1;

        public Duck cookThrower => _cookThrower;

        public float cookTimeOnThrow => _cookTimeOnThrow;

        public Grenade(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 1;
            _ammoType = new ATShrapnel();
            _ammoType.penetration = 0.4f;
            _type = "gun";
            _sprite = new SpriteMap(nameof(grenade), 16, 16);
            graphic = _sprite;
            center = new Vec2(7f, 8f);
            collisionOffset = new Vec2(-4f, -5f);
            collisionSize = new Vec2(8f, 10f);
            bouncy = 0.4f;
            friction = 0.05f;
            _fireRumble = RumbleIntensity.Kick;
            _editorName = nameof(Grenade);
            editorTooltip = "#1 Pull pin. #2 Throw grenade. Order of operations is important here.";
            _bio = "To cook grenade, pull pin and hold until feelings of terror run down your spine. Serves as many ducks as you can fit into a 3 meter radius.";
        }

        public override void Initialize()
        {
            gr = Grenade.grenade;
            ++Grenade.grenade;
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
            int num1 = 6;
            if (Graphics.effectsLevel < 2)
                num1 = 3;
            for (int index = 0; index < num1; ++index)
            {
                float deg = index * 60f + Rando.Float(-10f, 10f);
                float num2 = Rando.Float(12f, 20f);
                Level.Add(new ExplosionPart(x + (float)Math.Cos(Maths.DegToRad(deg)) * num2, ypos - (float)Math.Sin(Maths.DegToRad(deg)) * num2));
            }
            _explosionCreated = true;
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
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
                        float num1 = y - 2f;
                        Graphics.FlashScreen();
                        if (isServerForObject)
                        {
                            for (int index = 0; index < 20; ++index)
                            {
                                float num2 = (float)(index * 18.0 - 5.0) + Rando.Float(10f);
                                ATShrapnel type = new ATShrapnel
                                {
                                    range = 60f + Rando.Float(18f)
                                };
                                Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num2)) * 6.0), num1 - (float)(Math.Sin(Maths.DegToRad(num2)) * 6.0), type, num2)
                                {
                                    firedFrom = this
                                };
                                firedBullets.Add(bullet);
                                Level.Add(bullet);
                            }
                            foreach (Window ignore in Level.CheckCircleAll<Window>(position, 40f))
                            {
                                if (Level.CheckLine<Block>(position, ignore.position, ignore) == null)
                                    ignore.Destroy(new DTImpact(this));
                            }
                            bulletFireIndex += 20;
                            if (Network.isActive)
                            {
                                Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false), NetMessagePriority.ReliableOrdered);
                                firedBullets.Clear();
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

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (pullOnImpact)
                OnPressAction();
            base.OnSolidImpact(with, from);
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
