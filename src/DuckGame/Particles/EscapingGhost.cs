// Decompiled with JetBrains decompiler
// Type: DuckGame.EscapingGhost
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class EscapingGhost : Thing
    {
        private SpriteMap _sprite;

        public EscapingGhost(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("ghost", 32, 32);
            this._sprite.AddAnimation("wither", 0.5f, false, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
            this._sprite.SetAnimation("wither");
            this.center = new Vec2(16f, 32f);
            this.alpha = 0.6f;
            this.depth = (Depth)0.9f;
            this.graphic = _sprite;
        }

        public override void Update()
        {
            if (this._sprite.finished)
                Level.Remove(this);
            base.Update();
        }
    }
}
