// Decompiled with JetBrains decompiler
// Type: DuckGame.SpaceBackgroundMenu
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SpaceBackgroundMenu : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public SpaceBackgroundMenu(float xpos, float ypos, bool moving = false, float speedMult = 1f)
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
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.current.backgroundColor = new Color(0, 0, 0);
            this._parallax = new ParallaxBackground("background/spaceTransparent", 0f, 0f, 3);
            float speed = 0.4f * this._speedMult;
            this._parallax.AddZone(20, 0.93f, speed, this._moving);
            this._parallax.AddZone(21, 0.93f, speed, this._moving);
            this._parallax.AddZone(22, 0.87f, speed, this._moving);
            this._parallax.AddZone(23, 0.84f, speed, this._moving);
            this._parallax.AddZone(24, 0.81f, speed, this._moving);
            this._parallax.AddZone(25, 0.81f, speed, this._moving);
            this._parallax.AddZone(26, 0.81f, speed, this._moving);
            this._parallax.AddZone(27, 0.78f, speed, this._moving);
            this._parallax.AddZone(28, 0.78f, speed, this._moving);
            this._parallax.AddZone(29, 0.78f, speed, this._moving);
            this._extraYOffset = 16f;
            this._parallax.FUCKINGYOFFSET = 8f;
            Level.Add(_parallax);
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
