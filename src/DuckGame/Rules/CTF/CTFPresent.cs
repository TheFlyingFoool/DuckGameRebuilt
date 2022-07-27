// Decompiled with JetBrains decompiler
// Type: DuckGame.CTFPresent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CTFPresent : Present
    {
        private SpriteMap _sprite;

        public CTFPresent(float xpos, float ypos, bool team)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("ctf/present", 18, 17)
            {
                frame = team ? 0 : 1
            };
            this.graphic = _sprite;
            this.center = new Vec2(9f, 8f);
            this.collisionOffset = new Vec2(-9f, -6f);
            this.collisionSize = new Vec2(18f, 14f);
            this.weight = 7f;
            this.flammable = 0.8f;
        }

        public override void OnPressAction()
        {
            if (this.duck == null || this.duck.ctfTeamIndex == this._sprite.frame)
                return;
            base.OnPressAction();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (type is DTIncinerate)
            {
                Level.Remove(this);
                Level.Add(SmallSmoke.New(this.x, this.y));
            }
            return false;
        }
    }
}
