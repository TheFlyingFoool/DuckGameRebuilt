namespace DuckGame
{
    [ClientOnly]
    public class BigTrapped : TrappedDuck
    {
        public StateBinding _sizeBinding = new StateBinding("size");
        public float size;
        public BigTrapped(float xpos, float ypos, BigDuck d) : base(xpos, ypos, d)
        {
            size = d.duckSize;
        }
        public override void Draw()
        {

            base.Draw();
        }
        public override void Update()
        {
            weight = size * 4;
            collisionOffset = new Vec2(-8f * size, -8f);
            collisionSize = new Vec2(16f * size);
            scale = new Vec2(size);
            base.Update();
        }
    }
}
