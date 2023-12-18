namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class BlueBarrel : YellowBarrel
    {
        public BlueBarrel(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("blueBarrel");
            center = new Vec2(7f, 8f);
            _melting = new Sprite("blueBarrelMelting");
            _editorName = "Barrel";
            editorTooltip = "Your standard water barrel - for carrying delicious, refreshing water. Choose water!";
            flammable = 0.3f;
            _fluid = Fluid.Water;
            _toreUp = new SpriteMap("blueBarrelToreUp", 14, 17)
            {
                frame = 1,
                center = new Vec2(0f, -6f)
            };
        }
    }
}
