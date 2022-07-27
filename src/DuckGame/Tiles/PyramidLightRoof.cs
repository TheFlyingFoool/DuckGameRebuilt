// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidLightRoof
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Pyramid", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", true)]
    public class PyramidLightRoof : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private Block myBlock;
        private PointLight light;
        private bool did;

        public PyramidLightRoof(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("pyramidRoofLight");
            this.center = new Vec2(7f, 5f);
            this._collisionSize = new Vec2(14f, 6f);
            this._collisionOffset = new Vec2(-7f, -3f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Ceiling;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, -3f), this.position + new Vec2(-15f, 4f), new Color(1f, 0.9f, 0.8f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(15f, -3f), this.position + new Vec2(15f, 4f), new Color(1f, 0.9f, 0.8f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, -2f), this.position + new Vec2(15f, -2f), new Color(1f, 0.9f, 0.8f)));
            this.light = new PointLight(this.x, this.y - 1f, PyramidWallLight.lightColor, 110f, this._occluders, true);
            Level.Add(light);
            this._shade = new SpriteThing(this.x, this.y, new Sprite("pyramidRoofLightShade"))
            {
                center = this.center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }

        public override void Update()
        {
            if (!this.did)
            {
                this.myBlock = Level.CheckPoint<Block>(new Vec2(this.x, this.y - 8f));
                this.did = true;
            }
            if (this.myBlock != null && this.myBlock.removeFromLevel)
            {
                Level.Remove(this);
                Level.Remove(light);
                Level.Remove(_shade);
            }
            base.Update();
        }
    }
}
