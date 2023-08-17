using System;

namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class TreeTop : Thing
    {
        private Sprite _treeInside;

        public TreeTop(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("treeTop");
            _treeInside = new Sprite("treeTopInside")
            {
                center = new Vec2(24f, 24f),
                alpha = 0.8f,
                depth = (Depth)0.9f
            };
            center = new Vec2(24f, 24f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Ceiling | WallHug.Floor;
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
        }
        public float timer;
        public override void Update()
        {
            timer += 0.01f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 1)
            {
                timer = Rando.Float(0.3f);
                Level.Add(TreeLeaf.New(x + Rando.Float(-16, 16), y + Rando.Float(-16, 16), false));
            }
        }
        public SinWave sw = new SinWave(Rando.Float(0.05f, 0.15f), Rando.Float(-5, 5));
        public override void Draw()
        {
            graphic.flipH = offDir <= 0;
            float myX = x;

            angle += GameLevel.rainwind * sw * 0.02f;
            base.Draw();
            angle = 0;
            x = myX;
        }
    }
}
