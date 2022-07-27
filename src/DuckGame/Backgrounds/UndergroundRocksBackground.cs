// Decompiled with JetBrains decompiler
// Type: DuckGame.UndergroundRocksBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class UndergroundRocksBackground : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public UndergroundRocksBackground(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 1
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._speedMult = speedMult;
            this._moving = moving;
            this.visible = false;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.current.backgroundColor = new Color(0, 0, 0);
            this._parallax = new ParallaxBackground("background/rocksUnderground", 0.0f, 0.0f, 3);
            float speed = 0.8f * this._speedMult;
            float distance = 0.8f;
            for (int yPos = 0; yPos < 10; ++yPos)
                this._parallax.AddZone(yPos, distance, speed);
            this._parallax.AddZone(10, 0.85f, speed);
            this._parallax.AddZone(11, 0.9f, speed);
            this._parallax.AddZone(19, 0.9f, speed);
            this._parallax.AddZone(20, 0.85f, speed);
            this._parallax.restrictBottom = false;
            for (int index = 0; index < 11; ++index)
                this._parallax.AddZone(21 + index, distance, speed);
            this._parallax.depth = - 0.9f;
            this._parallax.layer = new Layer("PARALLAX2", 110, new Camera(0.0f, 0.0f, 320f, 200f))
            {
                aspectReliesOnGameLayer = true,
                allowTallAspect = true
            };
            this.overrideBaseScissorCall = true;
            Layer.Add(this._parallax.layer);
            Level.Add(_parallax);
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
