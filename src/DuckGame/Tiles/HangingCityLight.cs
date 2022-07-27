// Decompiled with JetBrains decompiler
// Type: DuckGame.HangingCityLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Normal)]
    [BaggedProperty("isInDemo", true)]
    public class HangingCityLight : Thing
    {
        //private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public HangingCityLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("hangingCityLight");
            this.center = new Vec2(8f, 5f);
            this._collisionSize = new Vec2(8f, 8f);
            this._collisionOffset = new Vec2(-4f, -5f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Ceiling;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Vec2 vec2 = new Vec2(this.x, this.y);
            this._occluders.Add(new LightOccluder(vec2 + new Vec2(-8f, 5f), vec2 + new Vec2(1f, -4f), new Color(0.4f, 0.4f, 0.4f)));
            this._occluders.Add(new LightOccluder(vec2 + new Vec2(-1f, -4f), vec2 + new Vec2(8f, 5f), new Color(0.4f, 0.4f, 0.4f)));
            Level.Add(new PointLight(vec2.x, vec2.y, new Color(247, 198, 120), 180f, this._occluders));
        }
    }
}
