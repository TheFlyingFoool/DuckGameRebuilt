// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._editorName = "Snow";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 15f;
            this.verticalWidth = 14f;
            this.horizontalHeight = 15f;
            this._tileset = "snowTileset";
            this._sprite = new SpriteMap("snowTileset", 16, 16);
            this.graphic = _sprite;
            this._sprite.frame = 40;
            this.cold = true;
            this.willHeat = true;
            this._impactThreshold = -1f;
            this.meltedTileset = tset;
        }

        public override void Initialize()
        {
            if (this.level != null)
                this.level.cold = true;
            base.Initialize();
        }

        public override void HeatUp(Vec2 location)
        {
            if (!this.melted)
            {
                this.melt += 0.05f;
                if (melt > 1.0)
                    this.Melt();
            }
            base.HeatUp(location);
        }

        public void Melt() => this.Melt(Network.isServer, false);

        public void Melt(bool pServer, bool pNetMessage)
        {
            if (this.melted || !(pServer | pNetMessage))
                return;
            this.melted = true;
            this._sprite = new SpriteMap(this.meltedTileset, 16, 16)
            {
                frame = (this.graphic as SpriteMap).frame
            };
            this.graphic = _sprite;
            this.DoPositioning();
            if (!Network.isActive || pNetMessage)
                return;
            Send.Message(new NMMeltTile(this.position));
        }

        public void Freeze() => this.Freeze(Network.isServer, false);

        public void Freeze(bool pServer, bool pNetMessage)
        {
            if (!this.melted || !(pServer | pNetMessage))
                return;
            this.melted = false;
            this._sprite = new SpriteMap(this.frozenTileset, 16, 16)
            {
                frame = (this.graphic as SpriteMap).frame
            };
            this.graphic = _sprite;
            this.DoPositioning();
            this.melt = 0f;
            if (this.melted || !Network.isActive || pNetMessage)
                return;
            Send.Message(new NMFreezeTile(this.position));
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this.melted)
            {
                if (with is PhysicsObject)
                {
                    (with as PhysicsObject).specialFrictionMod = 0.16f;
                    (with as PhysicsObject).modFric = true;
                }
            }
            else if (Graphics.frame - this.lastHitFrame > 5L && with.totalImpactPower > 2.5 && with.impactPowerV > 0.5)
            {
                this.lastHitFrame = Graphics.frame;
                int num = (int)(with.totalImpactPower * 0.5);
                if (num > 4)
                    num = 4;
                if (num < 2)
                    num = 2;
                switch (from)
                {
                    case ImpactedFrom.Left:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(this.right - Rando.Float(0f, 1f), with.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0.3f, 1f), Rando.Float(-0.5f, 0.5f))));
                        break;
                    case ImpactedFrom.Right:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(this.left - Rando.Float(0f, 1f), with.y + Rando.Float(-6f, 6f), new Vec2(-Rando.Float(0.3f, 1f), Rando.Float(-0.5f, 0.5f))));
                        break;
                    case ImpactedFrom.Top:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(with.x + Rando.Float(-6f, 6f), this.bottom + Rando.Float(0f, 1f), new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.3f, 1f))));
                        break;
                    case ImpactedFrom.Bottom:
                        for (int index = 0; index < num; ++index)
                            Level.Add(new SnowFallParticle(with.x + Rando.Float(-6f, 6f), this.top - Rando.Float(0f, 1f), new Vec2(Rando.Float(-0.5f, 0.5f), -Rando.Float(0.3f, 1f))));
                        break;
                }
            }
            this.OnSoftImpact(with, from);
        }
    }
}
