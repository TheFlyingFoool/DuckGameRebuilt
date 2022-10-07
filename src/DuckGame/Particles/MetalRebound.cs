// Decompiled with JetBrains decompiler
// Type: DuckGame.MetalRebound
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MetalRebound : Thing
    {
        public static int kMaxObjects = 32;
        public static MetalRebound[] _objects = new MetalRebound[MetalRebound.kMaxObjects];
        private static int _lastActiveObject = 0;
        private SpriteMap _sprite;

        public static MetalRebound New(float xpos, float ypos, int offDir)
        {
            MetalRebound metalRebound;
            if (MetalRebound._objects[MetalRebound._lastActiveObject] == null)
            {
                metalRebound = new MetalRebound();
                MetalRebound._objects[MetalRebound._lastActiveObject] = metalRebound;
            }
            else
                metalRebound = MetalRebound._objects[MetalRebound._lastActiveObject];
            MetalRebound._lastActiveObject = (MetalRebound._lastActiveObject + 1) % MetalRebound.kMaxObjects;
            metalRebound.Init(xpos, ypos, offDir);
            metalRebound.ResetProperties();
            return metalRebound;
        }

        public MetalRebound()
          : base()
        {
            _sprite = new SpriteMap("metalRebound", 16, 16);
            graphic = _sprite;
        }

        private void Init(float xpos, float ypos, int offDir)
        {
            position.x = xpos;
            position.y = ypos;
            alpha = 1f;
            _sprite.frame = Rando.Int(3);
            _sprite.flipH = offDir < 0;
            center = new Vec2(16f, 8f);
        }

        public override void Update()
        {
            alpha -= 0.1f;
            if (alpha >= 0.0)
                return;
            Level.Remove(this);
        }
    }
}
