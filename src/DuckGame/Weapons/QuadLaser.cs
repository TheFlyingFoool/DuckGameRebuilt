// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadLaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    public class QuadLaser : Gun
    {
        public QuadLaser(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 3;
            this._ammoType = new AT9mm();
            this._type = "gun";
            this.graphic = new Sprite("quadLaser");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 8f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._holdOffset = new Vec2(2f, -2f);
            this._bio = "Stop moving...";
            this._editorName = "Quad Laser";
            this.editorTooltip = "Shoots a slow-moving science block of doom that passes through walls.";
        }

        public override void OnPressAction()
        {
            if (this.ammo <= 0)
                return;
            Vec2 vec2 = this.Offset(this.barrelOffset);
            if (this.isServerForObject)
            {
                QuadLaserBullet quadLaserBullet = new QuadLaserBullet(vec2.x, vec2.y, this.barrelVector)
                {
                    killThingType = this.GetType()
                };
                Level.Add(quadLaserBullet);
                if (this.duck != null)
                {
                    RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    this.duck.hSpeed = (float)(-(double)this.barrelVector.x * 8.0);
                    this.duck.vSpeed = (float)(-(double)this.barrelVector.y * 4.0 - 2.0);
                    quadLaserBullet.responsibleProfile = this.duck.profile;
                }
            }
            --this.ammo;
            SFX.Play("laserBlast");
        }
    }
}
