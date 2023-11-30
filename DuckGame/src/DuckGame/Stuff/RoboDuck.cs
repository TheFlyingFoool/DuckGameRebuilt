using System;

namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class RoboDuck : PhysicsObject
    {
        private SpriteMap _sprite;
        public static float _waitDif;
        private float wait = 1f;

        public RoboDuck(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("survival/robot", 32, 32);
            _sprite.AddAnimation("idle", 0.4f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            _sprite.AddAnimation("walk", 0.2f, true, 8, 9, 10, 11, 12, 13, 14, 15);
            _sprite.SetAnimation("walk");
            graphic = _sprite;
            _collisionSize = new Vec2(8f, 22f);
            _collisionOffset = new Vec2(-4f, -7f);
            center = new Vec2(16f, 16f);
            wait = 0.1f + _waitDif;
            _waitDif += 0.1f;
            _visibleInGame = false;
        }

        public override void Update()
        {
            return; // no trust never run
            if (Network.isActive)
                return;
            try
            {
                wait -= 0.004f;
                if (wait >= 0)
                    return;
                wait = 1f;
                Duck t = new Duck(x, y, Profiles.DefaultPlayer1)
                {
                    ai = new DuckAI()
                };
                t.mindControl = t.ai;
                t.derpMindControl = false;
                (Level.current.camera as FollowCam).Add(t);
                Level.Add(t);
            }
            catch (Exception)
            {
            }
        }
    }
}
