﻿namespace DuckGame
{
    [EditorGroup("Details|Terrain")]
    [BaggedProperty("isInDemo", true)]
    public class TreeTopDead : Thing
    {
        private Sprite _treeInside;

        public TreeTopDead(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("treeTopDead");
            _treeInside = new Sprite("treeTopInsideDead")
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
        public bool LateInitialize;
        public override void Update()
        {
            if (!LateInitialize)
            {
                sw = new SinWave(this, Rando.Float(0.05f, 0.15f), Rando.Float(-5, 5));
                timer = Rando.Float(2);
                LateInitialize = true;
            }
            timer += 0.01f * DGRSettings.ActualParticleMultiplier;
            if (timer >= 1)
            {
                timer = Rando.Float(0.1f, 0.3f);
                Level.Add(TreeLeaf.New(x + Rando.Float(-16, 16), y + Rando.Float(-16, 16), 1));
            }
        }
        public SinWave sw;
        public override void Draw()
        {
            graphic.flipH = offDir <= 0;

            if (DGRSettings.AmbientParticles && sw != null)
            {
                float pAng = angle;
                angle += GameLevel.rainwind * sw * 0.02f;
                base.Draw();
                angle = pAng;
            }
            else base.Draw();
        }
    }
}
