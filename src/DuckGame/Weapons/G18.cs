// Decompiled with JetBrains decompiler
// Type: DuckGame.G18
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    public class G18 : Gun
    {
        private SpriteMap _sprite;

        public G18(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 32;
            _ammoType = new ATG18();
            _accuracyLost = 0.2f;
            maxAccuracyLost = 1f;
            _fullAuto = true;
            _fireWait = 0.3f;
            _type = "gun";
            _sprite = new SpriteMap("g18", 14, 8);
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
            _sprite.AddAnimation("fire", 0.8f, false, 1, 2, 3);
            _sprite.AddAnimation("empty", 1f, true, 2);
            graphic = _sprite;
            center = new Vec2(7f, 3f);
            collisionOffset = new Vec2(-7f, -3f);
            collisionSize = new Vec2(14f, 7f);
            _barrelOffsetTL = new Vec2(12f, 2f);
            _fireSound = "smg";
            _kickForce = 0.3f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, 0f);
            _bio = "Old faithful, the 9MM pistol.";
            _editorName = "Machine Pistol";
            editorTooltip = "Need to deliver a bunch of bullets to someone in a hurry? Try this.";
            physicsMaterial = PhysicsMaterial.Metal;
        }

        protected override void PlayFireSound() => SFX.Play(_fireSound, pitch: (0.6f + Rando.Float(0.2f)));

        public override void Update()
        {
            if (_sprite.currentAnimation == "fire" && _sprite.finished)
                _sprite.SetAnimation("idle");
            base.Update();
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                _sprite.SetAnimation("fire");
                for (int index = 0; index < 3; ++index)
                {
                    Vec2 vec2 = Offset(new Vec2(-9f, 0f));
                    Vec2 hitAngle = barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add(Spark.New(vec2.x, vec2.y, hitAngle, 0.1f));
                }
            }
            else
                _sprite.SetAnimation("empty");
            Fire();
        }
    }
}
