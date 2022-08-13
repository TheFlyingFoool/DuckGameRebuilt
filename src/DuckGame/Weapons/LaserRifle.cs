// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserRifle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    public class LaserRifle : Gun
    {
        public LaserRifle(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 30;
            _ammoType = new ATReboundLaser();
            _ammoType.barrelAngleDegrees = 45f;
            _type = "gun";
            graphic = new Sprite("laserRifle");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 10f);
            _barrelOffsetTL = new Vec2(26f, 14f);
            _fireSound = "laserRifle";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-2f, 0f);
            editorTooltip = "Fires a reflecting beam of deadly energy at an angle. Science makes it happen!";
        }
    }
}
