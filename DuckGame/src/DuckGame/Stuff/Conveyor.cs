namespace DuckGame
{
    [BaggedProperty("isInDemo", true)]
    public class Conveyor : Block
    {
        private SpriteMap _sprite;
        public bool up = true;
        protected ImpactedFrom _killImpact;

        public Conveyor(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("stuff/conveyor", 14, 10);
            _sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            _sprite.frame = Rando.Int(0, 7);
            _sprite.SetAnimation("convey");
            graphic = _sprite;
            center = new Vec2(7f, 5f);
            collisionOffset = new Vec2(-7f, -4f);
            collisionSize = new Vec2(14f, 8f);
            depth = (Depth)0.5f;
            _editorName = nameof(Conveyor);
            thickness = 100f;
            physicsMaterial = PhysicsMaterial.Metal;
            editorOffset = new Vec2(0f, 6f);
            hugWalls = WallHug.Floor;
            _editorImageCenter = true;
            _killImpact = ImpactedFrom.Top;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}
