namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    public class PineTreeSnowTileset : PineTree
    {
        private SpriteMap _snowFall;
        private bool didChange;
        private float snowWait = 1f;

        public PineTreeSnowTileset(float x, float y)
          : base(x, y, "pineTilesetSnow")
        {
            _editorName = "Pine Snow";
            physicsMaterial = PhysicsMaterial.Wood;
            verticalWidth = 14f;
            verticalWidthThick = 15f;
            horizontalHeight = 8f;
            _tileset = "pineTileset";
            depth = -0.55f;
            _snowFall = new SpriteMap("snowFall", 8, 24);
            _snowFall.AddAnimation("fall", (float)(0.2f + Rando.Float(0.1f)), false, 0, 1, 2, 3, 4);
            _snowFall.AddAnimation("idle", 0.4f, false, new int[1]);
            _snowFall.SetAnimation("idle");
            _snowFall.center = new Vec2(4f, 0f);
            snowWait = Rando.Float(4f);
        }

        public override void Initialize()
        {
            DoPositioning();
            //this override is here because pine tree's need to update, normal initialize makes them not be in the update loop -N
        }
        public override void KnockOffSnow(Vec2 dir, bool vertShake)
        {
            iterated = true;
            if (!knocked | vertShake)
            {
                int num = knocked ? 1 : 0;
                knocked = true;
                PineTree pineTree1 = Level.CheckPoint<PineTreeSnowTileset>(x - 8f, y, this);
                PineTree pineTree2 = Level.CheckPoint<PineTreeSnowTileset>(x + 8f, y, this);
                if (pineTree1 != null && !pineTree1.iterated && !pineTree1.knocked | vertShake)
                    pineTree1.KnockOffSnow(dir, vertShake);
                if (pineTree2 != null && !pineTree2.iterated && !pineTree2.knocked | vertShake)
                    pineTree2.KnockOffSnow(dir, vertShake);
                if (num == 0)
                {
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 2; ++index)
                        Level.Add(new SnowFallParticle(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f), dir * Rando.Float(1f) + new Vec2(Rando.Float(-0.1f, -0.1f), Rando.Float(-0.1f, -0.1f) - Rando.Float(0.1f, 0.3f))));
                }
            }
            if (_snowFall.currentAnimation == "idle")
                _snowFall.SetAnimation("fall");
            if (vertShake)
                _vertPush = 0.5f;
            knocked = true;
            iterated = false;
        }

        public override void Update()
        {
            if (!edge && !didChange)
            {
                snowWait -= 0.01f;
                if (snowWait <= 0)
                {
                    //bad code lol but idc as of now -NiK0

                    //old bad code was changed for a better solution -NiK0 again

                    //the new code was also wrong -NiK0 yet again
                    snowWait = Rando.Float(2f, 3f) / DGRSettings.ActualParticleMultiplier;
                    if (Rando.Float(1f) > 0.92f) Level.Add(new SnowFallParticle(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f), new Vec2(0f, 0f)));
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (!edge && _snowFall.currentAnimation != "idle" && !_snowFall.finished)
            {
                _snowFall.depth = -0.1f;
                _snowFall.scale = new Vec2(1f, (float)(_snowFall.frame / 5 * 0.04f + 0.2f));
                _snowFall.alpha = (float)(1 - _snowFall.frame / 5 * 1);
                Graphics.Draw(ref _snowFall, x, (float)(y - 7 + _snowFall.frame / 5 * 3));
            }
            if (_snowFall.currentAnimation != "idle" && (edge || _snowFall.frame == 1 && !didChange))
            {
                didChange = true;
                _sprite = new SpriteMap("pineTileset", 8, 16)
                {
                    frame = (graphic as SpriteMap).frame
                };
                graphic = _sprite;
            }
            base.Draw();
        }
    }
}
