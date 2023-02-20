// Decompiled with JetBrains decompiler
// Type: DuckGame.AK47
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Machine Guns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class AK47 : Gun
    {
        public AK47(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 30;
            _ammoType = new ATHighCalMachinegun();
            _type = "gun";
            graphic = new Sprite("ak47");
            center = new Vec2(16f, 15f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(18f, 10f);
            _barrelOffsetTL = new Vec2(32f, 14f);
            _fireSound = "deepMachineGun2";
            _fullAuto = true;
            _fireWait = 1.2f;
            _kickForce = 3.5f;
            _fireRumble = RumbleIntensity.Kick;
            loseAccuracy = 0.2f;
            maxAccuracyLost = 0.8f;
            editorTooltip = "Go-to weapon of all your favorite Duck Action Heroes.";
        }
    }
}
