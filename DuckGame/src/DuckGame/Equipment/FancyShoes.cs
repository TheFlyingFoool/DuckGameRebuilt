namespace DuckGame
{
    [EditorGroup("Equipment", EditorItemType.PowerUser)]
    [BaggedProperty("previewPriority", false)]
    [BaggedProperty("canSpawn", false)]
    public class FancyShoes : Boots
    {
        public FancyShoes(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _pickupSprite = new Sprite("fancyShoesPickup");
            _sprite = new SpriteMap("fancyShoes", 32, 32);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedDepth = 3;
        }
    }
}
