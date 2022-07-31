// Decompiled with JetBrains decompiler
// Type: DuckGame.BananaCluster
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Explosives")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("previewPriority", true)]
    public class BananaCluster : Gun
    {
        private SpriteMap _sprite;
        private int _ammoMax = 3;

        public BananaCluster(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 3;
            this._ammoType = new ATShrapnel();
            this._type = "gun";
            this._sprite = new SpriteMap("banana", 16, 16)
            {
                frame = 4
            };
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 11f);
            this.physicsMaterial = PhysicsMaterial.Rubber;
            this._holdOffset = new Vec2(0f, 2f);
            this.bouncy = 0.4f;
            this.friction = 0.05f;
            this.editorTooltip = "Need more than one banana? Have I got news for you...";
            this.isFatal = false;
        }

        public override void Update()
        {
            this._sprite.frame = 4 + this._ammoMax - this.ammo;
            if (this.ammo == 0 && this.owner != null)
            {
                if (this.owner is Duck owner)
                    owner.ThrowItem();
                Level.Remove(this);
            }
            if (this.owner == null && this.ammo == 1)
            {
                Banana banana = new Banana(this.x, this.y)
                {
                    hSpeed = this.hSpeed,
                    vSpeed = this.vSpeed
                };
                Level.Remove(this);
                Level.Add(banana);
            }
            base.Update();
        }

        public override void OnPressAction()
        {
            if (this.ammo <= 0)
                return;
            --this.ammo;
            SFX.Play("smallSplat", pitch: Rando.Float(-0.6f, 0.6f));
            if (!(this.owner is Duck owner))
                return;
            float num1 = 0f;
            float num2 = 0f;
            if (owner.inputProfile.Down("LEFT"))
                num1 -= 3f;
            if (owner.inputProfile.Down("RIGHT"))
                num1 += 3f;
            if (owner.inputProfile.Down("UP"))
                num2 -= 3f;
            if (owner.inputProfile.Down("DOWN"))
                num2 += 3f;
            if (!this.isServerForObject)
                return;
            Banana banana = new Banana(this.barrelPosition.x, this.barrelPosition.y);
            if (!owner.crouch)
            {
                banana.hSpeed = offDir * Rando.Float(3f, 3.5f) + num1;
                banana.vSpeed = num2 - 1.5f + Rando.Float(-0.5f, -1f);
            }
            banana.EatBanana();
            banana.clip.Add(owner);
            owner.clip.Add(banana);
            Level.Add(banana);
        }
    }
}
