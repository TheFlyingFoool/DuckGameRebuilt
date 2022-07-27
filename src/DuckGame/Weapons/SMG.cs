// Decompiled with JetBrains decompiler
// Type: DuckGame.SMG
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Machine Guns")]
    public class SMG : Gun
    {
        public SMG(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 30;
            this._ammoType = new AT9mm();
            this._ammoType.range = 110f;
            this._ammoType.accuracy = 0.6f;
            this._type = "gun";
            this.graphic = new Sprite("smg");
            this.center = new Vec2(8f, 4f);
            this.collisionOffset = new Vec2(-8f, -4f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(17f, 2f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-1f, 0.0f);
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.8f;
            this.editorTooltip = "Rapid-fire bullet-spitting machine. Great for making artisanal swiss cheese.";
        }
    }
}
