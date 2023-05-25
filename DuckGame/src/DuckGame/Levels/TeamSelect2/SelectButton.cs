// Decompiled with JetBrains decompiler
// Type: DuckGame.SelectButton
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class SelectButton : MaterialThing, IPlatform
    {
        private ProfileBox2 _box;
        private Sprite _button;
        private float _hit;

        public SelectButton(float xpos, float ypos, ProfileBox2 box)
          : base(xpos, ypos)
        {
            graphic = new Sprite("selectButtonAssembly");
            _box = box;
            depth = (Depth)0.2f;
            center = new Vec2(8f, 8f);
            _button = new Sprite("selectButton");
            _button.CenterOrigin();
            _collisionOffset = new Vec2(-6f, -3f);
            _collisionSize = new Vec2(12f, 12f);
        }

        public override void Update()
        {
            _hit = Maths.LerpTowards(_hit, 0f, 0.1f);
            if (Level.CheckPoint<Duck>(x, y + 10f) == null || _hit >= 0.01f)
                return;
            _hit = 1f;
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(_button, x, (float)(y + 2f - _hit * 4f));
        }
    }
}
