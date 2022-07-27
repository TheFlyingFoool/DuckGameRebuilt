// Decompiled with JetBrains decompiler
// Type: DuckGame.SnowBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("isInDemo", false)]
    public class SnowBackground : BackgroundUpdater
    {
        public SnowBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 7
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._editorName = "Snow BG";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this.backgroundColor = new Color(148, 178, 210);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = new ParallaxBackground("background/snowSky", 0.0f, 0.0f, 3);
            float speed = 0.2f;
            this._parallax.AddZone(0, 0.9f, speed);
            this._parallax.AddZone(1, 0.9f, speed);
            this._parallax.AddZone(2, 0.9f, speed);
            this._parallax.AddZone(3, 0.9f, speed);
            this._parallax.AddZone(4, 0.9f, speed);
            this._parallax.AddZone(5, 0.9f, speed);
            this._parallax.AddZone(6, 0.9f, speed);
            this._parallax.AddZone(7, 0.9f, speed);
            this._parallax.AddZone(8, 0.9f, speed);
            this._parallax.AddZone(9, 0.9f, speed);
            this._parallax.AddZone(10, 0.8f, speed);
            this._parallax.AddZone(11, 0.7f, speed);
            this._parallax.AddZone(12, 0.6f, speed);
            this._parallax.AddZone(13, 0.5f, speed);
            this._parallax.AddZone(14, 0.4f, speed);
            this._parallax.AddZone(15, 0.3f, speed);
            Vec2 vec2 = new Vec2(0.0f, -12f);
            Sprite s1 = new Sprite("background/bigBerg1_reflection")
            {
                depth = -0.9f,
                position = new Vec2(-30f, 113f) + vec2
            };
            this._parallax.AddZoneSprite(s1, 12, 0.0f, 0.0f, true);
            Sprite s2 = new Sprite("background/bigBerg1")
            {
                depth = -0.8f,
                position = new Vec2(-31f, 50f) + vec2
            };
            this._parallax.AddZoneSprite(s2, 12, 0.0f, 0.0f, true);
            Sprite s3 = new Sprite("background/bigBerg2_reflection")
            {
                depth = -0.9f,
                position = new Vec2(210f, 108f) + vec2
            };
            this._parallax.AddZoneSprite(s3, 12, 0.0f, 0.0f, true);
            Sprite s4 = new Sprite("background/bigBerg2")
            {
                depth = -0.8f,
                position = new Vec2(211f, 52f) + vec2
            };
            this._parallax.AddZoneSprite(s4, 12, 0.0f, 0.0f, true);
            Sprite s5 = new Sprite("background/berg1_reflection")
            {
                depth = -0.9f,
                position = new Vec2(119f, 131f) + vec2
            };
            this._parallax.AddZoneSprite(s5, 13, 0.0f, 0.0f, true);
            Sprite s6 = new Sprite("background/berg1")
            {
                depth = -0.8f,
                position = new Vec2(121f, 114f) + vec2
            };
            this._parallax.AddZoneSprite(s6, 13, 0.0f, 0.0f, true);
            vec2 = new Vec2(-30f, -20f);
            Sprite s7 = new Sprite("background/berg2_reflection")
            {
                depth = -0.9f,
                position = new Vec2(69f, 153f) + vec2
            };
            this._parallax.AddZoneSprite(s7, 14, 0.0f, 0.0f, true);
            Sprite s8 = new Sprite("background/berg2")
            {
                depth = -0.8f,
                position = new Vec2(71f, 154f) + vec2
            };
            this._parallax.AddZoneSprite(s8, 14, 0.0f, 0.0f, true);
            vec2 = new Vec2(200f, 2f);
            Sprite s9 = new Sprite("background/berg3_reflection")
            {
                depth = -0.9f,
                position = new Vec2(70f, 153f) + vec2
            };
            this._parallax.AddZoneSprite(s9, 15, 0.0f, 0.0f, true);
            Sprite s10 = new Sprite("background/berg3")
            {
                depth = -0.8f,
                position = new Vec2(71f, 154f) + vec2
            };
            this._parallax.AddZoneSprite(s10, 15, 0.0f, 0.0f, true);
            Level.Add(_parallax);
            if (this.level == null)
                return;
            this.level.cold = true;
        }

        public override void Update() => base.Update();

        public override void Terminate() => Level.Remove(_parallax);
    }
}
