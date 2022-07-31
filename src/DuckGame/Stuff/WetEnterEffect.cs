// Decompiled with JetBrains decompiler
// Type: DuckGame.WetEnterEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WetEnterEffect : Thing
    {
        private SpriteMap _sprite;

        public WetEnterEffect(float xpos, float ypos, Vec2 dir, Thing attach)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("wetEnter", 16, 16);
            this._sprite.AddAnimation("splash", 0.45f, false, 0, 1);
            this._sprite.SetAnimation("splash");
            this.center = new Vec2(0f, 7f);
            this.graphic = _sprite;
            this.depth = (Depth)0.7f;
            this.alpha = 0.6f;
            this.angle = Maths.DegToRad(-Maths.PointDirection(Vec2.Zero, dir));
            this.anchor = new Anchor(attach)
            {
                offset = new Vec2(xpos, ypos) - attach.position
            };
        }

        public override void Update()
        {
            if (!this._sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
