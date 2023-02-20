// Decompiled with JetBrains decompiler
// Type: DuckGame.Bulb
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            graphic = new Sprite("bulb");
            center = new Vec2(8f, 4f);
            _collisionSize = new Vec2(4f, 6f);
            _collisionOffset = new Vec2(-2f, -4f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.Add(new PointLight(x, y, new Color(155, 125, 100), 80f, _occluders));
            _shade = new SpriteThing(x, y, new Sprite("bulb"))
            {
                center = center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }
    }
}
