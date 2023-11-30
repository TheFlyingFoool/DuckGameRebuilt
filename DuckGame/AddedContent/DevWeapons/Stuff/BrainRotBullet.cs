namespace DuckGame
{
    [ClientOnly]
    public class BrainRotBullet : Thing, ITeleport
    {
        public StateBinding _positionBinding = new StateBinding("position");
        public StateBinding _travelBinding = new StateBinding("_travel");
        public SpriteMap orbSprite;
        public SpriteMap ringSprite;
        public BrainRotBullet(float xpos, float ypos, Vec2 travel) : base(xpos, ypos) 
        {
            _travel = travel;
            _collisionSize = new Vec2(12f, 12f);
            _collisionOffset = new Vec2(2f, 2f);
            orbSprite = new SpriteMap("brainrotorb", 18, 18);
            ringSprite = new SpriteMap("brainrotring", 18, 18);
            ringSprite.center = new Vec2(9f, 9f);
            orbSprite.center = new Vec2(9f, 9f);
            graphic = orbSprite;
            wobble = new aWobbleMaterial(this, 0.2f);
        }

        private Vec2 _travel;
        public int safeFrames = 15;

        public override void Update()
        {
            position += _travel * 1f;
            if (isServerForObject && (x > Level.current.bottomRight.x + 200f || x < Level.current.topLeft.x - 200f))
            {
                Level.Remove(this);
            }
            foreach (Duck duck in Level.CheckRectAll<Duck>(topLeft, bottomRight))
            {
                if (safeFrames <= 0 && duck.isServerForObject)
                {
                    duck.GiveBrainRot();
                }
            }
            if (safeFrames > 0) safeFrames--;
            base.Update();
        }
        
        public aWobbleMaterial wobble;
        public override void Draw()
        {
            orbSprite.imageIndex = 0;
            base.Draw();
            orbSprite.imageIndex = 1;
            Graphics.material = wobble;
            depth -= 1;
            base.Draw();
            depth += 1;
            Graphics.material = null;
            
            ringSprite.imageIndex = 0;
            ringSprite.position = position + new Vec2(9f, 9f);
            ringSprite.angle += 0.1f;
            ringSprite.Draw();
            ringSprite.depth -= 1;
            ringSprite.imageIndex = 1;
            Graphics.material = wobble;
            ringSprite.Draw();
            ringSprite.depth += 1;
            Graphics.material = null;
        }
    }
}