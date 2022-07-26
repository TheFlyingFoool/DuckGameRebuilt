// Decompiled with JetBrains decompiler
// Type: DuckGame.Confetti
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Confetti : Thing
    {
        private Color _color;
        private SinWaveManualUpdate _sin;
        private float _size;
        private float _speed;

        public Confetti(float xpos, float ypos)
          : base(xpos, ypos)
        {
            int num = Rando.ChooseInt(0, 1, 2, 3);
            if (num == 0)
                this._color = Color.Violet;
            if (num == 1)
                this._color = Color.SkyBlue;
            if (num == 2)
                this._color = Color.Wheat;
            if (num == 4)
                this._color = Color.GreenYellow;
            this._sin = new SinWaveManualUpdate(0.01f + Rando.Float(0.03f), Rando.Float(7f));
            this._size = 10f + Rando.Float(60f);
            this._speed = 0.8f + Rando.Float(1.4f);
            this.depth = (Depth)0.95f;
        }

        public override void Update()
        {
            this._sin.Update();
            this.y += this._speed;
        }

        public override void Draw()
        {
            Vec2 position = this.position;
            position.x += this._sin.value * this._size;
            Graphics.DrawRect(position, position + new Vec2(2f, 2f), this._color, this.depth);
        }
    }
}
