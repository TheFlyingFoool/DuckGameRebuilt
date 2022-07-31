// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserRifle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 30;
            this._ammoType = new ATReboundLaser();
            this._ammoType.barrelAngleDegrees = 45f;
            this._type = "gun";
            this.graphic = new Sprite("laserRifle");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(26f, 14f);
            this._fireSound = "laserRifle";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-2f, 0f);
            this.editorTooltip = "Fires a reflecting beam of deadly energy at an angle. Science makes it happen!";
        }
    }
}
