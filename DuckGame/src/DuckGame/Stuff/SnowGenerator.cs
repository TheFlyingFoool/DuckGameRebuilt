namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    public class SnowGenerator : Thing
    {
        private static bool initGen;
        private float snowWait = 1f;

        public SnowGenerator(float x, float y)
          : base(x, y)
        {
            _editorName = "Snow Machine";
            graphic = new Sprite("snowGenerator");
            center = new Vec2(8f, 8f);
            depth = (Depth)0.55f;
            _visibleInGame = false;
            snowWait = Rando.Float(4f);
            editorTooltip = "Let it snow!";
            solid = false;
            _collisionSize = new Vec2(0f, 0f);
            maxPlaceable = 32;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            if (DGRSettings.S_ParticleMultiplier == 0) return; //so sad, no snow :(
            snowWait -= Maths.IncFrameTimer();
            if (snowWait <= 0)
            {
                snowWait = Rando.Float(2f, 4f) / DGRSettings.ActualParticleMultiplier;
                Level.Add(new SnowFallParticle(x + Rando.Float(-8f, 8f), y + Rando.Float(-8f, 8f), new Vec2(0f, 0f)));
            }
            base.Update();
        }
    }
}
