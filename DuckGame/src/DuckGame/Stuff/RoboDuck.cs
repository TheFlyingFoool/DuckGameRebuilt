// Decompiled with JetBrains decompiler
// Type: DuckGame.RoboDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            wait = 0.1f + RoboDuck._waitDif;
            RoboDuck._waitDif += 0.1f;
            _visibleInGame = false;
        }

        public override void Update()
        {
            if (Network.isActive)
                return;
            try
            {
                wait -= 0.004f;
                if (wait >= 0.0)
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
