// Decompiled with JetBrains decompiler
// Type: DuckGame.NatureBackgroundMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NatureBackgroundMenu : BackgroundUpdater
    {
        public NatureBackgroundMenu(float xpos, float ypos)
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
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _parallax = new ParallaxBackground("background/forest", 0f, 0f, 3);
            float speed = 0.4f;
            _parallax.AddZone(0, 0.6f, speed, true);
            _parallax.AddZone(1, 0.6f, speed, true);
            _parallax.AddZone(2, 0.4f, speed, true);
            _parallax.AddZone(3, 0.4f, speed, true);
            _parallax.AddZone(4, 0.2f, speed, true);
            _parallax.AddZone(5, 0.2f, speed, true);
            _parallax.AddZone(6, 0.16f, speed, true);
            _parallax.AddZone(7, 0.71f, speed, vis: false);
            _parallax.AddZone(8, 0.71f, speed, vis: false);
            _parallax.AddZone(9, 0.68f, speed, vis: false);
            _parallax.AddZone(10, 0.68f, speed, vis: false);
            _parallax.AddZone(11, 0.65f, speed, vis: false);
            _parallax.AddZone(12, 0.6f, speed, vis: false);
            _parallax.AddZone(13, 0.6f, speed, vis: false);
            _parallax.AddZone(14, 0.6f, speed, vis: false);
            _parallax.AddZone(15, 0.6f, speed, vis: false);
            _parallax.AddZone(16, 0.6f, speed, vis: false);
            _parallax.AddZone(17, 0.6f, speed, vis: false);
            _parallax.AddZone(18, 0.6f, speed, vis: false);
            _parallax.AddZone(19, 0.6f, speed, vis: false);
            _parallax.AddZone(20, 0.6f, speed, vis: false);
            _parallax.AddZone(21, 0.5f, speed, vis: false);
            _parallax.AddZone(22, 0.45f, speed, vis: false);
            _parallax.AddZone(23, 0.4f, speed, vis: false);
            _parallax.AddZone(24, 0.35f, speed, vis: false);
            _parallax.AddZone(25, 0.3f, speed, vis: false);
            _parallax.AddZone(26, 0.25f, speed, vis: false);
            _parallax.AddZone(27, 0.2f, speed, vis: false);
            _parallax.AddZone(28, 0.15f, speed, vis: false);
            _parallax.AddZone(29, 0.1f, speed, vis: false);
            Level.Add(_parallax);
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
