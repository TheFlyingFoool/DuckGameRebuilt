﻿namespace DuckGame
{
    public class SpaceBackgroundMenu : BackgroundUpdater
    {
        private float _speedMult;
        private bool _moving;

        public SpaceBackgroundMenu(float xpos, float ypos, bool moving = false, float speedMult = 1f)
          : base(xpos, ypos)
        {
            graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 1
            };
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            depth = (Depth)0.9f;
            layer = Layer.Foreground;
            _visibleInGame = false;
            _speedMult = speedMult;
            _moving = moving;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            Level.current.backgroundColor = new Color(0, 0, 0);
            _parallax = new ParallaxBackground("background/spaceTransparent", 0f, 0f, 3);
            float speed = 0.4f * _speedMult;
            _parallax.AddZone(20, 0.93f, speed, _moving);
            _parallax.AddZone(21, 0.93f, speed, _moving);
            _parallax.AddZone(22, 0.87f, speed, _moving);
            _parallax.AddZone(23, 0.84f, speed, _moving);
            _parallax.AddZone(24, 0.81f, speed, _moving);
            _parallax.AddZone(25, 0.81f, speed, _moving);
            _parallax.AddZone(26, 0.81f, speed, _moving);
            _parallax.AddZone(27, 0.78f, speed, _moving);
            _parallax.AddZone(28, 0.78f, speed, _moving);
            _parallax.AddZone(29, 0.78f, speed, _moving);
            _extraYOffset = 16f;
            _parallax.FUCKINGYOFFSET = 8f;
            Level.Add(_parallax);
        }

        public override void Terminate() => Level.Remove(_parallax);
    }
}
