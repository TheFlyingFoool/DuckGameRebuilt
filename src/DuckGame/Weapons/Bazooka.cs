// Decompiled with JetBrains decompiler
// Type: DuckGame.Bazooka
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isInDemo", true)]
    public class Bazooka : TampingWeapon
    {
        public Bazooka(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = (AmmoType)new ATMissile();
            this._type = "gun";
            this.graphic = new Sprite("bazooka");
            this.center = new Vec2(15f, 5f);
            this.collisionOffset = new Vec2(-15f, -4f);
            this.collisionSize = new Vec2(30f, 10f);
            this._barrelOffsetTL = new Vec2(29f, 4f);
            this._fireSound = "missile";
            this._kickForce = 4f;
            this._fireRumble = RumbleIntensity.Light;
            this._holdOffset = new Vec2(-2f, -2f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._bio = "Old faithful, the 9MM pistol.";
            this._editorName = nameof(Bazooka);
            this.editorTooltip = "Funny name, serious firepower. Launches an explosive missile that can destroy terrain.";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void OnPressAction()
        {
            if (this._tamped)
            {
                base.OnPressAction();
                int num = 0;
                for (int index = 0; index < 14; ++index)
                {
                    MusketSmoke musketSmoke = new MusketSmoke(this.barrelPosition.x - 16f + Rando.Float(32f), this.barrelPosition.y - 16f + Rando.Float(32f));
                    musketSmoke.depth = (Depth)(float)(0.899999976158142 + (double)index * (1.0 / 1000.0));
                    if (num < 6)
                        musketSmoke.move -= this.barrelVector * Rando.Float(0.1f);
                    if (num > 5 && num < 10)
                        musketSmoke.fly += this.barrelVector * (2f + Rando.Float(7.8f));
                    Level.Add((Thing)musketSmoke);
                    ++num;
                }
                this._tampInc = 0.0f;
                if (this.infinite.value)
                    this._tampTime = 0.8f;
                else
                    this._tampTime = 0.5f;
                this._tamped = false;
            }
            else
            {
                if (this._raised || !(this.owner is Duck owner) || !owner.grounded)
                    return;
                owner.immobilized = true;
                owner.sliding = false;
                this._rotating = true;
            }
        }
    }
}
