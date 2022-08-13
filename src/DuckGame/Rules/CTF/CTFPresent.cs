// Decompiled with JetBrains decompiler
// Type: DuckGame.CTFPresent
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _sprite = new SpriteMap("ctf/present", 18, 17)
            {
                frame = team ? 0 : 1
            };
            graphic = _sprite;
            center = new Vec2(9f, 8f);
            collisionOffset = new Vec2(-9f, -6f);
            collisionSize = new Vec2(18f, 14f);
            weight = 7f;
            flammable = 0.8f;
        }

        public override void OnPressAction()
        {
            if (duck == null || duck.ctfTeamIndex == _sprite.frame)
                return;
            base.OnPressAction();
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (type is DTIncinerate)
            {
                Level.Remove(this);
                Level.Add(SmallSmoke.New(x, y));
            }
            return false;
        }
    }
}
