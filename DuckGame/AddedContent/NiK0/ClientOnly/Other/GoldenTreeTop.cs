﻿namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("previewPriority", true)]
    public class GoldenTreeTop : Thing
    {
        public SpriteMap sprite;
        public GoldenTreeTop(float xpos, float ypos)
          : base(xpos, ypos)
        {
            sprite = new SpriteMap("goldenTreeTop", 48, 48);
            graphic = sprite;
            center = new Vec2(24f, 24f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Left | WallHug.Right | WallHug.Ceiling | WallHug.Floor;
            shouldbeinupdateloop = DGRSettings.AmbientParticles;
            editorTooltip = "So fancy! Almost like it came from a Kingdom!";
            _editorName = "Tree Top Gold";
        }
        public float timer;
        public bool LateInitialize;
        public override void Update()
        {
            if (!LateInitialize)
            {
                sw = new SinWave(this, Rando.Float(0.1f, 0.15f), Rando.Float(-5, 5));
                timer = Rando.Float(2);
                LateInitialize = true;
            }
            timer += 0.01f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 2)
            {
                timer = Rando.Float(0.3f);
                Level.Add(TreeLeaf.New(x + Rando.Float(-16, 16), y + Rando.Float(-16, 16), 4));
            }
        }
        public SinWave sw;
        public override void Draw()
        {
            graphic.flipH = offDir <= 0;
            if (DGRSettings.AmbientParticles && sw != null)
            {
                float pAng = angle;
                angle += GameLevel.rainwind * sw * 0.03f;
                base.Draw();
                angle = pAng;
            }
            else base.Draw();
        }
    }
}
