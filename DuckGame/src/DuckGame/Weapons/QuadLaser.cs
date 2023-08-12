// Decompiled with JetBrains decompiler
// Type: DuckGame.QuadLaser
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            ammo = 3;
            _ammoType = new AT9mm();
            _type = "gun";
            graphic = new Sprite("quadLaser");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(20f, 8f);
            _fireSound = "pistolFire";
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _holdOffset = new Vec2(2f, -2f);
            _bio = "Stop moving...";
            _editorName = "Quad Laser";
            editorTooltip = "Shoots a slow-moving science block of doom that passes through walls.";
        }

        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is QuadLaser && pTaped.gun2 is QuadLaser ? new OctoLaser(x, y) : (pTaped.gun1 is QuadLaser && pTaped.gun2 is Phaser ? new QuadPhaser(x, y) : null);
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void OnPressAction()
        {
            if (ammo <= 0)
                return;
            Vec2 vec2 = Offset(barrelOffset);
            if (isServerForObject)
            {
                QuadLaserBullet quadLaserBullet = new QuadLaserBullet(vec2.x, vec2.y, barrelVector)
                {
                    killThingType = GetType()
                };
                Level.Add(quadLaserBullet);
                if (duck != null)
                {
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    duck.hSpeed = (float)(-barrelVector.x * 8);
                    duck.vSpeed = (float)(-barrelVector.y * 4 - 2);
                    quadLaserBullet.responsibleProfile = duck.profile;
                }
            }
            --ammo;
            SFX.Play("laserBlast");
        }
    }
}
