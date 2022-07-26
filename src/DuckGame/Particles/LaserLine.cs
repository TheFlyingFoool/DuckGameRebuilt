// Decompiled with JetBrains decompiler
// Type: DuckGame.LaserLine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class LaserLine : Thing
    {
        private float _moveSpeed;
        private float _thickness;
        private Color _color;
        private Vec2 _move;
        private Vec2 _target;
        private float fade = 0.06f;

        public LaserLine(
          Vec2 pos,
          Vec2 target,
          Vec2 moveVector,
          float moveSpeed,
          Color color,
          float thickness,
          float f = 0.06f)
          : base(pos.x, pos.y)
        {
            this._moveSpeed = moveSpeed;
            this._color = color;
            this._thickness = thickness;
            this._move = moveVector;
            this._target = target;
            this.fade = f;
        }

        public override void Update()
        {
            this.alpha -= this.fade;
            if ((double)this.alpha < 0.0)
                Level.Remove((Thing)this);
            this.x += this._move.x * this._moveSpeed;
            this.y += this._move.y * this._moveSpeed;
        }

        public override void Draw() => Graphics.DrawLine(this.position, this.position + this._target, this._color * this.alpha, this._thickness, (Depth)0.9f);
    }
}
