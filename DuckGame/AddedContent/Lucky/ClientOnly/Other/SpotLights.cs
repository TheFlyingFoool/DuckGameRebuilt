using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Details|Lights", EditorItemType.Normal)]
    public class SpotLights : Thing, IDontMove
    {
        public SpriteMap sprite;
        public SpotLights(float xpos, float ypos) : base(xpos, ypos)
        {
            sprite = new SpriteMap("spotLight", 16, 16);
            graphic = sprite;
            sprite.imageIndex = 1;
            frame = 1;
            center = new Vec2(8);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            depth = (Depth)0.9f;
            hugWalls = WallHug.Ceiling;
            layer = Layer.Game;
            _canFlip = false;
        }
        private List<LightOccluder> _occluders = new List<LightOccluder>();
        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Vec2 vec2 = new Vec2(x, y + 1f);
            _occluders.Add(new LightOccluder(vec2 - new Vec2(2, -2), vec2 - new Vec2(-1, 2), new Color(0.4f, 0.4f, 0.4f)));
            _occluders.Add(new LightOccluder(vec2 + new Vec2(2, 2), vec2 - new Vec2(1, 2), new Color(0.4f, 0.4f, 0.4f)));
            Level.Add(new PointLight(vec2.x, vec2.y, new Color(255, 255, 150), 170f, _occluders));
        }
        public bool hasSetFrame;
        public override void EditorUpdate()
        {
            if (!hasSetFrame) UpdateFrame();
        }
        public override void EditorObjectsChanged()
        {
            hasSetFrame = false;
            base.EditorObjectsChanged();
        }
        public void UpdateFrame()
        {
            bool left = Level.CheckPoint<SpotLights>(x - 16, y) != null;
            bool right = Level.CheckPoint<SpotLights>(x + 16, y) != null;
            if (left && right) frame = 1;
            else if (left) frame = 2;
            else if (right) frame = 0;
            else frame = 1;

            hasSetFrame = true;
        }
        public override void Update()
        {
            if (!hasSetFrame) UpdateFrame();
        }
    }
}
