// Decompiled with JetBrains decompiler
// Type: DuckGame.Lamp
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Lights", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class Lamp : Thing
    {
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public Lamp(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("lamp");
            center = new Vec2(7f, 28f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -15f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Floor;
            layer = Layer.Game;
            editorCycleType = typeof(OfficeLight);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(position + new Vec2(-7f, -16f), position + new Vec2(-3f, -28f), new Color(1f, 0.7f, 0.7f)));
            _occluders.Add(new LightOccluder(position + new Vec2(7f, -16f), position + new Vec2(3f, -28f), new Color(1f, 0.7f, 0.7f)));
            Level.Add(new PointLight(x, y - 24f, new Color((int)byte.MaxValue, (int)byte.MaxValue, 180), 100f, _occluders));
            _shade = new SpriteThing(x, y, new Sprite("lampShade"))
            {
                center = center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }
    }
}
