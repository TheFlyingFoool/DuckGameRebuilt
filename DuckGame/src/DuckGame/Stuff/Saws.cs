namespace DuckGame
{
    [EditorGroup("Stuff|Spikes")]
    public class Saws : MaterialThing, IDontMove
    {
        private SpriteMap _sprite;
        public bool up = true;

        public Saws(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("movingSpikes", 16, 21)
            {
                speed = 0.3f
            };
            graphic = _sprite;
            center = new Vec2(8f, 14f);
            collisionOffset = new Vec2(-6f, -2f);
            collisionSize = new Vec2(12f, 4f);
            depth = (Depth)0.28f;
            _editorName = "Saws Up";
            editorTooltip = "Deadly hazards, able to cut through even the strongest of boots";
            thickness = 3f;
            physicsMaterial = PhysicsMaterial.Metal;
            editorOffset = new Vec2(0f, 6f);
            hugWalls = WallHug.Floor;
            _editorImageCenter = true;
            editorCycleType = typeof(SawsRight);
            impactThreshold = 0.01f;
            shouldbeinupdateloop = false;
        }

        public override void Touch(MaterialThing with)
        {
            Duck duck = with as Duck;
            if (!with.isServerForObject || duck != null && duck.holdObject is Sword && (duck.holdObject as Sword)._slamStance || with.destroyed)
                return;
            with.Destroy(new DTImpale(this));
            with.vSpeed = -3f;
        }

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();
    }
}
