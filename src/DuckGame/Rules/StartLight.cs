// Decompiled with JetBrains decompiler
// Type: DuckGame.StartLight
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("trafficLight", 42, 23);
            this.center = new Vec2((float)(this._sprite.w / 2), (float)(this._sprite.h / 2));
            this.graphic = (Sprite)this._sprite;
            this.layer = Layer.HUD;
            this.x = Layer.HUD.camera.width / 2f;
            this.y = 20f;
        }

        public override void Update()
        {
        }
    }
}
