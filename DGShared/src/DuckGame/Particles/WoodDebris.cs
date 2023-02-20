// Decompiled with JetBrains decompiler
// Type: DuckGame.WoodDebris
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WoodDebris : PhysicsParticle
    {
        public static int kMaxObjects = 64;
        public static WoodDebris[] _objects = new WoodDebris[kMaxObjects];
        public static int _lastActiveObject = 0;
        private SpriteMap _sprite;

        public static WoodDebris New(float xpos, float ypos)
        {
            WoodDebris woodDebris;
            if (_objects[_lastActiveObject] == null)
            {
                woodDebris = new WoodDebris();
                _objects[_lastActiveObject] = woodDebris;
            }
            else
                woodDebris = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            woodDebris.ResetProperties();
            woodDebris.Init(xpos, ypos);
            woodDebris._sprite.globalIndex = GetGlobalIndex();
            woodDebris.globalIndex = GetGlobalIndex();
            return woodDebris;
        }

        public WoodDebris()
          : base(0f, 0f)
        {
            _sprite = new SpriteMap("woodDebris", 8, 8);
            graphic = _sprite;
            center = new Vec2(4f, 4f);
        }

        private void Init(float xpos, float ypos)
        {
            position.x = xpos;
            position.y = ypos;
            hSpeed = -4f - Rando.Float(3f);
            vSpeed = (float)-(Rando.Float(1.5f) + 1.0);
            _sprite.frame = Rando.Int(4);
            _bounceEfficiency = 0.3f;
        }

        public override void Update() => base.Update();
    }
}
