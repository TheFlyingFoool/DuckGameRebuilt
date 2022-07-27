// Decompiled with JetBrains decompiler
// Type: DuckGame.PyramidBLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Pyramid", EditorItemType.Pyramid)]
    [BaggedProperty("isInDemo", true)]
    public class PyramidBLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        private SpriteMap _sprite;

        public PyramidBLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("pyramidBackgroundLight", 14, 12);
            this._sprite.AddAnimation("go", 0.2f, true, 0, 1, 2, 3, 4);
            this._sprite.SetAnimation("go");
            this.graphic = _sprite;
            this.center = new Vec2(7f, 8f);
            this._collisionSize = new Vec2(8f, 8f);
            this._collisionOffset = new Vec2(-4f, -4f);
            this.depth = -0.9f;
            this.alpha = 0.7f;
            this.layer = Layer.Game;
            this.placementLayerOverride = Layer.Blocks;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add(new PointLight(this.x, this.y - 1f, PyramidWallLight.lightColor, 120f, strangeFalloff: true));
        }
    }
}
