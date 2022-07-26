// Decompiled with JetBrains decompiler
// Type: DuckGame.OfficeLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class OfficeLight : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public OfficeLight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("officeLight");
            this.center = new Vec2(16f, 3f);
            this._collisionSize = new Vec2(30f, 6f);
            this._collisionOffset = new Vec2(-15f, -3f);
            this.depth = (Depth)0.9f;
            this.hugWalls = WallHug.Ceiling;
            this.layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, -3f), this.position + new Vec2(-15f, 4f), new Color(1f, 1f, 1f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(15f, -3f), this.position + new Vec2(15f, 4f), new Color(1f, 1f, 1f)));
            this._occluders.Add(new LightOccluder(this.position + new Vec2(-15f, -2f), this.position + new Vec2(15f, -2f), new Color(1f, 1f, 1f)));
            Level.Add((Thing)new PointLight(this.x, this.y - 1f, new Color((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), 100f, this._occluders));
            this._shade = new SpriteThing(this.x, this.y, new Sprite("officeLight"));
            this._shade.center = this.center;
            this._shade.layer = Layer.Foreground;
            Level.Add((Thing)this._shade);
        }
    }
}
