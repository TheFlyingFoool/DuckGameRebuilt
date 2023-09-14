namespace DuckGame
{
    public class BrainRotBubble : Thing
    {
        public SpriteMap sprite;
        RotTypes type;
        private Duck target;
        public BrainRotBubble(float xval, float yval, Duck target, RotTypes type = RotTypes.Amogus) : base(xval, yval)
        { 
            this.type = type;
            this.target = target;

            switch (type)
            {
                case RotTypes.Amogus:
                    sprite = new SpriteMap("amogusbubble", 28, 28);
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
                else if (target._trappedInstance != null)
                {
                    position = new Vec2(target._trappedInstance.position + new Vec2(5, -34f));
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