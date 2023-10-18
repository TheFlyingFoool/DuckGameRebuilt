namespace DuckGame
{
    [ClientOnly]
    public class BrainRotBubble : Thing
    {
        public SpriteMap sprite;
        RotTypes type;
        private Duck target;

        public StateBinding _positionBinding = new StateBinding("position");
        public BrainRotBubble(float xval, float yval, Duck target, RotTypes type = RotTypes.Amogus) : base(xval, yval)
        { 
            this.type = type;
            this.target = target;

            switch (type)
            {
                case RotTypes.Amogus:
                    sprite = new SpriteMap("amongusbubble", 28, 28);
                    break;
            }
            graphic = sprite;
        }

        public override void Update()
        {
            if (isServerForObject)
            {
                if (target.ragdoll != null)
                {
                    position = new Vec2(target.ragdoll.position + new Vec2(5, -34f));
                }
                else if (target._trapped != null)
                {
                    position = new Vec2(target._trapped.position + new Vec2(5, -34f));
                }
                else
                {
                    position = new Vec2(target.position + new Vec2(5, -34f));
                }
            }
            base.Update();
        }
        
        public enum RotTypes
        {
            Random = 0,
            Amogus = 1,
        }
    }
}