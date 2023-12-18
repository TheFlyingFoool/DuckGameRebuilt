namespace DuckGame
{
    public class NMLightDuck : NMEvent
    {
        public Duck duck;

        public NMLightDuck(Duck pDuck) => duck = pDuck;

        public NMLightDuck()
        {
        }

        public override void Activate()
        {
            if (duck == null)
                return;
            duck.isBurnMessage = true;
            duck.Burn(duck.position, null);
            duck.isBurnMessage = false;
        }
    }
}
