// Decompiled with JetBrains decompiler
// Type: DuckGame.UndergroundBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 4
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._speedMult = speedMult;
            this._moving = moving;
            this._editorName = "Bunker BG";
            this._yParallax = false;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this.backgroundColor = new Color(0, 0, 0);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = new ParallaxBackground("background/underground", 0.0f, 0.0f, 5);
            float speed = 0.9f * this._speedMult;
            this._parallax.AddZone(11, 1f, speed);
            this._parallax.AddZone(12, 1f, speed);
            this._parallax.AddZone(13, 1f, speed);
            this._parallax.AddZone(14, 0.98f, speed);
            this._parallax.AddZone(15, 0.97f, speed);
            this._parallax.AddZone(16, 0.75f, speed);
            this._parallax.AddZone(17, 0.75f, speed);
            this._parallax.AddZone(18, 0.75f, speed);
            this._parallax.AddZone(19, 0.75f, speed);
            this._parallax.AddZone(20, 0.75f, speed);
            Level.Add(_parallax);
            this._parallax.x -= 340f;
            this._parallax.restrictBottom = false;
            this._undergroundRocks = new UndergroundRocksBackground(this.x, this.y);
            Level.Add(_undergroundRocks);
            this._skyline = new UndergroundSkyBackground(this.x, this.y);
            Level.Add(_skyline);
        }

        public override void Update()
        {
            int num1 = (int)Vec2.Transform(new Vec2(0.0f, 10f), Level.current.camera.getMatrix()).y;
            if (num1 < 0)
                num1 = 0;
            if (num1 > Resolution.current.y)
                num1 = Resolution.current.y;
            float num2 = Resolution.current.y / (float)Graphics.height;
            Vec2 wallScissor = BackgroundUpdater.GetWallScissor();
            this._undergroundRocks.scissor = new Rectangle((int)wallScissor.x, num1 * num2, (int)wallScissor.y, Resolution.current.y - num1);
            int height = (int)(Vec2.Transform(new Vec2(0.0f, -10f), Level.current.camera.getMatrix()).y * (double)num2);
            if (height < 0)
                height = 0;
            if (height > (double)Resolution.size.y)
                height = (int)Resolution.size.y;
            this._skyline.scissor = new Rectangle((int)wallScissor.x, 0.0f, (int)wallScissor.y, height);
            base.Update();
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
