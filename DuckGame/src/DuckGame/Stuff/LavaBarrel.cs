namespace DuckGame
{
    [EditorGroup("Stuff|Props|Barrels")]
    [BaggedProperty("noRandomSpawningOnline", true)]
    public class LavaBarrel : YellowBarrel
    {
        public LavaBarrel(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("lavaBarrel");
            center = new Vec2(7f, 8f);
            _melting = new Sprite("blueBarrelMelting");
            _editorName = "Barrel (Lava)";
            editorTooltip = "Your standard lava barrel - for carrying delicious, refreshing lava. Choose lava!";
            flammable = 0f;
            _fluid = Fluid.Lava;
            _toreUp = new SpriteMap("blueBarrelToreUp", 14, 17)
            {
                frame = 1,
                center = new Vec2(0f, -6f)
            };
        }
    }
}
