// Decompiled with JetBrains decompiler
// Type: DuckGame.CowboyPistol
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols", EditorItemType.PowerUser)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class CowboyPistol : Gun
    {
        private float rise;
        private float _angleOffset;

        public override float angle
        {
            get
            {
                if (this._raised || this.duck == null)
                    return base.angle;
                Vec2 p2 = this.duck.inputProfile.rightStick;
                if (p2.length < 0.1f)
                {
                    p2 = Vec2.Zero;
                    return base.angle;
                }
                return this.offDir > 0 ? Maths.DegToRad(Maths.PointDirection(Vec2.Zero, p2)) : Maths.DegToRad(Maths.PointDirection(Vec2.Zero, p2) + 180f);
            }
            set => this._angle = value;
        }

        public CowboyPistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = new ATMagnum();
            this._type = "gun";
            this.graphic = new Sprite("cowboyPistol");
            this.center = new Vec2(6f, 7f);
            this.collisionOffset = new Vec2(-5f, -7f);
            this.collisionSize = new Vec2(18f, 11f);
            this._barrelOffsetTL = new Vec2(21f, 3f);
            this._fireSound = "magnum";
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-2f, 1f);
            this._bio = "Standard issue .44 Magnum. Pretty great for killing things, really great for killing things that are trying to hide. Watch the kick, unless you're trying to shoot the ceiling.";
            this._editorName = "Cowboy Pistol";
        }

        public override void Update()
        {
            base.Update();
            this._angleOffset = this.owner == null ? 0f : (this.offDir >= 0 ? -Maths.DegToRad(this.rise * 65f) : -Maths.DegToRad((-this.rise * 65f)));
            if (rise > 0f)
                this.rise -= 0.013f;
            else
                this.rise = 0f;
            if (!this._raised)
                return;
            this._angleOffset = 0f;
        }

        public override void OnPressAction()
        {
            base.OnPressAction();
            if (this.ammo <= 0 || rise >= 1f)
                return;
            this.rise += 0.4f;
        }
    }
}
