using System.Linq;

namespace DuckGame
{
    [ClientOnly]
    public class SinkingBlock : Block
    {
        protected Rectangle _collisionRect
        {
            get
            {
                return new Rectangle(new Vec2(topLeft.x + 1f, topLeft.y - 1f), new Vec2(topRight.x - 1f, topRight.y));
            }
        }
        //topLeft
        //    .ButY(-1, true)
        //    .ButX(1f, true),
        //topRight
        //    .ButX(-1f, true)

        protected virtual bool ShouldSink => Level.CheckRectAll<Duck>(_collisionRect.tl, _collisionRect.br).Any();

        protected ProgressValue _movementProgress = 0f;
        private Vec2 _initialPosition;

        public readonly float SinkMultiplier;

        public SinkingBlock(float x, float y, float width, float height, float sinkMultiplier = 2f) : base(x, y, width, height)
        {
            SinkMultiplier = sinkMultiplier;
            _initialPosition = new Vec2(x, y);
        }

        public override void Update()
        {
            _movementProgress = ~_movementProgress;

            if (ShouldSink)
                _movementProgress++;
            else _movementProgress--;

         
            position = new Vec2(_initialPosition.x, _initialPosition.y + (float)Ease.Out.Cubic(_movementProgress) * SinkMultiplier);

            base.Update();
        }
    }
}