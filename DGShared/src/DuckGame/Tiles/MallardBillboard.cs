// Decompiled with JetBrains decompiler
// Type: DuckGame.MallardBillboard
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("billboard", 217, 126);
            graphic = _sprite;
            center = new Vec2(126f, 77f);
            _collisionSize = new Vec2(167f, 6f);
            _collisionOffset = new Vec2(-84f, -2f);
            editorOffset = new Vec2(0f, 40f);
            depth = -0.5f;
            _editorName = "Mallard Billboard";
            thickness = 0.2f;
            hugWalls = WallHug.Floor;
        }

        public override void Initialize() => base.Initialize();

        public override void Draw()
        {
            _sprite.frame = offDir > 0 ? 0 : 1;
            base.Draw();
        }
    }
}
