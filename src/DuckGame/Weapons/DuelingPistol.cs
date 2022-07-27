// Decompiled with JetBrains decompiler
// Type: DuckGame.DuelingPistol
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("isFatal", false)]
    public class DuelingPistol : Gun
    {
        public DuelingPistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 1;
            this._ammoType = new ATShrapnel();
            this._ammoType.range = 70f;
            this._ammoType.accuracy = 0.5f;
            this._ammoType.penetration = 0.4f;
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite("tinyGun");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 8f);
            this._barrelOffsetTL = new Vec2(20f, 15f);
            this._fireSound = "littleGun";
            this._kickForce = 0.0f;
            this._fireRumble = RumbleIntensity.Kick;
            this.editorTooltip = "The perfect weapon when a Duck has dishonored your family. One shot only.";
        }

        public static void ExplodeEffect(Vec2 position)
        {
            Level.Add(SmallSmoke.New(position.x, position.y));
            Level.Add(SmallSmoke.New(position.x, position.y));
            for (int index = 0; index < 8; ++index)
                Level.Add(Spark.New(position.x + Rando.Float(-3f, 3f), position.y + Rando.Float(-3f, 3f), new Vec2(Rando.Float(-3f, 3f), -Rando.Float(-3f, 3f)), 0.05f));
            SFX.Play("shotgun", pitch: 0.3f);
        }

        public override void OnPressAction()
        {
            if (this.plugged && this.isServerForObject)
            {
                this._kickForce = 3f;
                this.ApplyKick();
                DuelingPistol.ExplodeEffect(this.position);
                if (Network.isActive)
                    Send.Message(new NMPistolExplode(this.position));
                if (this.duck != null)
                    this.duck.Swear();
                Level.Remove(this);
            }
            else
                base.OnPressAction();
        }
    }
}
