// Decompiled with JetBrains decompiler
// Type: DuckGame.SelectButton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.graphic = new Sprite("selectButtonAssembly");
            this._box = box;
            this.depth = (Depth)0.2f;
            this.center = new Vec2(8f, 8f);
            this._button = new Sprite("selectButton");
            this._button.CenterOrigin();
            this._collisionOffset = new Vec2(-6f, -3f);
            this._collisionSize = new Vec2(12f, 12f);
        }

        public override void Update()
        {
            this._hit = Maths.LerpTowards(this._hit, 0.0f, 0.1f);
            if (Level.CheckPoint<Duck>(this.x, this.y + 10f) == null || (double)this._hit >= 0.00999999977648258)
                return;
            this._hit = 1f;
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(this._button, this.x, (float)((double)this.y + 2.0 - (double)this._hit * 4.0));
        }
    }
}
