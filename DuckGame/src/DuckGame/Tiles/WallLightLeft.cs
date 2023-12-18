using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Details|Arcade", EditorItemType.Lighting)]
    [BaggedProperty("isInDemo", true)]
    public class WallLightLeft : Thing
    {
        private PointLight _light;
        private SpriteThing _shade;
        private List<LightOccluder> _occluders = new List<LightOccluder>();

        public WallLightLeft(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("wallLight");
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(5f, 16f);
            _collisionOffset = new Vec2(-7f, -8f);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Left;
            layer = Layer.Game;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _occluders.Add(new LightOccluder(topLeft + new Vec2(-2f, 0f), topRight, new Color(1f, 0.8f, 0.8f)));
            _occluders.Add(new LightOccluder(bottomLeft + new Vec2(-2f, 0f), bottomRight, new Color(1f, 0.8f, 0.8f)));
            _light = new PointLight(x - 5f, y, new Color((int)byte.MaxValue, (int)byte.MaxValue, 190), 100f, _occluders);
            Level.Add(_light);
            _shade = new SpriteThing(x, y, new Sprite("wallLight"))
            {
                center = center,
                layer = Layer.Foreground
            };
            Level.Add(_shade);
        }

        public override void Update()
        {
            _light.visible = visible;
            base.Update();
        }

        public override void Draw() => base.Draw();
    }
}
