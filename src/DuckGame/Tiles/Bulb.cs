// Decompiled with JetBrains decompiler
// Type: DuckGame.Bulb
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class Bulb : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public Bulb(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("bulb");
            this.center = new Vec2(8f, 4f);
            this._collisionSize = new Vec2(4f, 6f);
            this._collisionOffset = new Vec2(-2f, -4f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Ceiling;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add(new PointLight(this.x, this.y, new Color(155, 125, 100), 80f, this._occluders));
            this._shade = new SpriteThing(this.x, this.y, new Sprite("bulb"))
            {
                center = this.center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }
    }
}
