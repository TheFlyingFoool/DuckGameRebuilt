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
                if (DGRSettings.S_ParticleMultiplier != 0) Level.Add(SmallSmoke.New(x, y));
            }
            return false;
        }
    }
}
