// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomParallaxSegment
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax|custom", EditorItemType.Custom)]
    public class CustomParallaxSegment : Thing
    {
        public EditorProperty<int> ystart = new EditorProperty<int>(0, max: 40f, increment: 1f);
        public EditorProperty<int> yend = new EditorProperty<int>(0, max: 40f, increment: 1f);
        public EditorProperty<float> speed = new EditorProperty<float>(0.5f, max: 2f);
        public EditorProperty<float> distance = new EditorProperty<float>(0f, increment: 0.05f);
        public EditorProperty<bool> moving = new EditorProperty<bool>(false);
        private bool initializedParallax;

        public CustomParallaxSegment(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 6
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Parallax Segment";
            _canFlip = false;
            _canHaveChance = false;
        }

        public override void Update()
        {
            if (!initializedParallax)
            {
                CustomParallax customParallax = Level.current.FirstOfType<CustomParallax>();
                if (customParallax != null)
                {
                    if (!customParallax.didInit)
                        customParallax.DoInitialize();
                    if (customParallax.parallax != null)
                    {
                        for (int ystart = (int)this.ystart; ystart <= (int)yend; ++ystart)
                            customParallax.parallax.AddZone(ystart, distance.value, speed.value, moving.value);
                    }
                }
                initializedParallax = true;
            }
            Initialize();
        }
    }
}
