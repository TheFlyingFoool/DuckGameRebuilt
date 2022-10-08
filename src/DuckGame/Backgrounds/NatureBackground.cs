// Decompiled with JetBrains decompiler
// Type: DuckGame.NatureBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class NatureBackground : BackgroundUpdater
    {
        public NatureBackground(float xpos, float ypos)
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
            _editorName = "Nature BG";
        }

        public override void Initialize()
        {
            if (Level.current == null || Level.current is Editor)
                return;
            if (Level.current is GameLevel gl && gl.Raining)
            {
                backgroundColor = new Color(104, 104, 104);
                Level.current.backgroundColor = backgroundColor;
                _parallax = new ParallaxBackground("background/rainyforest", 0f, 0f, 3);
                float speed1 = 0.4f;
                Sprite s1 = new Sprite("background/rcloud1")
                {
                    depth = -0.9f,
                    position = new Vec2(50f, 60f)
                };
                _parallax.AddZoneSprite(s1, 6, 0.72f, speed1, true);
                Sprite s2 = new Sprite("background/rcloud4")
                {
                    depth = -0.9f,
                    position = new Vec2(200f, 95f)
                };
                _parallax.AddZoneSprite(s2, 5, 0.72f, speed1, true);
                Sprite s3 = new Sprite("background/rcloud2")
                {
                    depth = -0.9f,
                    position = new Vec2(170f, 72f)
                };
                _parallax.AddZoneSprite(s3, 8, 0.82f, speed1, true);
                Sprite s4 = new Sprite("background/rcloud5")
                {
                    depth = -0.9f,
                    position = new Vec2(30f, 45f)
                };
                _parallax.AddZoneSprite(s4, 4, 0.82f, speed1, true);
                Sprite s5 = new Sprite("background/rcloud3")
                {
                    depth = -0.9f,
                    position = new Vec2(150f, 30f)
                };
                _parallax.AddZoneSprite(s5, 7, 0.91f, speed1, true);
                int num = 1;
                float speed2 = 0.1f;
                _parallax.AddZone(0, 0.75f, speed2);
                _parallax.AddZone(1, 0.8f, speed2);
                _parallax.AddZone(2, 0.8f, speed2);
                _parallax.AddZone(3, 0.8f, speed2);
                _parallax.AddZone(4, 0.85f, speed2);
                _parallax.AddZone(12 + num, 0.65f, speed1);
                _parallax.AddZone(13 + num, 0.65f, speed1);
                _parallax.AddZone(14 + num, 0.65f, speed1);
                _parallax.AddZone(15 + num, 0.6f, speed1);
                _parallax.AddZone(16 + num, 0.6f, speed1);
                _parallax.AddZone(17 + num, 0.6f, speed1);
                _parallax.AddZone(18 + num, 0.6f, speed1);
                _parallax.AddZone(19 + num, 0.6f, speed1);
                _parallax.AddZone(20 + num, 0.6f, speed1);
                _parallax.AddZone(21 + num, 0.6f, speed1);
                _parallax.AddZone(22 + num, 0.6f, speed1);
                _parallax.AddZone(23 + num, 0.55f, speed1);
                _parallax.AddZone(24 + num, 0.5f, speed1);
                _parallax.AddZone(25 + num, 0.45f, speed1);
                _parallax.AddZone(26 + num, 0.4f, speed1);
                _parallax.AddZone(27 + num, 0.35f, speed1);
                _parallax.AddZone(28 + num, 0.3f, speed1);
                _parallax.AddZone(29 + num, 0.25f, speed1);
                Level.Add(_parallax);
            }
            else
            {
                backgroundColor = new Color(129, 182, 218);
                Level.current.backgroundColor = backgroundColor;
                _parallax = new ParallaxBackground("background/forest5", 0f, 0f, 3);
                float speed1 = 0.4f;
                Sprite s1 = new Sprite("background/cloud1")
                {
                    depth = -0.9f,
                    position = new Vec2(50f, 60f)
                };
                _parallax.AddZoneSprite(s1, 6, 0.72f, speed1, true);
                Sprite s2 = new Sprite("background/cloud4")
                {
                    depth = -0.9f,
                    position = new Vec2(200f, 95f)
                };
                _parallax.AddZoneSprite(s2, 5, 0.72f, speed1, true);
                Sprite s3 = new Sprite("background/cloud2")
                {
                    depth = -0.9f,
                    position = new Vec2(170f, 72f)
                };
                _parallax.AddZoneSprite(s3, 8, 0.82f, speed1, true);
                Sprite s4 = new Sprite("background/cloud5")
                {
                    depth = -0.9f,
                    position = new Vec2(30f, 45f)
                };
                _parallax.AddZoneSprite(s4, 4, 0.82f, speed1, true);
                Sprite s5 = new Sprite("background/cloud3")
                {
                    depth = -0.9f,
                    position = new Vec2(150f, 30f)
                };
                _parallax.AddZoneSprite(s5, 7, 0.91f, speed1, true);
                int num = 1;
                float speed2 = 0.1f;
                _parallax.AddZone(0, 0.75f, speed2);
                _parallax.AddZone(1, 0.8f, speed2);
                _parallax.AddZone(2, 0.8f, speed2);
                _parallax.AddZone(3, 0.8f, speed2);
                _parallax.AddZone(4, 0.85f, speed2);
                _parallax.AddZone(12 + num, 0.65f, speed1);
                _parallax.AddZone(13 + num, 0.65f, speed1);
                _parallax.AddZone(14 + num, 0.65f, speed1);
                _parallax.AddZone(15 + num, 0.6f, speed1);
                _parallax.AddZone(16 + num, 0.6f, speed1);
                _parallax.AddZone(17 + num, 0.6f, speed1);
                _parallax.AddZone(18 + num, 0.6f, speed1);
                _parallax.AddZone(19 + num, 0.6f, speed1);
                _parallax.AddZone(20 + num, 0.6f, speed1);
                _parallax.AddZone(21 + num, 0.6f, speed1);
                _parallax.AddZone(22 + num, 0.6f, speed1);
                _parallax.AddZone(23 + num, 0.55f, speed1);
                _parallax.AddZone(24 + num, 0.5f, speed1);
                _parallax.AddZone(25 + num, 0.45f, speed1);
                _parallax.AddZone(26 + num, 0.4f, speed1);
                _parallax.AddZone(27 + num, 0.35f, speed1);
                _parallax.AddZone(28 + num, 0.3f, speed1);
                _parallax.AddZone(29 + num, 0.25f, speed1);
                Level.Add(_parallax);
            }
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
