// Decompiled with JetBrains decompiler
// Type: DuckGame.NetGun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class NetGun : Gun
    {
        private SpriteMap _barrelSteam;
        private SpriteMap _netGunGuage;

        public NetGun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = (AmmoType)new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._ammoType.penetration = -1f;
            this._type = "gun";
            this.graphic = new Sprite("netGun");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(27f, 14f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._netGunGuage = new SpriteMap("netGunGuage", 8, 8);
            this._barrelSteam = new SpriteMap("steamPuff", 16, 16);
            this._barrelSteam.center = new Vec2(0.0f, 14f);
            this._barrelSteam.AddAnimation("puff", 0.4f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            this._barrelSteam.SetAnimation("puff");
            this._barrelSteam.speed = 0.0f;
            this._bio = "C02 powered, shoots nets, traps ducks. Is that stubborn duck not moving? Why not trap it, and put it where it belongs.";
            this._editorName = "Net Gun";
            this.editorTooltip = "Fires entangling nets that hold Ducks in place *evil moustache twirl*";
            this.isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            this._netGunGuage.frame = 4 - Math.Min(this.ammo + 1, 4);
            if ((double)this._barrelSteam.speed > 0.0 && this._barrelSteam.finished)
                this._barrelSteam.speed = 0.0f;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if ((double)this._barrelSteam.speed > 0.0)
            {
                this._barrelSteam.alpha = 0.6f;
                this.Draw((Sprite)this._barrelSteam, new Vec2(9f, 1f));
            }
            this.Draw((Sprite)this._netGunGuage, new Vec2(-4f, -4f));
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                --this.ammo;
                if (this.duck != null)
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("netGunFire");
                this._barrelSteam.speed = 1f;
                this._barrelSteam.frame = 0;
                this.ApplyKick();
                Vec2 vec2 = this.Offset(this.barrelOffset);
                if (this.receivingPress)
                    return;
                Net t = new Net(vec2.x, vec2.y - 2f, this.duck);
                Level.Add((Thing)t);
                this.Fondle((Thing)t);
                if (this.owner != null)
                    t.responsibleProfile = this.owner.responsibleProfile;
                t.clip.Add(this.owner as MaterialThing);
                t.hSpeed = this.barrelVector.x * 10f;
                t.vSpeed = (float)((double)this.barrelVector.y * 7.0 - 1.5);
            }
            else
                this.DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
