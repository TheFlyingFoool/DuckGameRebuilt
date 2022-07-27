// Decompiled with JetBrains decompiler
// Type: DuckGame.Boots
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("previewPriority", true)]
    public class Boots : Equipment
    {
        protected SpriteMap _sprite;
        protected Sprite _pickupSprite;

        public Boots(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("bootsPickup");
            this._sprite = new SpriteMap("boots", 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 13f);
            this._equippedDepth = 3;
            this.editorTooltip = "Keeps feet safe and smashes bugs, among other things.";
            this.flammable = 0.3f;
            this.charThreshold = 0.8f;
        }

        protected override bool OnDestroy(DestroyType type = null) => !(type is DTIncinerate);

        public override void Update()
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 12f);
                this.graphic = _sprite;
                this.collisionOffset = new Vec2(0.0f, -9999f);
                this.collisionSize = new Vec2(0.0f, 0.0f);
                this.solid = false;
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 12;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;
            }
            else
            {
                this.center = new Vec2(8f, 8f);
                this.graphic = this._pickupSprite;
                this.collisionOffset = new Vec2(-6f, -6f);
                this.collisionSize = new Vec2(12f, 13f);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if ((double)this.alpha < 0.0)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            if (this._equippedDuck != null && this._equippedDuck._trapped != null)
                this.depth = this._equippedDuck._trapped.depth + 2;
            if (this._equippedDuck != null)
            {
                this._sprite.frame = this._equippedDuck._sprite.imageIndex;
                if (this._equippedDuck.ragdoll != null)
                    this._sprite.frame = 12;
                this._sprite.flipH = this._equippedDuck._sprite.flipH;
            }
            base.Draw();
        }
    }
}
