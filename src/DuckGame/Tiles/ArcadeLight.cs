// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class ArcadeLight : Thing
    {
        private PointLight _light;
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public ArcadeLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("arcadeLight");
            this.center = new Vec2(9f, 24f);
            this._collisionSize = new Vec2(16f, 24f);
            this._collisionOffset = new Vec2(-8f, -22f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Ceiling;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-8f, 2f), this.position + new Vec2(-8f, -8f), new Color(1f, 0.7f, 0.7f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(10f, 2f), this.position + new Vec2(10f, -8f), new Color(1f, 0.7f, 0.7f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-8f, -7f), this.position + new Vec2(10f, -7f), new Color(1f, 0.7f, 0.7f)));
            this._light = new PointLight(this.x + 1f, this.y - 6f, new Color((int)byte.MaxValue, (int)byte.MaxValue, 190), 130f, this._occluders);
            Level.Add(_light);
            this._shade = new SpriteThing(this.x, this.y, new Sprite("arcadeLight"))
            {
                center = this.center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }

        public override void Update()
        {
            this._light.visible = this.visible;
            this._shade.visible = this.visible;
            base.Update();
        }
    }
}
