// Decompiled with JetBrains decompiler
// Type: DuckGame.Pistol
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Pistols")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class Pistol : Gun
    {
        private SpriteMap _sprite;

        public Pistol(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 9;
            this._ammoType = (AmmoType)new AT9mm();
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(0.0f, -1f);
            this._type = "gun";
            this._sprite = new SpriteMap("pistol", 18, 10);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("fire", 0.8f, false, 1, 2, 2, 3, 3);
            this._sprite.AddAnimation("empty", 1f, true, 2);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(10f, 3f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(18f, 2f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(0.0f, 0.0f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._bio = "Old faithful, the 9MM pistol.";
            this._editorName = nameof(Pistol);
            this.editorTooltip = "Your average everyday pistol. Just workin' to keep its kids fed, never bothered nobody.";
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
