namespace DuckGame
{
    public class NMConversion : NMEvent
    {
        public Duck who;
        public Duck to;

        public NMConversion()
        {
        }

        public NMConversion(Duck pWho, Duck pTo)
        {
            who = pWho;
            to = pTo;
        }

        public Duck GetDuck(int index)
        {
            foreach (Duck duck in Level.current.things[typeof(Duck)])
            {
                if (duck.profile != null && duck.profile.networkIndex == index)
                    return duck;
            }
            return null;
        }

        public NMConversion(byte pWho, byte pTo)
        {
            who = GetDuck(pWho);
            to = GetDuck(pTo);
        }

        public override void Activate()
        {
            if (who != null && to != null)
            {
                who.isConversionMessage = true;
                who.ConvertDuck(to);
                who.isConversionMessage = false;
            }
            base.Activate();
        }
    }
}
