namespace DuckGame
{
    [EditorGroup("Equipment")]
    public class TinfoilHat : Hat
    {
        public TinfoilHat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _pickupSprite = new Sprite("tinfoilHatPickup");
            _sprite = new SpriteMap("tinfoilHat", 32, 32);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 8f);
            _sprite.CenterOrigin();
            thickness = 0.1f;
            physicsMaterial = PhysicsMaterial.Metal;
            editorTooltip = "Protects against the effects of mind control, spy satellites, and awkward social situations.";
        }

        public override void Update() => base.Update();
    }
}
