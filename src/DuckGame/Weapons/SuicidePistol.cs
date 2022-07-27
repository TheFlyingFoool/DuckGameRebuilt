// Decompiled with JetBrains decompiler
// Type: DuckGame.SuicidePistol
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    public class SuicidePistol : Gun
    {
        public SuicidePistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 6;
            this._ammoType = new AT9mm();
            this._ammoType.barrelAngleDegrees = 180f;
            this._ammoType.immediatelyDeadly = true;
            this._type = "gun";
            this.graphic = new Sprite("suicidePistol");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -5f);
            this.collisionSize = new Vec2(16f, 10f);
            this._barrelOffsetTL = new Vec2(8f, 13f);
            this._fireSound = "magnum";
            this._kickForce = -3.5f;
            this._fireRumble = RumbleIntensity.Kick;
            this.handOffset = new Vec2(6f, 0.0f);
            this._holdOffset = new Vec2(6f, 0.0f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this.editorTooltip = "There's something odd about this gun but I can't quite put my finger on it.";
        }

        public override void Update()
        {
            if (this._raised)
            {
                this.handOffset = new Vec2(0.0f, 0.0f);
                this._holdOffset = new Vec2(0.0f, 0.0f);
                this.collisionOffset = new Vec2(-8f, -5f);
                this.collisionSize = new Vec2(16f, 10f);
            }
            else
            {
                this.handOffset = new Vec2(7f, 0.0f);
                this._holdOffset = new Vec2(4f, -1f);
                this.collisionOffset = new Vec2(-8f, -5f);
                this.collisionSize = new Vec2(8f, 10f);
            }
            base.Update();
        }
    }
}
