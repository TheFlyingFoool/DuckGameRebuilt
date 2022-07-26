// Decompiled with JetBrains decompiler
// Type: DuckGame.MagBlaster
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 12;
            this._ammoType = (AmmoType)new ATMag();
            this._ammoType.penetration = 0.4f;
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(3f, 1f);
            this._type = "gun";
            this._sprite = new SpriteMap("magBlaster", 25, 19);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("fire", 0.8f, false, 1, 1, 2, 2, 3, 3);
            this._sprite.AddAnimation("empty", 1f, true, 4);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(12f, 8f);
            this.collisionOffset = new Vec2(-8f, -7f);
            this.collisionSize = new Vec2(16f, 14f);
            this._barrelOffsetTL = new Vec2(20f, 5f);
            this._fireSound = "magShot";
            this._kickForce = 5f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(1f, 0.0f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._bio = "Old faithful, the 9MM pistol.";
            this._editorName = "Mag Blaster";
            this.editorTooltip = "The preferred gun for enacting justice in a post-apocalyptic megacity.";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Update()
        {
            if (this._sprite.currentAnimation == "fire" && this._sprite.finished)
                this._sprite.SetAnimation("idle");
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo > 0)
            {
                this._sprite.SetAnimation("fire");
                for (int index = 0; index < 3; ++index)
                {
                    Vec2 vec2 = this.Offset(new Vec2(-9f, 0.0f));
                    Vec2 hitAngle = this.barrelVector.Rotate(Rando.Float(1f), Vec2.Zero);
                    Level.Add((Thing)Spark.New(vec2.x, vec2.y, hitAngle, 0.1f));
                }
            }
            else
                this._sprite.SetAnimation("empty");
            this.Fire();
        }
    }
}
