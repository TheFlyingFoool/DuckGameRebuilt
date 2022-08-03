// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterSplash
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WaterSplash : Thing
    {
        private SpriteMap _sprite;

        public WaterSplash(float xpos, float ypos, FluidData fluid)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("whiteSplash", 16, 16);
            _sprite.AddAnimation("splash", 0.42f, false, 0, 1, 2, 3);
            _sprite.SetAnimation("splash");
            _sprite.color = new Color(fluid.color);
            center = new Vec2(8f, 16f);
            graphic = _sprite;
            depth = (Depth)0.7f;
        }

        public override void Update()
        {
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
