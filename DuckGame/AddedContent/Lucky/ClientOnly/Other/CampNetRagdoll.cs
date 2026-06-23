namespace DuckGame
{
    [ClientOnly]
    public class CampNetRagdoll : Thing
    {
        public StateBinding _attatchedToBinding = new StateBinding("attatchedTo");
        public Ragdoll attatchedTo;
        public CampNetRagdoll(float xpos, float ypos) : base(xpos, ypos)
        {
            sp = new SpriteMap("campNetDuck", 32, 32);
            graphic = sp;
            center = new Vec2(16);
            collisionSize = new Vec2(16);
            _collisionOffset = new Vec2(-8);
            shouldbegraphicculled = false;
        }
        public SpriteMap sp; 
        public override void Draw()
        {
            if (attatchedTo != null)
            {
                RagdollPart ragdollPart = attatchedTo.part1;
                graphic.center = ragdollPart.center;
                graphic.angle = ragdollPart.angle;
                graphic.flipH = ragdollPart.offDir < 0;
                sp.imageIndex = 0;
                Graphics.Draw(graphic, ragdollPart.x, ragdollPart.y, ragdollPart.depth + 1);

                ragdollPart = attatchedTo.part3;
                graphic.center = ragdollPart.center;
                graphic.angle = ragdollPart.angle;
                graphic.flipH = ragdollPart.offDir < 0;
                sp.imageIndex = 1;
                Graphics.Draw(graphic, ragdollPart.x, ragdollPart.y, ragdollPart.depth + 1);

            }
        }
        public override void Update()
        {
            if (attatchedTo != null)
            {
                position = attatchedTo.position;
                if (isServerForObject)
                {
                    if (attatchedTo.sleepingBagHealth <= 60) Level.Remove(this);
                }
            }
        }
    }
}
