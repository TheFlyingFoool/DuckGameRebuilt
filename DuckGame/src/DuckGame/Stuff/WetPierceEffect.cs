namespace DuckGame
{
    public class WetPierceEffect : Thing
    {
        private SpriteMap _sprite;

        public WetPierceEffect(float xpos, float ypos, Vec2 dir, Thing attach)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("wetParticle", 16, 16);
            _sprite.AddAnimation("splash", 0.45f, false, 0, 1, 2, 3);
            _sprite.SetAnimation("splash");
            center = new Vec2(0f, 7f);
            graphic = _sprite;
            depth = (Depth)0.7f;
            alpha = 0.6f;
            angle = Maths.DegToRad(-Maths.PointDirection(Vec2.Zero, dir));
            anchor = new Anchor(attach)
            {
                offset = new Vec2(xpos, ypos) - attach.position
            };
        }

        public override void Update()
        {
            if (!currentlyDrawing) _sprite.UpdateFrame(true);
            if (!_sprite.finished)
                return;
            Level.Remove(this);
        }

        public override void Draw() => base.Draw();
    }
}
