// Decompiled with JetBrains decompiler
// Type: DuckGame.RomanCandle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 9;
            this._type = "gun";
            this._sprite = new SpriteMap("romanCandle", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 6f);
            this._barrelOffsetTL = new Vec2(16f, 9f);
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 1f;
            this._bio = "FOOM";
            this._editorName = "Roman Candle";
            this.editorTooltip = "Fireworks, but even more dangerous! Light the fuse and cross your fingers.";
            this.physicsMaterial = PhysicsMaterial.Paper;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (this._sprite == null)
                return;
            Vec2 vec2_1 = this.Offset(new Vec2(-6f, -4f));
            if (this._lit && (bool)this._timer)
                Level.Add((Thing)Spark.New(vec2_1.x, vec2_1.y, new Vec2(Rando.Float(-1f, 1f), -0.5f), 0.1f));
            if (this._lit && this._litTimer != null && (bool)this._litTimer && this._litStartTimer != null && (bool)this._litStartTimer)
            {
                if (this._sprite.frame == 0)
                    this._sprite.frame = 1;
                if (this.owner == null)
                    this.y -= 6f;
                --this.ammo;
                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);
                this.kick = 1f;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                if (this.isServerForObject)
                {
                    Vec2 vec2_2 = this.Offset(this.barrelOffset);
                    CandleBall t = new CandleBall(vec2_2.x, vec2_2.y, (FlareGun)this, 4);
                    this.Fondle((Thing)t);
                    Vec2 vec = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.1f, 0.1f));
                    t.hSpeed = vec.x * 14f;
                    t.vSpeed = vec.y * 14f;
                    Level.Add((Thing)t);
                }
                if (this.owner == null)
                {
                    this.hSpeed -= (float)this._flip * Rando.Float(1f, 5f);
                    this.vSpeed -= Rando.Float(1f, 7f);
                    this._flip = this._flip <= 0 ? 1 : -1;
                }
                this.offDir = (sbyte)this._flip;
            }
            if (this.ammo <= 0)
            {
                this._lit = false;
                this._sprite.frame = 2;
                if (this._burnSound != null)
                {
                    this._burnSound.Stop();
                    this._burnSound = (Sound)null;
                }
            }
            base.Update();
            if (this.owner != null)
                this._flip = (int)this.owner.offDir;
            else
                this.graphic.flipH = this._flip < 0;
        }

        public override void Terminate()
        {
            if (this._burnSound != null)
                this._burnSound.Kill();
            base.Terminate();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            this.Light();
            return true;
        }

        public override void Draw() => base.Draw();

        public void Light()
        {
            if (this._lit || this.ammo <= 0)
                return;
            this._lit = true;
            this._litTimer = (ActionTimer)0.03f;
            this._litStartTimer = new ActionTimer(0.01f, reset: false);
            this._burnSound = SFX.Play("fuseBurn", 0.5f, looped: true);
        }

        public override void OnPressAction() => this.Light();

        public override void Fire()
        {
        }
    }
}
