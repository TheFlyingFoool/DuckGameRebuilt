// Decompiled with JetBrains decompiler
// Type: DuckGame.G18
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 32;
            this._ammoType = (AmmoType)new ATG18();
            this._accuracyLost = 0.2f;
            this.maxAccuracyLost = 1f;
            this._fullAuto = true;
            this._fireWait = 0.3f;
            this._type = "gun";
            this._sprite = new SpriteMap("g18", 14, 8);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("fire", 0.8f, false, 1, 2, 3);
            this._sprite.AddAnimation("empty", 1f, true, 2);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(7f, 3f);
            this.collisionOffset = new Vec2(-7f, -3f);
            this.collisionSize = new Vec2(14f, 7f);
            this._barrelOffsetTL = new Vec2(12f, 2f);
            this._fireSound = "smg";
            this._kickForce = 0.3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-1f, 0.0f);
            this._bio = "Old faithful, the 9MM pistol.";
            this._editorName = "Machine Pistol";
            this.editorTooltip = "Need to deliver a bunch of bullets to someone in a hurry? Try this.";
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        protected override void PlayFireSound() => SFX.Play(this._fireSound, pitch: (0.6f + Rando.Float(0.2f)));

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
