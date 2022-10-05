// Decompiled with JetBrains decompiler
// Type: DuckGame.RomanCandle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    public class RomanCandle : FlareGun
    {
        public StateBinding _litBinding = new StateBinding(nameof(_lit));
        private SpriteMap _sprite;
        private bool _lit;
        private int _flip = 1;
        private ActionTimer _timer = (ActionTimer)0.5f;
        private ActionTimer _litTimer;
        private ActionTimer _litStartTimer;
        private Sound _burnSound;

        public RomanCandle(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 9;
            _type = "gun";
            _sprite = new SpriteMap("romanCandle", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 6f);
            _barrelOffsetTL = new Vec2(16f, 9f);
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            flammable = 1f;
            _bio = "FOOM";
            _editorName = "Roman Candle";
            editorTooltip = "Fireworks, but even more dangerous! Light the fuse and cross your fingers.";
            physicsMaterial = PhysicsMaterial.Paper;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (_sprite == null)
                return;
            Vec2 vec2_1 = Offset(new Vec2(-6f, -4f));
            if (_lit && (bool)_timer && Rando.Int(DGRSettings.S_ParticleMultiplier) > 0)
                Level.Add(Spark.New(vec2_1.x, vec2_1.y, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
            if (_lit && _litTimer != null && (bool)_litTimer && _litStartTimer != null && (bool)_litStartTimer)
            {
                if (_sprite.frame == 0)
                    _sprite.frame = 1;
                if (owner == null)
                    y -= 6f;
                --ammo;
                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);
                kick = 1f;
                if (duck != null)
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                if (isServerForObject)
                {
                    Vec2 vec2_2 = Offset(barrelOffset);
                    CandleBall t = new CandleBall(vec2_2.x, vec2_2.y, this, 4);
                    Fondle(t);
                    Vec2 vec = Maths.AngleToVec(barrelAngle + Rando.Float(-0.1f, 0.1f));
                    t.hSpeed = vec.x * 14f;
                    t.vSpeed = vec.y * 14f;
                    Level.Add(t);
                }
                if (owner == null)
                {
                    hSpeed -= _flip * Rando.Float(1f, 5f);
                    vSpeed -= Rando.Float(1f, 7f);
                    _flip = _flip <= 0 ? 1 : -1;
                }
                offDir = (sbyte)_flip;
            }
            if (ammo <= 0)
            {
                _lit = false;
                _sprite.frame = 2;
                if (_burnSound != null)
                {
                    _burnSound.Stop();
                    _burnSound = null;
                }
            }
            base.Update();
            if (owner != null)
                _flip = owner.offDir;
            else
                graphic.flipH = _flip < 0;
        }

        public override void Terminate()
        {
            if (_burnSound != null)
                _burnSound.Kill();
            base.Terminate();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            Light();
            return true;
        }

        public override void Draw() => base.Draw();

        public void Light()
        {
            if (_lit || ammo <= 0)
                return;
            _lit = true;
            _litTimer = (ActionTimer)0.03f;
            _litStartTimer = new ActionTimer(0.01f, reset: false);
            _burnSound = SFX.Play("fuseBurn", 0.5f, looped: true);
        }

        public override void OnPressAction() => Light();

        public override void Fire()
        {
        }
    }
}
