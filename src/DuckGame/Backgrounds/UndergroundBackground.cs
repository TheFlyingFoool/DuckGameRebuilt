// Decompiled with JetBrains decompiler
// Type: DuckGame.UndergroundBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    public class UndergroundBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;
        private UndergroundRocksBackground _undergroundRocks;
        private UndergroundSkyBackground _skyline;

        public UndergroundBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 4
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _speedMult = speedMult;
            _moving = moving;
            _editorName = "Bunker BG";
            _yParallax = false;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            backgroundColor = new Color(0, 0, 0);
            Level.current.backgroundColor = backgroundColor;
            _parallax = new ParallaxBackground("background/underground", 0f, 0f, 5);
            float speed = 0.9f * _speedMult;
            _parallax.AddZone(11, 1f, speed);
            _parallax.AddZone(12, 1f, speed);
            _parallax.AddZone(13, 1f, speed);
            _parallax.AddZone(14, 0.98f, speed);
            _parallax.AddZone(15, 0.97f, speed);
            _parallax.AddZone(16, 0.75f, speed);
            _parallax.AddZone(17, 0.75f, speed);
            _parallax.AddZone(18, 0.75f, speed);
            _parallax.AddZone(19, 0.75f, speed);
            _parallax.AddZone(20, 0.75f, speed);
            Level.Add(_parallax);
            _parallax.x -= 340f;
            _parallax.restrictBottom = false;
            _undergroundRocks = new UndergroundRocksBackground(x, y);
            Level.Add(_undergroundRocks);
            _skyline = new UndergroundSkyBackground(x, y);
            Level.Add(_skyline);
        }

        public override void Update()
        {
            int num1 = (int)Vec2.Transform(new Vec2(0f, 10f), Level.current.camera.getMatrix()).y;
            if (num1 < 0)
                num1 = 0;
            if (num1 > Resolution.current.y)
                num1 = Resolution.current.y;
            float num2 = Resolution.current.y / (float)Graphics.height;
            Vec2 wallScissor = BackgroundUpdater.GetWallScissor();
            _undergroundRocks.scissor = new Rectangle((int)wallScissor.x, num1 * num2, (int)wallScissor.y, Resolution.current.y - num1);
            int height = (int)(Vec2.Transform(new Vec2(0f, -10f), Level.current.camera.getMatrix()).y * num2);
            if (height < 0)
                height = 0;
            if (height > Resolution.size.y)
                height = (int)Resolution.size.y;
            _skyline.scissor = new Rectangle((int)wallScissor.x, 0f, (int)wallScissor.y, height);
            base.Update();
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
