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
            ammo = 30;
            _ammoType = new AT9mm();
            _ammoType.range = 110f;
            _ammoType.accuracy = 0.6f;
            _type = "gun";
            graphic = new Sprite("smg");
            center = new Vec2(8f, 4f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(17f, 2f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, 0f);
            loseAccuracy = 0.2f;
            maxAccuracyLost = 0.8f;
            editorTooltip = "Rapid-fire bullet-spitting machine. Great for making artisanal swiss cheese.";
        }
    }
}
