using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class MagicalInkSplat : MaterialThing
    {
        public SpriteMap sprite;
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _frameBinding = new StateBinding("frame");
        public StateBinding _timeBinding = new StateBinding("time");
        public MagicalInkSplat(float x, float y)
            : base(x, y)
        {
            sprite = new SpriteMap("inkStain", 16, 16);
            sprite.imageIndex = 5;
            graphic = sprite;
            center = new Vec2(8);

            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);

            frame = 5;
            layer = Layer.Blocks;
            depth = 2f;//5
            thickness = 5;

            Block b = Level.CheckCircle<Block>(position, 8);
            if (b != null)
            {
                top = b.top;
            }
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            base.OnSoftImpact(with, from);
        }
        public float time;
        public override void Update()
        {
            if (isServerForObject)
            {
                time += 0.01f;
                if (time > 1)
                {
                    collisionSize = new Vec2(collisionSize.x - 2, 12);
                    _collisionOffset = new Vec2(_collisionOffset.x + 1, -8);
                    time = 0;
                    if (frame == 0)
                    {
                        activeSpike = new InkSpike(x, y, new Vec2(0, -1));
                        Level.Add(activeSpike);
                        Level.Remove(this);
                    }
                    frame--;
                }
            }
            if (activeSpike == null || activeSpike.removeFromLevel)
            {
                IEnumerable<IAmADuck> iaads = Level.CheckRectAll<IAmADuck>(topLeft + new Vec2(1, 0), topRight - new Vec2(1, 2));
                foreach (IAmADuck iaad in iaads)
                {
                    MaterialThing mt = (MaterialThing)iaad;
                    if (mt.isServerForObject)
                    {
                        Duck d = Duck.GetAssociatedDuck(mt);
                        if (d != null && !d.dead)
                        {

                            activeSpike = new InkSpike(x, y, -(position - mt.position).normalized);
                            Level.Add(activeSpike);
                        }
                    }
                }
            }
            base.Update();
        }
        public InkSpike activeSpike;
        
        public override void Draw()
        {

            base.Draw();
        }
    }
}