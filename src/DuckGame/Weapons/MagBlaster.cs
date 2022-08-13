// Decompiled with JetBrains decompiler
// Type: DuckGame.MagBlaster
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    public class MagBlaster : Gun
    {
        private SpriteMap _sprite;

        public MagBlaster(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 12;
            _ammoType = new ATMag();
            _ammoType.penetration = 0.4f;
            wideBarrel = true;
            barrelInsertOffset = new Vec2(3f, 1f);
            _type = "gun";
            _sprite = new SpriteMap("magBlaster", 25, 19);
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
            _sprite.AddAnimation("fire", 0.8f, false, 1, 1, 2, 2, 3, 3);
            _sprite.AddAnimation("empty", 1f, true, 4);
            graphic = _sprite;
            center = new Vec2(12f, 8f);
            collisionOffset = new Vec2(-8f, -7f);
            collisionSize = new Vec2(16f, 14f);
            _barrelOffsetTL = new Vec2(20f, 5f);
            _fireSound = "magShot";
            _kickForce = 5f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(1f, 0f);
            loseAccuracy = 0.1f;
            maxAccuracyLost = 0.6f;
            _bio = "Old faithful, the 9MM pistol.";
            _editorName = "Mag Blaster";
            editorTooltip = "The preferred gun for enacting justice in a post-apocalyptic megacity.";
            physicsMaterial = PhysicsMaterial.Metal;
        }

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
