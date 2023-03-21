// Decompiled with JetBrains decompiler
// Type: DuckGame.SpaceBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("previewPriority", true)]
    public class SpaceBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public SpaceBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 3
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _speedMult = speedMult;
            _moving = moving;
            _editorName = "Space BG";
            editorCycleType = typeof(VirtualBackground);
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(0, 0, 0);
            level.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/space", 0f, 0f, 3);
            float speed = 0.4f * _speedMult;
            Sprite s = new Sprite("background/planet4")
            {
                depth = -0.9f,
                position = new Vec2(200f, 50f)
            };
            _parallax.AddZoneSprite(s, 19, 0.99f, speed);
            _parallax.AddZone(20, 0.93f, speed, _moving);
            _parallax.AddZone(21, 0.9f, speed, _moving);
            _parallax.AddZone(22, 0.87f, speed, _moving);
            _parallax.AddZone(23, 0.84f, speed, _moving);
            _parallax.AddZone(24, 0.81f, speed, _moving);
            _parallax.AddZone(25, 0.81f, speed, _moving);
            _parallax.AddZone(26, 0.78f, speed, _moving);
            _parallax.AddZone(27, 0.78f, speed, _moving);
            _parallax.AddZone(28, 0.75f, speed, _moving);
            _parallax.AddZone(29, 0.75f, speed, _moving);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
