namespace DuckGame
{
    [EditorGroup("Details")]
    public class BackgroundJets : Thing
    {
        public SpriteMap _leftJet;
        public SpriteMap _rightJet;
        private bool _leftAlternate;
        private bool _rightAlternate = true;

        public BackgroundJets(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("levelJetIdle", 32, 13);
            _leftJet = new SpriteMap("jet", 16, 16);
            _leftJet.AddAnimation("idle", 0.4f, true, 0, 1, 2);
            _leftJet.SetAnimation("idle");
            _leftJet.center = new Vec2(8f, 0f);
            _leftJet.alpha = 0.7f;
            _rightJet = new SpriteMap("jet", 16, 16);
            _rightJet.AddAnimation("idle", 0.4f, true, 1, 2, 0);
            _rightJet.SetAnimation("idle");
            _rightJet.center = new Vec2(8f, 0f);
            _rightJet.alpha = 0.7f;
            center = new Vec2(16f, 8f);
            _collisionSize = new Vec2(16f, 14f);
            _collisionOffset = new Vec2(-8f, -8f);
            editorTooltip = "Things gotta float somehow.";
            hugWalls = WallHug.Ceiling;
            _canFlip = false;
        }

        public float partTimer;
        public override void Update()
        {
            if (DGRSettings.AmbientParticles)
            {
                partTimer += 0.15f * DGRSettings.ActualParticleMultiplier;
                if (partTimer > 1)
                {
                    float rng = Rando.Float(-8, 8);
                    Vec2 v = position + new Vec2(rng - 8, Rando.Float(4, 5f));
                    Ember emb = new Ember(v.x, v.y);
                    emb.hSpeed = -rng / 30f;
                    emb._wave = new SinWaveManualUpdate(Rando.Float(0.01f, 0.03f));
                    emb.vSpeed = Rando.Float(1f, 2f);
                    emb.windAffected = false;
                    Level.Add(emb);

                    rng = Rando.Float(-8, 8);
                    v = position + new Vec2(rng + 8, Rando.Float(4, 5f));
                    emb = new Ember(v.x, v.y);
                    emb.hSpeed = -rng / 30f;
                    emb._wave = new SinWaveManualUpdate(Rando.Float(0.01f, 0.03f));
                    emb.vSpeed = Rando.Float(1, 2f);
                    emb.windAffected = false;
                    Level.Add(emb);
                    partTimer--;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(_leftJet, x - 8f, y + 5f);
            Graphics.Draw(_rightJet, x + 8f, y + 5f);
        }
    }
}
