namespace DuckGame
{
    public class NMPurpleBoxServed : NMEvent
    {
        public Duck duck;
        public PurpleBlock block;

        public NMPurpleBoxServed()
        {
        }

        public NMPurpleBoxServed(Duck pDuck, PurpleBlock pBlock)
        {
            duck = pDuck;
            block = pBlock;
        }

        public override void Activate()
        {
            if (duck != null && duck.profile != null && block != null && !block._served.Contains(duck.profile))
                block._served.Add(duck.profile);
            base.Activate();
        }
    }
}
