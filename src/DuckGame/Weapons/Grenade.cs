// Decompiled with JetBrains decompiler
// Type: DuckGame.Grenade
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public Duck cookThrower => this._cookThrower;

        public float cookTimeOnThrow => this._cookTimeOnThrow;

        public Grenade(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.penetration = 0.4f;
            this._type = "gun";
            this._sprite = new SpriteMap(nameof(grenade), 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(7f, 8f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 10f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this._fireRumble = RumbleIntensity.Kick;
            this._editorName = nameof(Grenade);
            this.editorTooltip = "#1 Pull pin. #2 Throw grenade. Order of operations is important here.";
            this._bio = "To cook grenade, pull pin and hold until feelings of terror run down your spine. Serves as many ducks as you can fit into a 3 meter radius.";
        }

        public override void Initialize()
        {
            this.gr = Grenade.grenade;
            ++Grenade.grenade;
        }

        public override void OnNetworkBulletsFired(Vec2 pos)
        {
            this._pin = false;
            this._localDidExplode = true;
            if (!this._explosionCreated)
                Graphics.FlashScreen();
            this.CreateExplosion(pos);
        }

        public void CreateExplosion(Vec2 pos)
        {
            if (this._explosionCreated)
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
            this._explosionCreated = true;
            SFX.Play("explode");
            RumbleManager.AddRumbleEvent(pos, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
        }

        public override void Update()
        {
            base.Update();
            if (!this._pin)
            {
                this._timer -= 0.01f;
                this.holsterable = false;
            }
            if (_timer < 0.5 && this.owner == null && !this._didBonus)
            {
                this._didBonus = true;
                if (Recorder.currentRecording != null)
                    Recorder.currentRecording.LogBonus();
            }
            if (!this._localDidExplode && _timer < 0.0)
            {
                if (this._explodeFrames < 0)
                {
                    this.CreateExplosion(this.position);
                    this._explodeFrames = 4;
                }
                else
                {
                    --this._explodeFrames;
                    if (this._explodeFrames == 0)
                    {
                        float x = this.x;
                        float num1 = this.y - 2f;
                        Graphics.FlashScreen();
                        if (this.isServerForObject)
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
                                this.firedBullets.Add(bullet);
                                Level.Add(bullet);
                            }
                            foreach (Window ignore in Level.CheckCircleAll<Window>(this.position, 40f))
                            {
                                if (Level.CheckLine<Block>(this.position, ignore.position, ignore) == null)
                                    ignore.Destroy(new DTImpact(this));
                            }
                            this.bulletFireIndex += 20;
                            if (Network.isActive)
                            {
                                Send.Message(new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false), NetMessagePriority.ReliableOrdered);
                                this.firedBullets.Clear();
                            }
                        }
                        Level.Remove(this);
                        this._destroyed = true;
                        this._explodeFrames = -1;
                    }
                }
            }
            if (this.prevOwner != null && this._cookThrower == null)
            {
                this._cookThrower = this.prevOwner as Duck;
                this._cookTimeOnThrow = this._timer;
            }
            this._sprite.frame = this._pin ? 0 : 1;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.pullOnImpact)
                this.OnPressAction();
            base.OnSolidImpact(with, from);
        }

        public override void OnPressAction()
        {
            if (!this._pin)
                return;
            this._pin = false;
            GrenadePin grenadePin = new GrenadePin(this.x, this.y)
            {
                hSpeed = -this.offDir * (1.5f + Rando.Float(0.5f)),
                vSpeed = -2f
            };
            Level.Add(grenadePin);
            if (this.duck != null)
                RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
            SFX.Play("pullPin");
        }
    }
}
