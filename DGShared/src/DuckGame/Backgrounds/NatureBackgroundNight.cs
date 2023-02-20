// Decompiled with JetBrains decompiler
// Type: DuckGame.NatureBackgroundNight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("isInDemo", true)]
    public class NatureBackgroundNight : BackgroundUpdater
    {
        public NatureBackgroundNight(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 0
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _editorName = "Nature BG Night";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(36, 42, 72);
            Level.current.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/forestNight", 0f, 0f, 3);
            float speed = 0.6f;
            _parallax.AddZone(10, 0.68f, speed);
            _parallax.AddZone(11, 0.65f, speed);
            _parallax.AddZone(12, 0.65f, speed);
            _parallax.AddZone(13, 0.65f, speed);
            _parallax.AddZone(14, 0.6f, speed);
            _parallax.AddZone(15, 0.6f, speed);
            _parallax.AddZone(16, 0.6f, speed);
            _parallax.AddZone(17, 0.6f, speed);
            _parallax.AddZone(18, 0.6f, speed);
            _parallax.AddZone(19, 0.6f, speed);
            _parallax.AddZone(20, 0.6f, speed);
            _parallax.AddZone(21, 0.6f, speed);
            _parallax.AddZone(22, 0.6f, speed);
            _parallax.AddZone(23, 0.55f, speed);
            _parallax.AddZone(24, 0.5f, speed);
            _parallax.AddZone(25, 0.45f, speed);
            _parallax.AddZone(26, 0.4f, speed);
            _parallax.AddZone(27, 0.35f, speed);
            _parallax.AddZone(28, 0.3f, speed);
            _parallax.AddZone(29, 0.25f, speed);
            Level.Add(_parallax);
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
