namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Details|Weather")]
    public class SnowTile : Thing
    {
        public EditorProperty<float> strength = new EditorProperty<float>(1, null, 0, 2, 0.01f);
        public SnowTile(float xpos, float ypos) : base(xpos, ypos)
        {
            graphic = new Sprite("blizzardThing");
            center = new Vec2(8);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            _visibleInGame = false;
            _editorName = "Snow";
            maxPlaceable = 1;
            editorTooltip = "Adds some nice snow to the level.";
        }
        public float snowTimer;
        public override void Update()
        {
            if (Level.First<RainTile>() != null)
            {
                Level.Remove(this);
                return;
            }
            snowTimer += 0.1f * DGRSettings.WeatherMultiplier * strength;
            if (snowTimer > 1)
            {
                for (int i = 0; i < snowTimer; i++)
                {
                    snowTimer -= 1;
                    Vec2 v = new Vec2(Rando.Float(Level.current.topLeft.x - 128, Level.current.bottomRight.x + 128), Level.current.topLeft.y - 100);
                    SnowFallParticle sn = new SnowFallParticle(v.x, v.y, new Vec2(0, 1), Rando.Int(2) == 0);
                    sn.life = Rando.Float(1, 2);
                    Level.Add(sn);
                }
            }
        }
    }
}