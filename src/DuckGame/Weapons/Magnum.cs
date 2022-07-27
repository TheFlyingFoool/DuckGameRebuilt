// Decompiled with JetBrains decompiler
// Type: DuckGame.Magnum
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    public class Magnum : Gun
    {
        public StateBinding _angleOffsetBinding = new StateBinding(nameof(_angleOffset));
        public StateBinding _riseBinding = new StateBinding(nameof(rise));
        public float rise;
        public float _angleOffset;

        public override float angle
        {
            get => base.angle + this._angleOffset;
            set => this._angle = value;
        }

        public Magnum(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = new ATMagnum();
            this._type = "gun";
            this.graphic = new Sprite("magnum");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -6f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(25f, 12f);
            this._fireSound = "magnum";
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Light;
            this._holdOffset = new Vec2(1f, 2f);
            this.handOffset = new Vec2(0.0f, 1f);
            this._bio = "Standard issue .44 Magnum. Pretty great for killing things, really great for killing things that are trying to hide. Watch the kick, unless you're trying to shoot the ceiling.";
            this._editorName = nameof(Magnum);
            this.editorTooltip = "Heavy duty pistol that pierces many objects. Cool shades not included.";
        }

        public override void Update()
        {
            base.Update();
            this._angleOffset = this.owner == null ? 0.0f : (this.offDir >= 0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((float)(-(double)this.rise * 65.0)));
            if (rise > 0.0)
                this.rise -= 0.013f;
            else
                this.rise = 0.0f;
            if (!this._raised)
                return;
            this._angleOffset = 0.0f;
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if (this.ammo <= 0 || rise >= 1.0)
                return;
            this.rise += 0.4f;
        }
    }
}
