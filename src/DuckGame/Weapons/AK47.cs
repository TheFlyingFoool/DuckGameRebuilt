// Decompiled with JetBrains decompiler
// Type: DuckGame.AK47
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 30;
            this._ammoType = new ATHighCalMachinegun();
            this._type = "gun";
            this.graphic = new Sprite("ak47");
            this.center = new Vec2(16f, 15f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(18f, 10f);
            this._barrelOffsetTL = new Vec2(32f, 14f);
            this._fireSound = "deepMachineGun2";
            this._fullAuto = true;
            this._fireWait = 1.2f;
            this._kickForce = 3.5f;
            this._fireRumble = RumbleIntensity.Kick;
            this.loseAccuracy = 0.2f;
            this.maxAccuracyLost = 0.8f;
            this.editorTooltip = "Go-to weapon of all your favorite Duck Action Heroes.";
        }
    }
}
