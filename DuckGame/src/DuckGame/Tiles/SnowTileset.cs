namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class SnowTileset : AutoBlock
    {
        protected string meltedTileset = "snowTileset";
        protected string frozenTileset = "snowIceTileset";
        private float melt;
        private bool melted = true;
        private long lastHitFrame;

        public SnowTileset(float x, float y, string tset = "snowTileset")
          : base(x, y, tset)
        {
            _editorName = "Snow";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidthThick = 15f;
            verticalWidth = 14f;
            horizontalHeight = 15f;
            _tileset = "snowTileset";
            _sprite = new SpriteMap("snowTileset", 16, 16);
            graphic = _sprite;
            _sprite.frame = 40;
            cold = true;
            willHeat = true;
            _impactThreshold = -1f;
            meltedTileset = tset;
        }

        public override void Initialize()
        {
            if (level != null)
                level.cold = true;
            base.Initialize();
        }

        public override void HeatUp(Vec2 location)
        {
            if (!melted)
            {
                melt += 0.05f;
                if (melt > 1)
                    Melt();
            }
            base.HeatUp(location);
        }

        public void Melt() => Melt(Network.isServer, false);

        public void Melt(bool pServer, bool pNetMessage)
        {
            if (melted || !(pServer | pNetMessage))
                return;
            melted = true;
            _sprite = new SpriteMap(meltedTileset, 16, 16)
            {
                frame = (graphic as SpriteMap).frame
            };
            graphic = _sprite;
            DoPositioning();
            if (!Network.isActive || pNetMessage)
                return;
            Send.Message(new NMMeltTile(position));
        }

        public void Freeze() => Freeze(Network.isServer, false);

        public void Freeze(bool pServer, bool pNetMessage)
        {
            if (!melted || !(pServer | pNetMessage))
                return;
            melted = false;
            _sprite = new SpriteMap(frozenTileset, 16, 16)
            {
                frame = (graphic as SpriteMap).frame
            };
            graphic = _sprite;
            DoPositioning();
            melt = 0f;
            if (melted || !Network.isActive || pNetMessage)
                return;
            Send.Message(new NMFreezeTile(position));
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!melted)
            {
                if (with is PhysicsObject)
                {
                    (with as PhysicsObject).specialFrictionMod = 0.16f;
                    (with as PhysicsObject).modFric = true;
                }
            }
            else if (Graphics.frame - lastHitFrame > 5L && with.totalImpactPower > 2.5 && with.impactPowerV > 0.5)
            {
                lastHitFrame = Graphics.frame;
                int num = (int)(with.totalImpactPower * 0.5);
                if (num > 4)
                    num = 4;
                if (num < 2)
                    num = 2;
                num = (int)(DGRSettings.ActualParticleMultiplier * num);
                switch (from)
                {
                    case ImpactedFrom.Left:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(right - Rando.Float(0f, 1f), with.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0.3f, 1f), Rando.Float(-0.5f, 0.5f))));
                        break;
                    case ImpactedFrom.Right:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(left - Rando.Float(0f, 1f), with.y + Rando.Float(-6f, 6f), new Vec2(-Rando.Float(0.3f, 1f), Rando.Float(-0.5f, 0.5f))));
                        break;
                    case ImpactedFrom.Top:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(with.x + Rando.Float(-6f, 6f), bottom + Rando.Float(0f, 1f), new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.3f, 1f))));
                        break;
                    case ImpactedFrom.Bottom:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(with.x + Rando.Float(-6f, 6f), top - Rando.Float(0f, 1f), new Vec2(Rando.Float(-0.5f, 0.5f), -Rando.Float(0.3f, 1f))));
                        break;
                }
            }
            OnSoftImpact(with, from);
        }
    }
}
