// Decompiled with JetBrains decompiler
// Type: DuckGame.StartLight
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class StartLight : Thing
    {
        private SpriteMap _sprite;

        public StartLight()
          : base()
        {
            _sprite = new SpriteMap("trafficLight", 42, 23);
            center = new Vec2(_sprite.w / 2, _sprite.h / 2);
            graphic = _sprite;
            layer = Layer.HUD;
            x = Layer.HUD.camera.width / 2f;
            y = 20f;
        }

        public override void Update()
        {
        }
    }
}
