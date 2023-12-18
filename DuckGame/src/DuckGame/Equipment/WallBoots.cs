namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("canSpawn", false)]
    public class WallBoots : Boots
    {
        public WallBoots(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _pickupSprite = new Sprite("walljumpBootsPickup");
            _sprite = new SpriteMap("walljumpBoots", 32, 32);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 13f);
            _equippedDepth = 3;
            editorTooltip = "Allows you to jump from walls. Why would you want to do this? Who can say.";
        }
    }
}
