// Decompiled with JetBrains decompiler
// Type: DuckGame.DuelingPistol
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            ammo = 1;
            _ammoType = new ATShrapnel();
            _ammoType.range = 70f;
            _ammoType.accuracy = 0.5f;
            _ammoType.penetration = 0.4f;
            wideBarrel = true;
            _type = "gun";
            graphic = new Sprite("tinyGun");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 8f);
            _barrelOffsetTL = new Vec2(20f, 15f);
            _fireSound = "littleGun";
            _kickForce = 0f;
            _fireRumble = RumbleIntensity.Kick;
            editorTooltip = "The perfect weapon when a Duck has dishonored your family. One shot only.";
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
            if (plugged && isServerForObject)
            {
                _kickForce = 3f;
                ApplyKick();
                DuelingPistol.ExplodeEffect(position);
                if (Network.isActive)
                    Send.Message(new NMPistolExplode(position));
                if (duck != null)
                    duck.Swear();
                Level.Remove(this);
            }
            else
                base.OnPressAction();
        }
    }
}
