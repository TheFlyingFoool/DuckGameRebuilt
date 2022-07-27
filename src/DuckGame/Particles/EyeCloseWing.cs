// Decompiled with JetBrains decompiler
// Type: DuckGame.EyeCloseWing
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EyeCloseWing : Thing
    {
        private SpriteMap _sprite;
        private float _move;
        private int _dir;
        private Duck _closer;

        public EyeCloseWing(float xpos, float ypos, int dir, SpriteMap s, Duck own, Duck closer)
          : base(xpos, ypos)
        {
            this._sprite = s.CloneMap();
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this._dir = dir;
            this.depth = (Depth)0.9f;
            if (this._dir < 0)
                this.angleDegrees = 70f;
            else
                this.angleDegrees = 120f;
            this.owner = own;
            this._closer = closer;
            if (this._dir >= 0)
                return;
            this.x += 14f;
        }

        public override void Update()
        {
            float num = 0.3f;
            this.x += _dir * num;
            this._move += num;
            if (this._dir < 0)
                this.angleDegrees += 2f;
            else
                this.angleDegrees -= 2f;
            if (_move > 4.0)
                this._closer.eyesClosed = true;
            if (_move <= 8.0)
                return;
            Level.Remove(this);
            (this._owner as Duck).closingEyes = false;
        }

        public override void Draw()
        {
            int frame = this._sprite.frame;
            this._sprite.flipV = this._dir <= 0;
            this._sprite.flipH = false;
            this._sprite.frame = 18;
            base.Draw();
            this._sprite.frame = frame;
        }
    }
}
