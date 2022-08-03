// Decompiled with JetBrains decompiler
// Type: DuckGame.ArcadeScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Arcade", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class ArcadeScreen : Thing
    {
        private PointLight _light;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public ArcadeScreen(float xpos, float ypos)
          : base(xpos, ypos)
        {
            center = new Vec2(5f, 4f);
            _collisionSize = new Vec2(16f, 24f);
            _collisionOffset = new Vec2(-8f, -22f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(position + new Vec2(-7f, -8f), position + new Vec2(7f, -8f), new Color(0.7f, 0.7f, 0.7f)));
            _light = new PointLight(x + 1f, y - 7f, new Color(100, 130, 180), 30f, _occluders);
            Level.Add(_light);
        }

        public override void Update()
        {
            _light.visible = visible;
            base.Update();
        }
    }
}
