// Decompiled with JetBrains decompiler
// Type: DuckGame.ChestPlate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isInDemo", true)]
    public class ChestPlate : Equipment
    {
        private SpriteMap _sprite;
        private SpriteMap _spriteOver;
        private Sprite _pickupSprite;

        public ChestPlate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("chestPlateAnim", 32, 32);
            this._spriteOver = new SpriteMap("chestPlateAnimOver", 32, 32);
            this._pickupSprite = new Sprite("chestPlatePickup");
            this._pickupSprite.CenterOrigin();
            this.graphic = this._pickupSprite;
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 8f);
            this._equippedCollisionOffset = new Vec2(-7f, -5f);
            this._equippedCollisionSize = new Vec2(12f, 11f);
            this._hasEquippedCollision = true;
            this.center = new Vec2(8f, 8f);
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._equippedDepth = 4;
            this._wearOffset = new Vec2(1f, 1f);
            this._isArmor = true;
            this._equippedThickness = 3f;
            this.editorTooltip = "Protects against impacts to the chest. Makes you look swole.";
        }

        public override void Update()
        {
            if (this._equippedDuck != null && this.duck == null)
                return;
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(16f, 16f);
                this.solid = false;
                this._sprite.flipH = this.duck._sprite.flipH;
                this._spriteOver.flipH = this.duck._sprite.flipH;
                this.graphic = _sprite;
                if (this._equippedDuck.sliding)
                {
                    this._equippedCollisionOffset = new Vec2(-5f, -5f);
                    this._equippedCollisionSize = new Vec2(10f, 13f);
                }
                else
                {
                    this._equippedCollisionOffset = new Vec2(-7f, -5f);
                    this._equippedCollisionSize = new Vec2(12f, 11f);
                }
            }
            else
            {
                this.center = new Vec2(this._pickupSprite.w / 2, this._pickupSprite.h / 2);
                this.solid = true;
                this._sprite.frame = 0;
                this._sprite.flipH = false;
                this.graphic = this._pickupSprite;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if (this.alpha < 0f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            if (this._equippedDuck != null && this._equippedDuck._trapped != null)
                this.depth = this._equippedDuck._trapped.depth + 1;
            base.Draw();
            if (this._equippedDuck != null && this.duck == null || this._equippedDuck == null)
                return;
            this._spriteOver.flipH = this.graphic.flipH;
            this._spriteOver.angle = this.angle;
            this._spriteOver.alpha = this.alpha;
            this._spriteOver.scale = this.scale;
            this._spriteOver.depth = this.owner.depth + (this.duck.holdObject != null ? 5 : 12);
            this._spriteOver.center = this.center;
            Graphics.Draw(_spriteOver, this.x, this.y);
        }
    }
}
