// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowIceTileset
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Blocks|Snow")]
    [BaggedProperty("isInDemo", false)]
    public class SnowIceTileset : AutoBlock
    {
        protected string meltedTileset = "snowTileset";
        protected string frozenTileset = "snowIceTileset";
        private float melt;
        private bool melted;

        public SnowIceTileset(float x, float y, string tset = "snowIceTileset")
          : base(x, y, tset)
        {
            this._editorName = "Snow Ice";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.verticalWidthThick = 15f;
            this.verticalWidth = 14f;
            this.horizontalHeight = 15f;
            this._impactThreshold = -1f;
            this.willHeat = true;
            this._tileset = "snowTileset";
            this._sprite = new SpriteMap("snowIceTileset", 16, 16)
            {
                frame = 40
            };
            this.graphic = _sprite;
            this.frozenTileset = tset;
        }

        public override void Initialize()
        {
            if (this.level != null)
                this.level.cold = true;
            base.Initialize();
        }

        public void Freeze()
        {
            if (Network.isActive)
                return;
            this.melted = false;
            this._sprite = new SpriteMap(this.frozenTileset, 16, 16)
            {
                frame = (this.graphic as SpriteMap).frame
            };
            this.graphic = _sprite;
            this.DoPositioning();
            this.melt = 0f;
        }

        public override void HeatUp(Vec2 location)
        {
            if (!Network.isActive)
            {
                this.melt += 0.05f;
                if (melt > 1.0)
                {
                    this.melted = true;
                    this._sprite = new SpriteMap(this.meltedTileset, 16, 16)
                    {
                        frame = (this.graphic as SpriteMap).frame
                    };
                    this.graphic = _sprite;
                    this.DoPositioning();
                }
            }
            base.HeatUp(location);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (!this.melted && with is PhysicsObject)
            {
                (with as PhysicsObject).specialFrictionMod = 0.16f;
                (with as PhysicsObject).modFric = true;
            }
            base.OnSolidImpact(with, from);
        }
    }
}
