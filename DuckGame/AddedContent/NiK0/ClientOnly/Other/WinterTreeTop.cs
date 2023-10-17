namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Details")]
    [BaggedProperty("previewPriority", true)]
    public class WinterTreeTop : Thing
    {
        public SpriteMap sprite;
        public EditorProperty<bool> snow = new EditorProperty<bool>(false);
        public WinterTreeTop(float xpos, float ypos)
          : base(xpos, ypos)
        {
            sprite = new SpriteMap("winterTreeTop", 48, 48);
            graphic = sprite;
            center = new Vec2(24f, 24f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Ceiling | WallHug.Floor;
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
            timer = Rando.Float(2);
        }
        public float timer;
        public override void Update()
        {
            timer += 0.01f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 2)
            {
                timer = Rando.Float(0.3f);
                Level.Add(TreeLeaf.New(x + Rando.Float(-16, 16), y + Rando.Float(-16, 16), 3));
            }
        }
        public SinWave sw = new SinWave(Rando.Float(0.03f, 0.05f), Rando.Float(-5, 5));
        public override void Draw()
        {
            sprite.frame = snow ? 1 : 0;
            graphic.flipH = offDir <= 0;
            float myX = x;

            angle += GameLevel.rainwind * sw * 0.028f;
            base.Draw();
            angle = 0;
            x = myX;
        }
    }
}
