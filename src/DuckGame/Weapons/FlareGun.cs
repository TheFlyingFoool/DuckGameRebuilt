// Decompiled with JetBrains decompiler
// Type: DuckGame.FlareGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    public class FlareGun : Gun
    {
        public FlareGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = new AT9mm();
            this._ammoType.combustable = true;
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite("flareGun");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(18f, 6f);
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._barrelAngleOffset = 4f;
            this._editorName = "Flare Gun";
            this.editorTooltip = "Shoots a flare at long range that spits fire on impact. Fun at parties!";
            this._bio = "For safety purposes, used to call help. What? No it's not a weapon. NO DON'T USE IT LIKE THAT!";
        }

        public override void Initialize() => base.Initialize();

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                this._barrelHeat += 0.15f;
                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                this.ApplyKick();
                if (this.receivingPress || !this.isServerForObject)
                    return;
                Vec2 vec2 = this.Offset(this.barrelOffset);
                Flare t = new Flare(vec2.x, vec2.y, this);
                this.Fondle(t);
                Vec2 vec = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.2f, 0.2f));
                t.hSpeed = vec.x * 14f;
                t.vSpeed = vec.y * 14f;
                Level.Add(t);
            }
            else
                this.DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
