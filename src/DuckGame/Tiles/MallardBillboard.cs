// Decompiled with JetBrains decompiler
// Type: DuckGame.MallardBillboard
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Details|Signs")]
    public class MallardBillboard : MaterialThing, IPlatform
    {
        private SpriteMap _sprite;

        public MallardBillboard(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("billboard", 217, 126);
            this.graphic = _sprite;
            this.center = new Vec2(126f, 77f);
            this._collisionSize = new Vec2(167f, 6f);
            this._collisionOffset = new Vec2(-84f, -2f);
            this.editorOffset = new Vec2(0.0f, 40f);
            this.depth = -0.5f;
            this._editorName = "Mallard Billboard";
            this.thickness = 0.2f;
            this.hugWalls = WallHug.Floor;
        }

        public override void Initialize() => base.Initialize();

        public override void Draw()
        {
            this._sprite.frame = this.offDir > 0 ? 0 : 1;
            base.Draw();
        }
    }
}
