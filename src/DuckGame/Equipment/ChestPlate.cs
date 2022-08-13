// Decompiled with JetBrains decompiler
// Type: DuckGame.ChestPlate
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("chestPlateAnim", 32, 32);
            _spriteOver = new SpriteMap("chestPlateAnimOver", 32, 32);
            _pickupSprite = new Sprite("chestPlatePickup");
            _pickupSprite.CenterOrigin();
            graphic = _pickupSprite;
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 8f);
            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            _hasEquippedCollision = true;
            center = new Vec2(8f, 8f);
            physicsMaterial = PhysicsMaterial.Metal;
            _equippedDepth = 4;
            _wearOffset = new Vec2(1f, 1f);
            _isArmor = true;
            _equippedThickness = 3f;
            editorTooltip = "Protects against impacts to the chest. Makes you look swole.";
        }

        public override void Update()
        {
            if (_equippedDuck != null && duck == null)
                return;
            if (_equippedDuck != null && !destroyed)
            {
                center = new Vec2(16f, 16f);
                solid = false;
                _sprite.flipH = duck._sprite.flipH;
                _spriteOver.flipH = duck._sprite.flipH;
                graphic = _sprite;
                if (_equippedDuck.sliding)
                {
                    _equippedCollisionOffset = new Vec2(-5f, -5f);
                    _equippedCollisionSize = new Vec2(10f, 13f);
                }
                else
                {
                    _equippedCollisionOffset = new Vec2(-7f, -5f);
                    _equippedCollisionSize = new Vec2(12f, 11f);
                }
            }
            else
            {
                center = new Vec2(_pickupSprite.w / 2, _pickupSprite.h / 2);
                solid = true;
                _sprite.frame = 0;
                _sprite.flipH = false;
                graphic = _pickupSprite;
            }
            if (destroyed)
                alpha -= 0.05f;
            if (alpha < 0f)
                Level.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            if (_equippedDuck != null && _equippedDuck._trapped != null)
                depth = _equippedDuck._trapped.depth + 1;
            base.Draw();
            if (_equippedDuck != null && duck == null || _equippedDuck == null)
                return;
            _spriteOver.flipH = graphic.flipH;
            _spriteOver.angle = angle;
            _spriteOver.alpha = alpha;
            _spriteOver.scale = scale;
            _spriteOver.depth = owner.depth + (duck.holdObject != null ? 5 : 12);
            _spriteOver.center = center;
            Graphics.Draw(_spriteOver, x, y);
        }
    }
}
